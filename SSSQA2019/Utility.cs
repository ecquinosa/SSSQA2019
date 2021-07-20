using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace ChipCodingDLL.Utilities
{
  public  class Utility
    {
        private static byte[] TMK = { };
        private static byte[] sharedkey = Encoding.UTF8.GetBytes("4LLC4RDT3CHN0L0G13502013");
        private static byte[] sharedvector = Encoding.UTF8.GetBytes("4LLC4RD5");

        /// <summary>
        /// Get Random Hex Number
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private string RandomHexString(int len)
        {
            // 64 character precision or 256-bits
            Random rdm = new Random();
            string hexValue = string.Empty;
            int num;

            for (int i = 0; i < len; i++)
            {
                num = rdm.Next(0, int.MaxValue);
                hexValue += num.ToString("X8");
            }

            return hexValue;
        }

        private Random random = new Random();
        /// <summary>
        /// Get Random Hex Number
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        public byte[] HexStringtoByte(string HexStringData)
        {
            byte[] Returnbyte;
            string[] data = Spacer(HexStringData, 2).Split(' ');

            Returnbyte = new byte[data.Length - 1];
            int counter = 0;
            for (int i = 0; i < Returnbyte.Length; i++)
            {
                string str = data[i];
                str = str.Replace(" ", "");
                if (str != null && str != "")
                {
                    try
                    {
                        byte floatVals = Convert.ToByte(str, 16);
                        Returnbyte[counter] = floatVals;
                    }
                    catch { }

                }
                counter++;
            }
            return Returnbyte;
        }

        public String Encrypt(String HEXDATA, CipherMode mode, ref string Vector, string KEY)
        {
            TMK = HexStringtoByte(KEY);
            sharedvector = HexStringtoByte(Vector);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = mode;
            tdes.Padding = PaddingMode.Zeros;
            byte[] toEncrypt = HexStringtoByte(HEXDATA);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(TMK, sharedvector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
            Vector = CByteToBCD(tdes.IV);
            return CByteToBCD(ms.ToArray()).ToUpper();
        }

        public string CByteToBCD(byte[] data) 
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString().ToUpper();
        }

        public string CByteToBCD(byte data)
        {
            StringBuilder sb = new StringBuilder(2);
            sb.AppendFormat("{0:x2}", data);
            return sb.ToString().ToUpper();
        }

        public byte[] CBcdToByte(string BCDData)
        {
            BCDData = BCDData.ToUpper();  
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(i.ToString("X2").ToUpper(), (byte)i);

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < BCDData.Length; i += 2)
                hexres.Add(hexindex[BCDData.Substring(i, 2)]);

            return hexres.ToArray();
        }

        public string CByteToStringHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:x2} ", b);
            }
            return sb.ToString();
        }

        public string Spacer(string str, int b4space)
        {
            string temp = null;
            string s = null;
            if (str != null)
            {
                for (int i = 0; i <= str.Length - 2; i += b4space)
                {
                    temp = str.Substring(i, b4space) + " ";
                    s = s + temp;
                }
                if (s != null)
                {
                    str = s.Trim();
                }
            }
            return str + " ";
        }

        public byte[] CHexToByte(string HexStringData)
        {
            byte[] Returnbyte;
            string[] data = Spacer(HexStringData, 2).Split(' ');

            Returnbyte = new byte[data.Length - 1];
            int counter = 0;
            for (int i = 0; i < Returnbyte.Length; i++)
            {
                string str = data[i];
                str = str.Replace(" ", "");
                if (str != null && str != "")
                {
                    try
                    {
                        byte floatVals = Convert.ToByte(str, 16);
                        Returnbyte[counter] = floatVals;
                    }
                    catch { }

                }
                counter++;
            }
            return Returnbyte;
        }

        public string CByteToAscii(byte[] Data)
        {
            string result = Encoding.ASCII.GetString(Data);
            return result; 
        }

        public byte[] CAsciiToByte(string AsciiData)
        {
            byte[] result = Encoding.ASCII.GetBytes(AsciiData);
            return result;
        }

        public string CAsciiToHexString(string AsciiData) 
        {
            StringBuilder sb = new StringBuilder();
            byte[] input = Encoding.UTF8.GetBytes(AsciiData);
            foreach (byte b in input) 
            {
                sb.Append(string.Format("{0:x2}", b));  
            }
            return sb.ToString(); 
        }

        public string DecToAmntBcdtFormat(decimal amount) 
        {
            string stramount = Decimal.Parse(amount.ToString("##,0.00")).ToString();
            stramount = stramount.Replace(".", "");
            stramount = stramount.Replace(",", "");
            stramount = stramount.PadLeft(12, '0');
            return stramount; 
        }

        public decimal AmntBcdToDecFormat(string BcdAmount) 
        {
            BcdAmount = BcdAmount.Insert(BcdAmount.Length - 2, ".");
            return Convert.ToDecimal(BcdAmount);
        }

        public bool isdecimal(string data)
        {
            string tString = data.Replace(".","");
            tString = tString.Replace(",", "");
            if (tString.Trim() == "") return false;
            for (int i = 0; i < tString.Length; i++)
            {
                if (!char.IsNumber(tString[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }

        public static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }

        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>byte</returns>
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }

        public byte[] SDES(byte[] Data, byte[] Key, CipherMode mode, ref byte[] Vector, PaddingMode padding) 
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            cryptoProvider.Mode = mode;
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
            cryptoProvider.CreateEncryptor(Key, Vector), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(Data);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            Vector = cryptoProvider.IV;
            writer.Flush();
            return memoryStream.ToArray();
        }

        public byte[] ISO97971M2_ALG3(byte[] Kmac, byte[] Data, ref byte[] Init_Vec)
        {
            //byte[] Kmac = CHexToByte(Key_MAC);

            // Split the 16 byte MAC key into two keys
            byte[] key1 = new byte[8];
            Array.Copy(Kmac, 0, key1, 0, 8);
            byte[] key2 = new byte[8];
            Array.Copy(Kmac, 8, key2, 0, 8);

            DES des1 = DES.Create();
            des1.BlockSize = 64;
            des1.Key = key1;
            des1.Mode = CipherMode.CBC;
            des1.Padding = PaddingMode.None;
            des1.IV = new byte[8];

            DES des2 = DES.Create();
            des2.BlockSize = 64;
            des2.Key = key2;
            des2.Mode = CipherMode.CBC;
            des2.Padding = PaddingMode.None;
            des2.IV = new byte[8];

            //// Padd the data with Padding Method 2 (Bit Padding)
            //System.IO.MemoryStream out_Renamed = new System.IO.MemoryStream();
            //out_Renamed.Write(eIFD, 0, eIFD.Length);
            //out_Renamed.WriteByte((byte)(0x80));
            //while (out_Renamed.Length % 8 != 0)
            //{
            //    out_Renamed.WriteByte((byte)0x00);
            //}
            //byte[] eIfd_padded = out_Renamed.ToArray();
            //int N_bytes = eIfd_padded.Length / 8;  // Number of Bytes 
            int N_bytes = Data.Length / 8;  // Number of Bytes 

            byte[] d1 = new byte[8];
            byte[] dN = new byte[8];
            byte[] hN = new byte[8];
            byte[] intN = new byte[8];

            // MAC Algorithm 3
            // Initial Transformation 1
            Array.Copy(Data, 0, d1, 0, 8);
            hN = des1.CreateEncryptor().TransformFinalBlock(d1, 0, 8);

            // Split the blocks
            // Iteration on the rest of blocks
            for (int j = 1; j < N_bytes; j++)
            {
                Array.Copy(Data, (8 * j), dN, 0, 8);
                // XOR
                for (int i = 0; i < 8; i++)
                    intN[i] = (byte)(hN[i] ^ dN[i]);

                // Encrypt
                hN = des1.CreateEncryptor().TransformFinalBlock(intN, 0, 8);
            }

            // Output Transformation 3
            byte[] hNdecrypt = des2.CreateDecryptor().TransformFinalBlock(hN, 0, 8);
            byte[] mIfd = des1.CreateEncryptor().TransformFinalBlock(hNdecrypt, 0, 8);

            //  Get check Sum CC
            return mIfd;
        }

        public byte[] DES3_CBC(byte[] Data, byte[] Key, CipherMode mode, ref byte[] Vector, PaddingMode padding)
        {
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = mode;
            tdes.Padding = padding;
            byte[] toEncrypt = Data;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(Key, Vector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
            Vector = tdes.IV;
            return ms.ToArray();
        }

        public byte[] CBC_MAC(string Key, string Data, string Init_Vec)
        {
            byte[] Kmac = CHexToByte(Key);
            byte[] eIFD = CHexToByte(Data);
            // Split the 16 byte MAC key into two keys
            byte[] key1 = new byte[8];
            Array.Copy(Kmac, 0, key1, 0, 8);
            byte[] key2 = new byte[8];
            Array.Copy(Kmac, 8, key2, 0, 8);

            DES des1 = DES.Create();
            des1.BlockSize = 64;
            des1.Key = key1;
            des1.Mode = CipherMode.CBC;
            des1.Padding = PaddingMode.None;
            des1.IV = new byte[8];

            DES des2 = DES.Create();
            des2.BlockSize = 64;
            des2.Key = key2;
            des2.Mode = CipherMode.CBC;
            des2.Padding = PaddingMode.None;
            des2.IV = new byte[8];

            // Padd the data with Padding Method 2 (Bit Padding)
            System.IO.MemoryStream out_Renamed = new System.IO.MemoryStream();
            out_Renamed.Write(eIFD, 0, eIFD.Length);
            out_Renamed.WriteByte((byte)(0x80));
            while (out_Renamed.Length % 8 != 0)
            {
                out_Renamed.WriteByte((byte)0x00);
            }
            byte[] eIfd_padded = out_Renamed.ToArray();
            int N_bytes = eIfd_padded.Length / 8;  // Number of Bytes 

            byte[] d1 = new byte[8];
            byte[] dN = new byte[8];
            byte[] hN = new byte[8];
            byte[] intN = new byte[8];

            // MAC Algorithm 3
            // Initial Transformation 1
            Array.Copy(eIfd_padded, 0, d1, 0, 8);
            hN = des1.CreateEncryptor().TransformFinalBlock(d1, 0, 8);

            // Split the blocks
            // Iteration on the rest of blocks
            for (int j = 1; j < N_bytes; j++)
            {
                Array.Copy(eIfd_padded, (8 * j), dN, 0, 8);
                // XOR
                for (int i = 0; i < 8; i++)
                    intN[i] = (byte)(hN[i] ^ dN[i]);

                // Encrypt
                hN = des1.CreateEncryptor().TransformFinalBlock(intN, 0, 8);
            }

            // Output Transformation 3
            byte[] hNdecrypt = des2.CreateDecryptor().TransformFinalBlock(hN, 0, 8);
            byte[] mIfd = des1.CreateEncryptor().TransformFinalBlock(hNdecrypt, 0, 8);

            //  Get check Sum CC
            return mIfd;
        }

      /// <summary>
      /// Convert Decimal to Hex
      /// </summary>
      /// <param name="dec"></param>
      /// <returns></returns>
        public string CDectoHex(int dec) 
        {
            return dec.ToString("X").PadLeft(2,'0');  
        }

      /// <summary>
      /// Slice Byte;
      /// </summary>
      /// <param name="inputbuffer"></param>
      /// <returns></returns>
        public byte[][] SliceByte(byte[] inputbuffer)
        {
            decimal len = inputbuffer.Length / 255;
            int remainder = inputbuffer.Length - (Convert.ToInt32(len) * 255);
            if (remainder != 0) len++;

            byte[][] bytearray = new byte[Convert.ToInt32( len)][];
            int offset = 0;
            int Lent = 0;
            //if (len == 1)
            //{
            //    if (inputbuffer.Length < 255)
            //    {
            //        Lent = inputbuffer.Length;
            //    }
            //    else { Lent = 255; }
            //}
            //else
            //{
            //    Lent = 255;

            //}

            for (int x = 0; x < bytearray.Length; x++)
            {
                if ((offset + 255) < inputbuffer.Length)
                {
                    Lent = 255;
                }
                else 
                {
                    Lent = inputbuffer.Length - offset;
                } 
                byte[] slice = new byte[Lent];
                Buffer.BlockCopy(inputbuffer, offset, slice, 0, Lent);
                bytearray[x] = slice;
                offset += Lent;
            }
            return bytearray;
        }

    }
}
