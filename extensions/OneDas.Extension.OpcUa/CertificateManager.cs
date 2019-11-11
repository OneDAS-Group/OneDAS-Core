using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OneDas.Extension.OpcUa
{
    public static class CertificateManager
    {
        public static async Task<X509Certificate2> CreateApplicationInstanceCertificate(ApplicationConfiguration configuration, ushort minimumKeySize = CertificateFactory.defaultKeySize, ushort lifeTimeInMonths = CertificateFactory.defaultLifeTime)
        {
            CertificateIdentifier id;
            IList<string> serverDomainNames;

            await CertificateManager.DeleteApplicationInstanceCertificate(configuration);

            id = configuration.SecurityConfiguration.ApplicationCertificate;
            serverDomainNames = configuration.GetServerDomainNames();

            if (serverDomainNames.Count == 0)
            {
                serverDomainNames.Add(Utils.GetHostName());
            }

            if (id.StoreType == CertificateStoreType.Directory)
            {
                Utils.GetAbsoluteDirectoryPath(id.StorePath, true, true, true);
            }

            X509Certificate2 certificate = CertificateFactory.CreateCertificate(
                id.StoreType,
                id.StorePath,
                null,
                configuration.ApplicationUri,
                configuration.ApplicationName,
                id.SubjectName,
                serverDomainNames,
                minimumKeySize,
                DateTime.UtcNow - TimeSpan.FromDays(1),
                lifeTimeInMonths,
                CertificateFactory.defaultHashSize,
                false,
                null,
                null
                );

            id.Certificate = certificate;

            // ensure the certificate is trusted.
            if (configuration.SecurityConfiguration.AddAppCertToTrustedStore)
            {
                await CertificateManager.AddToTrustedStore(configuration, certificate);
            }

            await configuration.CertificateValidator.Update(configuration.SecurityConfiguration);

            // reload the certificate from disk.
            await configuration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKey(null);

            return certificate;
        }

        public static async Task DeleteApplicationInstanceCertificate(ApplicationConfiguration configuration)
        {
            CertificateIdentifier id;
            X509Certificate2 certificate;

            id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                return;
            }

            certificate = await id.Find();

            // delete trusted peer certificate.
            if (configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                string thumbprint = id.Thumbprint;

                if (certificate != null)
                {
                    thumbprint = certificate.Thumbprint;
                }

                using (ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
                {
                    await store.Delete(thumbprint);
                }
            }

            // delete private key.
            if (certificate != null)
            {
                using (ICertificateStore store = id.OpenStore())
                {
                    await store.Delete(certificate.Thumbprint);
                }
            }
        }

        private static async Task AddToTrustedStore(ApplicationConfiguration configuration, X509Certificate2 certificate)
        {
            string storePath;

            X509Certificate2Collection existingCertificateSet;
            X509Certificate2Collection certificateSet;
            X509Certificate2 publicKey;

            ICertificateStore store;
            List<string> subjectName;

            storePath = null;

            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            if (configuration != null && configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                storePath = configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath;
            }

            if (String.IsNullOrEmpty(storePath))
                throw new ArgumentException(nameof(storePath));

            try
            {
                store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();

                if (store == null)
                {
                    throw new Exception($"Could not open trusted peer store. StorePath={storePath}");
                }

                try
                {
                    // check if it already exists.
                    existingCertificateSet = await store.FindByThumbprint(certificate.Thumbprint);

                    if (existingCertificateSet.Count > 0)
                    {
                        return;
                    }

                    subjectName = Utils.ParseDistinguishedName(certificate.Subject);

                    // check for old certificate.
                    certificateSet = await store.Enumerate();

                    for (int index = 0; index < certificateSet.Count; index++)
                    {
                        if (Utils.CompareDistinguishedName(certificateSet[index], subjectName))
                        {
                            if (certificateSet[index].Thumbprint == certificate.Thumbprint)
                            {
                                return;
                            }

                            await store.Delete(certificateSet[index].Thumbprint);
                            break;
                        }
                    }

                    // add new certificate.
                    publicKey = new X509Certificate2(certificate.RawData);
                    await store.Add(publicKey);
                }
                finally
                {
                    store.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not add certificate to trusted peer store. StorePath={storePath}", ex);
            }
        }
    }
}
