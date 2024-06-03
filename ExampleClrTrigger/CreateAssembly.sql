
EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;


-- Typically, you need to be a member of the sysadmin fixed server role 
-- or have the ALTER ANY ASSEMBLY permission. 
-- GRANT ALTER ANY ASSEMBLY TO [user_name];

-- SELECT * FROM sys.trusted_assemblies; 

EXEC sp_add_trusted_assembly 0xD3C7E5E113AF0E0E7D32D185D5880769D8A13D09429E422DDAF49AE54C2C57220E38B376EF7CF489777686DBC35E7179772044F6A3ABCA886C096C71E3E7D7BA, N'DatabaseVersionControl'; 
-- EXEC sp_drop_trusted_assembly 0xD3C7E5E113AF0E0E7D32D185D5880769D8A13D09429E422DDAF49AE54C2C57220E38B376EF7CF489777686DBC35E7179772044F6A3ABCA886C096C71E3E7D7BA; 



CREATE ASSEMBLY DatabaseVersionControl 
FROM N'D:\username\Documents\Visual Studio 2022\github\DatabaseVersionControlTrigger\DatabaseVersionControl\bin\Debug\DatabaseVersionControl.dll' 
-- WITH PERMISSION_SET = SAFE 
-- WITH PERMISSION_SET = EXTERNAL_ACCESS 
WITH PERMISSION_SET = UNSAFE 
; 


GO



CREATE FUNCTION dbo.HttpRequest
(
    @method national character varying(MAX),
    @url national character varying(MAX),
    @contentType national character varying(MAX),
    @payload national character varying(MAX),
    @timeout integer,
    @accept national character varying(MAX)
)
    -- RETURNS national character varying(MAX)
	RETURNS xml 
AS EXTERNAL NAME DatabaseVersionControl.[DatabaseVersionControl.DatabaseVersionControlTrigger].HttpRequest;


GO 




CREATE TRIGGER DatabaseVersionControlTrigger 
ON DATABASE
FOR DDL_DATABASE_LEVEL_EVENTS
AS EXTERNAL NAME DatabaseVersionControl.[DatabaseVersionControl.DatabaseVersionControlTrigger].OnDatabaseChange;
GO 


ENABLE TRIGGER DatabaseVersionControlTrigger ON DATABASE


GO

DISABLE TRIGGER DatabaseVersionControlTrigger ON DATABASE


GO 

DROP FUNCTION dbo.HttpRequest
GO 

DROP TRIGGER DatabaseVersionControlTrigger ON DATABASE
GO

DROP ASSEMBLY DatabaseVersionControl
GO




ALTER TABLE T_AP_Raum ADD xxxx int NULL; 
ALTER TABLE T_AP_Raum DROP COLUMN xxxx; 





-- DECLARE @xml nvarchar(MAX); 
DECLARE @xml xml; 
SET @xml = dbo.HttpRequest
	(
		 N'POST' -- method 
		,N'https://localhost:44302/reportSchemaChange' -- url
		,N'application/json' -- contentType
		,N'{ "hello": "kitty"}' -- payload
		,-1 -- timeout
		-- ,N'application/json' -- accept
		-- ,N'text/html' -- accept
		,N'application/xml' -- accept
	);


SELECT @json, 
	 JSON_VALUE(@json, '$.statusCode') AS statusCode 
	,JSON_VALUE(@json, '$.StatusDescription') AS statusDescription 
	,JSON_VALUE(@json, '$.responseText') AS responseText
; 
