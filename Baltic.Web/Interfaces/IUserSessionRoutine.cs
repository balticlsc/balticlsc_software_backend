namespace Baltic.Web.Interfaces
{
    public interface IUserSessionRoutine
    {
        public bool SaveSessionParam(string sid, string paramName, string paramValue);
        public string GetSessionParam(string sid, string paramName);
        public bool SaveUserPref(string userName, string prefName, string prefValue);
        public string GetUserPref(string userName, string prefName);
    }
}