
EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;


-- Typically, you need to be a member of the sysadmin fixed server role 
-- or have the ALTER ANY ASSEMBLY permission. 
-- GRANT ALTER ANY ASSEMBLY TO [user_name];

EXEC sp_add_trusted_assembly 0x70DDE609AF60B715CBC9200AD262E181CB8027121A65C43CD203A8C4C8EC9D9757FBFDD1D107244968A0F70CC836413752E1872A1F62300050F906F9D246E114, N'DatabaseVersionControl'; 
-- EXEC sp_drop_trusted_assembly 0x70DDE609AF60B715CBC9200AD262E181CB8027121A65C43CD203A8C4C8EC9D9757FBFDD1D107244968A0F70CC836413752E1872A1F62300050F906F9D246E114; 



CREATE ASSEMBLY DatabaseVersionControl 
FROM N'D:\username\Documents\Visual Studio 2017\Projects\ExampleClrTrigger\DatabaseVersionControl\bin\Debug\DatabaseVersionControl.dll' 
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
