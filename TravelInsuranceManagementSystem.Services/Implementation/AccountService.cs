using Microsoft.AspNetCore.Identity;

using TravelInsuranceManagementSystem.Repo.Models;

using TravelInsuranceManagementSystem.Services.Interfaces;

namespace TravelInsuranceManagementSystem.Services.Implementation

{

    public class AccountService : IAccountService

    {

        private readonly UserManager<User> _userManager;// Manages user data (create, delete, find)

        private readonly SignInManager<User> _signInManager;// Manages login (password check, cookies)

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)

        {

            _userManager = userManager;

            _signInManager = signInManager;

        }

        public async Task<SignInResult> Authenticate(string email, string password)

        {

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

        }

        public async Task<IdentityResult> RegisterUser(User user, string password)

        {           

            user.Role = "User";

            user.UserName = user.Email; // Identity requires UserName

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)

            {

                await _userManager.AddToRoleAsync(user, "User");

            }

            return result;

        }

        // New Method for Admin Dashboard

        // Inside AccountService.cs
        public async Task<IdentityResult> CreateAgent(User user, string password)
        {
            user.Role = "Agent";
            user.UserName = user.Email; // Identity requires UserName to be set
            user.EmailConfirmed = true; // Auto-confirm for agents created by Admin

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Agent");
            }
            return result;
        }

        public async Task<User> GetUserByEmail(string email) => await _userManager.FindByEmailAsync(email);

        public async Task<string> GeneratePasswordResetToken(User user) => await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityResult> ResetPassword(User user, string token, string newPassword)

        {

            return await _userManager.ResetPasswordAsync(user, token, newPassword);

        }

    }

}
