using CD_Vault.Models;

namespace CD_Vault.Pages;

public partial class AlbumDetailsPage : ContentPage
{
	public AlbumDetailsPage(CdItem item)
	{
		InitializeComponent();
		BindingContext = item;
	}
}