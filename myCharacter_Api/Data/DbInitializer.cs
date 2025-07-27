using Microsoft.AspNetCore.Identity;
using myCharacter.Models;

namespace myCharacter.Data
{
    public class DbInitializer

    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            string[] roles = { "Admin", "Player" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminList = await _userManager.GetUsersInRoleAsync("Admin");
            if (adminList.Count == 0)
            {
                var admin = new ApplicationUser
                {
                    UserName = _configuration["DefaultAdmin:Username"],
                    Email = _configuration["DefaultAdmin:Email"],
                    EmailConfirmed = true
                };

                var adminPassword = _configuration["DefaultAdmin:Password"];
                if (string.IsNullOrEmpty(adminPassword))
                    throw new Exception("DefaultAdmin:Password not found in configuration.");

                var result = await _userManager.CreateAsync(admin, adminPassword);

                if (!result.Succeeded)
                    throw new Exception("Failed to create admin user");

                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}