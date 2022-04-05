using System.Collections.Generic;
using Baltic.UnitRegistry.Models;

namespace UnitManager.DTO.ComputationAccounts
{
    public class UnitShelf 
    {
        //! public IList<ComputationUnit> Units { get; set; }   //TODO: rozwiązać problem

        public UserAccount  Developer { get; set; }
        public UserAccount  Owner { get; set; }
        public IList<ComputationUnit> Units { get; set; }
    }
}