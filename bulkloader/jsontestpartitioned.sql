
/****** Object:  Table [dbo].[JsonTestPartitioned]    Script Date: 31/03/2020 9.30.59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JsonTestPartitioned](
	[PartitionId] [uniqueidentifier] NOT NULL,
	[ObjectId] [uniqueidentifier] NOT NULL,
	[DynamicData1] [nvarchar](max) NOT NULL,
	[DynamicData2] [nvarchar](max) NOT NULL,
	[DynamicData3] [nvarchar](max) NOT NULL,
	[DynamicData4] [nvarchar](max) NOT NULL,
	[DynamicData5] [nvarchar](max) NOT NULL,
	[testproperty]  AS (json_value([DynamicData1],'$.PropertyClassesNumbers.subpropdynamic2')),
 CONSTRAINT [PK_JsonTestPartitioned] PRIMARY KEY CLUSTERED 
(
	[PartitionId] ASC,
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


