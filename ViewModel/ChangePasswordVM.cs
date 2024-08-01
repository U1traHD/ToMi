using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToMi.ViewModel;
using ToMi.ViewModel.Base;

namespace ToMi.ViewModel
{
    public class ChangePasswordVM : BaseViewModel
    {
        private string _newPassword;

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public ICommand SaveCommand { get; set; }

        public ChangePasswordVM()
        {
            SaveCommand = new Command(Save);
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(NewPassword))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Введите новый пароль", "OK");
                return;
            }

            // Сохраняем новый пароль
            //...

            await Shell.Current.Navigation.PopAsync();
        }
    }
}