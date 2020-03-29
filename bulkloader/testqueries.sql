--SELECT COUNT(*) FROM JsonTest

--SELECT DISTINCT PartitionId from JsonTest 
-- 65523B44-9CC0-4761-BCE1-2FB2DC5FC454

--SELECT TOP 10 * FROM JsonTest WHERE PartitionId = '4BDA30F4-1671-4AEA-AC62-BE1D8A3200B9'
DBCC FREEPROCCACHE;


--TRUNCATE TABLE jsontest
DECLARE @orderpath NVARCHAR(500) = '$.PropertyClassesNumbers.subpropdynamic2'
DECLARE @path NVARCHAR(500) = '$."PropertyClassesfoo"."subp fefe ef lekfwe fwefwef wefwefwefwef efewfropdynamic9"'
-- filter user based on nested property
SELECT 
	ObjectId
	,JSON_VALUE(DynamicData, '$.PropertyClassesNumbers.subpropdynamic2') AS column2
	--,JSON_VALUE(DynamicData, '$.PropertyClassesNumbers.subpropdynamic8') AS column8
	--JSON_VALUE(DynamicData, '$.PropertyClassesNumbers.subpropdynamic6') AS column6
	--,DynamicData
FROM JsonTest
WHERE 
	PartitionId = '00000000-0000-0000-0000-000000000005'
	--AND JSON_VALUE(DynamicData, '$.PropertyClassesNumbers.subpropdynamic8') > CAST(JSON_VALUE(DynamicData, '$.PropertyClassesNumbers.subpropdynamic6') AS int) * 1.2
	--AND JSON_VALUE(DynamicData, '$.PropertyClassesNumbers.subpropdynamic8') > 12000
	AND JSON_VALUE(DynamicData, '$.PropertyClassesNumbers.subpropdynamic2') > 19000
	--AND JSON_VALUE(DynamicData, @path) > 12000
ORDER BY ObjectId DESC
--ORDER BY JSON_VALUE(DynamicData, @orderpath)
--ORDER BY JSON_VALUE(DynamicData, '$."PropertyClassesNumbers".subpropdynamic2') ASC
OFFSET 0 ROWS
FETCH NEXT 50 ROWS ONLY

-- open first level
SELECT 	
	ObjectId, Attributes.*
FROM JsonTest CROSS APPLY OPENJSON (JsonTest.DynamicData) AS Attributes
WHERE PartitionId = '00000000-0000-0000-0000-000000000045'
ORDER BY ObjectId DESC
OFFSET 3000 ROWS
FETCH NEXT 200 ROWS ONLY

-- nested classes, ie eav type of thing
SELECT 	
	ObjectId, Classes.*
FROM JsonTest CROSS APPLY OPENJSON (JsonTest.DynamicData, '$.PropertyClasses') AS Classes
WHERE PartitionId = '00000000-0000-0000-0000-000000000001'
ORDER BY ObjectId DESC
OFFSET 4000 ROWS
FETCH NEXT 200 ROWS ONLY


-- nested classes count distinct, ie all properties
SELECT 	
	[key], COUNT(*)
FROM JsonTest CROSS APPLY OPENJSON (JsonTest.DynamicData, '$.PropertyClassesfoo') AS Classes
WHERE PartitionId = '00000000-0000-0000-0000-000000000001'
GROUP BY [key]



SELECT TOP 1 * FROM JsonTest