using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;

namespace DecentChat
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; set; }
        public App()
        {
            InitializeComponent();

            // Get the service provider
            //IServiceProvider serviceProvider = MauiProgram.CreateMauiApp().Services;
            //var cnode = serviceProvider.GetRequiredService<Communication_node>();
            Console.WriteLine("here");
            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            return new MyWindow(MainPage);
        }
    }
}
