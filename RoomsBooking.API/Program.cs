using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoomsBooking.API.Endpoints;
using RoomsBooking.API.Middleware;
using RoomsBooking.API.OpenApi;
using RoomsBooking.Application.Common.Authentication;
using RoomsBooking.Application.Common.Behaviors;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Infrastructure.Authentication;
using RoomsBooking.Infrastructure.Persistence;
using Scalar.AspNetCore;

namespace RoomsBooking.API;

internal sealed partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()
                         ?? throw new InvalidOperationException("Секция JwtOptions не найдена в конфигурации.");

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    // SecretKey подтянется из переменных окружения
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IAppDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        builder.Services.AddSingleton<UniqueConstraintResolver>(sp =>
        {
            // временный scope только для того, чтобы вытащить IModel
            using var scope = sp.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return new UniqueConstraintResolver(dbContext.Model);
        });

        builder.Services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

        builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

        builder.Services.AddValidatorsFromAssemblyContaining<JwtOptions>();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            // Чтобы энумы отображались строками, а не числами
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<JwtOptions>();

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        builder.Services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<BearerSecurityOperationTransformer>();
        });


        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        await ApplyMigrationsAsync(app.Services);

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .DisableAgent()
                    .DisableMcp()
                    .DisableTelemetry()
                    .HideClientButton();
            });
        }

        app.UseExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();

        var apiV1 = app.MapGroup("/api/v1");

        apiV1.MapAuthenticationEndpoints();
        apiV1.MapRoomEndpoints();

        Console.WriteLine(app.Environment.EnvironmentName);
        await app.RunAsync();
    }

    private static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            if (context.Database.IsRelational())
                await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            LogApplyMigrationsError(logger, ex);
        }
    }

    [LoggerMessage(LogLevel.Error, "Ошибка при применении миграций к базе")]
    static partial void LogApplyMigrationsError(ILogger<Program> logger, Exception exception);
}