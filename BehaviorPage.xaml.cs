using BehaviorTracker.Models;
using BehaviorTracker.Services;
using CommunityToolkit.Maui.Behaviors;

namespace BehaviorTracker
{
    public partial class BehaviorPage : ContentPage
    {
        private DatabaseService _database;
        private Child _child;

        public BehaviorPage(DatabaseService database, Child child)
        {
            InitializeComponent();
            _database = database;
            _child = child;
            ChildNameLabel.Text = _child.Name;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCustomBehaviors();
        }

        private async void OnBehaviorClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            string behaviorName = button.Text;
            bool isPositive = false;

            if (button.CommandParameter is string paramStr)
                isPositive = paramStr == "True";
            else if (button.CommandParameter is bool paramBool)
                isPositive = paramBool;

            string? notes = await DisplayPromptAsync(
                behaviorName,
                "Add a note (optional)",
                "Log It",
                "Cancel",
                placeholder: "What was happening?");

            if (notes == null)
                return;

            var entry = new BehaviorEntry
            {
                ChildId = _child.Id,
                ChildName = _child.Name,
                BehaviorType = behaviorName,
                IsPositive = isPositive,
                Notes = notes,
                LoggedAt = DateTime.Now
            };

            await _database.SaveBehaviorEntryAsync(entry);
            await DisplayAlertAsync("Logged!", $"{behaviorName} recorded for {_child.Name} on {DateTime.Now:MMM dd} at {DateTime.Now:hh:mm tt}", "OK");
        }

        private async void OnAddBehaviorClicked(object? sender, EventArgs e)
        {
            string? name = await DisplayPromptAsync(
                "Add Behavior",
                "What is the behavior called?",
                "Next",
                "Cancel",
                placeholder: "Behavior name");

            if (string.IsNullOrWhiteSpace(name))
                return;

            bool isPositive = await DisplayAlertAsync(
                "Behavior Type",
                $"Is '{name}' a positive behavior?",
                "Yes, Positive",
                "No, Negative");

            var behaviorType = new BehaviorType
            {
                Name = name,
                IsPositive = isPositive,
                ChildId = _child.Id
            };

            await _database.SaveBehaviorTypeAsync(behaviorType);
            await LoadCustomBehaviors();
        }

        private async Task LoadCustomBehaviors()
        {
            CustomBehaviorList.Children.Clear();

            var behaviors = await _database.GetBehaviorTypesForChildAsync(_child.Id);

            foreach (var behavior in behaviors)
            {
                var button = new Button
                {
                    Text = behavior.Name,
                    BackgroundColor = behavior.IsPositive ? Color.FromArgb("#DCFCE7") : Color.FromArgb("#FEE2E2"),
                    TextColor = behavior.IsPositive ? Color.FromArgb("#15803D") : Color.FromArgb("#B91C1C"),
                    CornerRadius = 16,
                    HeightRequest = 60,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    CommandParameter = behavior.IsPositive
                };

                button.Clicked += (s, args) => OnBehaviorClicked(button, args);

                var longPressCommand = new Command(async () =>
                {
                    bool confirm = await DisplayAlertAsync(
                        "Delete Behavior",
                        $"Remove '{behavior.Name}'?",
                        "Delete",
                        "Cancel");

                    if (confirm)
                    {
                        await _database.DeleteBehaviorTypeAsync(behavior);
                        await LoadCustomBehaviors();
                    }
                });

                TouchBehavior touchBehavior = new TouchBehavior();
                touchBehavior.LongPressCommand = longPressCommand;
                button.Behaviors.Add(touchBehavior);

                CustomBehaviorList.Children.Add(button);
            }
        }
    }
}