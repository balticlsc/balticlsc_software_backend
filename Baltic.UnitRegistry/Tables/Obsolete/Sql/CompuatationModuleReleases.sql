CREATE TABLE IF NOT EXISTS   CompuatationModuleReleases
(
    Id SERIAL PRIMARY KEY,
	UId VARCHAR(50) NOT NULL , 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    YAML VARCHAR(MAX) NOT NULL,
    ResourceReservationRangeId INT /* REFERENCES ResourceReservationRange(Id) */  NOT NULL, /* !! */
    Version VARCHAR(50) NOT NULL, 
    Name VARCHAR(50) NOT NULL,
    Status INT NOT NULL, 
    ComputationModuleId INT REFERENCES ComputationModules(Id) NOT NULL,  
    ListComputationModuleID INT REFERENCES ComputationModules(Id) ON DELETE SET NULL NULL,  
	
)
