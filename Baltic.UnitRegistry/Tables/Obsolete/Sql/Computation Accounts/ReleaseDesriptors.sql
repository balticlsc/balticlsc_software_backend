
CREATE TABLE IF NOT EXISTS  ReleaseDesriptorsForComputationModuleRelease
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Date DATETIME NOT NULL, 
    Description VARCHAR(50) NOT NULL, 
    OpenSource  BOOL NOT NULL, 
    UsageCounter BIGINT NOT NULL, 
    ComputationModuleReleaseId INT  REFERENCES ComputationModuleReleases(Id) ON DELETE CASCADE NOT NULL
)
CREATE TABLE IF NOT EXISTS  ReleaseDesriptorsForComputationApplicationRelease
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Date DATETIME NOT NULL, 
    Description VARCHAR(50) NOT NULL, 
    OpenSource  BOOL NOT NULL, 
    UsageCounter BIGINT NOT NULL, 
    ComputationApplicationReleaseId INT  REFERENCES ComputationApplicationReleases(Id) ON DELETE CASCADE NOT NULL
)
