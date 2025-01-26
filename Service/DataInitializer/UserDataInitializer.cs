using Common.Utilities;
using Data.Contracts;
using Entity;
using Microsoft.AspNetCore.Identity;

namespace Services.DataInitializer;

public class UserDataInitializer: IDataInitializer
{
    private readonly IUserRepository _userRepository;

    public UserDataInitializer(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public void InitializerData()
    {
        if (!_userRepository.TableNoTracking.Any(u => u.UserName == "admin"))
        {
            var passwordHasher = new PasswordHasher<User>();
            
            var user = new User
            {
                UserName = "admin",
                PhoneNumber = "09140758738",
                Email = "admin@gmail.com",
                NormalizedUserName = "ADMIN",
                FullName = "yousof",
                Gender = GenderType.Male,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            user.PasswordHash = passwordHasher.HashPassword(user, "1234");
            _userRepository.Add(user);
        }
    }
}