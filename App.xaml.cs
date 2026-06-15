using Microsoft.Extensions.DependencyInjection;

namespace BehaviorTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new AppShell()));
        }
    }
}