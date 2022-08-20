CREATE PROCEDURE [dbo].[AgeGroupsSeen] 
AS
BEGIN
    SELECT SUM(CASE WHEN ISNULL(FLOOR((CAST (GetDate() AS INTEGER) - CAST(DateOfBirth AS INTEGER)) / 365.25), 0) < 11 THEN 1 ELSE 0 END) AS Pediatrics,
	  SUM(CASE WHEN ISNULL(FLOOR((CAST (GetDate() AS INTEGER) - CAST(DateOfBirth AS INTEGER)) / 365.25), 0) BETWEEN 12 AND 18 THEN 1 ELSE 0 END) AS Teenage,
	  SUM(CASE WHEN ISNULL(FLOOR((CAST (GetDate() AS INTEGER) - CAST(DateOfBirth AS INTEGER)) / 365.25), 0) BETWEEN 19 AND 65 THEN 1 ELSE 0 END) AS Adults,
	  SUM(CASE WHEN ISNULL(FLOOR((CAST (GetDate() AS INTEGER) - CAST(DateOfBirth AS INTEGER)) / 365.25), 0) > 66 THEN 1 ELSE 0 END) AS Geriatrics,
	  SUM(CASE WHEN anu.Gender = 'Female' THEN 1 ELSE 0 END) AS Female,
	  SUM(CASE WHEN anu.Gender = 'Male' THEN 1 ELSE 0 END) AS Male
    FROM Doctor d
	   INNER JOIN AspNetUsers anu on d.UserId = anu.Id AND anu.IsActive = 1 AND anu.IsDeleted = 0
    WHERE 1 = 1 AND d.IsActive = 1 AND d.IsDeleted = 0;
END
