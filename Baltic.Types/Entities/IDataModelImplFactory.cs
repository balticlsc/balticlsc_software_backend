using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.Types.Protos;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Types.Entities
{
    public interface IDataModelImplFactory
    {
        CJobBatch CreateCJobBatch();

        CTask CreateCTask();

        CJob CreateCJob();
        
        CService CreateCService();

        CDataToken CreateCDataToken();

        BatchExecution CreateBatchExecution();

        JobExecution CreateJobExecution();

        JobInstance CreateJobInstance();

        BalticModuleBuild CreateBalticModuleBuild();
        
        BalticModuleBuild CreateBalticModuleBuild(XBalticModuleBuild xBuild);
        
        BalticModuleBuild CreateModuleManagerBuild();
    }
}