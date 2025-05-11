using System.Diagnostics;
using CodeSnapshot.Resources.Localization;

namespace CodeSnapshot;

public partial class MainPage : ContentPage
{
    private string _projectPath = string.Empty;
    private string _exportedFilePath = string.Empty;
    private string _selectedProjectType = ".NET";

    public MainPage()
    {
        InitializeComponent();

        ProjectTypePicker.SelectedIndexChanged += (s, e) =>
        {
            if (ProjectTypePicker.SelectedIndex >= 0)
            {
                _selectedProjectType = ProjectTypePicker.SelectedItem?.ToString() ?? ".NET";
            }
        };

    }

    // Botão: Selecionar pasta do projeto
    private async void OnSelectFolderClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = AppResources.SelectFolder
        });

        if (result != null)
        {
            // Pega a pasta onde o arquivo foi selecionado
            _projectPath = Path.GetDirectoryName(result.FullPath) ?? string.Empty;

            // Atualiza o label com o caminho
            SelectedPathLabel.Text = $"{AppResources.PathLabel} {_projectPath}";
        }
    }

    // Botão: Gerar snapshot (chamará o serviço real em breve)
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

        // Em breve: aqui entra a chamada do exportador real

        ExportProgressBar.IsVisible = false;
        LogEditor.Text += $"{AppResources.ExportFinished}\n";

        // Mostra o botão "Abrir Arquivo" se tivermos o caminho do exportado
        OpenFileButton.IsVisible = !string.IsNullOrEmpty(_exportedFilePath);
    }

    // Botão: Abrir o arquivo exportado no programa padrão
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
