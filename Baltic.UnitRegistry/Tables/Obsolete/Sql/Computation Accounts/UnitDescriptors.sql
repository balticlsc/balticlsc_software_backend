
CREATE TABLE IF NOT EXISTS  ApplicationDescriptors
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ShortDescription VARCHAR(MAX) NULL, 
    LongDescription VARCHAR(MAX) NULL, 
    Icon VARCHAR(MAX) NULL, 
    ComputationApplicationId VARCHAR(50)  REFERENCES ComputationApplication(Id) ON DELETE CASCADE NOT NULL 
)

CREATE TABLE IF NOT EXISTS  ModuleDescriptors
(
	Id SERIAL PRIMARY KEY, 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ShortDescription VARCHAR(MAX) NULL, 
    LongDescription VARCHAR(MAX) NULL, 
    Icon VARCHAR(MAX) NULL, 
   ComputationModuleId VARCHAR(50)  REFERENCES ComputationModule(Id) ON DELETE CASCADE NOT NULL 
)

