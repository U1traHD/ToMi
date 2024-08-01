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

        public async Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbContext.users.FirstOrDefaultAsync(u => u.phone_number == phoneNumber);
        }
    }
}