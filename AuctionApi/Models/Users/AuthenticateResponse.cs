﻿namespace AuctionApi.Models.Users;

public class AuthenticateResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? ProfilePicture { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
}