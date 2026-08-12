SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    /*
        Seed data for DotNetDrinksWebUI
        Tables:
          - Categories (Id, Name)
          - Brands (Id, Name, YearFounded)
          - Products (Id, Name, Price, Stock, Image, BrandId, CategoryId)

        Notes:
          - Script is idempotent: safe to run more than once.
          - Assumes default dbo schema.
    */

    -- 1) Categories
    INSERT INTO dbo.Categories (Name)
    SELECT v.Name
    FROM (VALUES
        (N'Soda'),
        (N'Juice'),
        (N'Sparkling Water'),
        (N'Iced Tea'),
        (N'Sports Drink')
    ) AS v(Name)
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Categories c
        WHERE c.Name = v.Name
    );

    -- 2) Brands
    INSERT INTO dbo.Brands (Name, YearFounded)
    SELECT v.Name, v.YearFounded
    FROM (VALUES
        (N'Coca-Cola', 1892),
        (N'Pepsi', 1898),
        (N'Tropicana', 1947),
        (N'Minute Maid', 1945),
        (N'Perrier', 1863),
        (N'Nestea', 1948),
        (N'Gatorade', 1965)
    ) AS v(Name, YearFounded)
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Brands b
        WHERE b.Name = v.Name
    );

    -- 3) Products
    ;WITH ProductSeed AS (
        SELECT *
        FROM (VALUES
            (N'Coca-Cola Original Can 355ml', CAST(1.79 AS decimal(18,2)), 140, N'/img/drinks/coke-original-355.png', N'Coca-Cola', N'Soda'),
            (N'Coca-Cola Zero Sugar Can 355ml', CAST(1.79 AS decimal(18,2)), 130, N'/img/drinks/coke-zero-355.png', N'Coca-Cola', N'Soda'),
            (N'Pepsi Cola Can 355ml', CAST(1.69 AS decimal(18,2)), 120, N'/img/drinks/pepsi-355.png', N'Pepsi', N'Soda'),
            (N'Tropicana Orange Juice 300ml', CAST(2.29 AS decimal(18,2)), 95, N'/img/drinks/tropicana-orange-300.png', N'Tropicana', N'Juice'),
            (N'Tropicana Apple Juice 300ml', CAST(2.19 AS decimal(18,2)), 90, N'/img/drinks/tropicana-apple-300.png', N'Tropicana', N'Juice'),
            (N'Minute Maid Fruit Punch 355ml', CAST(1.99 AS decimal(18,2)), 100, N'/img/drinks/minutemaid-fruitpunch-355.png', N'Minute Maid', N'Juice'),
            (N'Perrier Lime Sparkling Water 330ml', CAST(2.09 AS decimal(18,2)), 85, N'/img/drinks/perrier-lime-330.png', N'Perrier', N'Sparkling Water'),
            (N'Perrier Original Sparkling Water 330ml', CAST(1.99 AS decimal(18,2)), 88, N'/img/drinks/perrier-original-330.png', N'Perrier', N'Sparkling Water'),
            (N'Nestea Lemon Iced Tea 500ml', CAST(2.19 AS decimal(18,2)), 105, N'/img/drinks/nestea-lemon-500.png', N'Nestea', N'Iced Tea'),
            (N'Nestea Peach Iced Tea 500ml', CAST(2.19 AS decimal(18,2)), 98, N'/img/drinks/nestea-peach-500.png', N'Nestea', N'Iced Tea'),
            (N'Gatorade Lemon-Lime 591ml', CAST(2.49 AS decimal(18,2)), 110, N'/img/drinks/gatorade-lemonlime-591.png', N'Gatorade', N'Sports Drink'),
            (N'Gatorade Cool Blue 591ml', CAST(2.49 AS decimal(18,2)), 112, N'/img/drinks/gatorade-coolblue-591.png', N'Gatorade', N'Sports Drink')
        ) AS x(ProductName, Price, Stock, Image, BrandName, CategoryName)
    )
    INSERT INTO dbo.Products (Name, Price, Stock, Image, BrandId, CategoryId)
    SELECT
        s.ProductName,
        s.Price,
        s.Stock,
        s.Image,
        b.Id,
        c.Id
    FROM ProductSeed s
    INNER JOIN dbo.Brands b
        ON b.Name = s.BrandName
    INNER JOIN dbo.Categories c
        ON c.Name = s.CategoryName
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Products p
        WHERE p.Name = s.ProductName
          AND p.BrandId = b.Id
          AND p.CategoryId = c.Id
    );

    COMMIT TRANSACTION;

    SELECT
        (SELECT COUNT(*) FROM dbo.Categories) AS CategoryCount,
        (SELECT COUNT(*) FROM dbo.Brands) AS BrandCount,
        (SELECT COUNT(*) FROM dbo.Products) AS ProductCount;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

UPDATE dbo.Products
SET Image = N'/img/placeholder.png';
