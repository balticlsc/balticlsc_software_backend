using System.Linq;
using Baltic.Core.Utils;
using Google.Protobuf.Collections;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Types.Protos
{
    public partial class XBalticModuleBuild
    {
        public XBalticModuleBuild(BalticModuleBuild build, string batchInstanceUid = null, string jobInstanceUid = null)
        {
            DBMapper.Map<XBalticModuleBuild>(build, this);
            Scope = (Types.NetworkScope) build.Scope;
            EnvironmentVariables.AddRange(
                build.EnvironmentVariables.Select(v => new XEnvironmentVariable()
                {
                    Key = v.Key, Value = v.Value
                }));
            Resources = DBMapper.Map<XResourceRequest>(build.Resources, new XResourceRequest()
            {
                Gpus = DBMapper.Map<XGpuRequest>(build.Resources.Gpus, new XGpuRequest())
            });
            Volumes.AddRange(build.Volumes.Select(v => 
                DBMapper.Map<XVolumeDescription>(v, new XVolumeDescription())));
            
            CommandArguments.AddRange(build.CommandArguments);
            ConfigFiles.AddRange(build.ConfigFiles.Select(cf =>
                DBMapper.Map<XConfigFileDescription>(cf, new XConfigFileDescription())));
            PortMappings.AddRange(build.PortMappings.Select(pm => 
                DBMapper.Map<XPortMapping>(pm, new XPortMapping())));
            
            BatchId = batchInstanceUid ?? (build.BatchId ?? "");
            ModuleId = jobInstanceUid ?? (build.ModuleId ?? "");
        }
    }
}