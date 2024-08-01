using ToMi.ViewModel;

namespace ToMi.Pages;

public partial class Autorization : ContentPage
{
	public Autorization()
	{
		InitializeComponent();
    }

    private void Password_Entry_Complete(object sender, EventArgs e)
    {
        (BindingContext as AutorizationVM).NavigateCommand.Execute(null);
    }
}