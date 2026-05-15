namespace MidiToTTR
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            textBoxMidiPath = new TextBox();
            textBoxOutputPath = new TextBox();
            buttonBrowseMidi = new Button();
            buttonBrowseOutput = new Button();
            label3 = new Label();
            buttonConvert = new Button();
            label4 = new Label();
            linkLabelHelp = new LinkLabel();
            label5 = new Label();
            radioButtonBinary = new RadioButton();
            radioButtonXML = new RadioButton();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 0;
            label1.Text = "MIDI File Path:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 41);
            label2.Name = "label2";
            label2.Size = new Size(96, 15);
            label2.TabIndex = 1;
            label2.Text = "Output File Path:";
            // 
            // textBoxMidiPath
            // 
            textBoxMidiPath.Location = new Point(114, 9);
            textBoxMidiPath.Name = "textBoxMidiPath";
            textBoxMidiPath.Size = new Size(221, 23);
            textBoxMidiPath.TabIndex = 2;
            // 
            // textBoxOutputPath
            // 
            textBoxOutputPath.Location = new Point(114, 38);
            textBoxOutputPath.Name = "textBoxOutputPath";
            textBoxOutputPath.Size = new Size(221, 23);
            textBoxOutputPath.TabIndex = 3;
            // 
            // buttonBrowseMidi
            // 
            buttonBrowseMidi.Location = new Point(341, 9);
            buttonBrowseMidi.Name = "buttonBrowseMidi";
            buttonBrowseMidi.Size = new Size(75, 23);
            buttonBrowseMidi.TabIndex = 4;
            buttonBrowseMidi.Text = "Browse...";
            buttonBrowseMidi.UseVisualStyleBackColor = true;
            buttonBrowseMidi.Click += buttonBrowseMidi_Click;
            // 
            // buttonBrowseOutput
            // 
            buttonBrowseOutput.Location = new Point(341, 37);
            buttonBrowseOutput.Name = "buttonBrowseOutput";
            buttonBrowseOutput.Size = new Size(75, 23);
            buttonBrowseOutput.TabIndex = 5;
            buttonBrowseOutput.Text = "Browse...";
            buttonBrowseOutput.UseVisualStyleBackColor = true;
            buttonBrowseOutput.Click += buttonBrowseOutput_Click;
            // 
            // label3
            // 
            label3.BorderStyle = BorderStyle.Fixed3D;
            label3.Location = new Point(12, 95);
            label3.Name = "label3";
            label3.Size = new Size(404, 2);
            label3.TabIndex = 6;
            // 
            // buttonConvert
            // 
            buttonConvert.Location = new Point(341, 101);
            buttonConvert.Name = "buttonConvert";
            buttonConvert.Size = new Size(75, 23);
            buttonConvert.TabIndex = 7;
            buttonConvert.Text = "Convert";
            buttonConvert.UseVisualStyleBackColor = true;
            buttonConvert.Click += buttonConvert_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 105);
            label4.Name = "label4";
            label4.Size = new Size(114, 15);
            label4.TabIndex = 8;
            label4.Text = "Created by Arktisfox";
            // 
            // linkLabelHelp
            // 
            linkLabelHelp.AutoSize = true;
            linkLabelHelp.Location = new Point(315, 106);
            linkLabelHelp.Name = "linkLabelHelp";
            linkLabelHelp.Size = new Size(20, 15);
            linkLabelHelp.TabIndex = 9;
            linkLabelHelp.TabStop = true;
            linkLabelHelp.Text = "(?)";
            linkLabelHelp.LinkClicked += linkLabelHelp_LinkClicked;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 70);
            label5.Name = "label5";
            label5.Size = new Size(89, 15);
            label5.TabIndex = 10;
            label5.Text = "Output Format:";
            // 
            // radioButtonBinary
            // 
            radioButtonBinary.AutoSize = true;
            radioButtonBinary.Checked = true;
            radioButtonBinary.Location = new Point(114, 68);
            radioButtonBinary.Name = "radioButtonBinary";
            radioButtonBinary.Size = new Size(174, 19);
            radioButtonBinary.TabIndex = 11;
            radioButtonBinary.TabStop = true;
            radioButtonBinary.Text = "Binary (iOS, Recommended)";
            radioButtonBinary.UseVisualStyleBackColor = true;
            // 
            // radioButtonXML
            // 
            radioButtonXML.AutoSize = true;
            radioButtonXML.Location = new Point(294, 68);
            radioButtonXML.Name = "radioButtonXML";
            radioButtonXML.Size = new Size(103, 19);
            radioButtonXML.TabIndex = 12;
            radioButtonXML.TabStop = true;
            radioButtonXML.Text = "XML (Android)";
            radioButtonXML.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(428, 135);
            Controls.Add(radioButtonXML);
            Controls.Add(radioButtonBinary);
            Controls.Add(label5);
            Controls.Add(linkLabelHelp);
            Controls.Add(label4);
            Controls.Add(buttonConvert);
            Controls.Add(label3);
            Controls.Add(buttonBrowseOutput);
            Controls.Add(buttonBrowseMidi);
            Controls.Add(textBoxOutputPath);
            Controls.Add(textBoxMidiPath);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MIDI to ttr2_track";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBoxMidiPath;
        private TextBox textBoxOutputPath;
        private Button buttonBrowseMidi;
        private Button buttonBrowseOutput;
        private Label label3;
        private Button buttonConvert;
        private Label label4;
        private LinkLabel linkLabelHelp;
        private Label label5;
        private RadioButton radioButtonBinary;
        private RadioButton radioButtonXML;
    }
}