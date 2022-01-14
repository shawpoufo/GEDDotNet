/*Create table AccessFolder(
	id int primary key not null,
	authorId int foreign key REFERENCES [User](id) ,
	folderId int foreign key REFERENCES Folder(id),
	reader BIT default 0,
	write BIT default 0,
	download BIT default 0
)*/
insert into AccessFolder values (1,1,1,1,0,0);