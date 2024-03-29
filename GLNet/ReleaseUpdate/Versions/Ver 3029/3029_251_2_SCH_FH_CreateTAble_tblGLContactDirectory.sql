 IF not  EXISTS (select * from sys.tables  where name='tblGLContactDirectory')
BEGIN
CREATE TABLE [dbo].[tblGLContactDirectory](
	[InfoId] [int] IDENTITY(1,1) NOT NULL,
	[Account_id] [int] NOT NULL,
	[Contact_person] [varchar](50) COLLATE Arabic_CI_AS NULL,
	[Phone_office] [varchar](50) COLLATE Arabic_CI_AS NULL,
	[Mobile] [varchar](50) COLLATE Arabic_CI_AS NULL,
	[Fax] [varchar](50) COLLATE Arabic_CI_AS NULL,
	[Email] [varchar](50) COLLATE Arabic_CI_AS NULL,
	[Address] [varchar](250) COLLATE Arabic_CI_AS NULL,
	[Remarks] [varchar](250) COLLATE Arabic_CI_AS NULL,
 CONSTRAINT [PK_tblGLContactDirectory] PRIMARY KEY CLUSTERED 
(
	[InfoId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

 END 

 
