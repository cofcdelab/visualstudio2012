USE PEGettysburg2
GO
ALTER DATABASE PEGettysburg2 SET TRUSTWORTHY ON

CREATE ASSEMBLY Newtonsoft
FROM 'C:\Users\Gayathri\Downloads\Json60r8\Bin\Net20\Newtonsoft.Json.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE ASSEMBLY sysWeb
FROM 'C:\Windows\Microsoft.NET\Framework64\v2.0.50727\System.Web.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE ASSEMBLY PEGettysburg2
FROM 'C:\Users\Gayathri\Documents\Visual Studio 2012\Projects\PEGettysburg2\PEGettysburg2\obj\Debug\PEGettysburg2.dll'
WITH PERMISSION_SET = SAFE;
GO

CREATE FUNCTION [dbo].[SF_RestPost]
(@uri NVARCHAR (4000), @postData NVARCHAR (4000))
RETURNS NVARCHAR (4000)
AS
 EXTERNAL NAME [PEGettysburg2].[UserDefinedFunctions].[SF_RestPost]
GO




--EXEC [dbo].syncNewData

--select count(*) from tickets_temp
--select * from temptickets

--delete from contacts_temp