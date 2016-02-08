USE SFProject
GO
ALTER DATABASE SFProject SET TRUSTWORTHY ON

CREATE ASSEMBLY runtime
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Runtime.Serialization.dll'
WITH PERMISSION_SET = UNSAFE;
GO



CREATE ASSEMBLY Newtonsoft
FROM 'C:\Users\Gayathri\Downloads\Json80r2\Bin\Net45\Newtonsoft.Json.dll'
WITH PERMISSION_SET = UNSAFE;
GO


CREATE ASSEMBLY sysWeb
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Web.dll'
WITH PERMISSION_SET = UNSAFE;
GO


CREATE ASSEMBLY SFProject
FROM 'C:\Users\Gayathri\Documents\Visual Studio 2012\Projects\SFProject\SFProject\obj\Debug\SFProject.dll'
WITH PERMISSION_SET = UNSAFE;
GO

CREATE FUNCTION [dbo].[NYP_RestPost]
(@uri NVARCHAR (4000), @postData NVARCHAR (4000))
RETURNS NVARCHAR (4000)
AS
 EXTERNAL NAME [SFProject].[UserDefinedFunctions].[NYP_RestPost]
GO


Declare @url VARCHAR(255)
Declare @request VARCHAR(255)
Declare @response VARCHAR(2000)
Declare @body VARCHAR(2000) 
SET @body='<request><name>TestEvent6</name><externalID>345630</externalID></request>'
SET @request='/services/apexrest/Event'
EXECUTE @response=NYP_RestPost @request,@body
SELECT @response
GO
 
--******************************************************************************
-- Automation

-- *****************************************************************************
