use Agency;

/* TODO: Add the constraints. */

create table Employees (
    /* I don't indent the using this database by the
     * large agencies. */
    id tinyint primary key identity,

    name_first nvarchar(32) not null,
    name_last nvarchar(32) not null,
    patronymic nvarchar(32),
    email nvarchar(256) not null unique,

    /* Let the interface will check the collisions. */
    password_hash nvarchar(48) not null unique,

    /* The unique constraint with more than an one null is UB.
     * Therefore, a duplicate phones availability should be
     * checked by the interface. Further, the such fields will
     * be marked as [null unique]. */
    phone nvarchar(16),

    reg_date date not null
);

/* It would be much more economical to store the
 * corresponding integers instead of the full
 * department and position names, if a maximum number
 * of the employees would be greater than a
 * maximum smallint type value providing, likely. */
create table Positions (
    eid tinyint primary key,
    department nvarchar(16) not null,
    position nvarchar(32) not null
);

create table Leads (
    id int primary key identity,
    name_first nvarchar(32) not null,
    name_last nvarchar(32) not null,
    patronymic nvarchar(32),
    email nvarchar(256), -- [null unique]
    phone nvarchar(16), -- [null unique]
    prom_time smallint not null,
    appeal_date date not null
);

create table Groups (
    id tinyint primary key identity,
    pid tinyint not null,
    adid tinyint not null,
    gsid tinyint not null,
    cid tinyint not null,
    lid int
);

create table Campaigns (
    gid tinyint primary key,
    comp_date date not null,
    budget_starting money not null,

    /* A sum of the media and production contractors' prices,
     * related to a specific group id (gid). */
    budget_contractors money not null default 0,

    status bit not null default 0 -- 0 is active, 1 is finished.
);

create table OrdReqs (
    /* More than an one order can be requested per a lead. */
    id bigint primary key identity,

    prod_name nvarchar(64) not null,
    prod_quantity int not null,
    period_date date not null,
    pid tinyint not null,
    lid int
);

create table ContractorsMedia (
    id int primary key identity,
    name_first nvarchar(32) not null,
    name_last nvarchar(32) not null,
    patronymic nvarchar(32),
    email nvarchar(256), -- [null unique]
    phone nvarchar(16), -- [null unique]
    price money not null,
    lid int

    /* Represents the time values, when an advertisement
     * should be shown at a stream. It's used for the
     * TV channels, radio frequencies, etc.
     *
     * [An advertisement]    [A stream]
     *          +
     *          |           [ 12:00:40
     *          |           [ 12:30:00
     *          |           [ 13:00:00
     *          +---------> [ 13:30:00
     *                      [ 14:00:00
     *                      [ 14:30:15
     *                      [ ...
     *
     * The size of this column is defined as
     * ((n - 1) * 10 + 8), where n is a count of the
     * timestamps. Not hard to guess, that the current
     * timestamps column size assumes no more than the 24
     * values.
     *
     * I more inclined to, that this column is reduntant,
     * because I suppose, that this subtletie are a
     * media contractor's care - a media buyer specialist
     * and, generally, a lead with a rest working group
     * aren't interested in which the specific time's
     * moments the advertisement will be shown, but the
     * time's periods. In addition, a lot of the
     * advertisements, that I saw are shown around the clock.
     *
     * But, let it be stored in this script in case, if it
     * will be needed. */
    --timestamps nvarchar(238)
);

create table ContractorsProduction (
    ordid bigint primary key,
    name_first nvarchar(32) not null,
    name_last nvarchar(32) not null,
    patronymic nvarchar(32),
    email nvarchar(256), -- [null unique]
    phone nvarchar(16), -- [null unique]
    price money not null
);

create table Stock (
    ordid bigint primary key,
    rec_date date not null
);

alter table Positions
    add foreign key (eid)
        references Employees (id)
        on delete cascade;

alter table Groups
    add foreign key (lid)
        references Leads (id)
        on delete cascade;

alter table Campaigns
    add foreign key (gid)
        references Groups (id)
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
