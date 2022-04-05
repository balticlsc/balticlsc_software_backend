CREATE TABLE IF NOT EXISTS   AppShelf
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    OwnerUserAccountId int NOT NULL, -- REFERENCES UserAccounts(Id) ON DELETE SET NULL  
    ComputationApplicationReleaseId INT REFERENCES ComputationApplicationReleases(Id) ON DELETE SET NULL , NULL
    ComputationModuleReleaseId INT REFERENCES ComputationModuleReleases(Id) ON DELETE SET NULL , NULL


 )
CREATE TABLE IF NOT EXISTS   ToolBox
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    DeveloperUserAccountId int  L  NULL, --REFERENCES UserAccounts(Id) ON DELETE SET NUL 
    ComputationApplicationReleaseId INT REFERENCES ComputationApplicationReleases(Id) ON DELETE SET NULL , NULL
    ComputationModuleReleaseId INT REFERENCES ComputationModuleReleases(Id) ON DELETE SET NULL , NULL
    
)
