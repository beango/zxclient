using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Common.util
{
    public class EDncryptHelper
    {
        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="str">要加密的string</param>
        /// <returns>密文</returns>
        public static string MD5Encrypt16(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(str)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="str">要加密的string</param>
        /// <returns>密文</returns>
        public static string MD5Encrypt(string str)
        {
            string pwd = null;
            MD5 m = MD5.Create();
            byte[] s = m.ComputeHash(Encoding.Unicode.GetBytes(str));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }
        /// <summary>
        ///  Md5加密
        /// </summary>
        /// <param name="pToEncrypt">要加密的string</param>
        /// <param name="sKey">要加密的key</param>
        /// <returns></returns>
        public static string MD5Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length); cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }
        /// <summary>
            ///  Md5解密
            /// </summary>
            /// <param name="pToEncrypt">解密string</param>
            /// <param name="sKey">解密key(要8位数)</param>
            /// <returns></returns>
        public static string MD5Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
    }
    /**//// <summary>
    /// MD5
    /// </summary>
    public class MD5_
    {
        public MD5_()
        {
        }
        public static string Encrypt(string Source)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(Source);
            // This is one implementation of the abstract class MD5.
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            return Convert.ToBase64String(result);

        }
        public static byte[] Encrypt(byte[] Source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Source);
            return result;
        }
    }
    /**//// <summary>
    /// DES
    /// </summary>
    public class DES_
    {
        private DES mydes;
        public string Key;
        public string IV;
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public DES_(string key)
        {
            mydes = new DESCryptoServiceProvider();
            Key = key;
            IV = "728#$$%^TyguyshdsufhsfwofnhKJHJKHIYhfiusf98*(^%$^&&(*&()$##@%%$RHGJJHHJ";
        }
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public DES_(string key, string iv)
        {
            mydes = new DESCryptoServiceProvider();
            Key = key;
            IV = iv;
        }
        /**//// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = Key;
            mydes.GenerateKey();
            byte[] bytTemp = mydes.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = IV;
            mydes.GenerateIV();
            byte[] bytTemp = mydes.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public string Encrypt(string Source)
        {
            try
            {
                byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
                MemoryStream ms = new MemoryStream();
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String(bytOut);
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public string Decrypt(string Source)
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 加密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待加密的byte数组</param>
        /// <returns>经过加密的byte数组</returns>
        public byte[] Encrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream();
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return bytOut;
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待解密的byte数组</param>
        /// <returns>经过解密的byte数组</returns>
        public byte[] Decrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }


        /**//// <summary>
        /// 加密方法File to File
        /// </summary>
        /// <param name="inFileName">待加密文件的路径</param>
        /// <param name="outFileName">待加密后文件的输出路径</param>

        public void Encrypt(string inFileName, string outFileName)
        {
            try
            {

                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;

                ICryptoTransform encrypto = mydes.CreateEncryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法File to File
        /// </summary>
        /// <param name="inFileName">待解密文件的路径</param>
        /// <param name="outFileName">待解密后文件的输出路径</param>
        public void Decrypt(string inFileName, string outFileName)
        {
            try
            {
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateDecryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }

    }
    /**//// <summary>
    /// RC2
    /// </summary>
    public class RC2_
    {
        private RC2 rc;
        public string Key;
        public string IV;
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public RC2_(string key)
        {
            rc = new RC2CryptoServiceProvider();
            Key = key;
            IV = "#$^%&&*Yisifhsfjsljfslhgosdshf26382837sdfjskhf97(*&(*";
        }
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public RC2_(string key, string iv)
        {
            rc = new RC2CryptoServiceProvider();
            Key = key;
            IV = iv;
        }
        /**//// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = Key;
            rc.GenerateKey();
            byte[] bytTemp = rc.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = IV;
            rc.GenerateIV();
            byte[] bytTemp = rc.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public string Encrypt(string Source)
        {
            try
            {
                byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
                MemoryStream ms = new MemoryStream();
                rc.Key = GetLegalKey();
                rc.IV = GetLegalIV();
                ICryptoTransform encrypto = rc.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String(bytOut);
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public string Decrypt(string Source)
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                rc.Key = GetLegalKey();
                rc.IV = GetLegalIV();
                ICryptoTransform encrypto = rc.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 加密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待加密的byte数组</param>
        /// <returns>经过加密的byte数组</returns>
        public byte[] Encrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream();
                rc.Key = GetLegalKey();
                rc.IV = GetLegalIV();
                ICryptoTransform encrypto = rc.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return bytOut;
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待解密的byte数组</param>
        /// <returns>经过解密的byte数组</returns>
        public byte[] Decrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                rc.Key = GetLegalKey();
                rc.IV = GetLegalIV();
                ICryptoTransform encrypto = rc.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }


        /**//// <summary>
        /// 加密方法File to File
        /// </summary>
        /// <param name="inFileName">待加密文件的路径</param>
        /// <param name="outFileName">待加密后文件的输出路径</param>

        public void Encrypt(string inFileName, string outFileName)
        {
            try
            {

                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                rc.Key = GetLegalKey();
                rc.IV = GetLegalIV();

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;

                ICryptoTransform encrypto = rc.CreateEncryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法File to File
        /// </summary>
        /// <param name="inFileName">待解密文件的路径</param>
        /// <param name="outFileName">待解密后文件的输出路径</param>
        public void Decrypt(string inFileName, string outFileName)
        {
            try
            {
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                rc.Key = GetLegalKey();
                rc.IV = GetLegalIV();
                ICryptoTransform encrypto = rc.CreateDecryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }

    }
    /**//// <summary>
    /// Rijndael
    /// </summary>
    public class Rijndael_
    {

        private RijndaelManaged myRijndael;
        public string Key;
        public string IV;
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public Rijndael_(string key)
        {
            myRijndael = new RijndaelManaged();
            Key = key;
            IV = "67^%*(&(*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
        }
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public Rijndael_(string key, string iv)
        {
            myRijndael = new RijndaelManaged();
            Key = key;
            IV = iv;
        }
        /**//// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = Key;
            myRijndael.GenerateKey();
            byte[] bytTemp = myRijndael.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = IV;
            myRijndael.GenerateIV();
            byte[] bytTemp = myRijndael.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public string Encrypt(string Source)
        {
            try
            {
                byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
                MemoryStream ms = new MemoryStream();
                myRijndael.Key = GetLegalKey();
                myRijndael.IV = GetLegalIV();
                ICryptoTransform encrypto = myRijndael.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String(bytOut);
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public string Decrypt(string Source)
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                myRijndael.Key = GetLegalKey();
                myRijndael.IV = GetLegalIV();
                ICryptoTransform encrypto = myRijndael.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 加密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待加密的byte数组</param>
        /// <returns>经过加密的byte数组</returns>
        public byte[] Encrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream();
                myRijndael.Key = GetLegalKey();
                myRijndael.IV = GetLegalIV();
                ICryptoTransform encrypto = myRijndael.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return bytOut;
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待解密的byte数组</param>
        /// <returns>经过解密的byte数组</returns>
        public byte[] Decrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                myRijndael.Key = GetLegalKey();
                myRijndael.IV = GetLegalIV();
                ICryptoTransform encrypto = myRijndael.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }


        /**//// <summary>
        /// 加密方法File to File
        /// </summary>
        /// <param name="inFileName">待加密文件的路径</param>
        /// <param name="outFileName">待加密后文件的输出路径</param>

        public void Encrypt(string inFileName, string outFileName)
        {
            try
            {
                //byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
                //MemoryStream ms = new MemoryStream();

                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                myRijndael.Key = GetLegalKey();
                myRijndael.IV = GetLegalIV();

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;

                ICryptoTransform encrypto = myRijndael.CreateEncryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法File to File
        /// </summary>
        /// <param name="inFileName">待解密文件的路径</param>
        /// <param name="outFileName">待解密后文件的输出路径</param>
        public void Decrypt(string inFileName, string outFileName)
        {
            try
            {
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                myRijndael.Key = GetLegalKey();
                myRijndael.IV = GetLegalIV();
                ICryptoTransform encrypto = myRijndael.CreateDecryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }

    }
    /**//// <summary>
    /// 非对称RSA
    /// </summary>
    public class RSA_
    {
        private RSACryptoServiceProvider rsa;
        public RSA_()
        {
            rsa = new RSACryptoServiceProvider();
        }
        /**//// <summary>
        /// 得到公钥
        /// </summary>
        /// <returns></returns>
        public string GetPublicKey()
        {
            return rsa.ToXmlString(false);
        }
        /**//// <summary>
        /// 得到私钥
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            return rsa.ToXmlString(true);

        }
        /**//// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符串</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public string Encrypt(string Source, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            byte[] done = rsa.Encrypt(Convert.FromBase64String(Source), false);
            return Convert.ToBase64String(done);
        }
        /**//// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">待加密字符数组</param>
        /// <param name="PublicKey">公钥</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] Source, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            return rsa.Encrypt(Source, false);
        }
        /**//// <summary>
        /// 加密
        /// </summary>
        /// <param name="inFileName">待加密文件路径</param>
        /// <param name="outFileName">加密后文件路径</param>
        /// <param name="PublicKey">公钥</param>
        public void Encrypt(string inFileName, string outFileName, string PublicKey)
        {
            rsa.FromXmlString(PublicKey);
            FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            byte[] bin = new byte[1000];
            long rdlen = 0;
            long totlen = fin.Length;
            int len;

            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 1000);
                byte[] bout = rsa.Encrypt(bin, false);
                fout.Write(bout, 0, bout.Length);
                rdlen = rdlen + len;
            }

            fout.Close();
            fin.Close();

        }
        /**//// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符串</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public string Decrypt(string Source, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            byte[] done = rsa.Decrypt(Convert.FromBase64String(Source), false);
            return Convert.ToBase64String(done);
        }
        /**//// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">待解密字符数组</param>
        /// <param name="PrivateKey">私钥</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] Source, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            return rsa.Decrypt(Source, false);
        }
        /**//// <summary>
        /// 解密
        /// </summary>
        /// <param name="inFileName">待解密文件路径</param>
        /// <param name="outFileName">解密后文件路径</param>
        /// <param name="PrivateKey">私钥</param>
        public void Decrypt(string inFileName, string outFileName, string PrivateKey)
        {
            rsa.FromXmlString(PrivateKey);
            FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            byte[] bin = new byte[1000];
            long rdlen = 0;
            long totlen = fin.Length;
            int len;

            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 1000);
                byte[] bout = rsa.Decrypt(bin, false);
                fout.Write(bout, 0, bout.Length);
                rdlen = rdlen + len;
            }

            fout.Close();
            fin.Close();

        }
    }
    /**//// <summary>
    /// 三重DES
    /// </summary>
    public class TripleDES_
    {
        private TripleDES mydes;
        public string Key;
        public string IV;
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public TripleDES_(string key)
        {
            mydes = new TripleDESCryptoServiceProvider();
            Key = key;
            IV = "#$^%&&*Yisifhsfjsljfslhgosdshf26382837sdfjskhf97(*&(*";
        }
        /**//// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public TripleDES_(string key, string iv)
        {
            mydes = new TripleDESCryptoServiceProvider();
            Key = key;
            IV = iv;
        }
        /**//// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = Key;
            mydes.GenerateKey();
            byte[] bytTemp = mydes.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = IV;
            mydes.GenerateIV();
            byte[] bytTemp = mydes.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /**//// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public string Encrypt(string Source)
        {
            try
            {
                byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
                MemoryStream ms = new MemoryStream();
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String(bytOut);
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public string Decrypt(string Source)
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 加密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待加密的byte数组</param>
        /// <returns>经过加密的byte数组</returns>
        public byte[] Encrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream();
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return bytOut;
            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法byte[] to byte[]
        /// </summary>
        /// <param name="Source">待解密的byte数组</param>
        /// <returns>经过解密的byte数组</returns>
        public byte[] Decrypt(byte[] Source)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }


        /**//// <summary>
        /// 加密方法File to File
        /// </summary>
        /// <param name="inFileName">待加密文件的路径</param>
        /// <param name="outFileName">待加密后文件的输出路径</param>

        public void Encrypt(string inFileName, string outFileName)
        {
            try
            {

                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;

                ICryptoTransform encrypto = mydes.CreateEncryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件加密的时候出现错误！错误提示：  " + ex.Message);
            }
        }
        /**//// <summary>
        /// 解密方法File to File
        /// </summary>
        /// <param name="inFileName">待解密文件的路径</param>
        /// <param name="outFileName">待解密后文件的输出路径</param>
        public void Decrypt(string inFileName, string outFileName)
        {
            try
            {
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                mydes.Key = GetLegalKey();
                mydes.IV = GetLegalIV();
                ICryptoTransform encrypto = mydes.CreateDecryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen = rdlen + len;
                }
                cs.Close();
                fout.Close();
                fin.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("在文件解密的时候出现错误！错误提示：  " + ex.Message);
            }
        }

    }
}
