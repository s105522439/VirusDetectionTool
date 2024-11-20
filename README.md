This anti-malware software uses Signature-Based Detection to check files and implement the functions of finding, isolating and deleting viruses.
The application is based on the MVVM design pattern, a WPF application developed in C#, and uses a SqlServer database to store virus signature libraries
The structure of the SQl Server database table can be created using the following Sql statements.


CREATE TABLE [dbo].[VirusSignatures](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VirusName] [nvarchar](255) NOT NULL,
	[HashValue] [char](32) NOT NULL,
 PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

