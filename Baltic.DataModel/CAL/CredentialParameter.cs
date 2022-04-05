namespace Baltic.DataModel.CAL {
	public class CredentialParameter {
		public string EnvironmentVariableName;
		public string AccessCredentialName;
		public string DefaultCredentialValue;

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