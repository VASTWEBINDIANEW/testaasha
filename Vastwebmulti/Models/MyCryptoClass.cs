using com.paygate.ag.common.utils;

namespace Vastwebmulti.Models
{
    public class MyCryptoClass
    {
        public string encrypt(string plainText, string privateKey)
        {
            string va = string.Empty;
            string encryptText = PayGateCryptoUtils.encrypt(plainText, privateKey);
            return encryptText;
        }
        public string decrypt(string encryptText, string privateKey)
        {
            string va = string.Empty;
            string decryptText = PayGateCryptoUtils.decrypt(encryptText, privateKey);
            return decryptText;
        }
    }
}