CREATE TABLE IF NOT EXISTS   ComputationApplications
(
    Id SERIAL PRIMARY KEY,
	UId VARCHAR(50) NOT NULL , 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Name VARCHAR(50) NOT NULL, 
    ProblemClassId INT  REFERENCES ProblemClasses(Id) ON DELETE SET NULL NULL, 
    UserAccountId INT  NOT NULL, -- REFERENCES UserAccounts(UId)
    ComputationApplicationId INT  REFERENCES ComputationApplications(Id) ON DELETE SET NULL  NULL
)
