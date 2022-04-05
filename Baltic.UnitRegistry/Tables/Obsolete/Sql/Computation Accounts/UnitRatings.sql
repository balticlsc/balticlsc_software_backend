CREATE TABLE IF NOT EXISTS   UnitRatingsForComputationApplication
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Value INT NOT NULL, 
    Comment VARCHAR(MAX) NOT NULL, 
    UserAccountId INT   NOT NULL, -- REFERENCES UserAccounts(Id)  
    ApplicationDescriptorId INT  REFERENCESApplicationDescriptors(Id)ON DELETE SET NULL NULL 
)
CREATE TABLE IF NOT EXISTS UnitRatingsForComputationModule
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Value INT NOT NULL, 
    Comment VARCHAR(MAX) NOT NULL, 
    UserAccountId INT   NOT NULL, -- REFERENCES UserAccounts(Id)  
    ModuleDescriptorId INT  REFERENCES ModuleDescriptors(Id)ON DELETE SET NULL NULL 
)
