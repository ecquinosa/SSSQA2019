
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace SSSQA2019
{
    public partial class Form1 : Form
    {
        public delegate void VoidDelegate();
        private bool bStopThread = true;

        private bool IsProcessReady=true;
        private DataTable dtData;
        AllcardUMID au = new AllcardUMID();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            ResetForm();
            CreateTable();
            //timer1.Enabled = true;
            //timer1.Start();            
            //if (thread == null) StartThread();
        }

        private Thread thread;
        private void StartThread()
        {
            if (thread != null)
                StopThread();

            thread = new Thread(Run);
            thread.IsBackground = true;
            bStopThread = false;
            thread.Start();
        }

        private void Run()
        {
            while (!bStopThread)    // loop 1 - build list, then iterate
            {
                if (IsProcessReady)
                {
                    IsProcessReady = false;
                    ReadCard();
                    System.Threading.Thread.Sleep(Properties.Settings.Default.Interval);
                    IsProcessReady = true;
                }
                //ReadCard();

            }
        }

        private void StopThread()
        {
            if (thread != null)
            {
                bStopThread = true;
                Thread.Sleep(50);
            }
            if (thread != null)
                thread.Abort();
            if (thread != null)
                thread.Join();
            thread = null;
        }

        private void CreateTable()
        {
            if (dtData == null)
            {
                dtData = new DataTable();
                dtData.Columns.Add("CSN", Type.GetType("System.String"));
                dtData.Columns.Add("CRN", Type.GetType("System.String"));
                dtData.Columns.Add("CCDT", Type.GetType("System.String"));
                dtData.Columns.Add("STATUS", Type.GetType("System.String"));
                dtData.Columns.Add("SL1", Type.GetType("System.String"));
                dtData.Columns.Add("LP", Type.GetType("System.String"));
                dtData.Columns.Add("LB", Type.GetType("System.String"));
                dtData.Columns.Add("RP", Type.GetType("System.String"));
                dtData.Columns.Add("RB", Type.GetType("System.String"));
            }
            else
            { dtData.Clear(); }
        }

        private void ResetForm()
        {
            ResetPanel1();
            txtSearch.Clear();
            lblTotal.Text = "TOTAL : 0";
            CreateTable();
            if (dtData != null) grid.DataSource = dtData;
        }

        private void ResetPanel1()
        {
            lblCSN.Text = "";
            lblCRN.Text = "";
            lblCCDT.Text = "";
            lblStatus.Text = "";
            lblSL1.Text = "";
            lbLP.Text = "";
            lbLB.Text = "";
            lbRP.Text = "";
            lbRB.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invoke(new Action(ReadCard));
        }

        public void ReadCard()
        {
            ResetPanel1();
            //if (IsProcessReady)
            //{
            //    IsProcessReady = false;
            au = null;
            au = new AllcardUMID();
            if (!au.IsReaderConnected())
            {
                //ResetPanel1();
                LabelMessageStatus("PLEASE RE-CHECK CARD READER OR SAM", Color.OrangeRed);
                //IsProcessReady = true;
                return;
            }

            au = null;
            au = new AllcardUMID();
            if (au.InitSC())
            {
                if (!au.SelectApplet())
                {
                    //ResetPanel1();
                    LabelMessageStatus("PLACE CARD ON READER", Color.OrangeRed);
                    //IsProcessReady = true;
                    return;
                }
            }
            else
            {
                return;
            }

            au = null;
            au = new AllcardUMID();
            LabelMessageStatus("READING CARD PLEASE WAIT...", Color.ForestGreen);                
            if (au.ReadData())
                {
                    lblCSN.Text = au.CSN;
                    lblCRN.Text = au.CRN;
                    lblCCDT.Text = au.CCDT;
                    lblStatus.Text = au.STATUS;
                    lblSL1.Text = au.SL1;
                    if (lblStatus.Text == "ERROR") lblStatus.ForeColor = Color.OrangeRed;
                    else lblStatus.ForeColor = Color.Black;
                    if (lblSL1.Text != "SUCCESS") lblSL1.ForeColor = Color.OrangeRed;
                    else lblSL1.ForeColor = Color.Black;

                    lbLP.Text = au.LP;
                    lbLB.Text = au.LB;
                    lbRP.Text = au.RP;
                    lbRB.Text = au.RB;

                    if (lblCRN.Text != "")
                    {
                        //if (dtData.Select("CRN='" + lblCRN.Text.Trim() + "'").Length == 0)
                        //{
                            DataRow rw = dtData.NewRow();
                            rw["CRN"] = lblCRN.Text;
                            rw["CSN"] = lblCSN.Text;
                            rw["CCDT"] = lblCCDT.Text;
                            rw["STATUS"] = lblStatus.Text;
                            rw["SL1"] = lblSL1.Text;
                            rw["LP"] = lbLP.Text;
                            rw["LB"] = lbLB.Text;
                            rw["RP"] = lbRP.Text;
                            rw["RB"] = lbRB.Text;
                            dtData.Rows.Add(rw);
                            grid.DataSource = dtData;

                            lblTotal.Text = "TOTAL : " + dtData.DefaultView.Count.ToString("N0");

                            SaveToLog(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", lblCRN.Text, lblCSN.Text, lblCCDT.Text, lblStatus.Text, lblSL1.Text, lbLP.Text, lbLB.Text, lbRP.Text, lbRB.Text));

                            //ResetPanel1();
                        au.ResetData();

                        LabelMessageStatus("", Color.ForestGreen);
                            //MessageBox.Show("Done...", "UMID", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //}
                    }

                    //ResetForm();
                //}


                //IsProcessReady = true;
            }
        }

        private void pbSettings_Click(object sender, EventArgs e)
        {
            Settings frm = new Settings();
            frm.ShowDialog();
        }

        private void LabelMessageStatus(string status, Color color)
        {
            lblMessage.Text = status;
            lblMessage.ForeColor = color;
            Application.DoEvents();
        }

        private string TimeStamp()
        {
            return DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss tt ");
        }

        private void SaveToLog(string desc)
        {
            string logFolder = string.Format("Logs\\{0}",DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
            string logFile = string.Format("{0}\\Log.txt",logFolder);

            using (StreamWriter sw = new StreamWriter(logFile,true))
            {
                sw.WriteLine(TimeStamp() + desc);
                sw.Dispose();
                sw.Close();
            }
        }

        private void SaveToError(string desc)
        {
            string logFolder = string.Format("Logs\\{0}", DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
            string logFile = string.Format("{0}\\Error.txt", logFolder);

            using (StreamWriter sw = new StreamWriter(logFile, true))
            {
                sw.WriteLine(TimeStamp() + desc);
                sw.Dispose();
                sw.Close();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Settings frm = new Settings();
            frm.ShowDialog();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtSearch.Text != "")
                {
                    if (txtSearch.Text.Length == 20)
                    {
                        if (dtData.Select("CSN='" + txtSearch.Text + "'").Length > 0) grid.DataSource = dtData.Select("CSN='" + txtSearch.Text + "'").CopyToDataTable();
                        else grid.DataSource = dtData.Select("CSN='ERROR'").CopyToDataTable();
                    }
                    else
                    {
                        if (dtData.Select("CRN='" + txtSearch.Text + "'").Length > 0) grid.DataSource = dtData.Select("CRN='" + txtSearch.Text + "'").CopyToDataTable();
                        else grid.DataSource = dtData.Select("CSN='ERROR'").CopyToDataTable();
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void tsbStart_Click(object sender, EventArgs e)
        {
            if (thread == null)
            {
                StartThread();
                tsbStart.Text = "STOP";
            }
            else
            {
                bStopThread = true;
                StopThread();
                tsbStart.Text = "START";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AllcardUMID aa = new AllcardUMID();
            aa.Test();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            toolStrip1.Enabled = false;
            btnRead.Enabled = false;
            ReadCard();
            Cursor = Cursors.Default;
            toolStrip1.Enabled = true;
            btnRead.Enabled = true;
            btnRead.Focus();
        }
    }
}
