namespace BDBGlovesClient
{
    partial class BDBGCForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
       

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.printDataButton = new System.Windows.Forms.Button();
            this.StopPrintingButton = new System.Windows.Forms.Button();
            this.sendMidiNoteButton = new System.Windows.Forms.Button();
            this.StartIMUCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cameraOneImageBox = new Emgu.CV.UI.ImageBox();
            this.cameraTwoImageBox = new Emgu.CV.UI.ImageBox();
            this.cameraTwoCheckBox = new System.Windows.Forms.CheckBox();
            this.cameraOneCheckBox = new System.Windows.Forms.CheckBox();
            this.captureLabel1 = new System.Windows.Forms.Label();
            this.captureOneComboBox = new System.Windows.Forms.ComboBox();
            this.captureLabel2 = new System.Windows.Forms.Label();
            this.captureTwoComboBox = new System.Windows.Forms.ComboBox();
            this.captureOneStartButton = new System.Windows.Forms.Button();
            this.captureTwoStartButton = new System.Windows.Forms.Button();
            this.captureTwoMotionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cameraOneImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cameraTwoImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // printDataButton
            // 
            this.printDataButton.Location = new System.Drawing.Point(38, 44);
            this.printDataButton.Name = "printDataButton";
            this.printDataButton.Size = new System.Drawing.Size(123, 23);
            this.printDataButton.TabIndex = 0;
            this.printDataButton.Text = "Begin Printing Data";
            this.printDataButton.UseVisualStyleBackColor = true;
            this.printDataButton.Click += new System.EventHandler(this.printDataButton_Click);
            // 
            // StopPrintingButton
            // 
            this.StopPrintingButton.Location = new System.Drawing.Point(168, 43);
            this.StopPrintingButton.Name = "StopPrintingButton";
            this.StopPrintingButton.Size = new System.Drawing.Size(112, 23);
            this.StopPrintingButton.TabIndex = 1;
            this.StopPrintingButton.Text = "Stop Printing Data";
            this.StopPrintingButton.UseVisualStyleBackColor = true;
            this.StopPrintingButton.Click += new System.EventHandler(this.StopPrintingButton_Click);
            // 
            // sendMidiNoteButton
            // 
            this.sendMidiNoteButton.Location = new System.Drawing.Point(38, 73);
            this.sendMidiNoteButton.Name = "sendMidiNoteButton";
            this.sendMidiNoteButton.Size = new System.Drawing.Size(123, 23);
            this.sendMidiNoteButton.TabIndex = 3;
            this.sendMidiNoteButton.Text = "Send Note";
            this.sendMidiNoteButton.UseVisualStyleBackColor = true;
            this.sendMidiNoteButton.Click += new System.EventHandler(this.sendMidiNoteButton_Click);
            // 
            // StartIMUCheckBox
            // 
            this.StartIMUCheckBox.AutoSize = true;
            this.StartIMUCheckBox.Location = new System.Drawing.Point(168, 79);
            this.StartIMUCheckBox.Name = "StartIMUCheckBox";
            this.StartIMUCheckBox.Size = new System.Drawing.Size(107, 17);
            this.StartIMUCheckBox.TabIndex = 7;
            this.StartIMUCheckBox.Text = "Start IMU Trigger";
            this.StartIMUCheckBox.UseVisualStyleBackColor = true;
            this.StartIMUCheckBox.CheckedChanged += new System.EventHandler(this.StartIMUCheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Motion";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(309, 442);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Total Motions Found:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // cameraOneImageBox
            // 
            this.cameraOneImageBox.Location = new System.Drawing.Point(312, 44);
            this.cameraOneImageBox.Name = "cameraOneImageBox";
            this.cameraOneImageBox.Size = new System.Drawing.Size(397, 353);
            this.cameraOneImageBox.TabIndex = 2;
            this.cameraOneImageBox.TabStop = false;
            // 
            // cameraTwoImageBox
            // 
            this.cameraTwoImageBox.Location = new System.Drawing.Point(730, 43);
            this.cameraTwoImageBox.Name = "cameraTwoImageBox";
            this.cameraTwoImageBox.Size = new System.Drawing.Size(402, 354);
            this.cameraTwoImageBox.TabIndex = 2;
            this.cameraTwoImageBox.TabStop = false;
            // 
            // cameraTwoCheckBox
            // 
            this.cameraTwoCheckBox.AutoSize = true;
            this.cameraTwoCheckBox.Location = new System.Drawing.Point(727, 404);
            this.cameraTwoCheckBox.Name = "cameraTwoCheckBox";
            this.cameraTwoCheckBox.Size = new System.Drawing.Size(88, 17);
            this.cameraTwoCheckBox.TabIndex = 9;
            this.cameraTwoCheckBox.Text = "Show Motion";
            this.cameraTwoCheckBox.UseVisualStyleBackColor = true;
            this.cameraTwoCheckBox.CheckedChanged += new System.EventHandler(this.cameraTwoCheckBox_CheckedChanged);
            // 
            // cameraOneCheckBox
            // 
            this.cameraOneCheckBox.AutoSize = true;
            this.cameraOneCheckBox.Location = new System.Drawing.Point(312, 404);
            this.cameraOneCheckBox.Name = "cameraOneCheckBox";
            this.cameraOneCheckBox.Size = new System.Drawing.Size(88, 17);
            this.cameraOneCheckBox.TabIndex = 10;
            this.cameraOneCheckBox.Text = "Show Motion";
            this.cameraOneCheckBox.UseVisualStyleBackColor = true;
            this.cameraOneCheckBox.CheckedChanged += new System.EventHandler(this.cameraOneCheckBox_CheckedChanged);
            // 
            // captureLabel1
            // 
            this.captureLabel1.AutoSize = true;
            this.captureLabel1.Location = new System.Drawing.Point(406, 405);
            this.captureLabel1.Name = "captureLabel1";
            this.captureLabel1.Size = new System.Drawing.Size(72, 13);
            this.captureLabel1.TabIndex = 11;
            this.captureLabel1.Text = "Start Capture:";
            // 
            // captureOneComboBox
            // 
            this.captureOneComboBox.FormattingEnabled = true;
            this.captureOneComboBox.Location = new System.Drawing.Point(484, 402);
            this.captureOneComboBox.Name = "captureOneComboBox";
            this.captureOneComboBox.Size = new System.Drawing.Size(144, 21);
            this.captureOneComboBox.TabIndex = 12;
            this.captureOneComboBox.SelectedIndexChanged += new System.EventHandler(this.captureOneComboBox_SelectedIndexChanged);
            // 
            // captureLabel2
            // 
            this.captureLabel2.AutoSize = true;
            this.captureLabel2.Location = new System.Drawing.Point(821, 405);
            this.captureLabel2.Name = "captureLabel2";
            this.captureLabel2.Size = new System.Drawing.Size(72, 13);
            this.captureLabel2.TabIndex = 13;
            this.captureLabel2.Text = "Start Capture:";
            // 
            // captureTwoComboBox
            // 
            this.captureTwoComboBox.FormattingEnabled = true;
            this.captureTwoComboBox.Location = new System.Drawing.Point(900, 402);
            this.captureTwoComboBox.Name = "captureTwoComboBox";
            this.captureTwoComboBox.Size = new System.Drawing.Size(151, 21);
            this.captureTwoComboBox.TabIndex = 14;
            this.captureTwoComboBox.SelectedIndexChanged += new System.EventHandler(this.captureTwoComboBox_SelectedIndexChanged);
            // 
            // captureOneStartButton
            // 
            this.captureOneStartButton.Location = new System.Drawing.Point(634, 400);
            this.captureOneStartButton.Name = "captureOneStartButton";
            this.captureOneStartButton.Size = new System.Drawing.Size(75, 23);
            this.captureOneStartButton.TabIndex = 15;
            this.captureOneStartButton.Text = "START!";
            this.captureOneStartButton.UseVisualStyleBackColor = true;
            this.captureOneStartButton.Click += new System.EventHandler(this.captureOneStartButton_Click);
            // 
            // captureTwoStartButton
            // 
            this.captureTwoStartButton.Location = new System.Drawing.Point(1057, 400);
            this.captureTwoStartButton.Name = "captureTwoStartButton";
            this.captureTwoStartButton.Size = new System.Drawing.Size(75, 23);
            this.captureTwoStartButton.TabIndex = 16;
            this.captureTwoStartButton.Text = "START!";
            this.captureTwoStartButton.UseVisualStyleBackColor = true;
            this.captureTwoStartButton.Click += new System.EventHandler(this.captureTwoStartButton_Click);
            // 
            // captureTwoMotionLabel
            // 
            this.captureTwoMotionLabel.AutoSize = true;
            this.captureTwoMotionLabel.Location = new System.Drawing.Point(730, 442);
            this.captureTwoMotionLabel.Name = "captureTwoMotionLabel";
            this.captureTwoMotionLabel.Size = new System.Drawing.Size(104, 13);
            this.captureTwoMotionLabel.TabIndex = 17;
            this.captureTwoMotionLabel.Text = "Total Motions Found";
            // 
            // BDBGCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 482);
            this.Controls.Add(this.captureTwoMotionLabel);
            this.Controls.Add(this.captureTwoStartButton);
            this.Controls.Add(this.captureOneStartButton);
            this.Controls.Add(this.captureTwoComboBox);
            this.Controls.Add(this.captureLabel2);
            this.Controls.Add(this.captureOneComboBox);
            this.Controls.Add(this.captureLabel1);
            this.Controls.Add(this.cameraOneCheckBox);
            this.Controls.Add(this.cameraTwoCheckBox);
            this.Controls.Add(this.cameraTwoImageBox);
            this.Controls.Add(this.StartIMUCheckBox);
            this.Controls.Add(this.sendMidiNoteButton);
            this.Controls.Add(this.StopPrintingButton);
            this.Controls.Add(this.printDataButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cameraOneImageBox);
            this.Name = "BDBGCForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BDBGCForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BDBGCForm_FormClosed);
            this.Load += new System.EventHandler(this.BDBGCForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cameraOneImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cameraTwoImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button printDataButton;
        private System.Windows.Forms.Button StopPrintingButton;
        private System.Windows.Forms.Button sendMidiNoteButton;
        private System.Windows.Forms.CheckBox StartIMUCheckBox;
        private Emgu.CV.UI.ImageBox cameraOneImageBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Emgu.CV.UI.ImageBox cameraTwoImageBox;
        private System.Windows.Forms.CheckBox cameraTwoCheckBox;
        private System.Windows.Forms.CheckBox cameraOneCheckBox;
        private System.Windows.Forms.Label captureLabel1;
        private System.Windows.Forms.ComboBox captureOneComboBox;
        private System.Windows.Forms.Label captureLabel2;
        private System.Windows.Forms.ComboBox captureTwoComboBox;
        private System.Windows.Forms.Button captureOneStartButton;
        private System.Windows.Forms.Button captureTwoStartButton;
        private System.Windows.Forms.Label captureTwoMotionLabel;
    }
}

