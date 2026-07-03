namespace RoomsBooking.API;


public record RegisterRequest(string Email, string Password, string Name);
public record LoginRequest(string Email, string Password);