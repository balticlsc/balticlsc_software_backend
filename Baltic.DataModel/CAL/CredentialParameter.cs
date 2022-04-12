namespace Baltic.DataModel.CAL {
	public class CredentialParameter {
		public string EnvironmentVariableName { get; set; }
		public string AccessCredentialName { get; set; }
		public string DefaultCredentialValue { get; set; }

		public CredentialParameter(CredentialParameter source = null)
		{
			if (null == source)
				return;
			EnvironmentVariableName = source.EnvironmentVariableName;
			AccessCredentialName = source.AccessCredentialName;
			DefaultCredentialValue = source.DefaultCredentialValue;
		}

		public override string ToString()
		{
			return string.IsNullOrEmpty(AccessCredentialName) ? 
				"" : $"\"{AccessCredentialName}\": \"{DefaultCredentialValue}\"";
		}
	}
}