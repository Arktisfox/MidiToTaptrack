using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TaptrackTools;
using MsBoxIcon = MsBox.Avalonia.Enums.Icon;

namespace TTRToMidi;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void BrowseInput_Click(object? sender, RoutedEventArgs e)
    {
        string? fileName = await FileDialogs.OpenFile(this, "Taptrack Files", "ttr2_track", "ttr2_track.xml");
        if (fileName is null)
        {
            return;
        }

        textBoxInputPath.Text = fileName;

        // automagically set midi path if we don't have one already
        if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
        {
            textBoxOutputPath.Text = Path.Combine(Path.GetDirectoryName(fileName)!, "notes.mid");
        }
    }

    private async void BrowseOutput_Click(object? sender, RoutedEventArgs e)
    {
        string? fileName = await FileDialogs.SaveFile(this, "MIDI Files", "notes", "mid");
        if (fileName is not null)
        {
            textBoxOutputPath.Text = fileName;
        }
    }

    private async void Convert_Click(object? sender, RoutedEventArgs e)
    {
        string inputPath = textBoxInputPath.Text ?? string.Empty;
        string outputPath = textBoxOutputPath.Text ?? string.Empty;

        // sanity checking
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            await ShowMessage("Error", "Taptrack path not set.", ButtonEnum.Ok, MsBoxIcon.Error);
            return;
        }
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            await ShowMessage("Error", "Output path not set.", ButtonEnum.Ok, MsBoxIcon.Error);
            return;
        }
        if (outputPath.Equals(inputPath, StringComparison.CurrentCultureIgnoreCase))
        {
            await ShowMessage("Error", "Output path must not be the same as the Taptrack path.", ButtonEnum.Ok, MsBoxIcon.Error);
            return;
        }
        if (File.Exists(outputPath))
        {
            var res = await ShowMessage("Question", "Output path already exists. Do you want to overwrite it?",
                ButtonEnum.YesNo, MsBoxIcon.Question);
            if (res != ButtonResult.Yes)
            {
                return;
            }
        }

        // load in the ttr2_track
        try
        {
            new TTRTrack().LoadFromPlist(inputPath);
        }
        catch (Exception ex)
        {
            await ShowMessage("Taptrack Load Error", ex.Message, ButtonEnum.Ok, MsBoxIcon.Error);
        }

        // convert it to a midi sequence then save it
        try
        {
            MidiTaptrackConvertor.ConvertToMidi(inputPath, outputPath);
            await ShowMessage("Success", "Conversion was successful!", ButtonEnum.Ok, MsBoxIcon.Info);
        }
        catch (Exception ex)
        {
            await ShowMessage("Error", ex.Message, ButtonEnum.Ok, MsBoxIcon.Error);
        }
    }

    private Task<ButtonResult> ShowMessage(string title, string text, ButtonEnum buttons, MsBoxIcon icon) =>
        MessageBoxManager.GetMessageBoxStandard(title, text, buttons, icon).ShowWindowDialogAsync(this);
}
