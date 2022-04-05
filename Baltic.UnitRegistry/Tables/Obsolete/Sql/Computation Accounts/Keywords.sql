CREATE TABLE IF NOT EXIST KeywordsListForApplicationDescriptor
(
    Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
	ApplicationDescriptorId INT  REFERENCES ApplicationDescriptors(Id) ON DELETE CASCADE NOT NULL, 
    Keyword VARCHAR(150) NOT NULL 
)CREATE INDEX KeywordsForComputationApplicationIndex ON  KeywordsList (UnitDescriptorForComputationApplicationId)

CREATE TABLE IF NOT EXIST KeywordsListForComputationModule
(
     Stamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
	ModuleDescriptorId INT REFERENCES ModuleDescriptors(Id) ON DELETE CASCADE NOT NULL, 
    Keyword VARCHAR(150) NOT NULL 
)CREATE INDEX KeywordsForComputationModuleIndex ON  KeywordsList (UnitDescriptorForComputationModuleId)