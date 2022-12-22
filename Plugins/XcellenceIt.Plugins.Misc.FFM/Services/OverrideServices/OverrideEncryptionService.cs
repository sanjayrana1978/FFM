using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Security;
using Nop.Services.Logging;
using Nop.Services.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XcellenceIt.Plugins.Misc.FFM.Services.OverrideServices
{
    public class OverrideEncryptionService : EncryptionService
    {
        #region Fields

        private readonly SecuritySettings _securitySettings;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public OverrideEncryptionService(SecuritySettings securitySettings,
            IStaticCacheManager staticCacheManager,
            ILogger logger) : base(securitySettings)
        {
            _securitySettings = securitySettings;
            _staticCacheManager = staticCacheManager;
            _logger = logger;
        }

        #endregion

        #region Utilities

        private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, TripleDES.Create().CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                var toEncrypt = Encoding.Unicode.GetBytes(data);
                cs.Write(toEncrypt, 0, toEncrypt.Length);
                cs.FlushFinalBlock();
            }

            return ms.ToArray();
        }

        private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            using var ms = new MemoryStream(data);
            using var cs = new CryptoStream(ms, TripleDES.Create().CreateDecryptor(key, iv), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.Unicode);
            return sr.ReadToEnd();
        }

        protected bool IsBase64StringAsync(string base64, bool isEncrypting = false)
        {
            //_logger.InsertLogAsync(LogLevel.Information, base64).GetAwaiter().GetResult();

            if (string.IsNullOrEmpty(base64))
                return false;
            base64 = base64.Trim();
            if (isEncrypting && int.TryParse(base64, out _))
            {
                return false;
            }

            if (base64.Length % 4 == 0)
            {
                var baseStringChecked = false;
                if (!baseStringChecked && !Regex.IsMatch(base64, @"^(?=(.*\d){1,}).{12,}$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){0,})(?=(.*[a-z]){3,})(?=(.*[A-Z]){3,})(?=(.*[+/=]?)).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                if (!baseStringChecked && !Regex.IsMatch(base64, @"^(?=(.*\d){2,}).{12,}$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){1,})(?=(.*[a-z]){3,})(?=(.*[A-Z]){3,})(?=(.*[+/=]){1,}).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                if (!baseStringChecked && !Regex.IsMatch(base64, @"^(?=(.*[+/=]){1,})$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){2,})(?=(.*[a-z]){2,})(?=(.*[A-Z]){3,})(?=(.*[+/=]?)).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*[+/=]){1,})$", RegexOptions.Singleline))
                {
                    if (!baseStringChecked && Regex.IsMatch(base64, @"^(?=(.*\d){2,})(?=(.*[a-z]){3,})(?=(.*[A-Z]){3,})(?=(.*[+/=]){1,}).{12,}$", RegexOptions.Singleline))
                        baseStringChecked = true;
                }
                return baseStringChecked;
            }
            return false;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        public override string EncryptText(string plainText, string encryptionPrivateKey = "")
        {
            if (string.IsNullOrEmpty(plainText) || IsBase64StringAsync(plainText, true))
                return plainText;

            if (string.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerEncryption.EncryptCustomerByGuidCacheKey, plainText);
            return _staticCacheManager.Get(cacheKey, () =>
            {
                using var provider = TripleDES.Create();
                provider.Key = Encoding.ASCII.GetBytes(encryptionPrivateKey[0..16]);
                provider.IV = Encoding.ASCII.GetBytes(encryptionPrivateKey[8..16]);

                var encryptedBinary = EncryptTextToMemory(plainText, provider.Key, provider.IV);
                return Convert.ToBase64String(encryptedBinary);
            });
        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        public override string DecryptText(string cipherText, string encryptionPrivateKey = "")
        {
            if (string.IsNullOrEmpty(cipherText) || !IsBase64StringAsync(cipherText))
                return cipherText;

            if (string.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _securitySettings.EncryptionKey;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCustomerEncryption.DecryptCustomerByGuidCacheKey, cipherText);
            return _staticCacheManager.Get(cacheKey, () =>
            {
                using var provider = TripleDES.Create();
                provider.Key = Encoding.ASCII.GetBytes(encryptionPrivateKey[0..16]);
                provider.IV = Encoding.ASCII.GetBytes(encryptionPrivateKey[8..16]);

                var buffer = Convert.FromBase64String(cipherText);
                return DecryptTextFromMemory(buffer, provider.Key, provider.IV);
            });
        }

        #endregion
    }
}
