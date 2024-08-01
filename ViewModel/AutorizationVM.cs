using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToMi.Model;
using ToMi.Pages;
using System.Linq;

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

        private readonly AutorizationModel _autorizationModel;
        private ILookup<string, string> _users;

        public AutorizationVM()
        {
            var dbContext = new AutorizationDbContext();
            _autorizationModel = new AutorizationModel(dbContext);

            LoadUsers();

            NavigateCommand = new Command(OnNavigate);
            NavigateToRegistr = new Command(ToRegistration);
        }

        private async void LoadUsers()
        {
            _users = await _autorizationModel.GetUsersAsync();
        }

        private async void OnNavigate()
        {
            if (ValidateCredentials())
            {
                await NavigateToMainPage();
            }
            else
            {
                ShowErrorAlert();
            }
        }

        private bool ValidateCredentials()
        {
            var password = _users[PhoneNumber].FirstOrDefault();
            return password != null && password == Password;
        }

        private async Task NavigateToMainPage()
        {
            Application.Current.MainPage = new Loading();
            await Task.Delay(2000);
            Application.Current.MainPage = new MainPage();
        }

        private void ShowErrorAlert()
        {
            Application.Current.MainPage.DisplayAlert("Ошибка", "Пароль или номер телефона не верны", "ОК");
        }

        private void ToRegistration()
        {
            Application.Current.MainPage = new Registration();
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