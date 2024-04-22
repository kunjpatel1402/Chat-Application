using CommunityToolkit.Maui.Views;
using MauiApp = Microsoft.Maui.Controls.Application;

namespace DecentChat;


public partial class Edit_contact : Popup
{
    private Contact _contact;

    public Edit_contact(Contact contact)
    {
        InitializeComponent();
        this._contact = contact;
        this.BindingContext = _contact;
        New_contact_name.Text = _contact.name;
    }
    public void new_contact_name_submit_func(object sender, EventArgs e)
    {
        // This is where you handle the button click event
        // For example, you might want to retrieve the text from an Entry field and save it as a new contact name

        var newContactName = New_contact_name.Text; // replace 'Name_entry' with the x:Name of your Entry field
        var hash_val = int.Parse(Contact_hash_val.Text);
        Communication_node _node = ((App)MauiApp.Current).ServiceProvider.GetService<Communication_node>();
        int index = _node.contacts.IndexOf(_contact);
        // Assuming 'contact_list' is your DataTable
        _node.update_contact_list(newContactName, hash_val);

        if (index != -1)
        {
            _node.contacts[index] = new Contact(newContactName, _contact.hash_val);
            if (_node.selected_contact.hash_val == hash_val)
            {
                _node.selected_contact = _node.contacts[index];
            }
            _node.OnPropertyChanged(nameof(_node.selected_contact));
            //_node.OnPropertyChanged(nameof(_node.contacts));
        }
        this.Close();
        // Now you can use 'newContactName' to create a new contact
        // Remember to implement the logic for creating a new contact based on your data model and requirements
    }
}
