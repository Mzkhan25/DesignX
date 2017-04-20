#region
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace DesignX.Classes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinishPage : ContentPage
    {
        public FinishPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}