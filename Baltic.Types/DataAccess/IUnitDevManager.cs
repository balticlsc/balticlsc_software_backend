namespace Baltic.Types.DataAccess {
	public interface IUnitDevManager  {
		/// 
		/// <param name="name"></param>
		/// <param name="userUid"></param>
		string CreateApp(string name, string userUid);

		/// 
		/// <param name="appUid"></param>
		/// <param name="version"></param>
		string CreateAppRelease(string appUid, string version);

		/// 
		/// <param name="unitUid"></param>
		short DeleteUnit(string unitUid);
		
		/// 
		/// <param name="releaseUid"></param>
		short DeleteUnitRelease(string releaseUid);
	}
}