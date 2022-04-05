using System;
using System.Text.RegularExpressions;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CALMessages {
	public class PinsConfig : IComparable<PinsConfig>{
		
		public const string PinsConfigMountPath = "/app/configs/pins_config.json";
		
		public string PinName { get; set; }
		public DataBinding PinType { get; set; }
		public string AccessType { get; set; }
		public string AccessCredential { get; set; }
		public CMultiplicity DataMultiplicity  { get; set; }
		public CMultiplicity TokenMultiplicity { get; set; }
		public string AccessPath { get; set; }

		public PinsConfig(CDataToken token = null)
		{
			if (null == token)
				return;
			PinName = token.PinName;
			PinType = token.Binding;
			AccessType = string.IsNullOrEmpty(token.AccessType) ? "Direct" : token.AccessType;
			DataMultiplicity = token.DataMultiplicity;
			TokenMultiplicity = token.TokenMultiplicity;
			if (null != token.Service)
				AccessCredential = token.Service.GetCredentials();
			else
			{
				AccessCredential = token.AccessData?.Values;
				AccessPath = token.Data?.Values;
			}
		}

		private PinsConfig(string pinsFile)
		{
			
			Regex expression = new Regex("\\{\\s*\\\"PinName\\\":\\s*\\\"(.*)\\\",\\s*" +
			                             "\\\"PinType\\\":\\s*\\\"(.*)\\\",\\s*" +
			                             "\\\"AccessType\\\":\\s*\\\"(.*)\\\",\\s*" +
			                             "\\\"DataMultiplicity\\\":\\s*\\\"(.*)\\\"," +
			                             "\\s*\\\"TokenMultiplicity\\\":\\s*\\\"(\\w*)\\\"\\s*" +
			                             "(,\\s*\\\"AccessCredential\\\":\\s*(\\{.*\\})" +
			                             "(,\\s*\\\"AccessPath\\\":\\s*(\\{.*\\}))?" +
			                             ")?\\s*\\}",
				RegexOptions.Singleline);
			Match match = expression.Match(pinsFile);
			PinName = match.Groups[1].Value;
			PinType = "output" == match.Groups[2].Value ? DataBinding.Provided : 
				("external output" == match.Groups[2].Value ? DataBinding.ProvidedExternal : DataBinding.RequiredStrong);
			AccessType = match.Groups[3].Value;
			DataMultiplicity = string.Equals(CMultiplicity.Single.ToString(), match.Groups[4].Value,StringComparison.OrdinalIgnoreCase)
				? CMultiplicity.Single : CMultiplicity.Multiple;
			TokenMultiplicity = string.Equals(CMultiplicity.Single.ToString(), match.Groups[5].Value,StringComparison.OrdinalIgnoreCase)
				? CMultiplicity.Single : CMultiplicity.Multiple;
			AccessCredential = match.Groups[7].Value;
			AccessPath = match.Groups[9].Value;
		}

		public static PinsConfig Parse(string pinsFile)
		{
			return new PinsConfig(pinsFile);
		}

		public override string ToString()
		{
			return $"{{\n" +
			       $"\"PinName\": \"{PinName}\"," +
			       $"\n\"PinType\": \"{(PinType >= DataBinding.Provided ? (PinType == DataBinding.ProvidedExternal ? "external output" : "output") : "input")}\"," +
			       $"\n\"AccessType\": \"{AccessType}\"," +
			       $"\n\"DataMultiplicity\": \"{DataMultiplicity.ToString().ToLower()}\"," +
			       $"\n\"TokenMultiplicity\": \"{TokenMultiplicity.ToString().ToLower()}\"" +
			       (string.IsNullOrEmpty(AccessCredential) ? "" : $",\n\"AccessCredential\": {AccessCredential}") +
			       (string.IsNullOrEmpty(AccessPath) ? "" : $",\n\"AccessPath\": {AccessPath}") +
			       $"\n}}";
		}

		public int CompareTo(PinsConfig other)
		{
			if (ReferenceEquals(this, other)) return 0;
			if (ReferenceEquals(null, other)) return 1;
			// TODO - compare in groups
			var pinTypeComparison = PinType.CompareTo(other.PinType);
			if (pinTypeComparison != 0) return pinTypeComparison;
			return string.Compare(PinName, other.PinName, StringComparison.Ordinal);
		}
	}
}