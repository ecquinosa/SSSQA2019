using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMIDLibrary;

namespace SSSQA2019
{   

    class AllcardUMID
    {

        private AllCardTech_Smart_Card sc;

        public string CSN { get; set; }
        public string CRN { get; set; }
        public string CCDT { get; set; }
        public string STATUS { get; set; }
        public string SL1 { get; set; }
        public string LP { get; set; }
        public string LB { get; set; }
        public string RP { get; set; }
        public string RB { get; set; }
        public string ErrorMessage { get; set; }

        public bool InitSC()
        {
            if (sc == null)
            {
                sc = new AllCardTech_Smart_Card();
                sc.InitializeReaders();
                return true;
            }
            else
            {
                //sc.InitializeReaders();
                //return SelectApplet();
                return true;
            }
        }

        public bool SelectApplet()
        {
            try
            {
                if (sc.SelectApplet(Properties.Settings.Default.UMID, Properties.Settings.Default.SAM)) return true;                
                else return false;                
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        public void Test()
        {
            sc = new AllCardTech_Smart_Card();
            sc.InitializeReaders();
            sc.SelectApplet(1, 0);
            sc.Dispose();
            sc = null;
        }

        public bool IsReaderConnected()
        {
           
            sc = new AllCardTech_Smart_Card();
            sc.InitializeReaders();
            foreach (string reader in sc.ReaderList)
            {
                if (reader != null)
                {
                    sc.Dispose();
                    sc = null;
                    return true;
                }
            }
            
            return false;            
        }
        
        public void InitializeReader(ref StringBuilder sb)
        {            
            InitSC();
            sc.InitializeReaders();            
            foreach (String s in sc.ReaderList)
            {
                if (s != null)
                {
                    sb.AppendLine(s);                    
                }
            }
            sc.Dispose();
            sc = null;
        }

        public void ResetData()
        {
            CRN = "";
            CSN = "";
            CCDT = "";
            STATUS = "";
            SL1 = "";
            LP = "";
            LB = "";
            RP = "";
            RB = "";
        }

        public bool ReadData()
        {
            try
            {
                //housekeeping
                ResetData();

                string tempFolder = "Temp";
                if (!System.IO.Directory.Exists(tempFolder)) System.IO.Directory.CreateDirectory(tempFolder);
                foreach (string file in System.IO.Directory.GetFiles(tempFolder)) System.IO.File.Delete(file);
                //housekeeping

                sc = null;
                sc = new AllCardTech_Smart_Card();
                sc.InitializeReaders();
                sc.SelectApplet(1, 0);
                //if (InitSC())
                //{
                    AllCardTech_Util util = new AllCardTech_Util();
                    CRN = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CRN));
                    CSN = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CSN));

                    //byte[] tByte = new byte[0];
                    //string CCDT_ASCII = System.Text.ASCIIEncoding.ASCII.GetString(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CARD_CREATION_DATE));
                    //string CCDT_BCD = util.Hex2Str(util.ByteArrayToHexString(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CARD_CREATION_DATE)));

                    CCDT = util.ByteArrayToAscii(sc.get_getUmidData(AllCardTech_Smart_Card.UMID_Fields.CARD_CREATION_DATE));

                    string _status = "";
                    ////sc = null;
                    ////InitSC();
                    if (sc.GetCardStatus(ref _status)) { STATUS = _status; }

                    if (sc.AuthenticateSL1())
                    {
                        string LP_File = string.Format("{0}\\Lprimary.ansi-fmr", tempFolder);
                        string LB_File = string.Format("{0}\\Lbackup.ansi-fmr", tempFolder);
                        string RP_File = string.Format("{0}\\Rprimary.ansi-fmr", tempFolder);
                        string RB_File = string.Format("{0}\\Rbackup.ansi-fmr", tempFolder);

                        if (sc.getUmidFile(LP_File, UMIDLibrary.AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_LEFT_PRIMARY_FINGER)) LP = string.Format("{0}  {1} bytes", LP_File, new System.IO.FileInfo(LP_File).Length.ToString());                        
                        if (sc.getUmidFile(RP_File, UMIDLibrary.AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_RIGHT_PRIMARY_FINGER)) RP = string.Format("{0}  {1} bytes", RP_File, new System.IO.FileInfo(RP_File).Length.ToString());                      
                        if (sc.getUmidFile(LB_File, UMIDLibrary.AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_LEFT_SECONDARY_FINGER)) LB = string.Format("{0}  {1} bytes", LB_File, new System.IO.FileInfo(LB_File).Length.ToString());                        
                        if (sc.getUmidFile(RB_File, UMIDLibrary.AllCardTech_Smart_Card.UMID_Fields.BIOMETRIC_RIGHT_SECONDARY_FINGER)) RB = string.Format("{0}  {1} bytes", RB_File, new System.IO.FileInfo(RB_File).Length.ToString());
                                                         

                        SL1 = "SUCCESS";
                    }
                    else
                    {
                        SL1 = "FAILED";
                        //sc = null;
                    }

                AllCardTech_Smart_Card u = new AllCardTech_Smart_Card();
                //u.DisconnectCard(Properties.Settings.Default.CARDREADER);
                u.DisconnectApplet(); 

                System.Threading.Thread.Sleep(100);

                    return true;
                //}
                //else
                //{
                //    return false;
                //}

                
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
    
}
