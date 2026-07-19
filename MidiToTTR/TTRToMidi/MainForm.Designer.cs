namespace TTRToMidi
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label4 = new Label();
            buttonConvert = new Button();
            label3 = new Label();
            buttonBrowseOutput = new Button();
            buttonBrowseInput = new Button();
            textBoxOutputPath = new TextBox();
            textBoxInputPath = new TextBox();
            label2 = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlDark;
            label4.Location = new Point(12, 88);
            label4.Name = "label4";
            label4.Size = new Size(114, 15);
            label4.TabIndex = 24;
            label4.Text = "Created by Arktisfox";
            // 
            // buttonConvert
            // 
            buttonConvert.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonConvert.Location = new Point(341, 84);
            buttonConvert.Name = "buttonConvert";
            buttonConvert.Size = new Size(75, 23);
            buttonConvert.TabIndex = 23;
            buttonConvert.Text = "Convert";
            buttonConvert.UseVisualStyleBackColor = true;
            buttonConvert.Click += buttonConvert_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label3.BorderStyle = BorderStyle.Fixed3D;
            label3.Location = new Point(12, 76);
            label3.Name = "label3";
            label3.Size = new Size(404, 2);
            label3.TabIndex = 22;
            // 
            // buttonBrowseOutput
            // 
            buttonBrowseOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonBrowseOutput.Location = new Point(341, 38);
            buttonBrowseOutput.Name = "buttonBrowseOutput";
            buttonBrowseOutput.Size = new Size(75, 23);
            buttonBrowseOutput.TabIndex = 21;
            buttonBrowseOutput.Text = "Browse...";
            buttonBrowseOutput.UseVisualStyleBackColor = true;
            buttonBrowseOutput.Click += buttonBrowseOutput_Click;
            // 
            // buttonBrowseInput
            // 
            buttonBrowseInput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonBrowseInput.Location = new Point(341, 10);
            buttonBrowseInput.Name = "buttonBrowseInput";
            buttonBrowseInput.Size = new Size(75, 23);
            buttonBrowseInput.TabIndex = 20;
            buttonBrowseInput.Text = "Browse...";
            buttonBrowseInput.UseVisualStyleBackColor = true;
            buttonBrowseInput.Click += buttonBrowseInput_Click;
            // 
            // textBoxOutputPath
            // 
            textBoxOutputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxOutputPath.Location = new Point(125, 39);
            textBoxOutputPath.Name = "textBoxOutputPath";
            textBoxOutputPath.Size = new Size(210, 23);
            textBoxOutputPath.TabIndex = 19;
            // 
            // textBoxInputPath
            // 
            textBoxInputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxInputPath.Location = new Point(125, 10);
            textBoxInputPath.Name = "textBoxInputPath";
            textBoxInputPath.Size = new Size(210, 23);
            textBoxInputPath.TabIndex = 18;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 42);
            label2.Name = "label2";
            label2.Size = new Size(96, 15);
            label2.TabIndex = 17;
            label2.Text = "Output File Path:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 13);
            label1.Name = "label1";
            label1.Size = new Size(107, 15);
            label1.TabIndex = 16;
            label1.Text = "ttr2_track File Path:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(428, 115);
            Controls.Add(label4);
            Controls.Add(buttonConvert);
            Controls.Add(label3);
            Controls.Add(buttonBrowseOutput);
            Controls.Add(buttonBrowseInput);
            Controls.Add(textBoxOutputPath);
            Controls.Add(textBoxInputPath);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ttr2_track to MIDI";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label4;
        private Button buttonConvert;
        private Label label3;
        private Button buttonBrowseOutput;
        private Button buttonBrowseInput;
        private TextBox textBoxOutputPath;
        private TextBox textBoxInputPath;
        private Label label2;
        private Label label1;
    }
}
