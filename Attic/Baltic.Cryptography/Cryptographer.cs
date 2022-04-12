using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Serilog;

namespace Baltic.Cryptography
{
    public class Cryptographer
    {
        public readonly string CertificatePath;
        public readonly string CertificatePassword;
        private readonly X509Certificate2 _certificate;

        public Cryptographer(string certificatePath, string certificatePassword)
        {
            CertificatePath = certificatePath;
            CertificatePassword = certificatePassword;
            try
            {
                _certificate = new X509Certificate2(CertificatePath, CertificatePassword);
            }
            catch (CryptographicException e)
            {
                Log.Debug("Certificate {certificatePath} is improper. Error message: {message}", CertificatePath,
                    e.Message);
                throw new CertificateException(e.Message, certificatePath, ErrorType.CertificateError);
            }

            if (!Verify(CertificatePath, CertificatePassword))
            {
                Log.Debug("Certificate {certificatePath} is not valid.", CertificatePath);
                throw new CertificateException("Certificate is not valid.", CertificatePath, ErrorType.ValidationError);
            }
        }

        public string Encrypt(string contentToEncrypt)
        {
            var rsa = (RSA) _certificate.PrivateKey;
            (_certificate.PrivateKey as RSACng)?.Key.SetProperty(
                new CngProperty(
                    "Export Policy",
                    BitConverter.GetBytes((int) CngExportPolicies.AllowPlaintextExport),
                    CngPropertyOptions.Persist));

            var rsaParameters = rsa.ExportParameters(true);
            var csp = RSA.Create(rsaParameters);
            var bytesData = Encoding.UTF8.GetBytes(contentToEncrypt);
            var bytesEncrypted = csp.Encrypt(bytesData, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(bytesEncrypted);
        }

        public string Decrypt(string encryptedContent)
        {
            var rsa = (RSA) _certificate.PrivateKey;
            (_certificate.PrivateKey as RSACng)?.Key.SetProperty(
                new CngProperty(
                    "Export Policy",
                    BitConverter.GetBytes((int) CngExportPolicies.AllowPlaintextExport),
                    CngPropertyOptions.Persist));

            var rsaParameters = rsa.ExportParameters(true);
            var csp = RSA.Create(rsaParameters);
            var bytesEncrypted = Convert.FromBase64String(encryptedContent);
            var bytesDecrypted = csp.Decrypt(bytesEncrypted, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        public static bool Verify(string certificatePath)
        {
            X509Certificate2 cert = null;
            try
            {
                cert = new X509Certificate2(certificatePath);
            }
            catch (CryptographicException e)
            {
                throw new CertificateException(e.Message, certificatePath, ErrorType.CertificateError);
            }

            return cert.Verify();
        }

        public static bool Verify(string certificatePath, string certificatePassword)
        {
            X509Certificate2 cert = null;
            try
            {
                cert = new X509Certificate2(certificatePath, certificatePassword);
            }
            catch (CryptographicException e)
            {
                throw new CertificateException(e.Message, certificatePath, ErrorType.CertificateError);
            }

            return cert.Verify();
        }
    }
}