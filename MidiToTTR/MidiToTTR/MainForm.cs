using Claunia.PropertyList;
using MidiToTTR.KBMidiFile;
using Sanford.Multimedia.Midi;
using System.Data;
using System.Text;
using WindowsAPICodePack.Dialogs;

namespace MidiToTTR
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonBrowseMidi_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("MIDI Files", "mid;midi"));
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxMidiPath.Text = dialog.FileName;

                // automagically set ttr2_track path if we don't have one already
                if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
                {
                    textBoxOutputPath.Text = Path.Combine(Path.GetDirectoryName(dialog.FileName), "taptrack.ttr2_track");
                }
            }
        }

        private void buttonBrowseOutput_Click(object sender, EventArgs e)
        {
            var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("TTR Track Files", "ttr2_track"));
            dialog.DefaultFileName = "taptrack";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxOutputPath.Text = dialog.FileName;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            // sanity checking
            if (string.IsNullOrWhiteSpace(textBoxMidiPath.Text))
            {
                MessageBox.Show("MIDI path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
            {
                MessageBox.Show("Output path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxOutputPath.Text.Equals(textBoxMidiPath.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Output path must not be the same as the MIDI path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(textBoxOutputPath.Text))
            {
                var res = MessageBox.Show("Output path already exists. Do you want to overwrite it?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != DialogResult.Yes)
                {
                    return;
                }
            }

            // convert to a dictionary of KBMidiFile
            string inputPath = textBoxMidiPath.Text;
            string outputPath = textBoxOutputPath.Text;
            try
            {
                MidiTaptrackConvertor.ConvertToTaptrack(inputPath, outputPath, radioButtonReloaded.Checked, radioButtonBinary.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void linkLabelOutputHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@"[Output Format]
Use binary output format for iOS TTR, and Tap Tap Player
Use xml output format for Android TTR4 only
", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabelMappingHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@"[MIDI Note Mapping]
Easy difficulty MIDI notes: 60-71
Medium difficulty MIDI notes: 72-83
Hard difficulty MIDI notes: 84-95
Extreme difficulty MIDI notes: 96-107

All 12 notes per difficulty are available for use.
Typically TTR Themes are mapped to the following notes:
[0:Left Tap] [1:Left Shake] [2:Center Tap] [3:Right Shake] [4:Right Tap] [5:Center Shake]

[Tap Tap Reloaded Mapping]
When Tap Tap Reloaded mapping is selected, the first lanes are mapped as follows:
[0:Left Tap] [1:Middle Tap] [2:Right Tap] [4:Center Shake]", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
