using Sanford.Multimedia.Midi;
using WindowsAPICodePack.Dialogs;

namespace TTRToMidi
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void buttonBrowseInput_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Taptrack Files", "ttr2_track;ttr2_track.xml"));
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxInputPath.Text = dialog.FileName;

                // automagically set midi path if we don't have one already
                if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
                {
                    textBoxOutputPath.Text = Path.Combine(Path.GetDirectoryName(dialog.FileName), "notes.mid");
                }
            }
        }

        private void buttonBrowseOutput_Click(object sender, EventArgs e)
        {
            var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("MIDI Files", "mid"));
            dialog.DefaultFileName = "notes";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxOutputPath.Text = dialog.FileName;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            // sanity checking
            if (string.IsNullOrWhiteSpace(textBoxInputPath.Text))
            {
                MessageBox.Show("Taptrack path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
            {
                MessageBox.Show("Output path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxOutputPath.Text.Equals(textBoxInputPath.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Output path must not be the same as the Taptrack path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // load in the ttr2_track
            TTRTrack track = null;
            try
            {
                track = new TTRTrack();
                track.LoadFromPlist(textBoxInputPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Taptrack Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // convert it to a midi sequence then save it
            try
            {
                MidiTaptrackConvertor.ConvertToMidi(textBoxInputPath.Text, textBoxOutputPath.Text);
                MessageBox.Show("Conversion was successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
