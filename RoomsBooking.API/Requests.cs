namespace RoomsBooking.API;

// Authentication
public record RegisterRequest(string Email, string Password, string Name);
public record LoginRequest(string Email, string Password);

// Rooms
public record CreateRoomRequest(string Number, string? Description, short Capacity, short Floor);