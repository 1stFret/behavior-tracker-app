using BehaviorTracker.Services;
using BehaviorTracker.Models;
using CommunityToolkit.Maui.Behaviors;

namespace BehaviorTracker
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _database;

        public MainPage(DatabaseService database)
        {
            InitializeComponent();
            _database = database;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadChildren();
        }

        private async void OnAddChildClicked(object? sender, EventArgs e)
        {
            string name = await DisplayPromptAsync(
                "Add Child",
                "Enter your child's name",
                "Save",
                "Cancel");

            if (string.IsNullOrWhiteSpace(name))
                return;

            var child = new Child { Name = name };
            await _database.SaveChildAsync(child);
            await LoadChildren();
        }

        private async Task LoadChildren()
        {
            ChildrenList.Children.Clear();

            var children = await _database.GetChildrenAsync();

            System.Diagnostics.Debug.WriteLine($"Found {children.Count} children");

            foreach (var child in children)
            {
                var button = new Button
                {
                    Text = child.Name,
                    BackgroundColor = Color.FromArgb("#E8EDF8"),
                    TextColor = Color.FromArgb("#4A6CF7"),
                    CornerRadius = 20,
                    Padding = new Thickness(20, 10),
                    HorizontalOptions = LayoutOptions.Start,
                    HeightRequest = 75,
                    FontSize = 32,
                    FontAttributes = FontAttributes.Bold
                };

                button.Clicked += (s, args) => OnChildSelected(child);

                var longPressCommand = new Command(async () =>
                {
                    bool confirm = await DisplayAlertAsync(
                        "Delete Child",
                        $"Remove {child.Name}?",
                        "Delete",
                        "Cancel");

                    if (confirm)
                    {
                        await _database.DeleteChildAsync(child);
                        await LoadChildren();
                    }
                });

                TouchBehavior touchBehavior = new TouchBehavior();
                touchBehavior.LongPressCommand = longPressCommand;
                button.Behaviors.Add(touchBehavior);

                ChildrenList.Children.Add(button);
            }
        }

        private async void OnChildSelected(Child child)
        {
            var behaviorPage = new BehaviorPage(_database, child);
            await Navigation.PushAsync(behaviorPage);
        }

        private async void OnViewLogsClicked(object? sender, EventArgs e)
        {
            var logsPage = new LogsPage(_database);
            await Navigation.PushAsync(logsPage);
        }

    }
}