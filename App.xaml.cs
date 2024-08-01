using ToMi.Pages;
using System.Threading.Tasks;

namespace ToMi
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Loading());

            Task.Delay(5000).ContinueWith(async t =>
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    MainPage = new Autorization();
                });
            });
        }
    }
}
