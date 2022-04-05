using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;

namespace Baltic.Types.DataAccess
{
    public interface IUnitManagement : IUnitGeneral
    {
        /// 
        /// <param name="appName"></param>
        /// <param name="diagramUid"></param>
        /// <param name="userUid"></param>
        string CreateApp(string appName, string diagramUid, string userUid);
        
        /// 
        /// <param name="moduleName"></param>
        /// <param name="userUid"></param>
        string CreateModule(string moduleName, string userUid);

        /// 
        /// <param name="unitUid"></param>
        /// <param name="release"></param>
        string AddReleaseToUnit(string unitUid, ComputationUnitRelease release);

        /// 
        /// <param name="query"></param>
        List<ComputationUnit> FindUnits(UnitQuery query);
        
        /// 
        /// <param name="query"></param>
        List<ComputationUnitRelease> FindUnitReleases(UnitQuery query);

        /// 
        /// <param name="unit"></param>
        short UpdateUnit(ComputationUnit unit);

        /// 
        /// <param name="release"></param>
        short UpdateUnitRelease(ComputationUnitRelease release);

        /// 
        /// <param name="unitUid"></param>
        short DeleteUnit(string unitUid);

        /// 
        /// <param name="unitRelUid"></param>
        short DeleteUnitRelease(string unitRelUid);

        /// 
        /// <param name="releaseUid"></param>
        /// <param name="userUid"></param>
        /// <param name="toToolbox"></param>
        short AddUnitToShelf(string releaseUid, string userUid, bool toToolbox);
        
        /// 
        /// <param name="releaseUid"></param>
        /// <param name="userUid"></param>
        /// <param name="toToolbox"></param>
        short RemoveUnitFromShelf(string releaseUid, string userUid, bool fromToolbox);
        
    }
}