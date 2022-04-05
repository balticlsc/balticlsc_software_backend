CREATE TABLE IF NOT EXISTS BatchExecution ( 
	BatchMsgUid text,
	DataTransfer float,
	StorageFinish timestamp,
	EstimatedCredits float,
	ConsumedCredits float,
	ConsumedStorageCredits float,
	Id serial PRIMARY KEY NOT NULL,
	InstantiableExecutionId integer NOT NULL,
	BatchId integer,
	ActualReservationId integer
);

CREATE TABLE IF NOT EXISTS CDataToken ( 
	Uid text,
	TokenNo bigint,
	PinUid text,
	PinName text,
	DataType text,
	AccessType text,
	Binding smallint,
	DataMultiplicity smallint,
	TokenMultiplicity smallint,
	Direct boolean,
	Data text,
	AccessData text,
	Id serial PRIMARY KEY NOT NULL,
	CExecutableId integer,
	ServiceId serial
);

CREATE TABLE IF NOT EXISTS CExecutable ( 
	Uid text,
	Id serial PRIMARY KEY NOT NULL
);

CREATE TABLE IF NOT EXISTS CJob ( 
	Multiplicity integer,
	CallName text,
	IsMultitasking boolean,
	Id serial PRIMARY KEY NOT NULL,
	BatchId integer,
	CJobBatchElementId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS CJobBatch ( 
	DepthLevel integer,
	SerialNo integer,
	Id serial PRIMARY KEY NOT NULL,
	CExecutableId integer NOT NULL,
	TaskId integer,
	DerivedReservationRangeId integer
);

CREATE TABLE IF NOT EXISTS CJobBatchElement ( 
	ModuleReleaseUid text,
	Image text,
	Command text,
	Id serial PRIMARY KEY NOT NULL,
	CExecutableId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS CParameter ( 
	NameOrPath text,
	Value text,
	Type smallint,
	Id serial PRIMARY KEY NOT NULL,
	CJobBatchElementId integer
);

CREATE TABLE IF NOT EXISTS CService ( 
	Id serial PRIMARY KEY NOT NULL,
	BatchId integer,
	CJobBatchElementId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS CTask ( 
	ReleaseUid text,
	Id serial PRIMARY KEY NOT NULL,
	CExecutableId integer NOT NULL
);

CREATE TABLE IF NOT EXISTS Depth ( 
	Value integer,
	Id serial PRIMARY KEY NOT NULL,
	CDataTokenId integer
);

CREATE TABLE IF NOT EXISTS InstantiableExecution ( 
	Status smallint,
	Start timestamp,
	Finish timestamp,
	Id serial PRIMARY KEY NOT NULL
);

CREATE TABLE IF NOT EXISTS JobCommandArgument ( 
	Value text,
	Id serial PRIMARY KEY NOT NULL,
	CJobBatchElementId integer
);

CREATE TABLE IF NOT EXISTS JobExecution ( 
	JobMsgUid text,
	Progress bigint,
	EstimatedTime float,
	TokensReceived bigint,
	TokensProcessed bigint,
	Id serial PRIMARY KEY NOT NULL,
	InstantiableExecutionId integer NOT NULL,
	BatchExecutionId integer,
	JobId integer,
	JobInstanceId serial
);

CREATE TABLE IF NOT EXISTS JobInstance ( 
	InstanceUid text,
	Completed boolean,
	Id serial PRIMARY KEY NOT NULL,
	BatchExecutionId integer,
	JobId integer,
	CurrentExecutionId integer
);

CREATE TABLE IF NOT EXISTS ResourceReservation ( 
	CPUs integer,
	GPUs integer,
	Memory integer,
	Storage integer,
	Id serial PRIMARY KEY NOT NULL
);

CREATE TABLE IF NOT EXISTS ResourceReservationRange ( 
	Id serial PRIMARY KEY NOT NULL,
	ComputationUnitReleaseId integer,
	TaskParametersId integer,
	MinReservationId serial,
	MaxReservationId serial
);

CREATE TABLE IF NOT EXISTS ResourceUsage ( 
	TimeStamp timestamp,
	Kind smallint,
	Value float,
	Id serial PRIMARY KEY NOT NULL,
	JobId serial
);

CREATE TABLE IF NOT EXISTS ServiceCredentialParameter ( 
	EnvironmentVariableName text,
	AccessCredentialName text,
	DefaultCredentialValue text,
	Id serial PRIMARY KEY NOT NULL,
	CServiceId integer
);

CREATE TABLE IF NOT EXISTS ServiceExecution ( 
	Uid text,
	Id serial PRIMARY KEY NOT NULL,
	BatchExecutionId integer,
	ServiceId integer
);

CREATE TABLE IF NOT EXISTS TaskExecution ( 
	Status smallint,
	Start timestamp,
	Finish timestamp,
	ConsumedCredits float,
	IsArchived boolean,
	Id serial PRIMARY KEY NOT NULL,
	Task integer NOT NULL
);

CREATE TABLE IF NOT EXISTS TaskParameters ( 
	TaskName text,
	Priority integer,
	ClusterAllocation smallint,
	ClusterUid text,
	ReservedCredits float,
	AuxStorageCredits float,
	IsPrivate boolean,
	Id serial PRIMARY KEY NOT NULL,
	ResourceReservationRangeId integer,
	TaskExecutionId integer
);

CREATE TABLE IF NOT EXISTS UnitCallParameter ( 
	NameOrPath text,
	Value text,
	Type smallint,
	Id serial PRIMARY KEY NOT NULL,
	TaskParametersId integer
);

ALTER TABLE BatchExecution ADD CONSTRAINT FK_Batch 
	FOREIGN KEY (BatchId) REFERENCES CJobBatch (Id)
ON DELETE CASCADE;

ALTER TABLE BatchExecution ADD CONSTRAINT FK_InstantiableExecution 
	FOREIGN KEY (InstantiableExecutionId) REFERENCES InstantiableExecution (Id)
ON DELETE CASCADE;

ALTER TABLE BatchExecution ADD CONSTRAINT FK_ActualReservation 
	FOREIGN KEY (ActualReservationId) REFERENCES ResourceReservation (Id);

ALTER TABLE CDataToken ADD CONSTRAINT FK_CExecutable 
	FOREIGN KEY (CExecutableId) REFERENCES CExecutable (Id)
ON DELETE CASCADE;

ALTER TABLE CDataToken ADD CONSTRAINT FK_Service 
	FOREIGN KEY (ServiceId) REFERENCES CService (Id);

ALTER TABLE CJob ADD CONSTRAINT FK_Batch 
	FOREIGN KEY (BatchId) REFERENCES CJobBatch (Id)
ON DELETE CASCADE;

ALTER TABLE CJobBatch ADD CONSTRAINT FK_DerivedReservationRange 
	FOREIGN KEY (DerivedReservationRangeId) REFERENCES ResourceReservationRange (Id);

ALTER TABLE CJobBatch ADD CONSTRAINT FK_Task 
	FOREIGN KEY (TaskId) REFERENCES CTask (Id)
ON DELETE CASCADE;

ALTER TABLE CJobBatch ADD CONSTRAINT FK_CExecutable 
	FOREIGN KEY (CExecutableId) REFERENCES CExecutable (Id)
ON DELETE CASCADE;

ALTER TABLE CJobBatchElement ADD CONSTRAINT FK_CExecutable 
	FOREIGN KEY (CExecutableId) REFERENCES CExecutable (Id)
ON DELETE CASCADE;

ALTER TABLE CParameter ADD CONSTRAINT FK_CJobBatchElement 
	FOREIGN KEY (CJobBatchElementId) REFERENCES CJobBatchElement (Id)
ON DELETE CASCADE;

ALTER TABLE CService ADD CONSTRAINT FK_Batch 
	FOREIGN KEY (BatchId) REFERENCES CJobBatch (Id)
ON DELETE CASCADE;

ALTER TABLE CService ADD CONSTRAINT FK_CJobBatchElement 
	FOREIGN KEY (CJobBatchElementId) REFERENCES CJobBatchElement (Id)
ON DELETE CASCADE;

ALTER TABLE CTask ADD CONSTRAINT FK_CExecutable 
	FOREIGN KEY (CExecutableId) REFERENCES CExecutable (Id)
ON DELETE CASCADE;

ALTER TABLE Depth ADD CONSTRAINT FK_CDataToken 
	FOREIGN KEY (CDataTokenId) REFERENCES CDataToken (Id)
ON DELETE CASCADE;

ALTER TABLE JobCommandArgument ADD CONSTRAINT FK_CJobBatchElement 
	FOREIGN KEY (CJobBatchElementId) REFERENCES CJobBatchElement (Id)
ON DELETE CASCADE;

ALTER TABLE JobExecution ADD CONSTRAINT FK_BatchExecution 
	FOREIGN KEY (BatchExecutionId) REFERENCES BatchExecution (Id)
ON DELETE CASCADE;

ALTER TABLE JobExecution ADD CONSTRAINT FK_InstantiableExecution 
	FOREIGN KEY (InstantiableExecutionId) REFERENCES InstantiableExecution (Id)
ON DELETE CASCADE;

ALTER TABLE JobExecution ADD CONSTRAINT FK_Job 
	FOREIGN KEY (JobId) REFERENCES CJob (Id)
ON DELETE CASCADE;

ALTER TABLE JobExecution ADD CONSTRAINT FK_JobExecution_JobInstance 
	FOREIGN KEY (JobInstanceId) REFERENCES JobInstance (Id)
ON DELETE CASCADE;

ALTER TABLE JobInstance ADD CONSTRAINT FK_BatchExecution 
	FOREIGN KEY (BatchExecutionId) REFERENCES BatchExecution (Id)
ON DELETE CASCADE;

ALTER TABLE JobInstance ADD CONSTRAINT FK_CurrentExecution 
	FOREIGN KEY (CurrentExecutionId) REFERENCES JobExecution (Id);

ALTER TABLE JobInstance ADD CONSTRAINT FK_Job 
	FOREIGN KEY (JobId) REFERENCES CJob (Id)
ON DELETE CASCADE;

ALTER TABLE ResourceReservationRange ADD CONSTRAINT FK_MinReservation 
	FOREIGN KEY (MinReservationId) REFERENCES ResourceReservation (Id);

ALTER TABLE ResourceReservationRange ADD CONSTRAINT FK_MaxReservation 
	FOREIGN KEY (MaxReservationId) REFERENCES ResourceReservation (Id);

ALTER TABLE ResourceUsage ADD CONSTRAINT FK_Job 
	FOREIGN KEY (JobId) REFERENCES JobInstance (Id);

ALTER TABLE ServiceCredentialParameter ADD CONSTRAINT FK_CService
    FOREIGN KEY (CServiceId) REFERENCES CService (Id);

ALTER TABLE ServiceExecution ADD CONSTRAINT FK_BatchExecution 
	FOREIGN KEY (BatchExecutionId) REFERENCES BatchExecution (Id)
ON DELETE CASCADE;

ALTER TABLE ServiceExecution ADD CONSTRAINT FK_Service 
	FOREIGN KEY (ServiceId) REFERENCES CService (Id)
ON DELETE CASCADE;

ALTER TABLE TaskExecution ADD CONSTRAINT FK_Task 
	FOREIGN KEY (Task) REFERENCES CTask (Id)
ON DELETE CASCADE;

ALTER TABLE TaskParameters ADD CONSTRAINT FK_ReservationRange 
	FOREIGN KEY (ResourceReservationRangeId) REFERENCES ResourceReservationRange (Id);

ALTER TABLE TaskParameters ADD CONSTRAINT FK_TaskExecution 
	FOREIGN KEY (TaskExecutionId) REFERENCES TaskExecution (Id)
ON DELETE CASCADE;

ALTER TABLE UnitCallParameter ADD CONSTRAINT FK_TaskParameters 
	FOREIGN KEY (TaskParametersId) REFERENCES TaskParameters (Id)
ON DELETE CASCADE;
