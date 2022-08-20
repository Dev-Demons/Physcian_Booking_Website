ALTER PROCEDURE [dbo].[SearchPharmacyList] 
    @Search      NVARCHAR(MAX) = '',
    @Distance	  INT = 0,
    @DistanceSearch NVARCHAR(MAX) = '',
    @PharmacyId INT = 0, 
    @Sorting VARCHAR(MAX) = ''
AS
BEGIN
	DECLARE @Finalquery NVARCHAR(MAX);
	SET @Finalquery = 'SELECT  p.PharmacyId as PharmacyId, p.PharmacyName as PharmayName, anu.Prefix, anu.Suffix, 
		(anu.FirstName + '' '' + anu.MiddleName + '' '' + anu.LastName) as FullName, anu.ProfilePicture, anu.PhoneNumber, a.Address1, a.Address2, a.CityName, a.StateName, a.Country, a.ZipCode 
	FROM Pharmacy p
		INNER JOIN AspNetUsers anu on p.UserId = anu.Id AND anu.IsActive = 1 AND anu.IsDeleted = 0
		LEFT JOIN AddressView as a on a.PharmacyId = p.PharmacyId AND a.IsActive =1 AND a.IsDeleted = 0 AND a.IsDefault = 1
		INNER JOIN City c on c.CityId = a.CityId AND c.IsActive = 1 AND c.IsDeleted = 0
		INNER JOIN state s on s.StateId = a.StateId AND s.IsActive = 1 AND s.IsDeleted = 0
		LEFT JOIN Drug as d on d.PharmacyId = p.PharmacyId AND d.IsActive = 1 AND d.IsDeleted = 0
		LEFT JOIN Review as r on r.PharmacyId = p.PharmacyId AND r.IsActive = 1 AND r.IsDeleted = 0
	WHERE 
	    1 = 1 AND d.IsActive = 1 AND d.IsDeleted = 0 ';
    IF @Search != ''
    BEGIN
	   SET @Finalquery = @Finalquery + '  AND ( ((ISNULL(anu.Prefix,'''') + '' '' + ISNULL (anu.FirstName,'''') + '' '' + ISNULL(anu.MiddleName,'''') + '' '' + ISNULL(anu.LastName,'''')) like  ''%' + @Search + '%'') ';
	   SET @Finalquery = @Finalquery + '  Or ((ISNULL(a.Address1,'''') + '' '' + ISNULL (a.Address2,'''') + '' '' + ISNULL(a.ZipCode,'''') + '' '' + ISNULL (a.Country,''''))   like ''%' + @Search + '%'') ';
	   SET @Finalquery = @Finalquery + '  Or ((ISNULL(a.CityName,'''') + '' '' + ISNULL(a.StateName,''''+	''    	'') like ''%' + @Search + '%'') ';
	   SET @Finalquery = @Finalquery + ' ) ';
    END

    IF @PharmacyId > 0
    BEGIN
	   SET @Finalquery = @Finalquery + ' AND (p.PharmacyId = @PharmacyId)';  
    END

    SET @Finalquery = @Finalquery+' ORDER BY PharmayName'; 
    IF @Sorting = 'desc'   
    BEGIN
		  SET @Finalquery = @Finalquery + ' DESC';
    END

    EXEC sp_executesql @Finalquery, N'@Distance INT, @DistanceSearch NVARCHAR(MAX)', @Distance, @DistanceSearch;  

END;