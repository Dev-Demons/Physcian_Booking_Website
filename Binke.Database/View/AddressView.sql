CREATE VIEW [dbo].[AddressView]
AS
SELECT dbo.Address.*, dbo.City.CityName, dbo.State.StateName, dbo.State.StateCode
	FROM   dbo.Address 
INNER JOIN dbo.City ON dbo.Address.CityId = dbo.City.CityId 
INNER JOIN dbo.State ON dbo.Address.StateId = dbo.State.StateId AND dbo.City.StateId = dbo.State.StateId
GO
