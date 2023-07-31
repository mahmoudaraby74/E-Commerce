using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
namespace infrastructure.Identity
{
    public class AddIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Abdo",
                    Email = "abdulrahman97314@gmail.com",
                    UserName = "abdulrahman97314@gmail.com",
                    Address =new Address()
                    {
                         FirstName = "abdo",
                         LastName = "kotb",
                         Street = "10 St",
                         City = "Maadi",
                         State = "Eg",
                         ZipCode ="12548"
                    }
                };
                await userManager.CreateAsync(user,"Password123!");
            }
        }
    }
}
