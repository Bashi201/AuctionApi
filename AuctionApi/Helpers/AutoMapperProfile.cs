// Updated AutoMapperProfile.cs (adjust mappings to handle nulls)
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
        CreateMap<CreateProductRequest, Product>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Pending"));
        // Note: Images and other fields are handled manually in service
    }
}