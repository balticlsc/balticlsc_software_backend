﻿CREATE TABLE IF NOT EXISTS   ComputationModules
(
    Id SERIAL PRIMARY KEY,
	UId VARCHAR(50) NOT NULL , 
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    Name VARCHAR(50) NOT NULL, 
    ProblemClassId VARCHAR(50)  REFERENCES ProblemClass(Id) ON DELETE SET NULL NULL, 
    UserAccountId VARCHAR(50) NOT NULL, -- REFERENCES UserAccounts(UId)
      ComputationModulesId INT  REFERENCES ComputationModules(Id) ON DELETE SET NULL  NULL
)