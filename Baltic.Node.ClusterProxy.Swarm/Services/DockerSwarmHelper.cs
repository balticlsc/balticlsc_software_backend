using System.Security.Cryptography;
using System.Text;
using Baltic.Types.Protos;

namespace Baltic.Node.ClusterProxy.Swarm.Services
{
    
    public static class DockerSwarmHelper
    {
        public static string ToEncodedConfigData(this string configFile)
        {
            byte[] fileEncoded = System.Text.Encoding.ASCII.GetBytes(configFile);
            string encodedConfigData = System.Convert.ToBase64String(fileEncoded)
                .Replace('+', '-').Replace('/', '_');
            return encodedConfigData;
        }

        public static string CreateVolumeName(XVolumeDescription volumeDescription, string moduleId, string batchId)
        {
            return GetHashString(volumeDescription.MountPath + moduleId + batchId);
        }
        
        public static string CreateConfigName(XConfigFileDescription configFile, string moduleId, string batchId)
        {
            return GetHashString(configFile.MountPath + moduleId + batchId);
        }
        
        private static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static string ToEnvString(this XEnvironmentVariable envVariable)
        {
            return envVariable.Key + "=" + envVariable.Value;
        }
    }

}