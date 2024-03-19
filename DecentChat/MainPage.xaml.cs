
using System.Net;
namespace DecentChat
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void submit_node_info(object sender, EventArgs e)
        {
            var node = new Communication_node(node_name_text.Text, ip_address_text.Text, int.Parse(port_text.Text), null, null);
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton(node);
            await Navigation.PushAsync(new Chats());

        }
        private async void set_next_node(object sender, EventArgs e)
        {
            var node = new Communication_node(node_name_text.Text, ip_address_text.Text, int.Parse(port_text.Text), null, null);
            Microsoft.Extensions.DependencyInjection.ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(node);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            ((App)Application.Current).ServiceProvider = serviceProvider;
            await Shell.Current.GoToAsync($"//{nameof(Set_next_nodes)}");
        }
        private async void join_exsiting_node(object sender, EventArgs e)
        {
            var node = new Communication_node(node_name_text.Text, ip_address_text.Text, int.Parse(port_text.Text), null, null);
            Microsoft.Extensions.DependencyInjection.ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(node);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            ((App)Application.Current).ServiceProvider = serviceProvider;
            await Shell.Current.GoToAsync($"//{nameof(Join_exsiting_node)}");
        }
    }

}
