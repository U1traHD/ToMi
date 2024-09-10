using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToMi.ViewModel;

public class RadioButtonViewModel : INotifyPropertyChanged
{
    private string _selectedRole;
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

    public event EventHandler<string> SelectedRoleChanged;
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}