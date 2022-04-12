using System;
using Baltic.Cryptography;

namespace Test.Cryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Cryptographer.Verify(@"C:\Users\rados\Desktop\certyfikaty\balticlsc_ee_pw_edu_pl.crt"));
            Console.WriteLine(Cryptographer.Verify(@"C:\Users\rados\Desktop\certyfikaty\DigiCertCA.crt"));
            Console.WriteLine(Cryptographer.Verify(@"C:\Users\rados\Desktop\certyfikaty\certificate.crt"));
            Console.WriteLine(Cryptographer.Verify(@"C:\Users\rados\Desktop\certyfikaty\domain.pfx", "balticlsc"));

            const string certificatePath = @"C:\Users\rados\Desktop\certyfikaty\domain.pfx";
            const string certificatePassword = "balticlsc";

            var crypto = new Cryptographer(certificatePath, certificatePassword);

            var output = crypto.Encrypt("Hello world");
            Console.WriteLine(output);
            Console.WriteLine();
            Console.WriteLine(crypto.Decrypt(output));
            Console.WriteLine();
        }
    }
}