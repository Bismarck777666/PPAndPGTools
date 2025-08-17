using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GITIGamingWebsite.Common
{
    public class AES
    {
        public byte[] convertBase64ToByteArray(string base64Text)
        {
            byte[] bytes        = Convert.FromBase64String(base64Text);
            byte[] newBytes     = new byte[bytes.Length];
            for(int i = 0; i < bytes.Length; i++)
            {
                newBytes[i] = bytes[i];
            }
            return newBytes;
        }

        public string convertBase64ToHexString(string base64Text)
        {
            byte[] bytes        = Convert.FromBase64String(base64Text);
            byte[] newBytes     = new byte[bytes.Length];
            for(int i = 0; i < bytes.Length; i++)
            {
                newBytes[i] = bytes[i];
            }
            return BitConverter.ToString(newBytes).Replace("-", "");
        }

        public string doDecrypt(string textToDecrypt,byte[] DecryptKey, byte[] DecryptIV)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize         = 128; //AES128로 사용시 
                aes.FeedbackSize    = 128;
                aes.Mode            = CipherMode.CBC;
                aes.Padding         = PaddingMode.PKCS7;
                aes.Key             = DecryptKey;
                aes.IV              = DecryptIV;

                var decrypt = aes.CreateDecryptor();
                string Output = null;
                using (var ms = new MemoryStream(Convert.FromBase64String(textToDecrypt)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            Output = srDecrypt.ReadToEnd();
                        }
                    }
                }
                return Output;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception has been occured in decrypt: {0}", ex.Message);
                return string.Empty;
            }
        }
        
        public string doEncrypt(string textToEncrypt,byte[] EncryptKey,byte[] EncryptIV)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize         = 128; //AES128로 사용시 
                aes.FeedbackSize    = 128;
                aes.Mode            = CipherMode.CBC;
                aes.Padding         = PaddingMode.PKCS7;
                aes.Key             = EncryptKey;
                aes.IV              = EncryptIV;

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] buf = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(textToEncrypt);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    buf = ms.ToArray();
                }
                string Output = Convert.ToBase64String(buf);
                return Output;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception has been occured in encrypt: {0}", ex.Message);
                return ex.Message;
            }
        }

        public byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}