use Agency;

create table Employees (
	id tinyint primary key identity,
	name_first nvarchar(32) not null,
	name_last nvarchar(32) not null,
	patronymic nvarchar(32),
	email nvarchar(48) not null unique,
	password_hash nvarchar(64) not null unique,
	phone nvarchar(16),
	reg_date date not null
);

create table Positions (
    uid tinyint primary key,
    department nvarchar(16) not null,
    position nvarchar(32) not null
);

/* The person, who left their contact details is a lead. 
 * 
 * */

create table Leads (
	id int primary key identity,
	name_first nvarchar(32) not null,
	name_last nvarchar(32) not null,
	patronymic nvarchar(32),
	email nvarchar(48),
	phone nvarchar(16),
	prom_time smallint not null,
	appeal_date date not null
);

create table Groups (
	id tinyint primary key identity,
	pid tinyint not null,
	adid tinyint not null,
	gsid tinyint not null,
	cid tinyint not null,
	comp_date date not null,
	lid int
);

create table OrdReqs (
	id bigint primary key identity,
	prod_name nvarchar(64) not null,
	prod_quantity int not null,
	period_date date not null,
	pid	tinyint not null,
	lid int
);

create table ContractorsMedia (
	id int primary key identity,
	name_first nvarchar(32) not null,
	name_last nvarchar(32) not null,
	patronymic nvarchar(32),
	email nvarchar(48),
	phone nvarchar(16),
	price money not null,
	timestamps nvarchar(238),
	lid int
);

create table ContractorsProduction (
	ordid bigint primary key,
	name_first nvarchar(32) not null,
	name_last nvarchar(32) not null,
	patronymic nvarchar(32),
	email nvarchar(48),
	phone nvarchar(16),
	price money not null
);

create table Stock (
	ordid bigint primary key,
	rec_date date not null
);

alter table Positions
    add foreign key (uid)
        references Employees (id)
        on delete cascade;

alter table Groups
	add foreign key (lid)
		references Leads (id)
		on delete cascade;

alter table OrdReqs
	add foreign key (lid)
		references Leads (id)
		on delete cascade;

alter table ContractorsMedia
	add foreign key (lid)
	    references Leads (id)
		on delete cascade;

alter table ContractorsProduction
    add foreign key (ordid)
		references OrdReqs (id)
		on delete cascade;

alter table Stock
	add foreign key (ordid)
		references OrdReqs (id)
		on delete cascade;