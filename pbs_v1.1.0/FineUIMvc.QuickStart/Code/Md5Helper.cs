using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Configuration;

namespace FineUIMvc.QuickStart.Code
{
    public class Md5Helper
    {
        public static string Encrypt(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("X").PadLeft(2, '0');
            }
            return str;
        }

        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <param name="encryptKey">密匙（8位）</param>
        /// <returns></returns>
        public static string DESEncrypt(string encryptString)
        {

            string encryptKey = ConfigurationManager.AppSettings["encryptKey"];
            string returnValue;
            try
            {
                byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                DESCryptoServiceProvider dES = new DESCryptoServiceProvider();
                byte[] byteEncrypt = Encoding.Default.GetBytes(encryptString);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, dES.CreateEncryptor(Encoding.Default.GetBytes(encryptKey), temp), CryptoStreamMode.Write);
                cryptoStream.Write(byteEncrypt, 0, byteEncrypt.Length);
                cryptoStream.FlushFinalBlock();
                returnValue = Convert.ToBase64String(memoryStream.ToArray());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">密文</param>
        /// <param name="decryptKey">密匙（8位）</param>
        /// <returns></returns>
        public static string DESDecrypt(string decryptString)
        {
            string decryptKey = ConfigurationManager.AppSettings["encryptKey"];
            string returnValue;
            try
            {
                byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                DESCryptoServiceProvider dES = new DESCryptoServiceProvider();
                byte[] byteDecryptString = Convert.FromBase64String(decryptString);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, dES.CreateDecryptor(Encoding.Default.GetBytes(decryptKey), temp), CryptoStreamMode.Write);

                cryptoStream.Write(byteDecryptString, 0, byteDecryptString.Length);

                cryptoStream.FlushFinalBlock();

                returnValue = Encoding.Default.GetString(memoryStream.ToArray());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;

        }

        public static DateTime GMT2Local(string gmt)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                string pattern = "";
                if (gmt.IndexOf("+0") != -1)
                {
                    gmt = gmt.Replace("GMT", "");
                    pattern = "ddd MMM dd yyyy HH':'mm':'ss zzz";
                }
                if (gmt.ToUpper().IndexOf("GMT") != -1)
                {
                    gmt = gmt.Replace("GMT ", "+");
                    pattern = "ddd MMM dd yyyy HH':'mm':'ss zzz";
                }
                if (pattern != "")
                {
                    dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                    dt = dt.ToLocalTime();
                }
                else
                {
                    dt = Convert.ToDateTime(gmt);
                }
            }
            catch
            {
            }
            return dt;
        }


    }
}