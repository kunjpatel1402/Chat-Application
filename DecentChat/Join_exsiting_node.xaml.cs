namespace DecentChat;

public partial class Join_exsiting_node : ContentPage
{
    private static Communication_node _node;
	public Join_exsiting_node()
	{
		InitializeComponent();
        _node = ((App)Application.Current).ServiceProvider.GetService<Communication_node>();
        this.BindingContext = _node;
	}
    public async void submit_ip_address_and_port_info(object sender, EventArgs e)
    {
        Node temp_node = new Node("", -1, ip_address_text.Text, int.Parse(port_text.Text));
        if (_node.join(temp_node))
        {
            await Shell.Current.GoToAsync($"//{nameof(my_Start_node)}");
        }
    }
}

