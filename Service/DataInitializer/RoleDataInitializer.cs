using System.Security.Claims;
using Data.Contracts;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services.DataInitializer;

public class RoleDataInitializer(IRepository<Role> repository, RoleManager<Role> roleManager): IDataInitializer
{
    public async Task InitializerData()
    {
        var role = await roleManager.Roles.Where(i => i.Name == "Admin").FirstOrDefaultAsync();
        if (role is null)
        {
            role = new Role{ Name = "Admin", Description = "مدیرسیستم"};
            await roleManager.CreateAsync(role);
        }
        
    }
    
}