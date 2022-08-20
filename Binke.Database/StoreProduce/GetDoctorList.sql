ALTER PROCEDURE [dbo].[SearchDoctorList] 
    @Search      NVARCHAR(MAX) = '',
    @Distance	  INT = 0,
    @DistanceSearch NVARCHAR(MAX) = '',
    @IsAllowNewPatient BIT = 0,
    @IsNtPcp	  BIT = 0,
    @IsPrimaryCare	  BIT = 0,
    @Specialties INT = 0,
    @Affiliations  NVARCHAR(MAX) = '',
    @Insurance   NVARCHAR(MAX) = '',
    @AGS NVARCHAR(MAX) = '',
    @AGSFull NVARCHAR(MAX) = '',
    @Sorting VARCHAR(MAX) = ''

    
AS
     BEGIN
	   
	   DECLARE @Finalquery NVARCHAR(MAX);

	   SET @Finalquery = 'SELECT  d.DoctorId, MAX(anu.ProfilePicture) AS ProfilePicture, MAX(anu.Suffix) As Suffix, (MAX(anu.Prefix) + '' '' + MAX(anu.FirstName) + '' '' + MAX(anu.MiddleName) + '' '' + MAX(anu.LastName)) AS FullName, MAX(anu.PhoneNumber) AS PhoneNumber, 
MAX(d.Education) AS Education, MAX(d.NPI) AS NPI, MAX(d.ShortDescription) AS ShortDesc, MAX(d.LongDescription) AS LongDesc, MAX(CASE WHEN d.IsAllowNewPatient = 1 THEN 1 ELSE 0 END) AS IsAllow, MAX(CASE WHEN d.IsNtPcp = 1 THEN 1 ELSE 0 END) AS IsNtPcp,
ISNULL( (SELECT STUFF((SELECT '','' + CONVERT(VARCHAR(MAX), dspec.SpecialityId) FROM DoctorSpeciality as dspec WHERE dspec.IsActive = 1 AND dspec.IsDeleted = 0 AND dspec.DoctorId = d.DoctorId
    ORDER BY dspec.DoctorSpecialityId FOR XML PATH('''')), 1, 1, '''')),'''') AS SpecitiesIds,
ISNULL( (SELECT STUFF(( SELECT '','' + CONVERT(VARCHAR(MAX), dfaa.FacilityId) FROM DoctorFacilityAffiliation as dfaa WHERE dfaa.IsActive = 1 AND dfaa.IsDeleted = 0 AND dfaa.DoctorId = d.DoctorId 
    ORDER BY dfaa.AffiliationId FOR XML PATH('''')), 1, 1, '''')),'''') AS FacilityIds
FROM Doctor d
    INNER JOIN AspNetUsers anu on d.UserId = anu.Id AND anu.IsActive = 1 AND anu.IsDeleted = 0
    LEFT JOIN AddressView av on av.DoctorId = d.DoctorId AND av.IsActive = 1 AND av.IsDeleted = 0 AND av.IsDefault = 1
    LEFT JOIN DoctorSpeciality as ds on ds.DoctorId = d.DoctorId AND ds.IsActive = 1 AND ds.IsDeleted = 0
    LEFT JOIN Speciality as s on s.SpecialityId = ds.SpecialityId AND s.IsActive = 1 AND s.IsDeleted = 0
    LEFT JOIN DoctorFacilityAffiliation as dfa on dfa.DoctorId = d.DoctorId AND dfa.IsActive = 1 AND dfa.IsDeleted = 0
    LEFT JOIN Facility as f on f.FacilityId = dfa.FacilityId AND f.IsActive = 1 AND f.IsDeleted = 0
    WHERE 1 = 1 AND d.IsActive = 1 AND d.IsDeleted = 0 ';

	   IF @Search != ''
	   BEGIN
		  SET @Finalquery = @Finalquery + ' AND ((s.SpecialityName like ''%' + @Search + '%'' )';
		  SET @Finalquery = @Finalquery + ' OR (ISNULL(anu.Prefix,'''') + '' '' + ISNULL(anu.FirstName,'''') + '' '' + ISNULL(anu.MiddleName,'''') + '' '' + ISNULL(anu.LastName,'''') like ''%' + @Search + '%'')';
		  SET @Finalquery = @Finalquery + ' )';
	   END

	   SET @Finalquery = @Finalquery + ' AND (@DistanceSearch = '''' OR (ISNULL(av.CityName,'''') LIKE ''%''+@DistanceSearch+''%'' OR ISNULL(av.StateName,'''') LIKE ''%''+@DistanceSearch+''%'' OR ISNULL(av.ZipCode,'''') LIKE ''%''+@DistanceSearch+''%''))';  
	   SET @Finalquery = @Finalquery + ' AND (@Specialties = '''' OR s.SpecialityId = @Specialties)';  
	   SET @Finalquery = @Finalquery + ' AND (@Affiliations = '''' OR f.FacilityId IN (SELECT Value FROM dbo.Split(@Affiliations,'','')))';  
	   SET @Finalquery = @Finalquery + ' AND (@AGS = '''' OR ISNULL(anu.Gender,'''') IN (SELECT Value FROM dbo.Split(@AGS,'','')))';
	   --SET @Finalquery = @Finalquery + ' AND (@AGSFull = '''' OR ISNULL(anu.Gender,'''') LIKE ''%''+@AGSFull+''%'')';
	   SET @Finalquery = @Finalquery + ' AND (@IsNtPcp = 0 OR ISNULL(d.IsNtPcp,0) = @IsNtPcp)';  
	   SET @Finalquery = @Finalquery + ' AND (@IsAllowNewPatient = 0 OR ISNULL(d.IsAllowNewPatient,0) = @IsAllowNewPatient)';
	   
	   SET @Finalquery = @Finalquery + ' GROUP BY d.DoctorId';
	   SET @Finalquery = @Finalquery + ' ORDER BY FullName';  

	   IF @Sorting = 'desc'   
	   BEGIN
		 SET @Finalquery = @Finalquery + ' DESC';
	   END

	   EXEC sp_executesql @Finalquery, N'@Distance INT, @DistanceSearch NVARCHAR(MAX), @IsAllowNewPatient BIT, @IsNtPcp BIT, @IsPrimaryCare BIT, @Specialties INT, @Affiliations  NVARCHAR(MAX), @Insurance NVARCHAR(MAX), @AGS NVARCHAR(MAX), @AGSFull NVARCHAR(MAX)',
								  @Distance, @DistanceSearch, @IsAllowNewPatient, @IsNtPcp, @IsPrimaryCare, @Specialties, @Affiliations, @Insurance, @AGS, @AGSFull;  
     END;