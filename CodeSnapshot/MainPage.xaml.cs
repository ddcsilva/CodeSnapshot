using System.Diagnostics;
using CodeSnapshot.Resources.Localization;
using CodeSnapshot.Services;
#if WINDOWS
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using WinRT.Interop;
#endif

namespace CodeSnapshot;

public partial class MainPage : ContentPage
{
    private string _projectPath = string.Empty;
    private string _exportedFilePath = string.Empty;
    private string _selectedProjectType = "dotnet";
    private CancellationTokenSource _exportCancellation;

    public MainPage()
    {
        InitializeComponent();

        // Lista de tipos de projeto (nome exibido + chave interna)
        var projectTypes = new List<ProjectTypeOption>
        {
            new() { Name = AppResources.ProjectTypeDotnet, Key = "dotnet" },
            new() { Name = AppResources.ProjectTypeAngular, Key = "angular" }
        };

        ProjectTypePicker.ItemsSource = projectTypes;
        ProjectTypePicker.ItemDisplayBinding = new Binding("Name");
        ProjectTypePicker.SelectedIndex = 0;

        ProjectTypePicker.SelectedIndexChanged += (s, e) =>
        {
            if (ProjectTypePicker.SelectedIndex >= 0)
            {
                var selected = (ProjectTypeOption)ProjectTypePicker.SelectedItem;
                _selectedProjectType = selected.Key;
            }
        };
    }

    private async void OnSelectFolderClicked(object sender, EventArgs e)
    {
#if WINDOWS
        var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

        var picker = new FolderPicker();
        picker.FileTypeFilter.Add("*");
        InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();

        if (folder != null)
        {
            _projectPath = folder.Path;
            SelectedPathLabel.Text = $"{AppResources.PathLabel} {_projectPath}";

            // Add copy path button
            if (!SelectedPathLabel.GestureRecognizers.Any())
            {
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    await Clipboard.SetTextAsync(_projectPath);
                    await DisplayAlert("Success", "Path copied to clipboard", "OK");
                };
                SelectedPathLabel.GestureRecognizers.Add(tapGesture);
            }
        }
#else
        await DisplayAlert("Not supported", "Folder picking is only supported on Windows.", "OK");
#endif
    }

    private async void OnGenerateSnapshotClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_projectPath))
        {
            await DisplayAlert("Error", "Please select a project folder first.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Confirm Export",
            "Are you sure you want to generate a snapshot of this project?",
            "Yes", "No");

        if (!confirm) return;

        ExportProgressBar.Progress = 0;
        ExportProgressBar.IsVisible = true;
        LogEditor.Text = $"{AppResources.ExportStarted}\n";
        SaveLogButton.IsVisible = false;
        OpenFileButton.IsVisible = false;

        _exportCancellation = new CancellationTokenSource();

        try
        {
            var exporter = ProjectExporterFactory.GetExporter(_selectedProjectType);

            _exportedFilePath = await exporter.ExportAsync(
                _projectPath,
                FileSystem.AppDataDirectory,
                progress => MainThread.BeginInvokeOnMainThread(() => ExportProgressBar.Progress = progress),
                message => MainThread.BeginInvokeOnMainThread(() => LogEditor.Text += message + "\n"),
                _exportCancellation.Token
            );

            if (!_exportCancellation.Token.IsCancellationRequested)
            {
                await DisplayAlert("Success", "Snapshot generated successfully!", "OK");
                SaveLogButton.IsVisible = true;
                OpenFileButton.IsVisible = true;
            }
        }
        catch (OperationCanceledException)
        {
            LogEditor.Text += "Export cancelled by user.\n";
        }
        catch (Exception ex)
        {
            LogEditor.Text += $"Error: {ex.Message}\n";
            await DisplayAlert("Error", $"Failed to generate snapshot: {ex.Message}", "OK");
        }
        finally
        {
            ExportProgressBar.IsVisible = false;
        }
    }

    private async void OnSaveLogClicked(object sender, EventArgs e)
    {
        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "export-log.txt");
            await File.WriteAllTextAsync(filePath, LogEditor.Text);
            await DisplayAlert("Success", "Log saved successfully!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save log: {ex.Message}", "OK");
        }
    }

    private async void OnOpenFileClicked(object sender, EventArgs e)
    {
        try
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(_exportedFilePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to open file: {ex.Message}", "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _exportCancellation?.Cancel();
    }
}

public class ProjectTypeOption
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
