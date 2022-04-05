using System.Collections.Generic;
using Baltic.DataModel.Execution;

namespace Baltic.DataModel.CAL
{
    public class ComputationModuleRelease : ComputationUnitRelease
    {
        public string Image { get; set; }
        public string Command { get; set; }
        public List<string> CommandArguments { get; set; }
        public bool IsMultitasking { get; set; }
        
        public List<CredentialParameter> CredentialParameters { get; set; }
        
        public List<string> RequiredServiceUids { get; set; }

        public ComputationModuleRelease(ComputationModuleRelease release = null)
        {
            if (null == release)
            {
                Command = "";
                CommandArguments = new List<string>();
                SupportedResourcesRange = new ResourceReservationRange();
                CredentialParameters = new List<CredentialParameter>();
                RequiredServiceUids = new List<string>();
            }
            else
            {
                Image = release.Image;
                Command = release.Command;
                CommandArguments = new List<string>(release.CommandArguments);
                CredentialParameters = new List<CredentialParameter>(release.CredentialParameters);
                IsMultitasking = release.IsMultitasking;
                RequiredServiceUids = new List<string>(release.RequiredServiceUids);
            }
        }

    }
}