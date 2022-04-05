using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Types;

// ReSharper disable CheckNamespace

namespace Baltic.DataModel.CALMessages
{
    public partial class BalticModuleBuild
    {
        
        public BalticModuleBuild()
        {
            BatchId = "";
            ModuleId = "";
            Image = "";
            EnvironmentVariables = new Dictionary<string, string>();
            Command = "";
            CommandArguments = new List<string>();
            PortMappings = new List<PortMapping>();
            Volumes = new List<VolumeDescription>();
            ConfigFiles = new List<ConfigFileDescription>();
            Resources = new ResourceRequest()
            {
                Gpus = new GpuRequest()
            }; // TODO - remove when not needed
        }

        /// <summary>
        /// Initiates the Config Files, Environment Variables and Port Mappings
        /// </summary>
        /// <param name="jobParams">A list of CParameters attached to a Job or Service</param>
        public void InitializeParameters(List<CParameter> jobParams)
        {
            foreach (CParameter jobParam in jobParams)
                switch (jobParam.Type)
                {
                    case UnitParamType.Config:
                        ConfigFiles.Add(new ConfigFileDescription()
                        {
                            MountPath = jobParam.NameOrPath,
                            Data = jobParam.Value
                        });
                        break;
                    case UnitParamType.Port:
                    case UnitParamType.Variable:
                        if (UnitParamType.Port == jobParam.Type && uint.TryParse(jobParam.Value, out var portNumber))
                            PortMappings.Add(new PortMapping()
                            {
                                ContainerPort = portNumber,
                                PublishedPort = portNumber,
                                Protocol = CommProtocol.Tcp
                            });
                        if (!string.IsNullOrEmpty(jobParam.NameOrPath))
                            EnvironmentVariables.Add(jobParam.NameOrPath, jobParam.Value);
                        break;
                }
        }

        public void FinalizePinConfigFile(List<PinsConfig> pinConfigs)
        {
            string pinConfigContents = null;
            pinConfigContents = string.Join(",\n",pinConfigs);
            if (string.IsNullOrEmpty(pinConfigContents))
                return;
            ConfigFiles.Remove(ConfigFiles.Find(
                cf => cf.MountPath == PinsConfig.PinsConfigMountPath));
            ConfigFiles.Add(new ConfigFileDescription()
            {
                MountPath = PinsConfig.PinsConfigMountPath,
                Data = "[\n" + pinConfigContents + "]"
            });
        }
        
        public new string ToString()
        {
            string ret = "";
            ret += $"Batch ID: {BatchId}\nModule ID: {ModuleId}\nImage: {Image}\nConfig files:";
            foreach (ConfigFileDescription cf in ConfigFiles)
                ret += $"\n{cf.ToString()}";
            return ret;
        }
        
    }
}