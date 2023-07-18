
-- Enable CLR integration
sp_configure 'clr enabled', 1;
RECONFIGURE;

GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'compatibility')
BEGIN
    EXEC('CREATE SCHEMA compatibility; '); 
END

GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'compatibility.get_next_sequence_value') AND type IN (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
    EXECUTE(N'
CREATE FUNCTION compatibility.get_next_sequence_value
(
    @method NVARCHAR(256)
) 
RETURNS BIGINT
AS 
EXTERNAL NAME DatabaseVersionControl.[DatabaseVersionControl.SequenceValueGetter].GetNextSequenceValue
; ' );

    PRINT 'Function "compatibility.get_next_sequence_value" created successfully.'
END


GO 


DROP FUNCTION compatibility.get_next_sequence_value; 


GO 


DROP ASSEMBLY DatabaseVersionControl; 
