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
    private string _selectedProjectType = "dotnet"; // valor técnico usado internamente

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

    // Botão: Selecionar pasta do projeto
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
        }
#else
        await DisplayAlert("Not supported", "Folder picking is only supported on Windows.", "OK");
#endif
    }

    // Botão: Gerar snapshot
    private async void OnGenerateSnapshotClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_projectPath))
        {
            await DisplayAlert("Error", "Please select a project folder first.", "OK");
            return;
        }

        ExportProgressBar.Progress = 0;
        ExportProgressBar.IsVisible = true;
        LogEditor.Text = $"{AppResources.ExportStarted}\n";

        try
        {
            var exporter = ProjectExporterFactory.GetExporter(_selectedProjectType);

            _exportedFilePath = await exporter.ExportAsync(
                _projectPath,
                FileSystem.AppDataDirectory,
                progress => MainThread.BeginInvokeOnMainThread(() => ExportProgressBar.Progress = progress),
                message => MainThread.BeginInvokeOnMainThread(() => LogEditor.Text += message + "\n")
            );
        }
        catch (Exception ex)
        {
            LogEditor.Text += $"Error: {ex.Message}\n";
        }

        ExportProgressBar.IsVisible = false;
        LogEditor.Text += $"{AppResources.ExportFinished}\n";

        OpenFileButton.IsVisible = !string.IsNullOrEmpty(_exportedFilePath);
    }

    // Botão: Abrir o arquivo exportado
    private void OnOpenFileClicked(object sender, EventArgs e)
    {
        try
        {
            if (File.Exists(_exportedFilePath))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = _exportedFilePath,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }
        catch (Exception ex)
        {
            LogEditor.Text += $"Error opening file: {ex.Message}\n";
        }
    }
}

public class ProjectTypeOption
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
