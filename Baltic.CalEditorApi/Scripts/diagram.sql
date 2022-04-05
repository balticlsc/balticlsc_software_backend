create table if not exists diagrams(
	_Id serial primary key,
	DiagramUuid uuid,
	name varchar,
	data varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists boxes(
	_Id serial primary key,
	_DiagramId integer references diagrams(_Id) on delete cascade,
	ElementUuid uuid,
	DiagramUuid uuid,
	ElementTypeId varchar,
	data varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists lines(
	_Id serial primary key,
	_DiagramId integer references diagrams(_Id) on delete cascade,
	ElementUuid uuid,
	DiagramUuid uuid,
	ElementTypeId varchar,
	StartElement varchar,
	EndElement varchar,
	Points int[],
	Data varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists ports(
	_Id serial primary key,
	_DiagramId integer references diagrams(_Id) on delete cascade,
	ElementUuid uuid,
	DiagramUuid uuid,
	ElementTypeId varchar,
	Data varchar,
	ParentId varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists box_element_style(
	Id integer primary key, 
	ElementId integer references boxes(_Id) on delete cascade,
	Fill varchar,
	Opacity int,
	Stroke varchar,
	StrokeWidth int,
	Shape varchar,
	PerfectDrawEnabled bool,
	Dash int[],
	TimeStamp timestamptz not null default now()
);

create table if not exists line_element_style(
	Id integer primary key, 
	ElementId integer references lines(_Id) on delete cascade,
	Fill varchar,
	Opacity int,
	Stroke varchar,
	StrokeWidth int,
	Shape varchar,
	PerfectDrawEnabled bool,
	Dash int[],
	TimeStamp timestamptz not null default now()
);

create table if not exists line_style(
	Id integer primary key, 
	LineId integer references lines(_Id) on delete cascade,
	LineType varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists line_end_style_start_shape(
	Id integer primary key, 
	LineId integer references lines(_Id) on delete cascade,
	Shape varchar,
	Stroke varchar,
	StrokeWidth int,
	Radius int,
	Height int,
	Width int,
	Fill varchar,
	PerfectDrawEnabled bool,
	Listening bool,
	TimeStamp timestamptz not null default now()
);

create table if not exists line_end_style_end_shape(
	Id integer primary key, 
	LineId integer references lines(_Id) on delete cascade,
	Shape varchar,
	Stroke varchar,
	StrokeWidth int,
	Radius int,
	Height int,
	Width int,
	Fill varchar,
	PerfectDrawEnabled bool,
	Listening bool,
	TimeStamp timestamptz not null default now()
);

create table if not exists port_element_style(
	Id integer primary key, 
	ElementId integer references ports(_Id) on delete cascade,
	Fill varchar,
	Opacity int,
	Stroke varchar,
	StrokeWidth int,
	Shape varchar,
	PerfectDrawEnabled bool,
	Dash int[],
	TimeStamp timestamptz not null default now()
);

create table if not exists box_location(
	Id integer primary key, 
	ElementId integer references boxes(_Id) on delete cascade,
	Height int,
	Width int,
	X int,
	Y int,
	TimeStamp timestamptz not null default now()
);

create table if not exists port_location(
	Id integer primary key, 
	ElementId integer references ports(_Id) on delete cascade,
	Height int,
	Width int,
	X int,
	Y int,
	TimeStamp timestamptz not null default now()
);

create table if not exists box_compartments(
	_Id serial primary key,
	_ElementId integer references boxes(_Id) on delete cascade,
	CompartmentUuid varchar,
	ElementUuid uuid,
	Input varchar,
	Value varchar,
	CompartmentTypeId varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists line_compartments(
	_Id serial primary key,
	_ElementId integer references lines(_Id) on delete cascade,
	CompartmentUuid varchar,
	ElementUuid uuid,
	Input varchar,
	Value varchar,
	CompartmentTypeId varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists port_compartments(
	_Id serial primary key,
	_ElementId integer references ports(_Id) on delete cascade,
	CompartmentUuid varchar,
	Input varchar,
	Value varchar,
	ElementUuid uuid,
	CompartmentTypeId varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists box_compartment_style(
	Id integer primary key, 
	CompartmentId integer references box_compartments(_Id) on delete cascade,
	Align varchar,
	Fill varchar,
	FontFamily varchar,
	FontSize varchar,
	FontStyle varchar,
	FontVariant varchar,
	StrokeWidth varchar,
	Visible bool,
	Y int,
	Text varchar,
	Listening bool,
	PerfectDrawEnabled bool,
	Width varchar,
	Height varchar,
	Padding int,
	Placement varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists port_compartment_style(
	Id integer primary key, 
	CompartmentId integer references port_compartments(_Id) on delete cascade,
	Align varchar,
	Fill varchar,
	FontFamily varchar,
	FontSize varchar,
	FontStyle varchar,
	FontVariant varchar,
	StrokeWidth varchar,
	Visible bool,
	Y int,
	Text varchar,
	Listening bool,
	PerfectDrawEnabled bool,
	Width varchar,
	Height varchar,
	Padding int,
	Placement varchar,
	TimeStamp timestamptz not null default now()
);

create table if not exists line_compartment_style(
	Id integer primary key, 
	CompartmentId integer references line_compartments(_Id) on delete cascade,
	Align varchar,
	Fill varchar,
	FontFamily varchar,
	FontSize varchar,
	FontStyle varchar,
	FontVariant varchar,
	StrokeWidth varchar,
	Visible bool,
	Y int,
	Text varchar,
	Listening bool,
	PerfectDrawEnabled bool,
	Width varchar,
	Height varchar,
	Padding int,
	Placement varchar,
	TimeStamp timestamptz not null default now()
);
