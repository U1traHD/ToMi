using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ToMi.ViewModel
{
    public class ProfileVM : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetValue(ref _phoneNumber, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetValue(ref _address, value);
        }

        public ICommand EditProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileVM()
        {
            EditProfileCommand = new Command(OnEditProfile);
            LogoutCommand = new Command(OnLogout);
        }

        private void OnEditProfile()
        {
            // Логика редактирования профиля
        }

        private void OnLogout()
        {
            // Логика выхода из аккаунта
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
