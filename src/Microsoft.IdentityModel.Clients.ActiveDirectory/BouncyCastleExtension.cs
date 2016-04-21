using System;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Sandboxable
{
    public static class BouncyCastleExtension
    {
        public static AsymmetricKeyParameter PrivateKey(this Pkcs12Store store, string alias = null)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                alias = store.Aliases.Cast<string>().First(store.IsKeyEntry);
            }

            return store.GetKey(alias).Key;
        }

        public static X509Certificate PublicKey(this Pkcs12Store store, string alias = null)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                alias = store.Aliases.Cast<string>().First(store.IsKeyEntry);
            }

            return store.GetCertificateChain(alias).First().Certificate;
        }

        public static byte[] GetCertHash(this X509Certificate certificate, string algorithm = "SHA-1")
        {
            var digest = DigestUtilities.GetDigest(algorithm);

            var encodedCertificate = certificate.GetEncoded();

            digest.BlockUpdate(encodedCertificate, 0, encodedCertificate.Length);

            var output = new byte[digest.GetDigestSize()];

            digest.DoFinal(output, 0);

            return output;
        }

        public static byte[] SignData(this AsymmetricKeyParameter asymmetricKeyParameter, byte[] buffer, string algorithm = "SHA256withRSA")
        {
            if (asymmetricKeyParameter == null)
            {
                throw new ArgumentNullException(nameof(asymmetricKeyParameter));
            }

            var signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(true, asymmetricKeyParameter);
            signer.BlockUpdate(buffer, 0, buffer.Length);

            return signer.GenerateSignature();
        }
    }
}