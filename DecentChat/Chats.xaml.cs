using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using System.Threading;
namespace DecentChat;
using MauiApp = Microsoft.Maui.Controls.Application;

public partial class Chats : ContentPage
{
	private Communication_node _node;
	public Chats()
	{
		InitializeComponent();
		_node = ((App)MauiApp.Current).ServiceProvider.GetService<Communication_node>();
		this.BindingContext = _node;
	}
	private void send_button_func(object sender, EventArgs e)
	{
		string message_text = MessageEntry.Text;
		int to = (int)(((Contact)ContactsListView.SelectedItem).hash_val);
        Dictionary<string, Object> task = new Dictionary<string, Object>();
        task["hash_val"] = to;
        task["message"] = message_text;
        task["type"] = "send_message";
        _node.add_task(task);
        MessageEntry.Text = string.Empty;
		//_node.Update_messages();
    }
    private void MoreButton_Clicked(object sender, EventArgs e)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            AddContactPopup.IsVisible = true;
        });

    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        if (menuItem != null)
        {
            var contact = menuItem.CommandParameter as Contact;
            if (contact != null)
            { 
                this.ShowPopup(new Edit_contact(contact));
            }
        }
    }
    private void AddContactButton_Clicked(object sender, EventArgs e)
    {
        // Add your logic to add the contact here
        // You can access the entered name and email with NameEntry.Text and EmailEntry.Text
        string contact_name = Name_entry.Text;
        int hash_val = int.Parse(Hash_val_entry.Text);
        Console.WriteLine(contact_name + " " + hash_val);
        string msg = _node.Add_contact(contact_name, hash_val);
        Device.BeginInvokeOnMainThread(() =>
        {
            AddContactPopup.IsVisible = false;
        });
        //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //ToastDuration duration = ToastDuration.Short; 
        //double fontSize = 14;
        //var toast = Toast.Make(msg, duration, fontSize);
        //toast.Show(cancellationTokenSource.Token);
        //Device.BeginInvokeOnMainThread(() =>
        //{
        //    AddContactPopup.IsVisible = false;
        //});
        //Name_entry.Text = string.Empty;
        //Hash_val_entry.Text = string.Empty;
        this.ShowPopup(new Message_popup(msg));
    }
    //private void OnEntryKeyDown(object sender, KeyEventArgs e)
    //{
    //    if (e.Key == Key.Enter)
    //    {
    //        SendButton.PerformClick();
    //    }
    //}
}