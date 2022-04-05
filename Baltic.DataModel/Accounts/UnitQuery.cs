namespace Baltic.DataModel.Accounts
{
    public class UnitQuery
    {
	    public bool IsApp { get; set; } = true;
	    public bool AllUnits { get; set; } = true;
	    public bool OnlyLastRelease { get; set; }
	    public bool IsInToolbox { get; set; }
	    public string UserUid { get; set; }
	    public string AuthorUid { get; set; }
    }
}