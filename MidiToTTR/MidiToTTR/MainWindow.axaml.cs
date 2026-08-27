using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TaptrackTools;
using MsBoxIcon = MsBox.Avalonia.Enums.Icon;

namespace MidiToTTR;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void BrowseMidi_Click(object? sender, RoutedEventArgs e)
    {
        string? fileName = await FileDialogs.OpenFile(this, "MIDI Files", "mid", "midi");
        if (fileName is null)
        {
            return;
        }

        textBoxMidiPath.Text = fileName;

        // automagically set ttr2_track path if we don't have one already
        if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
        {
            textBoxOutputPath.Text = Path.Combine(Path.GetDirectoryName(fileName)!, "taptrack.ttr2_track");
        }
    }

    private async void BrowseOutput_Click(object? sender, RoutedEventArgs e)
    {
        string? fileName = await FileDialogs.SaveFile(this, "TTR Track Files", "taptrack", "ttr2_track");
        if (fileName is not null)
        {
            textBoxOutputPath.Text = fileName;
        }
    }

    private async void Convert_Click(object? sender, RoutedEventArgs e)
    {
        string inputPath = textBoxMidiPath.Text ?? string.Empty;
        string outputPath = textBoxOutputPath.Text ?? string.Empty;

        // sanity checking
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            await ShowMessage("Error", "MIDI path not set.", ButtonEnum.Ok, MsBoxIcon.Error);
            return;
        }
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            await ShowMessage("Error", "Output path not set.", ButtonEnum.Ok, MsBoxIcon.Error);
            return;
        }
        if (outputPath.Equals(inputPath, StringComparison.CurrentCultureIgnoreCase))
        {
            await ShowMessage("Error", "Output path must not be the same as the MIDI path.", ButtonEnum.Ok, MsBoxIcon.Error);
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

        try
        {
            MidiTaptrackConvertor.ConvertToTaptrack(inputPath, outputPath,
                radioButtonReloaded.IsChecked == true, radioButtonBinary.IsChecked == true);
        }
        catch (Exception ex)
        {
            await ShowMessage("Error", ex.Message, ButtonEnum.Ok, MsBoxIcon.Error);
        }
    }

    private async void OutputHelp_Click(object? sender, RoutedEventArgs e)
    {
        await ShowMessage("Help", @"[Output Format]
Use binary output format for iOS TTR, and Tap Tap Player
Use xml output format for Android TTR4 only
", ButtonEnum.Ok, MsBoxIcon.Info);
    }

    private async void MappingHelp_Click(object? sender, RoutedEventArgs e)
    {
        await ShowMessage("Help", @"[MIDI Note Mapping]
Easy difficulty MIDI notes: 60-71
Medium difficulty MIDI notes: 72-83
Hard difficulty MIDI notes: 84-95
Extreme difficulty MIDI notes: 96-107

All 12 notes per difficulty are available for use.
Typically TTR Themes are mapped to the following notes:
[0:Left Tap] [1:Left Shake] [2:Center Tap] [3:Right Shake] [4:Right Tap] [5:Center Shake]

[Tap Tap Reloaded Mapping]
When Tap Tap Reloaded mapping is selected, the first lanes are mapped as follows:
[0:Left Tap] [1:Middle Tap] [2:Right Tap] [4:Center Shake]", ButtonEnum.Ok, MsBoxIcon.Info);
    }

    private Task<ButtonResult> ShowMessage(string title, string text, ButtonEnum buttons, MsBoxIcon icon) =>
        MessageBoxManager.GetMessageBoxStandard(title, text, buttons, icon).ShowWindowDialogAsync(this);
}
