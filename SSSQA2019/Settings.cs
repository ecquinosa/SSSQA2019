using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSSQA2019
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            AllcardUMID au = new AllcardUMID();
            StringBuilder sb = new StringBuilder();
            au.InitializeReader(ref sb);
            foreach (string reader in sb.ToString().Split('\n'))
            {
                if (reader != "")
                {
                    cboCardReader.Items.Add(reader);
                    cboSAM.Items.Add(reader);
                    cboUMID.Items.Add(reader);
                }
            }
            //au.InitializeReader(ref cboUMID);
            //au.InitializeReader(ref cboSAM);
            au = null;

            if(cboCardReader.Items.Count>0)cboCardReader.Text = Properties.Settings.Default.CARDREADER;
            if (cboUMID.Items.Count > 0) cboUMID.SelectedIndex = Properties.Settings.Default.UMID;
            if (cboSAM.Items.Count > 0) cboSAM.SelectedIndex = Properties.Settings.Default.SAM;
            txtInterval.Text = Properties.Settings.Default.Interval.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboCardReader.Text == "")
            {
                MessageBox.Show("Please select card reader...", "SETTINGS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboUMID.Text == "")
            {
                MessageBox.Show("Please select item in UMID item(s)...", "SETTINGS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboSAM.Text == "")
            {
                MessageBox.Show("Please select item in SAM item(s)...", "SETTINGS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Properties.Settings.Default.CARDREADER = cboCardReader.Text;
            Properties.Settings.Default.UMID = cboUMID.SelectedIndex;
            Properties.Settings.Default.SAM = cboSAM.SelectedIndex;
            Properties.Settings.Default.Interval = Convert.ToInt32(txtInterval.Text);
            Properties.Settings.Default.Save();            
            MessageBox.Show("Changes has been saved!", "SETTINGS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cboCardReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cboUMID.SelectedIndex = cboCardReader.SelectedIndex;
            }
            catch { }
        }
    }
}
