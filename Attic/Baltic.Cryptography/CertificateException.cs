using System;
using static System.String;

namespace Baltic.Cryptography
{
    public class CertificateException : Exception
    {
        private string CertificatePath { get; }
        private ErrorType ErrorType { get; }

        public CertificateException(string message, string certificatePath, ErrorType errorType) : base(message)
        {
            CertificatePath = certificatePath;
            ErrorType = errorType;
        }
    }

    public enum ErrorType
    {
        CertificateError,        //for example when certificate file not found, incorrect password, certificate file is improper (file is not proper certificate or something like that) 
        ValidationError          //when Verify() return false - invalid certificate
    }
}