CREATE PROCEDURE [dbo].[spProduct_Insert]
	@Id INT OUTPUT,
	@ProductName NVARCHAR(100),
	@Description NVARCHAR(MAX),
	@RetailPrice MONEY,
	@QuantityInStock INT,
	@IsTaxable BIT,
	@ProductImage NVARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Product] ([ProductName], [Description], [RetailPrice], [QuantityInStock], [IsTaxable], [ProductImage])
	VALUES (@ProductName, @Description, @RetailPrice, @QuantityInStock, @IsTaxable, @ProductImage);

	SELECT @Id = Scope_Identity();
END
