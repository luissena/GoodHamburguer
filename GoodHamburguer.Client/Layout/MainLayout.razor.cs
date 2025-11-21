namespace GoodHamburguer.Client.Layout;

public partial class MainLayout
{
    private bool IsMenuOpen { get; set; } = false;

    private void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    private void CloseMenu()
    {
        IsMenuOpen = false;
    }
}

