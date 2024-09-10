using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToMi.Pages;
using ToMi.Model;
using System.ComponentModel;

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
                CheckFields();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                CheckFields();
            }
        }

        public string MiddleName
        {
            get => _middleName;
            set
            {
                _middleName = value;
                OnPropertyChanged();
                CheckFields();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
                CheckFields();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                CheckFields();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                CheckFields();
            }
        }

        public string RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                _repeatPassword = value;
                OnPropertyChanged();
                CheckFields();
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
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToAutorization { get; }

        public RegistrationVM()
        {
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            NavigateToAutorization = new Command(async () => await NavigateCommandAsync()); 
            IsEnabled = false;
        }

        public RadioButtonViewModel RadioButtonViewModel { get; set; } = new RadioButtonViewModel();

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
            using (var dbContext = new AutorizationDbContext())
            {
                var existingUser = await dbContext.users.FirstOrDefaultAsync(u => u.phone_number == phoneNumber);
                return existingUser != null;
            }
        }

        private async Task<bool> ValidatePasswordsAsync()
        {
            if (Password != RepeatPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Пароли не совпадают", "ОК");
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
                password = Password,
                role = string.IsNullOrEmpty(SelectedRole) ? "Клиент" : SelectedRole
            };
        }

        private async Task SaveUserAsync(User user)
        {
            using (var dbContext = new AutorizationDbContext())
            {
                var existingUser = await dbContext.users
               .FirstOrDefaultAsync(u => u.phone_number == user.phone_number);

                if (existingUser != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Пользователь с таким номером телефона уже зарегистрирован", "ОК");
                    return;
                }

                dbContext.users.Add(user);
                await dbContext.SaveChangesAsync();
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
            await Task.Delay(1); // Добавляем задержку, чтобы страница успела загрузиться
            Application.Current.MainPage = new Autorization();
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }
            field = value;
            OnPropertyChanged(propertyName);
        }

        private void CheckFields()
        {
            IsEnabled = !string.IsNullOrEmpty(FirstName) &&
                       !string.IsNullOrEmpty(LastName) &&
                       !string.IsNullOrEmpty(MiddleName) &&
                       !string.IsNullOrEmpty(Address) &&
                       !string.IsNullOrEmpty(PhoneNumber) &&
                       !string.IsNullOrEmpty(Password) &&
                       !string.IsNullOrEmpty(RepeatPassword) &&
                       !string.IsNullOrEmpty(SelectedRole);
        }
        public event EventHandler<string> SelectedRoleChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}