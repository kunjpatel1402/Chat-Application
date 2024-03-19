using Microsoft.Maui.Controls;
using System;

namespace DecentChat;

public partial class my_Start_node : ContentPage
{
    private static Communication_node _node;

    public my_Start_node()
    {
        InitializeComponent();
        _node = ((App)Application.Current).ServiceProvider.GetService<Communication_node>();
        this.BindingContext = _node;
    }
    public async void Start_node_func(object sender, EventArgs e)
    {
        _node.start();
        await Shell.Current.GoToAsync($"//{nameof(Chats)}");
    }
}
