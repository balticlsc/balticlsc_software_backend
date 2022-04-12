using System.Collections.Generic;
using System.Linq;
using Baltic.DataModel.CAL;
using Baltic.Types.DataAccess;
using Baltic.Types.Entities;

namespace Baltic.UnitManager.Models
{
	public class XModuleReleaseCrude : XUnitReleaseCrude
	{
		public string Image { get; set; }
		
		public string Command{ get; set; }

		public List<string> CommandArguments{ get; set; }
		
		public List<XEnvironmentVariableMapping> VariableMappings { get; set; }
		
		public List<string> RequiredServices{ get; set; }

		public XModuleReleaseCrude(ComputationModuleRelease release = null) : base(release)
		{
			if (null == release) return;
			Image = release.Image;
			Command = release.Command;
			CommandArguments = release.CommandArguments.Select(a => new string(a)).ToList();
			VariableMappings = release.CredentialParameters.Select(m => new XEnvironmentVariableMapping(m)).ToList();
			RequiredServices = release.RequiredServiceUids.Select(s => new string(s)).ToList();
		}

		public override ComputationUnitRelease ToModelObject(IUnitManagement unitRegistry, ComputationUnit unit)
		{
			ComputationModuleRelease release = (ComputationModuleRelease) ToModelObject(new ComputationModuleRelease(){Unit = unit}, unitRegistry);
			release.Image = Image;
			release.Command = Command ?? "";
			release.CommandArguments = CommandArguments.Select(a => new string(a)).ToList();
			release.CredentialParameters = VariableMappings.Select(m => m.ToModelObject()).ToList();
			release.RequiredServiceUids = RequiredServices.Select(s => new string(s)).ToList();
			return release;
		}

	}
}