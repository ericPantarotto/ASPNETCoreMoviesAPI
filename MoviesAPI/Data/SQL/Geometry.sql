DECLARE @MyLocation GEOGRAPHY = 'POINT(-69.938996 18.481204)'

SELECT TOP (1000) [Id]
      ,[Name]
      ,[Location]
      ,[Location].ToString() as LocationString
      ,[Location].STDistance(@MyLocation) AS distance
  FROM [MoviesAP].[dbo].[MovieTheaters]