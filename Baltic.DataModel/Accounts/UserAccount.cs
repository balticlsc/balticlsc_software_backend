using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Accounts
{
    public class UserAccount
    {
        public string Uid { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public AccountStatus Status { get; set; }
        public IEnumerable<UserRole> UserRoles { get; set; }
        public UserData Details { get; set; }
        public List<BillingEntry> BillingInformation { get; set; }
        //public List<CTask> Computations { get; set; }  //  TODO: rozwiązać problem....
        public UnitShelf Toolbox { get; set; }
        public UnitShelf AppShelf { get; set; }
    }
}