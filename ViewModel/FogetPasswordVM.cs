using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ToMi.Pages;
using ToMi.ViewModel.Base;
using Twilio;

namespace ToMi.ViewModel
{
    public class FogetPasswordVM : BaseViewModel
    {
        private string _phoneNumber;
        private string _code;
        private string _newPassword;
        private string _twilioPhoneNumber = "YOUR_TWILIO_PHONE_NUMBER";
        private string _authToken = "YOUR_TWILIO_AUTH_TOKEN";
        private string _accountSid = "YOUR_TWILIO_ACCOUNT_SID";

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        public string Code
        {
            get => _code;
            set
            {
                _code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public ICommand SendCodeCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public FogetPasswordVM()
        {
            //SendCodeCommand = new Command(SendCode);
            ChangePasswordCommand = new Command(ChangePassword);
            BackToAutorizationCommand = new Command(BackToAutorization);
        }

        //private async void SendCode()
        //{
        //    if (string.IsNullOrEmpty(PhoneNumber))
        //    {
        //        await Shell.Current.DisplayAlert("Ошибка", "Введите номер телефона", "OK");
        //        return;
        //    }

        //    var code = GenerateCode();
        //    var message = $"Ваш код: {code}";

        //    try
        //    {
        //        var twilioClient = new TwilioClient(_accountSid);
        //        var messageResource = await twilioClient.SendMessageAsync(
        //            from: _twilioPhoneNumber,
        //            to: PhoneNumber,
        //            body: message);
        //        await Shell.Current.DisplayAlert("Код отправлен", "Введите код", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
        //    }
        //}

        private async void ChangePassword()
        {
            if (string.IsNullOrEmpty(Code))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Введите код", "OK");
                return;
            }

            // Проверяем код
            if (Code == GenerateCode())
            {
                // Разрешаем смену пароля
                await Shell.Current.Navigation.PushAsync(new ToMi.Pages.ChangePassword());
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Неправильный код", "OK");
            }
        }

        private string GenerateCode()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString();
        }

        public ICommand BackToAutorizationCommand { get; }

        private void BackToAutorization()
        {
            Application.Current.MainPage = new Autorization();
        }
    }
}