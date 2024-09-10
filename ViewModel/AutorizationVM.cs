using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToMi.Model;
using ToMi.Pages;
using System.Threading.Tasks;

namespace ToMi.ViewModel
{
    internal class AutorizationVM : INotifyPropertyChanged
    {
        private const int DelayMilliseconds = 2000;

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetValue(ref _phoneNumber, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }

        public ICommand NavigateCommand { get; }
        public ICommand NavigateToRegistr { get; }
        public ICommand ForgotPasswordCommand { get; }

        private readonly AutorizationDbContext _dbContext;
        private readonly AutorizationModel _autorizationModel;

        public AutorizationVM(AutorizationDbContext dbContext)
        {
            _dbContext = dbContext;
            _autorizationModel = new AutorizationModel(_dbContext);
            NavigateCommand = new Command(async () => await OnNavigateAsync());
            NavigateToRegistr = new Command(async () => await ToRegistrationAsync());
            ForgotPasswordCommand = new Command(async () => await ForgotPasswordAsync());
        }

        private async Task OnNavigateAsync()
        {
            await ShowLoadingPageAsync();
            if (await ValidateCredentialsAsync())
            {
                await NavigateToProfilePageAsync();
            }
            else
            {
                await ShowErrorAlertAsync();
            }
        }

        private async Task ShowLoadingPageAsync()
        {
            Application.Current.MainPage = new Loading();
            await Task.Delay(DelayMilliseconds);
        }

        private async Task<bool> ValidateCredentialsAsync()
        {
            var user = await _autorizationModel.GetUserByPhoneNumberAsync(PhoneNumber);
            return user?.password == Password;
        }

        private async Task NavigateToProfilePageAsync()
        {
            try
            {
                if (!_dbContext.Database.CanConnect())
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Нет соединения с базой данных.", "Ок");
                    return;
                }

                var user = await _autorizationModel.GetUserByPhoneNumberAsync(PhoneNumber);
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Пользователь не найден.", "Ок");
                    return;
                }

                var profileVM = new ProfileVM
                {
                    Name = user.name,
                    PhoneNumber = user.phone_number,
                    Address = user.address
                };

                Application.Current.MainPage = new MainPage { BindingContext = profileVM };
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"{ex.Message}", "Ок");
            }
        }

        private async Task ShowErrorAlertAsync()
        {
            await ShowAlertAsync("Ошибка", "Номер телефона или пароль не верны", "ОК");
            Application.Current.MainPage = new Autorization { BindingContext = this };
        }

        private async Task ShowAlertAsync(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        private async Task ToRegistrationAsync()
        {
            Application.Current.MainPage = new Registration();
        }

        private async Task ForgotPasswordAsync()
        {
            Application.Current.MainPage = new FogetPassword();
        }

        private void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }
            field = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}