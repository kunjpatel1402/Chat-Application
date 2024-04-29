using CommunityToolkit.Maui.Views;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace DecentChat;

public partial class Message_popup : Popup
{
	public Message_popup(string message)
	{
		InitializeComponent();
		this.BindingContext = message;
	}
	public void HandleOKButtonClicked(object sender, EventArgs e)
	{
		this.Close();
	}
}