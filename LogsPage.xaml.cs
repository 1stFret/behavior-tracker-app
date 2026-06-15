using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using BehaviorTracker.Models;
using BehaviorTracker.Services;
using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls.Shapes;

namespace BehaviorTracker
{
    public partial class LogsPage : ContentPage
    {
        private DatabaseService? _database;
        private List<BehaviorEntry> _allEntries = new();

        public LogsPage(DatabaseService database)
        {
            InitializeComponent();
            _database = database;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadLogs();
        }

        private async Task LoadLogs()
        {
            _allEntries = await _database.GetAllEntriesAsync();

            // Populate child filter
            var childNames = _allEntries
                .Select(e => e.ChildName)
                .Distinct()
                .ToList();

            ChildFilter.Items.Clear();
            ChildFilter.Items.Add("All");
            foreach (var name in childNames)
                ChildFilter.Items.Add(name);

            // Populate behavior filter
            var behaviorNames = _allEntries
                .Select(e => e.BehaviorType)
                .Distinct()
                .ToList();

            BehaviorFilter.Items.Clear();
            BehaviorFilter.Items.Add("All");
            foreach (var name in behaviorNames)
                BehaviorFilter.Items.Add(name);

            ApplyFilters();
        }

        private void OnFilterChanged(object? sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allEntries.AsEnumerable();

            // Child filter
            if (ChildFilter.SelectedIndex > 0)
                filtered = filtered.Where(e => e.ChildName == ChildFilter.SelectedItem.ToString());

            // Polarity filter
            if (PolarityFilter.SelectedIndex == 1)
                filtered = filtered.Where(e => e.IsPositive);
            else if (PolarityFilter.SelectedIndex == 2)
                filtered = filtered.Where(e => !e.IsPositive);

            // Behavior filter
            if (BehaviorFilter.SelectedIndex > 0)
                filtered = filtered.Where(e => e.BehaviorType == BehaviorFilter.SelectedItem.ToString());

            RenderLogs(filtered.ToList());
        }

        private void RenderLogs(List<BehaviorEntry> entries)
        {
            LogsList.Children.Clear();

            if (entries.Count == 0)
            {
                LogsList.Children.Add(new Label
                {
                    Text = "No entries found.",
                    TextColor = Colors.Gray,
                    FontSize = 14,
                    Margin = new Thickness(0, 20)
                });
                return;
            }

            foreach (var entry in entries)
            {
                var border = new Border
                {
                    BackgroundColor = entry.IsPositive ? Color.FromArgb("#DCFCE7") : Color.FromArgb("#FEE2E2"),
                    Stroke = entry.IsPositive ? Color.FromArgb("#15803D") : Color.FromArgb("#B91C1C"),
                    StrokeThickness = 1,
                    StrokeShape = new RoundRectangle { CornerRadius = 12 },
                    Padding = new Thickness(14, 10)
                };

                var stack = new VerticalStackLayout { Spacing = 4 };

                stack.Children.Add(new Label
                {
                    Text = entry.BehaviorType,
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = entry.IsPositive ? Color.FromArgb("#15803D") : Color.FromArgb("#B91C1C")
                });

                stack.Children.Add(new Label
                {
                    Text = $"{entry.ChildName} · {entry.LoggedAt:MMM dd hh:mm tt}",
                    FontSize = 12,
                    TextColor = Colors.Gray
                });

                border.Content = stack;

                var longPressCommand = new Command(async () =>
                {
                    string action = await DisplayActionSheetAsync(
                        $"{entry.BehaviorType}",
                        "Cancel",
                        null,
                        "View/Edit Notes",
                        "Delete");

                    if (action == "Delete")
                    {
                        bool confirm = await DisplayAlertAsync(
                            "Delete Entry",
                            $"Delete this {entry.BehaviorType} entry?",
                            "Delete",
                            "Cancel");

                        if (confirm)
                        {
                            await _database.DeleteBehaviorEntryAsync(entry);
                            await LoadLogs();
                        }
                    }
                    else if (action == "Edit Notes")
                    {
                        string? newNotes = await DisplayPromptAsync(
                            "Edit Notes",
                            "Update the notes for this entry",
                            "Save",
                            "Cancel",
                            placeholder: "What was happening?",
                            initialValue: entry.Notes ?? "");

                        if (newNotes == null)
                            return;

                        entry.Notes = newNotes;
                        await _database.UpdateBehaviorEntryAsync(entry);
                        await LoadLogs();
                    }
                });

                TouchBehavior touchBehavior = new TouchBehavior();
                touchBehavior.LongPressCommand = longPressCommand;
                border.Behaviors.Add(touchBehavior);

                LogsList.Children.Add(border);
            }
        }
    }
}