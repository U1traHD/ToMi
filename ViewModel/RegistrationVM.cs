using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using ToMi.Pages;
using ToMi.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

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
        private string _selectedRole;
        private bool _isClientCheked;
        private bool _isEmployeeChecked;

        public ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string>
        {
            "Клиент",
            "Сотрудник"
        };

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(RepeatPassword) && !string.IsNullOrEmpty(SelectedRole);
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(RepeatPassword) && !string.IsNullOrEmpty(SelectedRole);
            }
        }

        public string MiddleName
        {
            get => _middleName;
            set
            {
                _middleName = value;
                OnPropertyChanged();
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(RepeatPassword) && !string.IsNullOrEmpty(SelectedRole);
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(RepeatPassword) && !string.IsNullOrEmpty(SelectedRole);
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(RepeatPassword) && !string.IsNullOrEmpty(SelectedRole);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(RepeatPassword) && !string.IsNullOrEmpty(SelectedRole);
            }
        }

        public string RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                _repeatPassword = value;
                OnPropertyChanged();
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(SelectedRole);
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
                SelectedRoleChanged?.Invoke(this, value);
                IsEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(RepeatPassword);
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public bool IsClientCheked
        {
            get => _isClientCheked;
            set
            {
                _isClientCheked = value;
                OnPropertyChanged();
                IsEmployeeChecked = !value;
            }
        }

        public bool IsEmployeeChecked
        {
            get => _isEmployeeChecked;
            set
            {
                _isEmployeeChecked = value;
                OnPropertyChanged();
                IsClientCheked = !value;
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToAutorization { get; }

        public RegistrationVM()
        {
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            NavigateToAutorization = new Command(async () => await NavigateCommandAsync());
            IsEnabled = false;
        }

        private async Task OnRegisterAsync()
        {
            if (!await ValidatePasswordsAsync())
            {
                return;
            }

            if (await CheckIfUserExistsAsync(PhoneNumber))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Пользователь с таким номером телефона уже зарегистрирован", "ОК");
                return;
            }

            var user = CreateUser();
            await SaveUserAsync(user);
            await ShowSuccessMessageAsync();
        }

        private async Task<bool> CheckIfUserExistsAsync(string phoneNumber)
        {
            using var dbContext = new AutorizationDbContext();
            try
            {
                return await dbContext.users.AnyAsync(u => u.phone_number == phoneNumber);
            }
            catch (Exception ex)
            {
                // Обработайте исключение, например, покажите сообщение об ошибке пользователю
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось проверить existence пользователя: {ex.Message}", "ОК");
                return false;
            }
        }

        private async Task<bool> ValidatePasswordsAsync()
        {
            return Password == RepeatPassword;
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
                password = Password,
                role = string.IsNullOrEmpty(SelectedRole) ? "Клиент" : SelectedRole
            };
        }

        private async Task SaveUserAsync(User user) 
        {
            using var dbContext = new AutorizationDbContext();
            try
            {
                await dbContext.users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Обработайте исключение, например, покажите сообщение об ошибке пользователю
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось сохранить пользователя: {ex.Message}", "ОК");
            }
        }

        private async Task ShowSuccessMessageAsync()
        {
            await Application.Current.MainPage.DisplayAlert("Успешно", "Пользователь зарегистрирован", "ОК");
            await Task.Delay(800);
            Application.Current.MainPage = new Loading();
            await Task.Delay(1500);
            Application.Current.MainPage = new Autorization();
        }

        private async Task NavigateCommandAsync()
        {
            Application.Current.MainPage = new Autorization();
        }

        public event EventHandler<string> SelectedRoleChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}