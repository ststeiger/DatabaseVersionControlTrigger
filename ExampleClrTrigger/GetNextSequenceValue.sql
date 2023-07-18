
-- Enable CLR integration
sp_configure 'clr enabled', 1;
RECONFIGURE;

GO


CREATE FUNCTION dbo.GetNextSequenceValue
(
    @method national character varying(MAX) 
) RETURNS bigint  
AS EXTERNAL NAME DatabaseVersionControl.[DatabaseVersionControl.SequenceValueGetter].GetNextSequenceValue
;


GO 


DROP FUNCTION dbo.GetNextSequenceValue; 


GO 


DROP ASSEMBLY DatabaseVersionControl; 
