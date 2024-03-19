namespace DecentChat;

public partial class Set_next_nodes : ContentPage
{
    private static Communication_node _node;
    public Set_next_nodes()
	{
		InitializeComponent();
        _node = ((App)Application.Current).ServiceProvider.GetService<Communication_node>();
        this.BindingContext = _node;
    }
    public async void submit_node_info(object sender, EventArgs e)
    {
        Node temp_node = new Node(node_name_text.Text, int.Parse(hash_val_text.Text), ip_address_text.Text, int.Parse(port_text.Text));
        _node.next_node = temp_node;
        _node.prev_node = temp_node;
        await Shell.Current.GoToAsync($"//{nameof(my_Start_node)}");
    }
}