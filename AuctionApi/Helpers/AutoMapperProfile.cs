using AuctionApi.Entities;
using AuctionApi.Models.Users;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuctionApi.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<RegisterRequest, User>();
        CreateMap<User, AuthenticateResponse>();
    }
}