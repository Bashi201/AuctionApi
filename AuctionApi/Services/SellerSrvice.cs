using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Sellers;
using AuctionApi.Models.Users;

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Services;

public interface ISellerService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
 
    void RegisterSeller(SellerRegisterRequestAsSeller model, string rootPath);
    IEnumerable<Seller> GetAllSeller();
    Seller GetSellerById(int id);
    void UpdateSeller(int id, UpdateSellerRequest model, string rootPath);
    void DeleteSeller(int id);
}

public class SellerService : ISellerService
{
    private readonly DataContext _context;
    private readonly IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public SellerService(DataContext context, IJwtUtils jwtUtils, IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var seller = _context.Users
            .SingleOrDefault(x => x.Email == model.Email && x.Password == model.Password);
        if (seller == null) throw new AppException("Email or password is incorrect");

        var response = _mapper.Map<AuthenticateResponse>(seller);
        response.Token = _jwtUtils.GenerateToken(seller);
        return response;
    }

   

    public void RegisterSeller(SellerRegisterRequestAsSeller model, string rootPath)
    {
        CreateSeller(model, rootPath);
    }

    private void CreateSeller(SellerRegisterRequestAsSeller model, string rootPath)
    {
        if (_context.Seller.Any(x => x.Email == model.Email))
            throw new AppException($"Email '{model.Email}' is already taken");

        //if (model.Password != model.ConfirmPassword)
        //    throw new AppException("Passwords do not match");

        var seller = _mapper.Map<Seller>(model);

        if (model.ProfilePicture != null)
        {
            seller.ProfilePicture = SaveProfilePicture(model.ProfilePicture, rootPath);
        }

        _context.Seller.Add(seller);
        _context.SaveChanges();
    }

    public IEnumerable<Seller> GetAllSeller() => _context.Seller.ToList();

    public Seller GetSellerById(int id)
    {
        var seller = _context.Seller.Find(id);
        if (seller == null) throw new AppException("Seller not found");
        return seller;
    }

    public void UpdateSeller(int id, UpdateSellerRequest model, string rootPath)
    {
        var seller = _context.Seller.Find(id);
        if (seller == null) throw new AppException("Seller not found");

        if (!string.Equals(model.Email, seller.Email, StringComparison.OrdinalIgnoreCase) &&
            _context.Seller.Any(x => x.Email == model.Email))
            throw new AppException($"Email '{model.Email}' is already taken");

        if (!string.IsNullOrEmpty(model.Password) && model.Password != model.ConfirmPassword)
            throw new AppException("Passwords do not match");

        seller.Name = model.Name ?? seller.Name;
        seller.Email = model.Email ?? seller.Email;
        if (!string.IsNullOrEmpty(model.Password))
            seller.Password = model.Password;

        if (model.ProfilePicture != null)
        {
            seller.ProfilePicture = SaveProfilePicture(model.ProfilePicture, rootPath);
        }

        _context.Seller.Update(seller);
        _context.SaveChanges();
    }

    public void DeleteSeller(int id)
    {
        var seller = _context.Seller.Find(id);
        if (seller == null) throw new AppException("Seller not found");

        _context.Seller.Remove(seller);
        _context.SaveChanges();
    }

    private string SaveProfilePicture(IFormFile file, string rootPath)
    {
        var uniqueFileName = Path.GetFileNameWithoutExtension(file.FileName)
                             + "_" + Guid.NewGuid().ToString()[..4]
                             + Path.GetExtension(file.FileName);

        var uploadsFolder = Path.Combine(rootPath, "images/profiles");
        Directory.CreateDirectory(uploadsFolder);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);

        return "/images/profiles/" + uniqueFileName;
    }
}
