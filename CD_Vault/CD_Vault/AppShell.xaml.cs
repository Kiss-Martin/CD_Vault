using CD_Vault.Pages;

namespace CD_Vault
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(CollectionPage), typeof(CollectionPage));
            Routing.RegisterRoute(nameof(DiscoverPage), typeof(DiscoverPage));
        }
    }
}
