using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToMi.Pages;
using ToMi.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ToMi.ViewModel
{
    internal class RegistrationVM : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private string _address;
        private string _phoneNumber;
        private string _password;
        private string _repeatPassword;
        private bool _isEnabled;

        public ObservableCollection<string> Names { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> LastNames { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> MiddleNames { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Addresses { get; set; } = new ObservableCollection<string>();

        public string SelectedName { get; set; }
        public string SelectedLastName { get; set; }
        public string SelectedMiddleName { get; set; }
        public string SelectedAddress { get; set; }

        public string FirstName
        {
            get => _firstName;
            set => SetValue(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetValue(ref _lastName, value);
        }

        public string MiddleName
        {
            get => _middleName;
            set => SetValue(ref _middleName, value);
        }

        public string Address
        {
            get => _address;
            set => SetValue(ref _address, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetValue(ref _phoneNumber, value);
        }

        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }

        public string RepeatPassword
        {
            get => _repeatPassword;
            set => SetValue(ref _repeatPassword, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToAutorization { get; }

        public RegistrationVM()
        {
            RegisterCommand = new Command(RegisterUser);
            NavigateToAutorization = new Command(NavigateCommand);
            IsEnabled = false;
        }

        private void RegisterUser()
        {
            if (!ValidatePasswords())
            {
                return;
            }

            var user = CreateUser();
            SaveUser(user);
            ShowSuccessMessage();
        }

        private bool ValidatePasswords()
        {
            if (Password != RepeatPassword)
            {
                Application.Current.MainPage.DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
                return false;
            }
            return true;
        }

        private User CreateUser()
        {
            return new User
            {
                name = FirstName,
                firstname = LastName,
                pantromimic = MiddleName,
                address = Address,
                phone_number = PhoneNumber,
                password = Password
            };
        }

        private void SaveUser(User user)
        {
            using (var dbContext = new AutorizationDbContext())
            {
                var existingUser = dbContext.users
                 .FirstOrDefault(u => u.phone_number == user.phone_number);

                if (existingUser != null)
                {
                    _errorShown = true;
                    Application.Current.MainPage.DisplayAlert("Ошибка", "Пользователь с таким номером телефона уже зарегистрирован", "ОК");
                    return;
                }

                dbContext.users.Add(user);
                dbContext.SaveChanges();
            }
        }

        private async void ShowSuccessMessage()
        {
            if (!_errorShown)
            {
                await Application.Current.MainPage.DisplayAlert("Успешно", "Пользователь зарегистрирован", "ОК");
                await Task.Delay(800);
                Application.Current.MainPage = new Loading();
                await Task.Delay(1500);
                Application.Current.MainPage = new Autorization();
            }
            _errorShown = false;
        }

        private bool _errorShown = false;

        private void NavigateCommand()
        {
            Application.Current.MainPage = new Autorization();
        }

        private void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }
            field = value;
            OnPropertyChanged(propertyName);
            if (propertyName != nameof(IsEnabled))
            {
                CheckFields();
            }
        }

        private void CheckFields()
        {
            bool isEnabled = !string.IsNullOrEmpty(FirstName) &&
                          !string.IsNullOrEmpty(LastName) &&
                          !string.IsNullOrEmpty(MiddleName) &&
                          !string.IsNullOrEmpty(Address) &&
                          !string.IsNullOrEmpty(PhoneNumber) &&
                          !string.IsNullOrEmpty(Password) &&
                          !string.IsNullOrEmpty(RepeatPassword);
            if (isEnabled != IsEnabled)
            {
                IsEnabled = isEnabled;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}