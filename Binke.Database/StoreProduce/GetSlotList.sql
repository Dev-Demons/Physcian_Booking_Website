CREATE PRPROCEDURE [dbo].[GetSlotList] 
    @DoctorId      INT,
    @iDisplayStart  INT,
    @iDisplayLength INT,
    @SortColumn     VARCHAR(MAX),
    @SortDir        VARCHAR(MAX),
    @Search         VARCHAR(MAX),
    @SearchRecords  INT OUT
AS
     BEGIN
	   
	   DECLARE @Finalquery NVARCHAR(MAX);
	   ----------------------------------- ProviderSpeciality Gap ----------------------------------- 
	  
		   SET @Finalquery = 'SELECT CAST(SlotDate AS DATE) AS SlotDate, MAX(s.CreatedDate) AS Created, MAX(UpdatedDate) AS Updated, COUNT(1) OVER() As TotalRecords
FROM Slot s  
WHERE 1 = 1 AND IsActive = 1 AND IsDeleted = 0 AND DoctorId = @DoctorId
';

		   IF @Search != ''
             BEGIN
                  SET @Finalquery = @Finalquery+' AND CAST(SlotDate AS DATE) LIKE ''%''+@Search+''%''';
		   END;
		   SET @Finalquery = @Finalquery+' GROUP BY CAST(SlotDate AS DATE)';
		   SET @Finalquery = @Finalquery+' ORDER BY '+REPLACE(@SortColumn, '''', '');
		   IF @SortDir = 'desc'
              BEGIN
                  SET @Finalquery = @Finalquery+' DESC';
		   END;
		   SET @Finalquery = @Finalquery+' OFFSET @iDisplayStart ROWS FETCH NEXT @iDisplayLength ROWS ONLY;';
		   SET @Finalquery = @Finalquery+'      SELECT @SearchRecords = @@ROWCOUNT;';
		   
		   EXEC sp_executesql @Finalquery, N'@DoctorId INT, @Search VARCHAR(MAX), @iDisplayStart INT, @iDisplayLength INT, @SortColumn VARCHAR(1000), @SearchRecords INT OUTPUT',
               @DoctorId, @Search, @iDisplayStart, @iDisplayLength, @SortColumn, @SearchRecords OUTPUT;
     END;
GO