using System;
using Microsoft.EntityFrameworkCore;
using FurnitureMovement.Data;

namespace FurnitureMovement.Services
{
    public class AuthorizationService
    {
        private readonly OrderContext _context;

        public AuthorizationService(OrderContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return (false, "Пользователь не найден!");

            if (user.IsLocked)
                return (false, "Аккаунт заблокирован после 7 неудачных попыток.");

            if (user.PasswordHash == password)
            {
                user.FailedAttempts = 0;
                await _context.SaveChangesAsync();
                return (true, "Успешный вход!");
            }
            else
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 7)
                    user.IsLocked = true;

                await _context.SaveChangesAsync();
                return (false, "Неверный пароль!");
            }
        }
    }
}
