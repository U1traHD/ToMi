using ToMi.ViewModel;

namespace ToMi.Pages;

public partial class Registration : ContentPage
{
	public Registration()
	{
		InitializeComponent();
        BindingContext = new RegistrationVM();
	}

    private void Entry_Completed(object sender, EventArgs e)
    {
        (BindingContext as RegistrationVM).RegisterCommand.Execute(null);
    }
}