using AuctionApi.Entities;
using AuctionApi.Helpers;
using AuctionApi.Models.Users;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuctionApi.Services;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    void Register(RegisterRequest model, string rootPath);
    void RegisterSeller(RegisterRequest model, string rootPath); // NEW: For sellers
}

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public UserService(DataContext context, IJwtUtils jwtUtils, IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.Email == model.Email && x.Password == model.Password);
        if (user == null) throw new AppException("Email or password is incorrect");

        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);
        return response;
    }

    public void Register(RegisterRequest model, string rootPath)
    {
        if (_context.Users.Any(x => x.Email == model.Email))
            throw new AppException("Email '" + model.Email + "' is already taken");

        if (model.Password != model.ConfirmPassword)
            throw new AppException("Passwords do not match");

        var user = _mapper.Map<User>(model);
        user.Role = "Admin"; // Explicit for clarity, though default

        // Handle profile picture upload
        if (model.ProfilePicture != null)
        {
            var uniqueFileName = GetUniqueFileName(model.ProfilePicture.FileName);
            var uploadsFolder = Path.Combine(rootPath, "images", "profiles");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            model.ProfilePicture.CopyTo(new FileStream(filePath, FileMode.Create));
            user.ProfilePicture = "/images/profiles/" + uniqueFileName;
        }

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    // NEW: Seller registration method
    public void RegisterSeller(RegisterRequest model, string rootPath)
    {
        if (_context.Users.Any(x => x.Email == model.Email))
            throw new AppException("Email '" + model.Email + "' is already taken");

        if (model.Password != model.ConfirmPassword)
            throw new AppException("Passwords do not match");

        var user = _mapper.Map<User>(model);
        user.Role = "Seller"; // Set role to Seller

        // Handle profile picture upload (same as admin)
        if (model.ProfilePicture != null)
        {
            var uniqueFileName = GetUniqueFileName(model.ProfilePicture.FileName);
            var uploadsFolder = Path.Combine(rootPath, "images", "profiles");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            model.ProfilePicture.CopyTo(new FileStream(filePath, FileMode.Create));
            user.ProfilePicture = "/images/profiles/" + uniqueFileName;
        }

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    private string GetUniqueFileName(string fileName)
    {
        fileName = Path.GetFileName(fileName);
        return Path.GetFileNameWithoutExtension(fileName)
            + "_" + Guid.NewGuid().ToString()[..4]
            + Path.GetExtension(fileName);
    }
}