namespace Baltic.UnitManager.Models
{
    public class XUnitQuery
    {
	    public bool IsApp { get; set; }
	    public bool AllUnits { get; set; }
	    public bool OnlyLastRelease { get; set; }
	    public string AuthorUid { get; set; }
    }
}