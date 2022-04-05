-- UNIT TABLES -------------------------------------------

CREATE TABLE IF NOT EXISTS AccessType ( 
	Uid text,
	Name text,
	Version text,
	IsBuiltIn boolean,
	AccessSchema text,
	PathSchema text,
	Id serial PRIMARY KEY NOT NULL,
	ParentId integer,
	StorageId integer
);

CREATE TABLE IF NOT EXISTS CommandArgument ( 
	Value text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationModuleReleaseId integer
);

CREATE TABLE IF NOT EXISTS ComputationApplication ( 
	DiagramUid text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS ComputationApplicationRelease ( 
	DiagramUid text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitReleaseId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS ComputationModule ( 
	IsService boolean,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS ComputationModuleRelease ( 
	Image text,
	Command text,
	IsMultitasking boolean,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitReleaseId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS ComputationUnit ( 
	Name text,
	Uid text,
	AuthorUid text,
	IsApplication boolean,
	Id serial PRIMARY KEY NOT NULL,
	ClassId integer,
	ForkParentId integer
);

CREATE TABLE IF NOT EXISTS ComputationUnitRelease ( 
	Version text,
	Uid text,
	Status smallint,
	IsApplication boolean,
	Id serial PRIMARY KEY NOT NULL,
	UnitId integer
);

CREATE TABLE IF NOT EXISTS CredentialParameter ( 
	EnvironmentVariableName text,
	AccessCredentialName text,
	DefaultCredentialValue text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationModuleReleaseId integer
);

CREATE TABLE IF NOT EXISTS DataStructure ( 
	Uid text,
	Name text,
	Version text,
	IsBuiltin boolean,
	DataSchema text,
	Id serial PRIMARY KEY NOT NULL
);

CREATE TABLE IF NOT EXISTS DataType ( 
	Uid text,
	Name text,
	Version text,
	IsBuiltin boolean,
	IsStructured boolean,
	Id serial PRIMARY KEY NOT NULL,
	ParentId integer
);

CREATE TABLE IF NOT EXISTS DataTypeToAccessType (
    Id serial PRIMARY KEY NOT NULL,
	AccessTypeId integer,
	DataTypeId integer
);

CREATE TABLE IF NOT EXISTS DeclaredDataPin ( 
	Name text,
	Uid text,
	Binding smallint,
	DataMultiplicity smallint,
	TokenMultiplicity smallint,
	Id serial PRIMARY KEY NOT NULL,
    ComputationUnitReleaseId integer,
	TypeId integer NOT NULL,
	StructureId integer,
	AccessId integer
);

CREATE TABLE IF NOT EXISTS Keyword ( 
	Value text,
	Id serial PRIMARY KEY NOT NULL,
	UnitDescriptorId integer
);

CREATE TABLE IF NOT EXISTS ProblemClass ( 
	Name text,
	Id serial PRIMARY KEY NOT NULL
);

CREATE TABLE IF NOT EXISTS ReleaseDescriptor ( 
	Date timestamp,
	Description text,
	IsOpenSource boolean,
	UsageCounter bigint,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitReleaseId integer
);

CREATE TABLE IF NOT EXISTS RequiredService ( 
	Id serial PRIMARY KEY NOT NULL,
	ModuleId integer,
	ServiceId integer
);

CREATE TABLE IF NOT EXISTS UnitDescriptor ( 
	ShortDescription text,
	LongDescription text,
	Icon text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitId integer
);

CREATE TABLE IF NOT EXISTS UnitParameter ( 
	NameOrPath text,
	DefaultValue text,
	Type smallint,
	IsManadatory boolean,
	Uid text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitReleaseId integer,
	TargetParameterId integer
);

ALTER TABLE AccessType ADD CONSTRAINT FK_Storage 
	FOREIGN KEY (StorageId) REFERENCES ComputationModuleRelease (Id);

ALTER TABLE AccessType ADD CONSTRAINT FK_Parent 
	FOREIGN KEY (ParentId) REFERENCES AccessType (Id);

ALTER TABLE CommandArgument ADD CONSTRAINT FK_ComputationModuleRelease 
	FOREIGN KEY (ComputationModuleReleaseId) REFERENCES ComputationModuleRelease (Id)
ON DELETE CASCADE;

ALTER TABLE ComputationApplication ADD CONSTRAINT FK_ComputationUnit 
	FOREIGN KEY (ComputationUnitId) REFERENCES ComputationUnit (Id)
ON DELETE CASCADE;

ALTER TABLE ComputationApplicationRelease ADD CONSTRAINT FK_ComputationUnitRelease 
	FOREIGN KEY (ComputationUnitReleaseId) REFERENCES ComputationUnitRelease (Id)
ON DELETE CASCADE;

ALTER TABLE ComputationModule ADD CONSTRAINT FK_ComputationUnit 
	FOREIGN KEY (ComputationUnitId) REFERENCES ComputationUnit (Id)
ON DELETE CASCADE;

ALTER TABLE ComputationModuleRelease ADD CONSTRAINT FK_ComputationUnitRelease 
	FOREIGN KEY (ComputationUnitReleaseId) REFERENCES ComputationUnitRelease (Id)
ON DELETE CASCADE;

ALTER TABLE ComputationUnit ADD CONSTRAINT FK_Class 
	FOREIGN KEY (ClassId) REFERENCES ProblemClass (Id);

ALTER TABLE ComputationUnit ADD CONSTRAINT FK_ForkParent 
	FOREIGN KEY (ForkParentId) REFERENCES ComputationUnit (Id);

ALTER TABLE ComputationUnitRelease ADD CONSTRAINT FK_Unit 
	FOREIGN KEY (UnitId) REFERENCES ComputationUnit (Id)
ON DELETE CASCADE;

ALTER TABLE CredentialParameter ADD CONSTRAINT FK_ComputationModuleRelease 
	FOREIGN KEY (ComputationModuleReleaseId) REFERENCES ComputationModuleRelease (Id)
ON DELETE CASCADE;

ALTER TABLE DataType ADD CONSTRAINT FK_Parent 
	FOREIGN KEY (ParentId) REFERENCES DataType (Id);

ALTER TABLE DataTypeToAccessType ADD CONSTRAINT AccessType 
	FOREIGN KEY (AccessTypeId) REFERENCES AccessType (Id)
ON DELETE CASCADE;

ALTER TABLE DataTypeToAccessType ADD CONSTRAINT DataType 
	FOREIGN KEY (DataTypeId) REFERENCES DataType (Id)
ON DELETE CASCADE;

ALTER TABLE DeclaredDataPin ADD CONSTRAINT FK_Access 
	FOREIGN KEY (AccessId) REFERENCES AccessType (Id);

ALTER TABLE DeclaredDataPin ADD CONSTRAINT FK_ComputationUnitRelease 
	FOREIGN KEY (ComputationUnitReleaseId) REFERENCES ComputationUnitRelease (Id)
ON DELETE CASCADE;

ALTER TABLE DeclaredDataPin ADD CONSTRAINT FK_Structure 
	FOREIGN KEY (StructureId) REFERENCES DataStructure (Id);

ALTER TABLE DeclaredDataPin ADD CONSTRAINT FK_Type 
	FOREIGN KEY (TypeId) REFERENCES DataType (Id);

ALTER TABLE Keyword ADD CONSTRAINT FK_UnitDescriptor 
	FOREIGN KEY (UnitDescriptorId) REFERENCES UnitDescriptor (Id)
ON DELETE CASCADE;

ALTER TABLE ReleaseDescriptor ADD CONSTRAINT FK_ComputationUnitRelease 
	FOREIGN KEY (ComputationUnitReleaseId) REFERENCES ComputationUnitRelease (Id)
ON DELETE CASCADE;

ALTER TABLE RequiredService ADD CONSTRAINT FK_Service 
	FOREIGN KEY (ServiceId) REFERENCES ComputationModuleRelease (Id)
ON DELETE CASCADE;

ALTER TABLE RequiredService ADD CONSTRAINT FK_Module 
	FOREIGN KEY (ModuleId) REFERENCES ComputationModuleRelease (Id)
ON DELETE CASCADE;

ALTER TABLE UnitDescriptor ADD CONSTRAINT FK_ComputationUnit 
	FOREIGN KEY (ComputationUnitId) REFERENCES ComputationUnit (Id)
ON DELETE CASCADE;

ALTER TABLE UnitParameter ADD CONSTRAINT FK_TargetParameter 
	FOREIGN KEY (TargetParameterId) REFERENCES UnitParameter (Id);

ALTER TABLE UnitParameter ADD CONSTRAINT FK_ComputationUnitRelease 
	FOREIGN KEY (ComputationUnitReleaseId) REFERENCES ComputationUnitRelease (Id)
ON DELETE CASCADE;


-- SHELF TABLES -------------------------------------------

CREATE TABLE IF NOT EXISTS AppShelf ( 
	UserUid text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitReleaseId integer
);

CREATE TABLE TaskDataSet ( 
	Uid text,
	Name text,
	Multiplicity smallint,
	UserUid text,
	Data text,
	AccessData text,
	Id serial PRIMARY KEY NOT NULL,
	TypeId integer,
	StructureId integer,
	AccessId integer
);

CREATE TABLE Toolbox ( 
	UserUid text,
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitReleaseId integer
);

ALTER TABLE AppShelf ADD CONSTRAINT FK_AppShelf 
	FOREIGN KEY (ComputationUnitReleaseId) REFERENCES ComputationUnitRelease (Id)
ON DELETE CASCADE;

ALTER TABLE TaskDataSet ADD CONSTRAINT FK_Access 
	FOREIGN KEY (AccessId) REFERENCES AccessType (Id);

ALTER TABLE TaskDataSet ADD CONSTRAINT FK_Structure 
	FOREIGN KEY (StructureId) REFERENCES DataStructure (Id);

ALTER TABLE TaskDataSet ADD CONSTRAINT FK_Type 
	FOREIGN KEY (TypeId) REFERENCES DataType (Id);

ALTER TABLE Toolbox ADD CONSTRAINT FK_Toolbox 
	FOREIGN KEY (ComputationUnitReleaseId) REFERENCES ComputationUnitRelease (Id)
ON DELETE CASCADE;

