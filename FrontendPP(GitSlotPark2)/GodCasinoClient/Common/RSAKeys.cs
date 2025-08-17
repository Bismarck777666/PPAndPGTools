using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GITIGamingWebsite.Common
{
    public class RSAKeys
    {
        public RsaPrivateCrtKeyParameters ImportPrivateKey(string rsaPrivateKey)
        {
            var byteArray = Encoding.UTF8.GetBytes(rsaPrivateKey);
            using (var ms = new MemoryStream(byteArray))
            {
                using (var sr = new StreamReader(ms))
                {
                    var pemReader   = new Org.BouncyCastle.Utilities.IO.Pem.PemReader(sr);
                    var pem         = pemReader.ReadPemObject();
                    var privateKey  = PrivateKeyFactory.CreateKey(pem.Content);

                    return (privateKey as RsaPrivateCrtKeyParameters);
                }
            }
        }

        public RsaKeyParameters ImportPublicKey(string pem)
        {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            return (RsaKeyParameters)publicKey;
        }

        public byte[] EncryptData(ICipherParameters keyParam, string data)
        {
            var cipher = CipherUtilities.GetCipher("RSA/NONE/PKCS1PADDING");
            cipher.Init(true, keyParam); // true == 암호화

            byte[] plain = Encoding.UTF8.GetBytes(data);
            return cipher.DoFinal(plain);
        }

        public string DecryptData(ICipherParameters keyParam, byte[] encrypted)
        {
            var cipher = CipherUtilities.GetCipher("RSA/NONE/PKCS1PADDING");
            cipher.Init(false, keyParam); // false == 복호화

            return Encoding.UTF8.GetString(cipher.DoFinal(encrypted));
        }
    }
}