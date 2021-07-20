using System;
using System.Collections.Generic;
using System.Text;
using SpringCardPCSC;
using ChipCodingDLL.Utilities;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

namespace ChipCodingDLL.Class
{
    public class UMIDClass
    {
        public event EventHandler<PluginInterface.ChipEncodeEventArgs> OnChipEncodeEvent;
        public event EventHandler<PluginInterface.ChipEncodeEventArgs> OnChipRespondEvent;
        public event EventHandler<PluginInterface.ChipEncodeEventArgs> OnChipCodingEvent;
        public event EventHandler<PluginInterface.ChipEncodeEventArgs> OnReaderStatusChanged;
        public event EventHandler<PluginInterface.ChipEncodeEventArgs> OnError;
        public event EventHandler<PluginInterface.ChipEncodeEventArgs> OnSuccess;

        public bool isConnected { get; private set; }
        public string CardReader { get; set; }
        public string ResCode { get; private set; }
        public string SW1 { get; private set; }
        public string SW2 { get; private set; }
        public string STATUSWORD { get; private set; }
        public string ENCKey { private get; set; }
        public string MACKey { private get; set; }
        public string KEKKey { private get; set; }
        public string CARDUID { get; set; }

        public string CardATR { private set; get; }
        public string Protocol { private set; get; }
        public string ShareMode { private set; get; }

        public const uint SHARE_EXCLUSIVE = 1;
        public const uint SHARE_SHARED = 2;
        public const uint SHARE_DIRECT = 3;

        public const uint PROTOCOL_NONE = 0;
        public const uint PROTOCOL_T0 = 1;
        public const uint PROTOCOL_T1 = 2;
        public const uint PROTOCOL_RAW = 4;

        public const uint LEAVE_CARD = 0; // Don't do anything special on close
        public const uint RESET_CARD = 1; // Reset the card on close
        public const uint UNPOWER_CARD = 2; // Power down the card on close
        public const uint EJECT_CARD = 3; // Eject the card on close

        SCardReader reader;
        SCardChannel cchannel;

        private Utility util = new Utility();

        #region Card Process ************************************************************************************************************
        string SC = "";
        public bool SCP02_PutKey(string oldENCKey, string oldMACKey, string newENCKey, string newMACKey) 
        {
            Log("Change Master Key");
            string ResData = "";
            if (!SendCommand(Constants.SELECTCMK, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            if (!SCP02_Authenticate(oldENCKey, oldMACKey)) 
            {
                return false;
            }
           
            byte[] oDEK = util.CHexToByte(Constants.PublicDefaultKey);
            byte[] vector = util.CHexToByte("0000000000000000");
            Log("DEK: " + util.CByteToBCD(oDEK));

            //Derivation Data, DD(KEK) = Constant (01 81) + SC + padding (12 bytes '00')
            string sDD = "0181"+ SC +"000000000000000000000000";
            Log("Derivation Data: " + sDD);

            byte[] oDD = util.CHexToByte(sDD);
            byte[] oSessionDEK = util.DES3_CBC(oDD, oDEK, CipherMode.CBC, ref vector, PaddingMode.None);
            Log("Session DEK  = 3DES_CBC(DEK)[DD]: " + util.CByteToBCD(oSessionDEK));

            byte[] null8Bytes = util.CHexToByte("0000000000000000");
            byte[] oNewKeyENC = util.CHexToByte(newENCKey);
            byte[] oENCKey = util.DES3_CBC(oNewKeyENC, oSessionDEK, CipherMode.ECB, ref vector, PaddingMode.None);
            Log("3DES_EBC(Session DEK)[New ENC key]: " + util.CByteToBCD(oENCKey));
            byte[] oENCKeyKCV = util.DES3_CBC(null8Bytes, oNewKeyENC, CipherMode.ECB, ref vector, PaddingMode.None);
            Log("3DES_EBC(New ENC key)[8 bytes '00']: " + util.CByteToBCD(oENCKeyKCV));

            byte[] oNewKeyMAC= util.CHexToByte(newMACKey);
            byte[] oMACKey = util.DES3_CBC(oNewKeyMAC, oSessionDEK, CipherMode.ECB, ref vector, PaddingMode.None);
            Log("3DES_EBC(Session DEK)[New MAC key]: " + util.CByteToBCD(oMACKey));
            byte[] oMACKeyKCV = util.DES3_CBC(null8Bytes, oNewKeyMAC, CipherMode.ECB, ref vector, PaddingMode.None);
            Log("3DES_EBC(New MAC key)[8 bytes '00']: " + util.CByteToBCD(oMACKeyKCV));

            byte[] oNewKeyDEK = util.CHexToByte(Constants.NEWDEKKEY);
            byte[] oDEKKey = util.DES3_CBC(oNewKeyDEK, oSessionDEK, CipherMode.ECB, ref vector, PaddingMode.None);
            Log("3DES_EBC(Session DEK)[New KEK key]: " + util.CByteToBCD(oDEKKey));
            byte[] oDEKKeyKCV = util.DES3_CBC(null8Bytes, oNewKeyDEK, CipherMode.ECB, ref vector, PaddingMode.None);
            Log("3DES_EBC(New KEK key)[8 bytes '00']: " + util.CByteToBCD(oDEKKeyKCV));
            
            string sENCKey = "8010" + util.CByteToBCD(oENCKey) + "03" +  util.CByteToBCD(oENCKeyKCV).Substring(0, 6);
            string sMACKey = "8010" + util.CByteToBCD(oMACKey) + "03" + util.CByteToBCD(oMACKeyKCV).Substring(0, 6);
            string sDEKKey = "8010" + util.CByteToBCD(oDEKKey) + "03" + util.CByteToBCD(oDEKKeyKCV).Substring(0, 6);
            string keys = sENCKey + sMACKey + sDEKKey;
            string PutKeyCommand = "80D80081" + util.CDectoHex((keys.Length / 2) + 1)+ "01" + keys;

            if (!SendCommand(PutKeyCommand, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            
            return true;
        }

        //set Card State Initialized
        public bool SetCardStateInitialize() 
        {
            Log("Set Initialize Card");
            string ResData = "";
            //Select Card Manager
            if (!SendCommand(Constants.SELECTCMK, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            //Authenticate
            if (!SCP02_Authenticate()) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            if (!SendCommand("80f2800008A00000015100000000", ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            string CardState = ResData.Substring(18, 2);
            if (CardState != "07") 
            {
                if (!SendCommand(Constants.SET_STATE_INITIALIZED, ref ResData)) return false;
                if (SW1 != "90" || SW2 != "00") return false;
            }

            return true;
        }

        //set to secured
        public bool SetCardStateSecured(String encKey, String macKey)
        {
            Log("Set Secured Card");
            string ResData = "";
            //Select Card Manager
            if (!SendCommand(Constants.SELECTCMK, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;            

            //Authenticate
            if (!SCP02_Authenticate(encKey, macKey)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            if (!SendCommand("80f2800008A00000015100000000", ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            string CardState = ResData.Substring(18, 2);
            if (CardState != "0F")
            {
                if (!SendCommand(Constants.SET_STATE_SECURED, ref ResData)) return false;
                if (SW1 != "90" || SW2 != "00") return false;
            }

            return true;
        }

        public bool ExternalAuth(string MACKey, string ENCKey, string CardUID) 
        {
            byte[] IV = util.CHexToByte(Constants.sIV);
            string CH = "";//Card Challenge
            string HC = "";//Host Cryptogram
            string MAC = "";
            string ResData = "";
            string Com = Constants.SET_SECURITY_ENVIRONMENT;
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            Com = Constants.CARDCHALLENGE;
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            CH = ResData.Substring(0, 16);
            CH = CH + CardUID;
            IV = util.CHexToByte(Constants.sIV);
            HC = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(CH), util.CHexToByte(ENCKey), CipherMode.CBC, ref IV, PaddingMode.None));
            string HCPadded = HC + "8000000000000000";

            IV = util.CHexToByte(Constants.sIV);
            MAC = util.CByteToBCD(util.ISO97971M2_ALG3(util.CHexToByte(MACKey), util.CHexToByte(HCPadded), ref IV));
            Com = HC + MAC;
            string Len = util.CDectoHex(Com.Length / 2);
            Com = Constants.EXTAUTH +Len + Com;

            //Send External Authentication
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }

        /// <summary>
        /// Raw Authentication (SCP02)
        /// </summary>
        /// <param name="ENCKey"></param>
        /// <param name="MACKey"></param>
        /// <param name="KEKKey"></param>
        /// <param name="ResCode"></param>
        /// <returns></returns>
        public bool SCP02_Authenticate()
        {
            Log("Athenticating....");

            byte[] IV = util.CHexToByte(Constants.sIV);
            string ResData = "";
            string CR = "";//Card Random Number
            string SN = "";//Sequence Number
            string HR = util.GetRandomHexNumber(16);//Host Random Number
            string CC = ""; //Card Cryptogram
            string DD = "";//Derivation Data
            string SKE = "";//Session Key ENC
            string CCI = "";//Card Cryptogram Input
            string HCI = "";//Host Cryptogram Input
            string HC = "";//Host Cryptogram
            string SKM = ""; //Session Key MAC
            string RMI = "";// Retail MAC Input
            string RMAC = "";//Retail MAC

            //HR = "3504185388096CCB";
            Log("Host Random No: " + HR);
            string Com = Constants.GPHostChallenge + util.CDectoHex(HR.Length / 2) + HR;
            //Send GetChallenge Command
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            //ResData = "00004286004533064792FF020005CE8E6715858930CD36A114B1A0BF";
                      
            Log("Card Response: " + ResData);
            //Parse Response Data
            SN = ResData.Substring(24, 4);
            SC = SN;
            CR = ResData.Substring(28, 12);
            CC = ResData.Substring(40, 16);

            Log("Sequence Number: " + SN);
            Log("Card Random No.: " + CR);
            Log("Card Cryptogram: " + CC);

            //Calculate Derivation Data
            DD = Constants.DDConst + SN + "000000000000000000000000";
            SKE = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(DD),util.CHexToByte(ENCKey), CipherMode.CBC, ref IV, PaddingMode.None));

            Log("Derivation Data: " + DD);
            Log("Session Key Enc: " + SKE);

            //Calculate Card Cryptogram
            CCI = HR + SN + CR + "8000000000000000";
            IV = util.CHexToByte(Constants.sIV);
            CC = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(CCI), util.CHexToByte(SKE), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("IV: " + Constants.sIV);
            Log("Card Cryptogram Input: " + CCI);
            Log("Card Cryptogram: " + CC);
            CC = CC.Substring(CC.Length - 16, 16);
            Log("Card Cryptogram [32,16]: " + CC);

            //Calculate the host cryptogram
            HCI = SN + CR + HR + "8000000000000000";
            IV = util.CHexToByte(Constants.sIV);
            HC = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(HCI), util.CHexToByte(SKE), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("IV: " + Constants.sIV);
            Log("Host Crytogram Input: " + HCI);
            Log("Host Cryptogram: " + HC);
            HC = HC.Substring(HC.Length - 16, 16);
            Log("Host Cryptogram [32,16]: " + HC);

            //Calculate the session MAC key
            DD = Constants.MACKeyConst + SN + "000000000000000000000000";
            IV = util.CHexToByte(Constants.sIV);
            SKM = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(DD), util.CHexToByte(MACKey), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("Derivation Data: " + DD);
            Log("IV: " + Constants.sIV);
            Log("Session Key MAC: " + SKM);

            //Calculate Retail MAC
            RMI = Constants.ExAuthHeader + util.CDectoHex(HC.Length) + HC;
            RMAC = util.CByteToBCD(util.CBC_MAC(SKM, RMI, Constants.sIV));
            Log("Retail MAC Input: " + RMI);
            Log("Retail MAC: " + RMAC);

            Com = Constants.ExAuthHeader + util.CDectoHex((HC + RMAC).Length / 2) + HC + RMAC;
            //Com = "84820100" + util.CDectoHex((HC + RMAC).Length / 2) + HC + RMAC;
            Log("ExAuth: " + Com);
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }


        /// <summary>
        /// Raw Authentication (SCP02)
        /// </summary>
        /// <param name="ENCKey"></param>
        /// <param name="MACKey"></param>
        /// <param name="KEKKey"></param>
        /// <param name="ResCode"></param>
        /// <returns></returns>
        public bool SCP02_Authenticate(String encKey, String macKey)
        {
            Log("Athenticating....");

            byte[] IV = util.CHexToByte(Constants.sIV);
            string ResData = "";
            string CR = "";//Card Random Number
            string SN = "";//Sequence Number
            string HR = util.GetRandomHexNumber(16);//Host Random Number
            string CC = ""; //Card Cryptogram
            string DD = "";//Derivation Data
            string SKE = "";//Session Key ENC
            string CCI = "";//Card Cryptogram Input
            string HCI = "";//Host Cryptogram Input
            string HC = "";//Host Cryptogram
            string SKM = ""; //Session Key MAC
            string RMI = "";// Retail MAC Input
            string RMAC = "";//Retail MAC

            //HR = "3504185388096CCB";
            Log("Host Random No: " + HR);
            string Com = Constants.GPHostChallenge + util.CDectoHex(HR.Length / 2) + HR;
            //Send GetChallenge Command
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            //ResData = "00004286004533064792FF020005CE8E6715858930CD36A114B1A0BF";

            Log("Card Response: " + ResData);
            //Parse Response Data
            SN = ResData.Substring(24, 4);
            SC = SN;
            CR = ResData.Substring(28, 12);
            CC = ResData.Substring(40, 16);

            Log("Sequence Number: " + SN);
            Log("Card Random No.: " + CR);
            Log("Card Cryptogram: " + CC);

            //Calculate Derivation Data
            DD = Constants.DDConst + SN + "000000000000000000000000";
            SKE = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(DD), util.CHexToByte(encKey), CipherMode.CBC, ref IV, PaddingMode.None));

            Log("Derivation Data: " + DD);
            Log("Session Key Enc: " + SKE);

            //Calculate Card Cryptogram
            CCI = HR + SN + CR + "8000000000000000";
            IV = util.CHexToByte(Constants.sIV);
            CC = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(CCI), util.CHexToByte(SKE), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("IV: " + Constants.sIV);
            Log("Card Cryptogram Input: " + CCI);
            Log("Card Cryptogram: " + CC);
            CC = CC.Substring(CC.Length - 16, 16);
            Log("Card Cryptogram [32,16]: " + CC);

            //Calculate the host cryptogram
            HCI = SN + CR + HR + "8000000000000000";
            IV = util.CHexToByte(Constants.sIV);
            HC = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(HCI), util.CHexToByte(SKE), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("IV: " + Constants.sIV);
            Log("Host Crytogram Input: " + HCI);
            Log("Host Cryptogram: " + HC);
            HC = HC.Substring(HC.Length - 16, 16);
            Log("Host Cryptogram [32,16]: " + HC);

            //Calculate the session MAC key
            DD = Constants.MACKeyConst + SN + "000000000000000000000000";
            IV = util.CHexToByte(Constants.sIV);
            SKM = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(DD), util.CHexToByte(macKey), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("Derivation Data: " + DD);
            Log("IV: " + Constants.sIV);
            Log("Session Key MAC: " + SKM);

            //Calculate Retail MAC
            RMI = Constants.ExAuthHeader + util.CDectoHex(HC.Length) + HC;
            RMAC = util.CByteToBCD(util.CBC_MAC(SKM, RMI, Constants.sIV));
            Log("Retail MAC Input: " + RMI);
            Log("Retail MAC: " + RMAC);

            Com = Constants.ExAuthHeader + util.CDectoHex((HC + RMAC).Length / 2) + HC + RMAC;
            //Com = "84820100" + util.CDectoHex((HC + RMAC).Length / 2) + HC + RMAC;
            Log("ExAuth: " + Com);
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }
       

        /// <summary>
        /// Initialize OS1
        /// </summary>
        /// <param name="MTKey">Master Transport Key</param>
        /// <param name="ResCode"></param>
        /// <returns></returns>
        public bool InitOS(string MTKey, ref string ResCode)
        {
            Log("Init OS....");
            byte[] IV = util.CHexToByte(Constants.sIV);
            string DD = "";//Diversification Data
            string CC = "";//Card Challenge
            string CCR = "";// Card Cryptogram
            string HC = ""; //Host Challenge
            string HCR = ""; //Host Cryptogram
            string TK = "";// Transport Key
            string HCC = ""; // Host Calculated Card Cryptogram
            string TK1 = "";
            string TK2 = "";
            string ResData = "";

            HC = util.GetRandomHexNumber(16);//"0102030405060708";
            string Com = Constants.InitUpdate + util.CDectoHex(8) + HC;
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            //ResData = "0000000000000000D77D709ACF949937C36EB937F841260C";

            DD = ResData.Substring(0, 16);
            CC = ResData.Substring(16, 16);
            CCR = ResData.Substring(32, 16);

            Log("Master Transport Key: " + MTKey);
            Log("Host Challenge: " + HC);
            Log("Derivation Data: " + DD);
            Log("Card Challenge: " + CC);
            Log("Card Cryptogram: " + CCR);

            TK = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(DD + DD), util.CHexToByte(MTKey), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("Transport Key [ENC(DD + DD) MTK(" + MTKey + ") ]: " + TK);

            IV = util.CHexToByte(Constants.sIV);
            string EncData = HC + CC + "8000000000000000";
            TK1 = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(EncData), util.CHexToByte(TK), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("ENC(CC + HC) TK(" + TK + ") : " + TK1);
            HCC = TK1.Substring(32, 16);
            Log("Calculated Card Cryptogram: " + HCC);

            Log("Check If host.card.cryptogram == card.cryptogram....");
            Log(HCC + " = " + CCR);
            if (HCC != CCR) return false;

            IV = util.CHexToByte(Constants.sIV);
            EncData = CC + HC + "8000000000000000";
            TK2 = util.CByteToBCD(util.DES3_CBC(util.CHexToByte(EncData), util.CHexToByte(TK), CipherMode.CBC, ref IV, PaddingMode.None));
            Log("ENC(HC + CC) TK(" + TK + ") : " + TK2);
            HCR = TK2.Substring(32, 16);
            Log("Calculated Host Cryptogram: " + HCR);

            //Send EXTERNAL AUTHENTICATE
            Com = "FEFEFB01" + util.CDectoHex(HCR.Length / 2) + HCR;
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            //terminate (turn off transport mode)
            if (!SendCommand("FEFEFB02", ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            return true;
        }

        //instantiate new instance with keys
        /// <summary>
        /// reset (applet becomes selected by default)
        /// </summary>
        /// <returns></returns>
        public bool Instantiate()
        {
            Log("Install Instance....");
            string ResData = "";
            ////Select AID
            if (!SendCommand(Constants.SelectCom + "08" + Constants.CardManager, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            //Authenticate
            if (!SCP02_Authenticate()) return false;
            if (!SendCommand(Constants.InstantiateCom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            Disconnect(SCARD.UNPOWER_CARD);
            return true;
        }

        SCardReader creader;

        /// <summary>
        /// Delete an Applet Instance
        /// </summary>
        /// <returns></returns>
        public bool Delete_Instance()
        {
            Log("Deleting Instance....");
            string ResData = "";

            //Select Applet
            string SelectCom = Constants.AppletName;
            if (!SendCommand(SelectCom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            //Select CMK
            string Com = Constants.SelectCom + util.CDectoHex(Constants.CardManager.Length / 2) + Constants.CardManager;
            if (!SendCommand(Com, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            //Authenticate
            if (!SCP02_Authenticate()) return false;
            if (!SendCommand(Constants.DeleteInstance, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }
        /// <summary>
        /// Select Card Manager Key
        /// </summary>
        /// <returns></returns>
        public bool SelectCMK(ref string ResCode) 
        {
            //Select CMK
            string ResData = "";
            string Com = Constants.SelectCom + util.CDectoHex(Constants.CardManager.Length / 2) + Constants.CardManager;
            if (!SendCommand(Com, ref ResData)) return false;
            ResCode = String.Format("{0}{1}", SW1, SW2);
            if (SW1 != "90" || SW2 != "00") return false;

            return true;
        }
        /// <summary>
        /// Select Dedicated File
        /// </summary>
        /// <param name="DFName"></param>
        /// <returns></returns>
        public bool SelectDF(string DFName)
        {
            string ResData = "";
            //Select DF 
            Log("Select DF Name:" + DFName);
            string DFLEN = util.CDectoHex(DFName.Length / 2);
            string EFCom = Constants.SelectCom + DFLEN + DFName;
            if (!SendCommand(EFCom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }

        /// <summary>
        /// Create Dedicated Files
        /// </summary>
        /// <param name="DFName">DF Name [0-16 bytes]</param>
        /// <param name="ENCKey">Encryption Key [16bytes]</param>
        /// <param name="MACKey">MAC Key [16bytes]</param>
        /// <returns></returns>
        public bool CreateDF(string DFName, string ENCKey, string MACKey)
        {
            return CreateDF(DFName, ENCKey, MACKey, true);
        }

        public bool CreateDF(String DFName, string ENCKey, String MACKey, bool withPIN) {

            string ResData = "";
            //Select Applet
            //string SelectCom = Constants.AppletName;
            //if (!SendCommand(SelectCom, ref ResData)) return false;
            //if (SW1 != "90" || SW2 != "00") return false;

            Log("Create DF:" + DFName);

            //About to create ADF 
            DFName = util.CDectoHex(DFName.Length / 2) + DFName;
            string CreateADFCom = "00E0000032623082013884" + DFName + "8101018F020001A1148C087F2E2E2E2E2E2E2E9C087F2E2E2E2E2E2E2E";
            if (!SendCommand(CreateADFCom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;

            if (withPIN)
            {
                //Create SDO_PIN PIN 0x01 (0xBF8101)
                string HexPIN = util.CAsciiToHexString(Constants.DefaultPIN);
                HexPIN += "0000";
                string PIN = util.CDectoHex(HexPIN.Length / 2) + HexPIN;
                string CreatePINObjCom = "00E18101337031A01CA1108C06F300002EFFFF9C06F300002EFFFF9A01039B0103800200087F411080011481010682" + PIN;
                if (!SendCommand(CreatePINObjCom, ref ResData)) return false;
                if (SW1 != "90" || SW2 != "00") return false;
            }

            //Create SDO_SYM SM_Keys 0x02 (0xBF8A02)
            string CreateSDOCom = "00E18A02447042A01AA10E8C05AB00002E009C05AB00002E009A01039B010380020010A2249010" + MACKey + "9110" + ENCKey;
            if (!SendCommand(CreateSDOCom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;


            string CreateSSECom = "00E1FB0E387036A006A1048C0281007B2CA409800100830181950108A40995018083018280011CB40980010C830182950130B80980010C830182950130";
            //Create SSE-SP 0x0E (0xBFFB0E)             
            if (!SendCommand(CreateSSECom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;

        }

        /// <summary>
        /// Create EF
        /// </summary>
        /// <param name="Commands"></param>
        /// <param name="DF"></param>
        /// <returns></returns>
        public bool CreateEF(string[,] Commands, string DF)
        {
            string ResData = "";
            for (int i = 0; i <= Commands.GetUpperBound(0); i++)
            {
                string EFNo = Commands[i, 0];
                Log("EF :" + EFNo + "  Create EF Com:" + Commands[i, 1]);
                string commands = Commands[i, 1];
                if (!SendCommand(commands, ref ResData)) return false;
                Application.DoEvents();
                if (SW1 != "90" || SW2 != "00") return false;
            }
            return false;
        }

        /// <summary>
        /// Create EF
        /// </summary>
        /// <param name="Commands"></param>
        /// <param name="DF"></param>
        /// <returns></returns>
        public bool CreateEF(string Commands, string DF)
        {
            Log("Creating EF...");
            string ResData = "";
            if (!SendCommand(Commands, ref ResData)) return false;
            Application.DoEvents();
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }


        /// <summary>
        /// Create EF Files
        /// </summary>
        /// <param name="EF"></param>
        /// <param name="DF"></param>
        /// <param name="ResCode"></param>
        /// <returns></returns>
        public bool CreateEF(int EFNo, int EFSize, string SecurityConditions)
        {
            string ResData = "";
            string EFID = util.CDectoHex(EFNo).PadLeft(4, '0'); //FID
            string EFIDLen = util.CDectoHex(EFID.Length / 2);
            string EFHSize = util.CDectoHex(EFSize).PadLeft(4, '0');//Size Int
            string EFHSizeLen = util.CDectoHex(EFHSize.Length / 2);

            string FCP = "";//File Control Parameter
            string FCPLen = "";//File Control Parameter Len
            string Len = "";//Length of Command
            string SecurityConditionsLen = util.CDectoHex(SecurityConditions.Length / 2);//Access Conditions Len

            FCP = Constants.FileLenTag + EFHSizeLen + EFHSize + //File Len
            Constants.FileDescriptor + // File Descriptor 
            Constants.SFITag + // Short File Identifier
            Constants.FIDTag + EFIDLen + EFID + // File ID
            Constants.LifeCycleStatus + //LCS
            Constants.SecurityAtTag + SecurityConditionsLen + SecurityConditions; //Security Attributes

            FCPLen = util.CDectoHex(FCP.Length / 2);

            FCP = Constants.FCPTag + FCPLen + FCP;
            Len = util.CDectoHex((FCP).Length / 2);

            string EFCom = "";
            //Create EF
            EFCom = Constants.CreateEFCom + Len + FCP;
            Log("EF :" + EFNo.ToString() + " EF Com:" + EFCom);
            if (!SendCommand(EFCom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            Log("SW1:" + SW1 + " SW2:" + SW2 + " ResCode:" + ResCode);
            return true;
        }

        /// <summary>
        /// Update Binary / Write Data to the Card
        /// </summary>
        /// <param name="Data">Data in Hex</param>
        /// <param name="EF">EF File in Hex</param>
        /// <param name="ResCode"></param>
        /// <returns></returns>
        public bool UpdateBinaryData(string Data, string DF, string EF, ref string ResCode)
        {
            Log("Update Binary Data....");
            string ResData = "";
            byte[] bufferdata = util.CHexToByte(Data);
            byte[][] slicedData = util.SliceByte(bufferdata);

            if (!SelectDF(DF)) return false;

            //Select EF
            string EFLen = util.CDectoHex(EF.Length / 2); 
            if (!SendCommand(Constants.SelectEFCom + EFLen +EF, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            int offset = 0;
            string sOffset = "";
            foreach (byte[] buffdata in slicedData)
            {
                //Get the Data Len and Convert it to Hex
                string HexData = util.CByteToBCD(buffdata);
                string len = util.CDectoHex(HexData.Length / 2);
                sOffset = util.CDectoHex(offset).PadLeft(4, '0');
                string UBCom = Constants.UpdateBinaryCom + sOffset + len + HexData;
                //Send Update Binary Command
                if (!SendCommand(UBCom, ref ResData)) return false;
                if (SW1 != "90" || SW2 != "00") return false;
                offset += (HexData.Length / 2);
            }
            return true;
        }

        public bool SelectEF(string EF, ref string ResData, ref string ResCode)
        {
            string Len = util.CDectoHex (EF.Length / 2);
            string Com = Constants.SelectEFCom + Len + EF;
            if (!SendCommand(Com, ref ResData)) return false;
            ResCode = SW1 + SW2;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }

        public bool DeactivateEF(string EF, ref string ResData, ref string ResCode) 
        {
            string Len = util.CDectoHex(EF.Length / 2);
            string Com = Constants.DEACTIVATEEF;
            if (!SendCommand(Com, ref ResData)) return false;
            ResCode = SW1 + SW2;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }


        /// <summary>
        /// Update Binary / Write Data to the Card
        /// </summary>
        /// <param name="Data">Data in Hex</param>
        /// <param name="EF">EF File in Hex</param>
        /// <param name="ResCode"></param>
        /// <returns></returns>
        public bool ReadBinaryData(string DF, string EF, int len, ref string Data, ref string ResCode)
        {
            string ResData = "";
            //Select ADF
            if (!SendCommand(Constants.SelectDFCom + util.CDectoHex(DF.Length / 2) + DF, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            //Select EF
            if (!SendCommand(Constants.SelectEFCom + EF + "00", ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            string slen = util.CDectoHex(len);
            string RBCom = Constants.ReadBinaryCom + slen;
            //Send Update Binary Command
            if (!SendCommand(RBCom, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }

        /// <summary>
        /// Commit Perso
        /// </summary>
        /// <returns></returns>
        public bool CommitPerso()
        {
            string ResData = "";
            if (!SendCommand(Constants.commitPerso, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }

        public bool  GetSerialNo(ref string SerialNo) 
        {
            if (!SendCommand(Constants.AppletName, ref SerialNo)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            if (!SendCommand(Constants.GetSerialCom, ref SerialNo)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            CARDUID = SerialNo;
            return true;
        }

        public bool SelectApplet() 
        {
            string ResData = "";
            if (!SendCommand(Constants.AppletName, ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }

        public bool GetCardState()
        {
            string ResData = "";
            SendCommand("00A4040008A00000015100000000", ref  ResData); 
            if (!SCP02_Authenticate()) 
            {
                return false;
            }

            if (!SendCommand("80f2800008A000000151000000", ref ResData)) return false;
            if (SW1 != "90" || SW2 != "00") return false;
            return true;
        }

#endregion ************************************************************************************************************

        #region PCSC ******************************************************************************************************************
        /// <summary>
        /// Get Reader List
        /// </summary>
        /// <returns></returns>
        public string[] GetReaderList()
        {
            return SCARD.Readers;
        }

        /// <summary>
        /// Connect to Card
        /// </summary>
        /// <param name="CardReaderName"></param>
        /// <returns></returns>
        public bool Connect(uint ShareMode, uint Protocol)
        {
            if (cchannel != null)
            {
                if (cchannel.Connected)
                cchannel.DisconnectReset();
                cchannel = null;
            }

            cchannel = new SCardChannel(reader);
            cchannel.ShareMode = ShareMode;
            cchannel.Protocol = Protocol;
            cchannel.Connect();
            return cchannel.Connected;
        }

        /// <summary>
        /// Start Monitoring of Cards Presence
        /// </summary>
        public void StartMonitor() 
        {
            reader = new SCardReader(CardReader);
            reader.StartMonitor(new SCardReader.StatusChangeCallback(ReaderStatusChanged));
        }

        /// <summary>
        /// Stop Monitoring of Cards Presence
        /// </summary>
        public void StopMonitor()
        {
            if (reader != null) 
            {
                reader.StopMonitor();
            }
        }

        private void ReaderStatusChanged(uint ReaderState, CardBuffer CardAtr)
        {
            if (OnReaderStatusChanged != null) 
            {
                PluginInterface.ChipEncodeEventArgs args = new PluginInterface.ChipEncodeEventArgs();
                if (CardAtr != null) args.CardAtr = util.CByteToBCD(CardAtr.GetBytes());
                args.ReaderState = SCARD.ReaderStatusToString(ReaderState);   
                OnReaderStatusChanged(this,args);
            }  
        }

        public void Disconnect(uint Disposition)
        {
            cchannel.Disconnect(Disposition);
        }

        public void Log(string msg)
        {
            PluginInterface.ChipEncodeEventArgs args = new PluginInterface.ChipEncodeEventArgs();
            if (OnChipCodingEvent != null)
            {
                args.StatusCode = msg;
                OnChipCodingEvent(this, args);
                Application.DoEvents();
            }
        }

        /// <summary>
        /// On Error Event
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="StatusCode"></param>
        /// <param name="StatusWord"></param>
        public void OnErrorLog(string msg, string StatusCode, string StatusWord)
        {
            PluginInterface.ChipEncodeEventArgs args = new PluginInterface.ChipEncodeEventArgs();
            if (OnChipCodingEvent != null)
            {
                args.Message = msg;
                args.StatusCode = StatusCode;
                args.SWString = StatusWord; 
                OnError(this, args);
                Application.DoEvents();
            }
        }

        /// <summary>
        /// On Error Event
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="StatusCode"></param>
        /// <param name="StatusWord"></param>
        public void OnSuccessLog(string msg, string StatusCode, string StatusWord)
        {
            PluginInterface.ChipEncodeEventArgs args = new PluginInterface.ChipEncodeEventArgs();
            if (OnChipCodingEvent != null)
            {
                args.Message = msg;
                args.StatusCode = StatusCode;
                args.SWString = StatusWord;
                OnSuccess(this, args);
                Application.DoEvents();
            }
        }

        private void SendLog(CAPDU capdu)
        {
            PluginInterface.ChipEncodeEventArgs args = new PluginInterface.ChipEncodeEventArgs();
            args.CLA = util.CByteToBCD(capdu.CLA).ToUpper();
            args.INS = util.CByteToBCD(capdu.INS).ToUpper();
            args.P1 = util.CByteToBCD(capdu.P1).ToUpper();
            args.P2 = util.CByteToBCD(capdu.P2).ToUpper();
            args.Le = util.CByteToBCD(capdu.Le).ToUpper();
            args.Lc = util.CByteToBCD(capdu.Lc).ToUpper();
            args.Data = util.CByteToBCD(capdu.GetBytes()).ToUpper();
            if (OnChipEncodeEvent != null)
            {
                OnChipEncodeEvent(this, args);
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Send Command to Card Reader
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="SW1"></param>
        /// <param name="SW2"></param>
        /// <param name="ResCode"></param>
        /// <returns></returns>
        private bool SendCommand(string Command, ref string Data)
        {
            try
            {
                SW1 = ""; SW2 = "";
                byte[] com = null;
                ResCode = "NC";
                //if (!Connect(CardReader)) return false;
                if (!cchannel.Connected)
                {
                    Log("Not Connected to Card");
                    return false;
                }
                com = util.CHexToByte(Command);
                CAPDU capdu = new CAPDU(com);
                RAPDU rapdu = null;
                cchannel.Command = capdu;
                if (!cchannel.Transmit()) return false;
                rapdu = cchannel.Response;
                SW1 = util.CByteToBCD(rapdu.SW1);
                SW2 = util.CByteToBCD(rapdu.SW2);
                ResCode = rapdu.SWString;
                STATUSWORD = rapdu.SWString;
                if (rapdu.hasData)
                {
                    CardBuffer cbuffer = rapdu.data;
                    Data = util.CByteToBCD(cbuffer.GetBytes());
                }
                //Log Send Command
                SendLog(capdu);
                //Response Log
                ResLog(rapdu);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void ResLog(RAPDU rapdu)
        {
            if (rapdu == null) return;
            PluginInterface.ChipEncodeEventArgs args = new PluginInterface.ChipEncodeEventArgs();
            args.SWString = rapdu.SWString.ToUpper();
            args.SW1 = util.CByteToBCD(rapdu.SW1).ToUpper();
            args.SW2 = util.CByteToBCD(rapdu.SW2).ToUpper();
            args.Data = rapdu.data.AsString().ToUpper();
            if (OnChipRespondEvent != null)
            {
                OnChipRespondEvent(this, args);
                Application.DoEvents();
            }
        }

        #endregion*********************************************************************************************************************

    }
}
