namespace SSSQA2019
{
    partial class Settings
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboCardReader = new System.Windows.Forms.ComboBox();
            this.cboUMID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboSAM = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CARD READER";
            // 
            // cboCardReader
            // 
            this.cboCardReader.FormattingEnabled = true;
            this.cboCardReader.Location = new System.Drawing.Point(124, 29);
            this.cboCardReader.Name = "cboCardReader";
            this.cboCardReader.Size = new System.Drawing.Size(303, 21);
            this.cboCardReader.TabIndex = 1;
            this.cboCardReader.SelectedIndexChanged += new System.EventHandler(this.cboCardReader_SelectedIndexChanged);
            // 
            // cboUMID
            // 
            this.cboUMID.FormattingEnabled = true;
            this.cboUMID.Location = new System.Drawing.Point(124, 92);
            this.cboUMID.Name = "cboUMID";
            this.cboUMID.Size = new System.Drawing.Size(303, 21);
            this.cboUMID.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "UMID";
            // 
            // cboSAM
            // 
            this.cboSAM.FormattingEnabled = true;
            this.cboSAM.Location = new System.Drawing.Point(124, 62);
            this.cboSAM.Name = "cboSAM";
            this.cboSAM.Size = new System.Drawing.Size(303, 21);
            this.cboSAM.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "SAM";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 178);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 41);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(124, 130);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(303, 21);
            this.txtInterval.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Interval";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 231);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtInterval);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cboSAM);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboUMID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboCardReader);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SETTINGS";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboCardReader;
        private System.Windows.Forms.ComboBox cboUMID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboSAM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label label4;
    }
}