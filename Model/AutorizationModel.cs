using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToMi.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace ToMi.Model
{
    internal class AutorizationModel
    {
        private readonly AutorizationDbContext _dbContext;

        public AutorizationModel(AutorizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckCredentialsAsync(string phoneNumber, string password)
        {
            return await _dbContext.users.AnyAsync(u => u.phone_number == phoneNumber && u.password == password);
        }
        public async Task<ILookup<string, string>> GetUsersAsync()
        {
            var users = await _dbContext.users.ToListAsync();
            return users.ToLookup(u => u.phone_number, u => u.password);
        }
    }
}