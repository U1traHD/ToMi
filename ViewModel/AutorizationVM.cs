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

        private readonly AutorizationModel _autorizationModel;

        public AutorizationVM(AutorizationDbContext dbContext)
        {
            _autorizationModel = new AutorizationModel(dbContext);
            NavigateCommand = new Command(OnNavigate);
            NavigateToRegistr = new Command(ToRegistration);
            ForgotPasswordCommand = new Command(ForgotPassword);
        }

        private async void OnNavigate()
        {
            Application.Current.MainPage = new Loading();

            await Task.Delay(2000);

            if (await ValidateCredentialsAsync())
            {
                await NavigateToProfilePage();
            }
            else
            {
                ShowErrorAlert();
            }
        }

        private async Task<bool> ValidateCredentialsAsync()
        {
            if (string.IsNullOrEmpty(Password))
            {
                return false;
            }

            var user = await _autorizationModel.GetUserByPhoneNumberAsync(PhoneNumber);
            return user != null && user.password == Password;
        }

        private async Task NavigateToProfilePage()
        {
            var user = await _autorizationModel.GetUserByPhoneNumberAsync(PhoneNumber);
            var profileVM = new ProfileVM
            {
                Name = user.name,
                PhoneNumber = user.phone_number,
                Address = user.address
            };

            Application.Current.MainPage = new MainPage { BindingContext = profileVM };
        }

        private void ShowErrorAlert()
        {
            Application.Current.MainPage.DisplayAlert("Ошибка", "Номер телефона или пароль не верны", "ОК");
            var autorizationPage = new Autorization();
            autorizationPage.BindingContext = this; // Используем существующий экземпляр AutorizationVM
            Application.Current.MainPage = autorizationPage;
        }

        private void ToRegistration()
        {
            Application.Current.MainPage = new Registration();
        }

        private void ForgotPassword()
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