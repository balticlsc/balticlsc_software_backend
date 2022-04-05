using System.Collections.Generic;
using System.Linq;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;
using Baltic.Types.Entities;

namespace Baltic.UnitManager.Models
{
    public class XModuleRelease : XUnitRelease
    {
        public string Image { get; set; }
        
        public string Command{ get; set; }

        public List<string> CommandArguments{ get; set; }
        
        public List<XEnvironmentVariableMapping> VariableMappings { get; set; }
        
        public List<string> RequiredServices{ get; set; }
        
        public XModuleRelease(ComputationModuleRelease release = null) : base(release)
        {
            if (null == release) return;
            Image = release.Image;
            Command = release.Command;
            CommandArguments = release.CommandArguments.Select(a => new string(a)).ToList();
            VariableMappings = release.CredentialParameters.Select(m => new XEnvironmentVariableMapping(m)).ToList();
            RequiredServices = release.RequiredServiceUids.Select(s => new string(s)).ToList();
            // TODO - check code below if needed (changes ProvidedExternal to Provided when sent to the frontend)
            foreach (var pin in Pins)
            {
                if (pin.Binding == DataBinding.ProvidedExternal)
                    pin.Binding = DataBinding.Provided;
            }
        }

        public override ComputationUnitRelease ToModelObject(IUnitManagement unitRegistry, ComputationUnit unit)
        {
            ComputationModuleRelease release = (ComputationModuleRelease) ToModelObject(new ComputationModuleRelease(), unitRegistry);
            release.Image = Image;
            release.Command = Command;
            release.CommandArguments = CommandArguments.Select(a => new string(a)).ToList();
            release.CredentialParameters = VariableMappings.Select(m => m.ToModelObject()).ToList();
            release.RequiredServiceUids = RequiredServices.Select(s => new string(s)).ToList();
            return release;
        }
    }
}