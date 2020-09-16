USE [BikeStores]
GO

/****** Object:  StoredProcedure [production].[Update_Brands]    Script Date: 9/15/2020 5:54:23 PM ******/
DROP PROCEDURE [production].[Update_Brands]
GO

/****** Object:  StoredProcedure [production].[Update_Brands]    Script Date: 9/15/2020 5:54:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [production].[Update_Brands]
	@BrandID int,
	@BrandName varchar(255)
AS

BEGIN 

	--SET NOCOUNT ON 

	update 
		production.brands 
	set 
		brandname = @BrandName 
	where 
		brandid = @BrandID

	RETURN @@ERROR

END
GO


