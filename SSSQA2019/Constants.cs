using System;
using System.Collections.Generic;
using System.Text;

namespace ChipCodingDLL.Class
{
    public class Constants
    {
        public const string SET_STATE_INITIALIZED = "80F0800708A000000151000000";
        public const string SET_STATE_SECURED = "80F0800F08A000000151000000";

        public const string NEWDEKKEY = "505152535455565758595A5B5C5D5E5F"; 
        public const string SELECTCMK = "00A4040008A00000015100000000";
        public const string SET_SECURITY_ENVIRONMENT = "002281A40680011C830182";
        public const string CARDCHALLENGE = "0084000008";
        //public const string SET_STATE_INITIALIZED = "80F0800708A000000151000000";
        public const string DEACTIVATEEF = "0004000000";

        public const string PublicDefaultKey = "404142434445464748494A4B4C4D4E4F";
        public const string TransportKey = "6B3921B6F9252C7277AB890595E67862";
        public const string InitUpdate = "FEFEFB00";
        public const string GPHostChallenge = "80500000";
        public const string DDConst = "0182";
        public const string MACKeyConst = "0101";
        public const string sIV = "0000000000000000";
        public const string ExAuthHeader = "84820000";
        public const string EXTAUTH = "00820000";
        public const string SelectCom = "00A40400"; //Select Command 
        public const string InstanceName = "A000000003000000"; //Instance Name
        public const string AppletName = "00A4000C023F00";
        public const string CardManager = "A000000151000000"; // Card Manager
        public const string DeleteInstance = "80E40000094F07A0000000003F0000";//Delete Instance Command

        public const string GetSerialCom = "00CB7FFF08";
        public const string InstantiateCom = "80E60C002E0EA0000001644941534543437632010FA0000001644941534543437632010107A0000000003F00010403C901000000";
        public const string UpdateBinaryCom = "00D6";
        public const string ReadBinaryCom = "00B00000";
        public const string SelectEFCom = "00A40204";
        public const string SelectDFCom = "00A40400";
        public const string commitPerso = "80440000";

        public const string CreateEFCom = "00E00000";
        public const string ABSecurityConditions = "8C077BAEAEAEAECEFF9C077BAEAEAEAECEFF";
        public const string FileDescriptor = "820101";
        public const string LifeCycleStatus = "8A0101";
        public const string FCPTag = "62";
        public const string FileLenTag = "80";
        public const string FIDTag = "83";
        public const string SecurityAtTag = "A1";
        public const string SFITag = "8800"; 
        public const string DefaultPIN = "123456";

        public const string SharedDF = "412A2A556E6974546573742A2A2A";
        public const string SSSDF =        "535353534543555245424C4F434B2A2A";
        public const string GSISDF =       "47534953534543555245424C4F434B2A";
        public const string PHILHEALTHDF = "5048534543555245424C4F434B2A2A2A";
        public const string PAGIBIGDF =    "5049534543555245424C4F434B2A2A2A";


        public  static  string[,] SharedEFSecurity = 
	    {
            {"1", "8C077BAEAEAEAEFF009C077BAEAEAEAEFF00"},
	        {"2", "8C077BAEAEAEAECEBE9C077BAEAEAEAECEBE"},
            {"3", "8C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"4", "8C077BAEAEAEAEFFBE9C077BAEAEAEAEFFBE"},
            {"5", "8C077BAEAEAEAECEAE9C077BAEAEAEAECEAE"},
            {"6", "8C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"7", "8C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"8", "8C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"9", "8C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"}
	    };

        public static string[,] CreateShareEFCOM = 
	    {
            {"01", "00E00000266224800200448201018800830200018A0101A1128C077BAEAEAEAEFF009C077BAEAEAEAEFF00"},
	        {"02", "00E00000266224800200018201018800830200028A0101A1128C077BAEAEAEAECEBE9C077BAEAEAEAECEBE"},
            {"03", "00E00000266224800201758201018800830200038A0101A1128C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"04", "00E00000266224800206008201018800830200048A0101A1128C077BAEAEAEAEFFBE9C077BAEAEAEAEFFBE"},
            {"05", "00E000002662248002004C8201018800830200058A0101A1128C077BAEAEAEAECEAE9C077BAEAEAEAECEAE"},
            {"06", "00E0000026622480023C008201018800830200068A0101A1128C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"07", "00E00000266224800210048201018800830200078A0101A1128C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"08", "00E000002662248002014D8201018800830200088A0101A1128C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
            {"09", "00E00000266224800201008201018800830200098A0101A1128C077BAEAEAEAEFFAE9C077BAEAEAEAEFFAE"},
	    };

        public static string[,] CreateAGENCYEFCOM = 
	    {
            {"01", "00E00000266224800204008201018800830200018A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
	        {"02", "00E00000266224800202008201018800830200028A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"03", "00E00000266224800201008201018800830200038A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"04", "00E00000266224800201008201018800830200048A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"05", "00e00000266224800201008201018800830200058A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"06", "00E00000266224800201008201018800830200068A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"07", "00E00000266224800200808201018800830200078A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"08", "00E00000266224800200808201018800830200088A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"09", "00E00000266224800200808201018800830200098A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"10", "00E000002662248002008082010188008302000A8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"11", "00E000002662248002008082010188008302000B8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"12", "00E000002662248002008082010188008302000C8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"13", "00E000002662248002008082010188008302000D8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
	        {"14", "00E000002662248002008082010188008302000E8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"15", "00E000002662248002008082010188008302000F8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"16", "00E00000266224800200808201018800830200108A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"17", "00e00000266224800200408201018800830200118A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"18", "00E00000266224800200408201018800830200128A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"19", "00E00000266224800200408201018800830200138A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"20", "00E00000266224800200408201018800830200148A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"21", "00E00000266224800200408201018800830200158A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"22", "00E00000266224800200408201018800830200168A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"23", "00E00000266224800200408201018800830200178A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"24", "00E00000266224800200408201018800830200188A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"25", "00E00000266224800200408201018800830200198A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
	        {"26", "00E000002662248002004082010188008302001A8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"27", "00E000002662248002004082010188008302001B8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"28", "00E000002662248002004082010188008302001C8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"29", "00e000002662248002004082010188008302001D8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"30", "00E000002662248002004082010188008302001E8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"31", "00E000002662248002004082010188008302001F8A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"32", "00E00000266224800200408201018800830200208A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"33", "00E00000266224800200408201018800830200218A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"34", "00E00000266224800200408201018800830200228A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"35", "00E00000266224800200408201018800830200238A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"},
            {"36", "00E00000266224800200408201018800830200248A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE"}
	    };


          public const string CreateAgencyCommandSSS =        "00E00000266224800214008201018800830253018A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE";
          public const string CreateAgencyCommandGSIS =       "00E00000266224800214008201018800830247018A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE";
          public const string CreateAgencyCommandPAGIBIG =    "00E00000266224800214008201018800830250018A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE";
          public const string CreateAgencyCommandPHILHEALTH = "00E00000266224800214008201018800830249018A0101A1128C077BAEAEAEAECECE9C077BAEAEAEAECECE";
         
        //Shared File
        public static int[,] SharedEF = 
        {
            {1,68}, {2,1}, {3,373}, {4,1536},{5,76},{6,15630},{7,4100},{8,333},{9,256},
        };

        //start 5301 = 21249 [53=S]
        public static int[,] SSSEF = 
        {
          {1,1}, {2,512}, {3,256}, {4,256},{5,256},{6,256},{7,128},{8,128},{9,128},{10,128},
          {11,128}, {12,128}, {13,128}, {14,128},{15,128},{16,128},{17,64},{18,64},{19,64},{20,64},
          {21,64}, {22,64}, {23,64}, {24,64},{25,64},{26,64},{27,64},{28,64},{29,64},{37,64},
          {38,64}, {32,64}, {33,64}, {34,64},{35,64},{36,64}
        };

        //start 4701 = 18177 [47=G]
        public static int[,] GSISEF = 
        {
          {1,1}, {2,512}, {3,256}, {4,256},{5,256},{6,256},{7,128},{8,128},{9,128},{10,128},
          {11,128}, {12,128}, {13,128}, {14,128},{15,128},{16,128},{17,64},{18,64},{19,64},{20,64},
          {21,64}, {22,64}, {23,64}, {24,64},{25,64},{26,64},{27,64},{28,64},{29,64},{30,64},
          {31,64}, {32,64}, {33,64}, {34,64},{35,64},{36,64}
        };

        //start 5001 = 20481 [50=P]
        public static  int[,] PHILHEALTHEF = 
        {
            {20481,1}, {20482,512}, {20483,256}, {20484,256},{20485,256},{20486,256},{20487,128},{20488,128},{20489,128},{20490,128},
            {20491,128}, {20492,128}, {20493,128}, {20494,128},{20495,128},{20496,128},{20497,64},{20498,64},{20499,64},{20500,64},
            {20501,64}, {20502,64}, {20503,64}, {20504,64},{20505,64},{20506,64},{20507,64},{20508,64},{20509,64},{20510,64},
            {20511,64}, {20512,64}, {20513,64}, {20514,64},{20515,64},{20516,64}
        };

        //start 4901 = 18689 [49=I]
        public static  int[,] PAGIBIGEF = 
        {
            {18689,1}, {18690,512}, {18691,256}, {18692,256},{18693,256},{18694,256},{18695,128},{18696,128},{18697,128},{18698,128},
            {18699,128}, {18700,128}, {18701,128}, {18702,128},{18703,128},{18704,128},{18705,64},{18706,64},{18707,64},{18708,64},
            {18709,64}, {18710,64}, {18711,64}, {18712,64},{18713,64},{18714,64},{18715,64},{18716,64},{18717,64},{18718 ,64},
            {18719,64}, {18720,64}, {18721,64}, {18722,64},{18723,64},{18724,64}
        };
    }
}
