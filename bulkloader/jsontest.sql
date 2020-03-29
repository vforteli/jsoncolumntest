/****** Object:  Table [dbo].[JsonTest]    Script Date: 29/03/2020 11.15.31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[JsonTest](
	[PartitionId] [uniqueidentifier] NOT NULL,
	[ObjectId] [uniqueidentifier] NOT NULL,
	[DynamicData] [nvarchar](max) NOT NULL,
	[testproperty]  AS (json_value([DynamicData],'$.PropertyClassesNumbers.subpropdynamic2')),
 CONSTRAINT [PK_JsonTest] PRIMARY KEY CLUSTERED 
(
	[PartitionId] ASC,
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


