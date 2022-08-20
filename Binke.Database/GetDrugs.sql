Create PROCEDURE GetDrugs
	-- Add the parameters for the stored procedure here
	@SearchKey nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DrugId,DrugName,ShortDescription,[Description], IsGeneric from Dbo.Drug
	Where DrugName Like '%'+@SearchKey+'%'
END
GO
