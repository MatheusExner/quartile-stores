CREATE OR ALTER FUNCTION dbo.fn_GetProductsJson
(
    @StoreId UNIQUEIDENTIFIER
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @json NVARCHAR(MAX);
        SET @json = (
            SELECT
                p.Id,
                p.StoreId,
                p.Name,
                p.Description,
                p.Price
            FROM dbo.Product AS p
            WHERE p.StoreId = @StoreId
            ORDER BY p.Id
            FOR JSON PATH
        );

    RETURN ISNULL(@json, N'[]');
END;
