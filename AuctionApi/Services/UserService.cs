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
    void RegisterSeller(RegisterRequest model, string rootPath);
    IEnumerable<User> GetAllUsers(); // NEW
    User GetUserById(int id); // NEW
    void UpdateUser(int id, UpdateUserRequest model, string rootPath); // NEW
    void DeleteUser(int id); // NEW
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
        user.Role = "Admin";

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

    public void RegisterSeller(RegisterRequest model, string rootPath)
    {
        if (_context.Users.Any(x => x.Email == model.Email))
            throw new AppException("Email '" + model.Email + "' is already taken");

        if (model.Password != model.ConfirmPassword)
            throw new AppException("Passwords do not match");

        var user = _mapper.Map<User>(model);
        user.Role = "Seller";

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

    // NEW: Get all users
    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    // NEW: Get user by ID
    public User GetUserById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new AppException("User not found");
        return user;
    }

    // NEW: Update user
    public void UpdateUser(int id, UpdateUserRequest model, string rootPath)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new AppException("User not found");

        if (model.Email != user.Email && _context.Users.Any(x => x.Email == model.Email))
            throw new AppException("Email '" + model.Email + "' is already taken");

        if (!string.IsNullOrEmpty(model.Password) && model.Password != model.ConfirmPassword)
            throw new AppException("Passwords do not match");

        user.Name = model.Name ?? user.Name;
        user.Email = model.Email ?? user.Email;
        if (!string.IsNullOrEmpty(model.Password))
            user.Password = model.Password;
        user.Role = model.Role ?? user.Role;

        if (model.ProfilePicture != null)
        {
            var uniqueFileName = GetUniqueFileName(model.ProfilePicture.FileName);
            var uploadsFolder = Path.Combine(rootPath, "images", "profiles");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            model.ProfilePicture.CopyTo(new FileStream(filePath, FileMode.Create));
            user.ProfilePicture = "/images/profiles/" + uniqueFileName;
        }

        _context.Users.Update(user);
        _context.SaveChanges();
    }

    // NEW: Delete user
    public void DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new AppException("User not found");

        _context.Users.Remove(user);
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