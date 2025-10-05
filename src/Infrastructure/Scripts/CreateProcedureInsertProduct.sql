CREATE OR ALTER PROCEDURE dbo.InsertProduct
    @StoreId     UNIQUEIDENTIFIER,
    @Name        NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @Price       DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewId UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.Product (Id, StoreId, Name, Description, Price)
    VALUES (@NewId, @StoreId, @Name, @Description, @Price);

    SELECT
        p.Id,
        p.StoreId,
        p.Name,
        p.Description,
        p.Price
    FROM dbo.Product AS p
    WHERE p.Id = @NewId
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
END;
GO
