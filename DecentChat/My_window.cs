using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecentChat
{
    public class MyWindow : Window
    {
        public MyWindow(Page page) : base(page)
        {
        }

        protected override void OnCreated()
        {
            base.OnCreated();
            // Code to run when the window is created
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Code to run when the window is activated
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            // Code to run when the window is deactivated
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            // Code to run when the window is no longer visible
        }

        protected override void OnDestroying()
        {
            base.OnDestroying();
            var _node = ((App)Application.Current).ServiceProvider.GetService<Communication_node>();
            if (_node != null)
            _node.Dispose();
            // Code to run before the window is destroyed
        }
    }

}
