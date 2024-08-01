using ToMi.Pages;
using System.Threading.Tasks;
using ToMi.ViewModel;
using ToMi.Model;

using Microsoft.Maui.Storage;

namespace ToMi
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Loading(); // показываем страницу загрузки

            Task.Delay(5000).ContinueWith(async t =>
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    var autorizationModel = new AutorizationModel(new AutorizationDbContext());
                    var autorizationVM = new AutorizationVM(new AutorizationDbContext());

                    Application.Current.MainPage = new Autorization { BindingContext = autorizationVM }; // показываем страницу авторизации
                });
            });
        }
    }
}