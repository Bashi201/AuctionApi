using AuctionApi.Entities;
using AuctionApi.Models.Auctions;
using AuctionApi.Models.Products;
using AuctionApi.Models.Users;
using AutoMapper;

namespace AuctionApi.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<RegisterRequest, User>();
        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<User, AuthenticateResponse>();
        CreateMap<CreateAuctionRequest, Auction>();
        CreateMap<CreateProductRequest, Product>();
    }
}