create database CMDB;

use CMDB;

create table configurationItem(
	configurationItemId int primary key identity(1,1),
	nombre varchar(max) not null,
	[version] varchar(20) not null,
	descripcion varchar(max)	 
)
go

create table dependencies(
	dependencyId int primary key identity(1,1),
	dependeeId int foreign key references configurationItem(configurationItemId) not null,
	dependsOnId int foreign key references configurationItem(configurationItemId) not null
)