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
            label5 = new Label();
            radioButtonBinary = new RadioButton();
            radioButtonXML = new RadioButton();
            panel1 = new Panel();
            linkLabelOutputHelp = new LinkLabel();
            label6 = new Label();
            panel2 = new Panel();
            linkLabelMappingHelp = new LinkLabel();
            radioButtonReloaded = new RadioButton();
            radioButtonRevenge = new RadioButton();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
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
            textBoxMidiPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxMidiPath.Location = new Point(114, 9);
            textBoxMidiPath.Name = "textBoxMidiPath";
            textBoxMidiPath.Size = new Size(221, 23);
            textBoxMidiPath.TabIndex = 2;
            // 
            // textBoxOutputPath
            // 
            textBoxOutputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxOutputPath.Location = new Point(114, 38);
            textBoxOutputPath.Name = "textBoxOutputPath";
            textBoxOutputPath.Size = new Size(221, 23);
            textBoxOutputPath.TabIndex = 3;
            // 
            // buttonBrowseMidi
            // 
            buttonBrowseMidi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            buttonBrowseOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label3.BorderStyle = BorderStyle.Fixed3D;
            label3.Location = new Point(12, 128);
            label3.Name = "label3";
            label3.Size = new Size(404, 2);
            label3.TabIndex = 6;
            // 
            // buttonConvert
            // 
            buttonConvert.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonConvert.Location = new Point(341, 136);
            buttonConvert.Name = "buttonConvert";
            buttonConvert.Size = new Size(75, 23);
            buttonConvert.TabIndex = 7;
            buttonConvert.Text = "Convert";
            buttonConvert.UseVisualStyleBackColor = true;
            buttonConvert.Click += buttonConvert_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlDark;
            label4.Location = new Point(12, 140);
            label4.Name = "label4";
            label4.Size = new Size(114, 15);
            label4.TabIndex = 8;
            label4.Text = "Created by Arktisfox";
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
            radioButtonBinary.Location = new Point(0, 0);
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
            radioButtonXML.Location = new Point(177, 0);
            radioButtonXML.Name = "radioButtonXML";
            radioButtonXML.Size = new Size(103, 19);
            radioButtonXML.TabIndex = 12;
            radioButtonXML.TabStop = true;
            radioButtonXML.Text = "XML (Android)";
            radioButtonXML.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(linkLabelOutputHelp);
            panel1.Controls.Add(radioButtonXML);
            panel1.Controls.Add(radioButtonBinary);
            panel1.Location = new Point(114, 70);
            panel1.Name = "panel1";
            panel1.Size = new Size(302, 17);
            panel1.TabIndex = 13;
            // 
            // linkLabelOutputHelp
            // 
            linkLabelOutputHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabelOutputHelp.AutoSize = true;
            linkLabelOutputHelp.Location = new Point(279, 0);
            linkLabelOutputHelp.Name = "linkLabelOutputHelp";
            linkLabelOutputHelp.Size = new Size(20, 15);
            linkLabelOutputHelp.TabIndex = 17;
            linkLabelOutputHelp.TabStop = true;
            linkLabelOutputHelp.Text = "(?)";
            linkLabelOutputHelp.LinkClicked += linkLabelOutputHelp_LinkClicked;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 99);
            label6.Name = "label6";
            label6.Size = new Size(87, 15);
            label6.TabIndex = 14;
            label6.Text = "Note Mapping:";
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(linkLabelMappingHelp);
            panel2.Controls.Add(radioButtonReloaded);
            panel2.Controls.Add(radioButtonRevenge);
            panel2.Location = new Point(114, 99);
            panel2.Name = "panel2";
            panel2.Size = new Size(302, 17);
            panel2.TabIndex = 15;
            // 
            // linkLabelMappingHelp
            // 
            linkLabelMappingHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabelMappingHelp.AutoSize = true;
            linkLabelMappingHelp.Location = new Point(279, 0);
            linkLabelMappingHelp.Name = "linkLabelMappingHelp";
            linkLabelMappingHelp.Size = new Size(20, 15);
            linkLabelMappingHelp.TabIndex = 16;
            linkLabelMappingHelp.TabStop = true;
            linkLabelMappingHelp.Text = "(?)";
            linkLabelMappingHelp.LinkClicked += linkLabelMappingHelp_LinkClicked;
            // 
            // radioButtonReloaded
            // 
            radioButtonReloaded.AutoSize = true;
            radioButtonReloaded.Location = new Point(138, 0);
            radioButtonReloaded.Name = "radioButtonReloaded";
            radioButtonReloaded.Size = new Size(116, 19);
            radioButtonReloaded.TabIndex = 12;
            radioButtonReloaded.TabStop = true;
            radioButtonReloaded.Text = "Tap Tap Reloaded";
            radioButtonReloaded.UseVisualStyleBackColor = true;
            // 
            // radioButtonRevenge
            // 
            radioButtonRevenge.AutoSize = true;
            radioButtonRevenge.Checked = true;
            radioButtonRevenge.Location = new Point(0, 0);
            radioButtonRevenge.Name = "radioButtonRevenge";
            radioButtonRevenge.Size = new Size(112, 19);
            radioButtonRevenge.TabIndex = 11;
            radioButtonRevenge.TabStop = true;
            radioButtonRevenge.Text = "Tap Tap Revenge";
            radioButtonRevenge.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(428, 168);
            Controls.Add(label6);
            Controls.Add(panel2);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(buttonConvert);
            Controls.Add(label3);
            Controls.Add(buttonBrowseOutput);
            Controls.Add(buttonBrowseMidi);
            Controls.Add(textBoxOutputPath);
            Controls.Add(textBoxMidiPath);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MIDI to ttr2_track";
            Load += MainForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
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
        private Label label5;
        private RadioButton radioButtonBinary;
        private RadioButton radioButtonXML;
        private Panel panel1;
        private Label label6;
        private Panel panel2;
        private RadioButton radioButtonReloaded;
        private RadioButton radioButtonRevenge;
        private LinkLabel linkLabelMappingHelp;
        private LinkLabel linkLabelOutputHelp;
    }
}