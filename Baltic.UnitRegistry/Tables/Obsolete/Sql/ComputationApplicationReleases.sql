CREATE TABLE IF NOT EXISTS   ComputationApplicationReleases
(
    Id SERIAL PRIMARY KEY,
	UId VARCHAR(50) NOT NULL , 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Name VARCHAR(50) NOT NULL, 
    Version VARCHAR(50) NOT NULL, 
    Status INT NOT NULL, 
    CUFamilyId VARCHAR(50)  REFERENCES CUFamily(Uid) NOT NULL,
  
    ComputationApplicationId INT REFERENCES ComputationApplications(Id) NOT NULL,  
    ListComputationApplicationID INT REFERENCES ComputationApplications(Id) ON DELETE SET NULL NULL,
) 
