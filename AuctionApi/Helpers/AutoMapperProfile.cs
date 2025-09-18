using AuctionApi.Entities;
using AuctionApi.Models.Users;
using AutoMapper;

namespace AuctionApi.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<RegisterRequest, User>();
        CreateMap<User, AuthenticateResponse>();
        CreateMap<UpdateUserRequest, User>().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // NEW: Map UpdateUserRequest to User
    }
}