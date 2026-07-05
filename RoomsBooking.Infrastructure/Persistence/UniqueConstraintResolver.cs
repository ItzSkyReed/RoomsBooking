using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RoomsBooking.Infrastructure.Persistence;

public sealed class UniqueConstraintResolver
{
    private readonly Dictionary<string, string[]> _constraints;

    public UniqueConstraintResolver(IModel model)
    {
        _constraints = model
            .GetEntityTypes()
            .SelectMany(t => t.GetIndexes())
            .Where(i => i.IsUnique && i.GetDatabaseName() != null)
            .ToDictionary(
                i => i.GetDatabaseName()!,
                // Если индекс составной
                i => i.Properties.Select(p => p.Name).ToArray());
    }

    public bool TryGetFields(string constraintName, out string[] fields)
    {
        return _constraints.TryGetValue(constraintName, out fields!);
    }
}