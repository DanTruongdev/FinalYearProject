using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShopping.Migrations
{
    public partial class createDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Ward = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    District = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Provine = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressOwner = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DoB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Spent = table.Column<double>(type: "float", nullable: true),
                    Debit = table.Column<double>(type: "float", nullable: true),
                    Point = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    IsActivated = table.Column<bool>(type: "bit", nullable: true),
========
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    CollectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionId);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.ColorId);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    LabelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabelName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.LabelId);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                });

            migrationBuilder.CreateTable(
                name: "Woods",
                columns: table => new
                {
                    WoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WoodType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Woods", x => x.WoodId);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    RepositoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepositoryName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<double>(type: "float", nullable: false),
                    IsFull = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.RepositoryId);
                    table.ForeignKey(
                        name: "FK_Repositories_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                });

            migrationBuilder.CreateTable(
                name: "Supliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    SupplierName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SupplierAddressId = table.Column<int>(type: "int", nullable: false),
                    SupplierImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierPhoneNums = table.Column<string>(type: "nvarchar(max)", nullable: false)
========
                    SuplierName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SuplierAddressId = table.Column<int>(type: "int", maxLength: 60, nullable: true),
                    SuplierImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuplierEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuplierPhoneNums = table.Column<string>(type: "nvarchar(max)", nullable: false)
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supliers", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_Supliers_Addresses_SupplierAddressId",
                        column: x => x.SupplierAddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.AnnouncementId);
                    table.ForeignKey(
                        name: "FK_Announcements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_Logs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PointHistories",
                columns: table => new
                {
                    PointHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    History = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointHistories", x => x.PointHistoryId);
                    table.ForeignKey(
                        name: "FK_PointHistories_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
========
                name: "Points",
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LatestUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => new { x.UserId, x.AddressId });
                    table.ForeignKey(
                        name: "FK_UserAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_UserAddresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WishLists",
                columns: table => new
                {
                    WishlistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishLists", x => x.WishlistId);
                    table.ForeignKey(
                        name: "FK_WishLists_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Furnitures",
                columns: table => new
                {
                    FurnitureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FurnitureName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    LabelId = table.Column<int>(type: "int", nullable: true),
                    CollectionId = table.Column<int>(type: "int", nullable: true),
                    AppopriateRoom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Like = table.Column<int>(type: "int", nullable: false),
                    Sold = table.Column<int>(type: "int", nullable: false),
                    VoteStar = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Furnitures", x => x.FurnitureId);
                    table.ForeignKey(
                        name: "FK_Furnitures_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_Furnitures_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId");
                    table.ForeignKey(
                        name: "FK_Furnitures_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "LabelId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    UsedPoint = table.Column<int>(type: "int", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalCost = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "PaymentId");
                });

            migrationBuilder.CreateTable(
                name: "CustomizeFurnitures",
                columns: table => new
                {
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    CustomizeFurnitureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
========
                    CustomizeFurnitureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomizeFurnitureName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Length = table.Column<double>(type: "float", nullable: false),
                    WoodId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DesiredCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizeFurnitures", x => x.CustomizeFurnitureId);
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitures_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitures_Categories_WoodId",
                        column: x => x.WoodId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitures_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "ColorId");
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitures_Woods_WoodId",
                        column: x => x.WoodId,
                        principalTable: "Woods",
                        principalColumn: "WoodId");
                });

            migrationBuilder.CreateTable(
                name: "Imports",
                columns: table => new
                {
                    ImportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    BillImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.ImportId);
                    table.ForeignKey(
                        name: "FK_Imports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imports_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "RepositoryId");
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MaterialPrice = table.Column<int>(type: "int", nullable: false),
                    MaterialImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSuplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.MaterialId);
                    table.ForeignKey(
                        name: "FK_Materials_Supliers_DefaultSuplierId",
                        column: x => x.DefaultSuplierId,
                        principalTable: "Supliers",
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                        principalColumn: "SupplierId");
========
                        principalColumn: "SuplierId");
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                });

            migrationBuilder.CreateTable(
                name: "FurnitureSpecifications",
                columns: table => new
                {
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FurnitureSpecificationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FurnitureId = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Length = table.Column<double>(type: "float", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    WoodId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnitureSpecifications", x => x.FurnitureSpecificationId);
                    table.ForeignKey(
                        name: "FK_FurnitureSpecifications_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "ColorId");
                    table.ForeignKey(
                        name: "FK_FurnitureSpecifications_Furnitures_FurnitureId",
                        column: x => x.FurnitureId,
                        principalTable: "Furnitures",
                        principalColumn: "FurnitureId");
                    table.ForeignKey(
                        name: "FK_FurnitureSpecifications_Woods_WoodId",
                        column: x => x.WoodId,
                        principalTable: "Woods",
                        principalColumn: "WoodId");
                });

            migrationBuilder.CreateTable(
                name: "WishListDetails",
                columns: table => new
                {
                    WishListDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishListId = table.Column<int>(type: "int", nullable: false),
                    FurnitureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishListDetails", x => x.WishListDetailId);
                    table.ForeignKey(
                        name: "FK_WishListDetails_Furnitures_FurnitureId",
                        column: x => x.FurnitureId,
                        principalTable: "Furnitures",
                        principalColumn: "FurnitureId");
                    table.ForeignKey(
                        name: "FK_WishListDetails_WishLists_WishListId",
                        column: x => x.WishListId,
                        principalTable: "WishLists",
                        principalColumn: "WishlistId");
                });

            migrationBuilder.CreateTable(
                name: "Warranties",
                columns: table => new
                {
                    WarrantyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    WarrantyReasons = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstimatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warranties", x => x.WarrantyId);
                    table.ForeignKey(
                        name: "FK_Warranties_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Warranties_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "CustomizeFurnitureAttachments",
                columns: table => new
                {
                    CustomizeFurnitureAttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    CustomizeFurnitureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
========
                    CustomizeFurnitureId = table.Column<int>(type: "int", nullable: false),
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                    AttachmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizeFurnitureAttachments", x => x.CustomizeFurnitureAttachmentId);
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitureAttachments_CustomizeFurnitures_CustomizeFurnitureId",
                        column: x => x.CustomizeFurnitureId,
                        principalTable: "CustomizeFurnitures",
                        principalColumn: "CustomizeFurnitureId");
                });

            migrationBuilder.CreateTable(
                name: "CustomizeFurnitureOrderDetails",
                columns: table => new
                {
                    CustomizeFurnitureOrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomizeFunitureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizeFurnitureOrderDetails", x => x.CustomizeFurnitureOrderDetailId);
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitureOrderDetails_CustomizeFurnitures_CustomizeFunitureId",
                        column: x => x.CustomizeFunitureId,
                        principalTable: "CustomizeFurnitures",
                        principalColumn: "CustomizeFurnitureId");
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitureOrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    CustomizeFurnitureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
========
                    CustomizeFurnitureId = table.Column<int>(type: "int", nullable: false),
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                    ActualCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedPrice = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_Results_CustomizeFurnitures_CustomizeFurnitureId",
                        column: x => x.CustomizeFurnitureId,
                        principalTable: "CustomizeFurnitures",
                        principalColumn: "CustomizeFurnitureId");
                });

            migrationBuilder.CreateTable(
                name: "ImportDetais",
                columns: table => new
                {
                    ImportDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportDetais", x => x.ImportDetailId);
                    table.ForeignKey(
                        name: "FK_ImportDetais_Imports_ImportId",
                        column: x => x.ImportId,
                        principalTable: "Imports",
                        principalColumn: "ImportId");
                    table.ForeignKey(
                        name: "FK_ImportDetais_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId");
                });

            migrationBuilder.CreateTable(
                name: "MaterialRepositories",
                columns: table => new
                {
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Available = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRepositories", x => new { x.RepositoryId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_MaterialRepositories_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId");
                    table.ForeignKey(
                        name: "FK_MaterialRepositories_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "RepositoryId");
                });

            migrationBuilder.CreateTable(
                name: "MaterialRepositoryHistories",
                columns: table => new
                {
                    MaterialRepositoryHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssistantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRepositoryHistories", x => x.MaterialRepositoryHistoryId);
                    table.ForeignKey(
                        name: "FK_MaterialRepositoryHistories_AspNetUsers_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialRepositoryHistories_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId");
                    table.ForeignKey(
                        name: "FK_MaterialRepositoryHistories_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "RepositoryId");
                });

            migrationBuilder.CreateTable(
                name: "CartDetails",
                columns: table => new
                {
                    CartDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetails", x => x.CartDetailId);
                    table.ForeignKey(
                        name: "FK_CartDetails_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId");
                    table.ForeignKey(
                        name: "FK_CartDetails_FurnitureSpecifications_FurnitureSpecificationId",
                        column: x => x.FurnitureSpecificationId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
========
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false),
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                    Content = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    VoteStar = table.Column<int>(type: "int", nullable: false),
                    Anonymous = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Feedbacks_FurnitureSpecifications_FurnitureSpecificationId",
                        column: x => x.FurnitureSpecificationId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                    table.ForeignKey(
                        name: "FK_Feedbacks_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "FurnitureRepositories",
                columns: table => new
                {
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Available = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnitureRepositories", x => new { x.RepositoryId, x.FurnitureSpecificationId });
                    table.ForeignKey(
                        name: "FK_FurnitureRepositories_FurnitureSpecifications_FurnitureSpecificationId",
                        column: x => x.FurnitureSpecificationId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                    table.ForeignKey(
                        name: "FK_FurnitureRepositories_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "RepositoryId");
                });

            migrationBuilder.CreateTable(
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                name: "FurnitureRepositoryHistories",
                columns: table => new
                {
                    FurnitureRepositoryHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssistantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnitureRepositoryHistories", x => x.FurnitureRepositoryHistoryId);
                    table.ForeignKey(
                        name: "FK_FurnitureRepositoryHistories_AspNetUsers_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FurnitureRepositoryHistories_FurnitureSpecifications_FurnitureSpecificationId",
                        column: x => x.FurnitureSpecificationId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                    table.ForeignKey(
                        name: "FK_FurnitureRepositoryHistories_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "RepositoryId");
                });

            migrationBuilder.CreateTable(
========
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                name: "FurnitureSpecificationAttachments",
                columns: table => new
                {
                    FurnitureSpecificationAttachemnetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
========
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false),
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                    AttachmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FurnitureSpecificationAttachments", x => x.FurnitureSpecificationAttachemnetId);
                    table.ForeignKey(
                        name: "FK_FurnitureSpecificationAttachments_FurnitureSpecifications_FurnitureSpecificationId",
                        column: x => x.FurnitureSpecificationId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                });

            migrationBuilder.CreateTable(
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                name: "OrderDetails",
========
                name: "OrderDetais",
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
========
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false),
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_FurnitureSpecifications_FurnitureSpecificationId",
                        column: x => x.FurnitureSpecificationId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "WarrantyAttachments",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarrantyId = table.Column<int>(type: "int", nullable: false),
                    AttachmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyAttachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_WarrantyAttachments_Warranties_WarrantyId",
                        column: x => x.WarrantyId,
                        principalTable: "Warranties",
                        principalColumn: "WarrantyId");
                });

            migrationBuilder.CreateTable(
                name: "FeedbackAttachments",
                columns: table => new
                {
                    FeedbackAttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedbackId = table.Column<int>(type: "int", nullable: false),
                    AttachmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackAttachments", x => x.FeedbackAttachmentId);
                    table.ForeignKey(
                        name: "FK_FeedbackAttachments_Feedbacks_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedbacks",
                        principalColumn: "FeedbackId");
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressId", "AddressOwner", "District", "Provine", "Street", "Ward" },
                values: new object[,]
                {
                    { 1, "USER", "District 1", "Provine 1", "Street 1", "Commune 1" },
                    { 2, "REPOSITORY", "District 2", "Provine 2", "Street 2", "Commune 2" },
                    { 3, "SUPLIER", "District 3", "Provine 3", "Street 3", "Commune 3" }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "1", "CUSTOMER", "CUSTOMER" },
                    { "2", "2", "ASSISTANT", "ASSISTANT" },
                    { "3", "3", "SHOP_OWNER", "SHOP_OWNER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreationDate", "Debit", "Discriminator", "DoB", "Email", "EmailConfirmed", "FirstName", "Gender", "IsActivated", "LastName", "LatestUpdate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Point", "SecurityStamp", "Spent", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "", "e63031e1-99ba-4997-8063-53a705ae6902", new DateTime(2023, 11, 8, 12, 51, 50, 434, DateTimeKind.Local).AddTicks(644), 0.0, "User", new DateTime(2002, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer1@gmail.com", false, "Customer", "Male", true, "Customer", null, false, null, null, "customer1", "AQAAAAEAACcQAAAAEDq71Hb9VLJ42/0ZHOCnzjTmq4gCse1mUv/0awkEFLXG4n+lXkfx0Cnks5N7IQpfiw==", "1234567890", false, 0, "6966498b-6fef-4b96-801f-99df8b3c2946", 0.0, false, "customer1" },
                    { "2", 0, "", "3a490559-ac6c-41d6-8ca8-f1025a078ae6", new DateTime(2023, 11, 8, 12, 51, 50, 434, DateTimeKind.Local).AddTicks(733), 0.0, "User", new DateTime(2002, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "assistant1@gmail.com", false, "Assistant", "Female", true, "Assistant", null, false, null, null, "assistant1", "AQAAAAEAACcQAAAAEEb0z6vQmzNwdio1hKrnfRV2eSHFpYBtE0b98iWBuCy5ftzd+vSKrHcUo4K/WiZxqg==", "1234567890", false, 0, "203ffab4-1556-443a-a540-b1e2c2127356", 0.0, false, "assistant1" },
                    { "3", 0, "", "bf310ca5-1a62-4c2a-b676-311d2e5b42fa", new DateTime(2023, 11, 8, 12, 51, 50, 434, DateTimeKind.Local).AddTicks(765), 0.0, "User", new DateTime(2000, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager1@gmail.com", false, "Manager", "Male", true, "Manager", null, false, null, null, "manager1", "AQAAAAEAACcQAAAAEPoKI8/rsf3dHAl35hTO7QWUwbag7hj1sO47jS1BRtE8v/2ec8sr+BdM7sPk0GNt5Q==", "1234567890", false, 0, "61a15692-fdff-4c30-991e-b284f1eed67d", 0.0, false, "manager1" }
========
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreationDate", "Debit", "Discriminator", "DoB", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LatestUpdate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Spent", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "customer.jpg", "cbd44045-0ca9-4237-99a1-24050eb8d7fd", new DateTime(2023, 8, 29, 23, 21, 24, 323, DateTimeKind.Local).AddTicks(6341), 0.0, "User", new DateTime(2002, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer1@gmail.com", false, "Customer", "Male", "Customer", null, false, null, null, "customer1", "AQAAAAEAACcQAAAAEBwU7Zn7BiLGn5y3guGnRzDczwUcbkZyUzmhdYP+SPwUCamBAYIoG4ROng3GEGGncQ==", "1234567890", false, "17cad0b3-54bc-48d9-b3fb-d09f5a11bb5b", 0.0, "Activated", false, "customer1" },
                    { "2", 0, "assistant.jpg", "5fbe3b83-9d43-4d5b-bd4a-9e3644924bf9", new DateTime(2023, 8, 29, 23, 21, 24, 323, DateTimeKind.Local).AddTicks(6370), 0.0, "User", new DateTime(2002, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "assistant1@gmail.com", false, "Assistant", "Female", "Assistant", null, false, null, null, "assistant1", "AQAAAAEAACcQAAAAEPPPkbBtKoLrpwFfcJwqFDfreD3pfyo1KYjXbWcw2sHxJwGnz7izrLBqmKbL2ApzDA==", "1234567890", false, "ff5fb428-ae2e-48b3-8b09-1846ee7d8cef", 0.0, "Activated", false, "assistant1" },
                    { "3", 0, "manager.jpg", "99d76af5-a13a-40ef-8f5b-20b6db043d1b", new DateTime(2023, 8, 29, 23, 21, 24, 323, DateTimeKind.Local).AddTicks(6382), 0.0, "User", new DateTime(2000, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager1@gmail.com", false, "Manager", "Male", "Manager", null, false, null, null, "manager1", "AQAAAAEAACcQAAAAEEJ7Gln/bW6asVktVGb1M9Kl5vNXbUeakE1ilgP0uj/w7ptVGOeMiOk0ksVHDXDzkg==", "1234567890", false, "351089ff-175f-4e37-a47e-aaef8cda35e3", 0.0, "Activated", false, "manager1" },
                    { "4", 0, "admin.jpg", "29f731f6-82f7-4761-a4a1-6983295c7997", new DateTime(2023, 8, 29, 23, 21, 24, 323, DateTimeKind.Local).AddTicks(6391), 0.0, "User", new DateTime(2001, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin1@gmail.com", false, "Admin", "Male", "Admin", null, false, null, null, "admin1", "AQAAAAEAACcQAAAAEL2Q17L4gI0k1O7naEuvB9HjSf0jMV0ZfhvPJXxMQ9L1m56hK+AjLtBJd5vAwkpetQ==", "1234567890", false, "57749cf3-bc56-4b14-93a4-3b721d20e936", 0.0, "Activated", false, "admin1" }
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 1, "Chair" },
                    { 2, "Table" },
                    { 3, "Bed" }
                });

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "CollectionId", "CollectionName" },
                values: new object[] { 1, "Collection 1" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ColorId", "ColorName" },
                values: new object[,]
                {
                    { 1, "Red" },
                    { 2, "Green" },
                    { 3, "Blue" }
                });

            migrationBuilder.InsertData(
                table: "Labels",
                columns: new[] { "LabelId", "LabelName" },
                values: new object[,]
                {
                    { 1, "New" },
                    { 2, "Hot Sale" },
                    { 3, "Best Saller" }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "PaymentMethod" },
                values: new object[,]
                {
                    { 1, "CASH" },
                    { 2, "VNPAYQR" },
                    { 3, "VNBANK" },
                    { 4, "INTBANK" }
                });

            migrationBuilder.InsertData(
                table: "Woods",
                columns: new[] { "WoodId", "WoodType" },
                values: new object[,]
                {
                    { 1, "Cherry" },
                    { 2, "Ebony" },
                    { 3, "Eucalyptus" }
                });

            migrationBuilder.InsertData(
                table: "Announcements",
                columns: new[] { "AnnouncementId", "Content", "CreationDate", "Title", "UserId" },
                values: new object[] { 1, "Welcome to the furniture shopping website!", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Welcome", "1" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "1" },
                    { "2", "2" },
                    { "3", "3" }
                });

            migrationBuilder.InsertData(
                table: "Furnitures",
                columns: new[] { "FurnitureId", "AppopriateRoom", "CategoryId", "CollectionId", "FurnitureName", "LabelId", "Like", "Sold", "VoteStar" },
                values: new object[,]
                {
                    { 1, "Living room", 1, 1, "Testing Furniture 1", 1, 100, 100, 5.0 },
                    { 2, "Bed room", 2, 1, "Testing Furniture 2", 2, 10, 120, 4.5 }
                });

            migrationBuilder.InsertData(
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                table: "Repositories",
                columns: new[] { "RepositoryId", "AddressId", "Capacity", "CreationDate", "IsFull", "RepositoryName" },
                values: new object[] { 1, 1, 50.0, new DateTime(2023, 11, 8, 12, 51, 50, 507, DateTimeKind.Local).AddTicks(1608), false, "Repository 1" });

            migrationBuilder.InsertData(
                table: "Supliers",
                columns: new[] { "SupplierId", "SupplierAddressId", "SupplierEmail", "SupplierImage", "SupplierName", "SupplierPhoneNums" },
========
                table: "Points",
                columns: new[] { "PointId", "CustomerId", "Description", "History", "TotalPoint" },
                values: new object[] { 1, "1", "Create account successfully +500 points", new DateTime(2023, 8, 29, 23, 21, 24, 352, DateTimeKind.Local).AddTicks(7238), 500 });

            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "RepositoryId", "AddressId", "Capacity", "CreationDate", "RepositoryName" },
                values: new object[] { 1, 1, 50.0, new DateTime(2023, 8, 29, 23, 21, 24, 354, DateTimeKind.Local).AddTicks(1920), "Repository 1" });

            migrationBuilder.InsertData(
                table: "Supliers",
                columns: new[] { "SuplierId", "SuplierAddressId", "SuplierEmail", "SuplierImage", "SuplierName", "SuplierPhoneNums" },
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                values: new object[] { 1, 3, "suplieremail1@gmail.com", null, "Suplier 1", "012334546677" });

            migrationBuilder.InsertData(
                table: "UserAddresses",
                columns: new[] { "AddressId", "UserId", "AddressType" },
                values: new object[] { 1, "1", "USER" });

            migrationBuilder.InsertData(
                table: "FurnitureSpecifications",
                columns: new[] { "FurnitureSpecificationId", "ColorId", "Description", "FurnitureId", "FurnitureSpecificationName", "Height", "Length", "Price", "Width", "WoodId" },
                values: new object[,]
                {
                    { "FS-11b65f5a-f776-4dad-bd03-edae7ac2976f", 1, " This is testing decription", 2, "Purple", 30.0, 70.0, 320.0, 20.0, 2 },
                    { "FS-3f5d9f11-e8d7-4ca4-9cf8-4ce957368bb4", 1, " This is testing decription", 1, "Yellow", 50.0, 50.0, 50.0, 50.0, 1 },
                    { "FS-8d27ac5a-6c84-403a-9dac-491caa2c8601", 2, " This is testing decription", 2, "Black", 50.0, 30.0, 120.0, 60.0, 1 },
                    { "FS-cd7e8bb9-099b-4da3-ad98-e0fbd49a46d8", 2, " This is testing decription", 1, "Red", 60.0, 40.0, 50.0, 50.0, 2 }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "MaterialId", "DefaultSuplierId", "Description", "MaterialImage", "MaterialName", "MaterialPrice" },
                values: new object[,]
                {
                    { 1, 1, "Material Descriptint 1", null, "Material 1", 5 },
                    { 2, 1, "Material Descriptint 2", null, "Material 2", 4 },
                    { 3, 1, "Material Descriptint 3", null, "Material 3", 7 }
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
========
                });

            migrationBuilder.InsertData(
                table: "FurnitureRepositories",
                columns: new[] { "FurnitureSpecificationId", "RepositoryId", "Available" },
                values: new object[,]
                {
                    { 1, 1, 0 },
                    { 2, 1, 0 }
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                });

            migrationBuilder.InsertData(
                table: "MaterialRepositories",
                columns: new[] { "MaterialId", "RepositoryId", "Available" },
                values: new object[] { 1, 1, 0 });

            migrationBuilder.InsertData(
                table: "MaterialRepositories",
                columns: new[] { "MaterialId", "RepositoryId", "Available" },
                values: new object[] { 2, 1, 0 });

            migrationBuilder.InsertData(
                table: "MaterialRepositories",
                columns: new[] { "MaterialId", "RepositoryId", "Available" },
                values: new object[] { 3, 1, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_UserId",
                table: "Announcements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartId",
                table: "CartDetails",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_FurnitureSpecificationId",
                table: "CartDetails",
                column: "FurnitureSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitureAttachments_CustomizeFurnitureId",
                table: "CustomizeFurnitureAttachments",
                column: "CustomizeFurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitureOrderDetails_CustomizeFunitureId",
                table: "CustomizeFurnitureOrderDetails",
                column: "CustomizeFunitureId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitureOrderDetails_OrderId",
                table: "CustomizeFurnitureOrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitures_ColorId",
                table: "CustomizeFurnitures",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitures_CustomerId",
                table: "CustomizeFurnitures",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitures_WoodId",
                table: "CustomizeFurnitures",
                column: "WoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackAttachments_FeedbackId",
                table: "FeedbackAttachments",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CustomerId",
                table: "Feedbacks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FurnitureSpecificationId",
                table: "Feedbacks",
                column: "FurnitureSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_OrderId",
                table: "Feedbacks",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureRepositories_FurnitureSpecificationId",
                table: "FurnitureRepositories",
                column: "FurnitureSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureRepositoryHistories_AssistantId",
                table: "FurnitureRepositoryHistories",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureRepositoryHistories_FurnitureSpecificationId",
                table: "FurnitureRepositoryHistories",
                column: "FurnitureSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureRepositoryHistories_RepositoryId",
                table: "FurnitureRepositoryHistories",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Furnitures_CategoryId",
                table: "Furnitures",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Furnitures_CollectionId",
                table: "Furnitures",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Furnitures_LabelId",
                table: "Furnitures",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureSpecificationAttachments_FurnitureSpecificationId",
                table: "FurnitureSpecificationAttachments",
                column: "FurnitureSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureSpecifications_ColorId",
                table: "FurnitureSpecifications",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureSpecifications_FurnitureId",
                table: "FurnitureSpecifications",
                column: "FurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_FurnitureSpecifications_WoodId",
                table: "FurnitureSpecifications",
                column: "WoodId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetais_ImportId",
                table: "ImportDetais",
                column: "ImportId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetais_MaterialId",
                table: "ImportDetais",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_RepositoryId",
                table: "Imports",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_UserId",
                table: "Imports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRepositories_MaterialId",
                table: "MaterialRepositories",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRepositoryHistories_AssistantId",
                table: "MaterialRepositoryHistories",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRepositoryHistories_MaterialId",
                table: "MaterialRepositoryHistories",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRepositoryHistories_RepositoryId",
                table: "MaterialRepositoryHistories",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_DefaultSuplierId",
                table: "Materials",
                column: "DefaultSuplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_FurnitureSpecificationId",
                table: "OrderDetails",
                column: "FurnitureSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentId",
                table: "Orders",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PointHistories_CustomerId",
                table: "PointHistories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_AddressId",
                table: "Repositories",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_CustomizeFurnitureId",
                table: "Results",
                column: "CustomizeFurnitureId",
                unique: true);

            migrationBuilder.CreateIndex(
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                name: "IX_Supliers_SupplierAddressId",
========
                name: "IX_Supliers_SuplierAddressId",
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                table: "Supliers",
                column: "SupplierAddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_AddressId",
                table: "UserAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_OrderId",
                table: "Warranties",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_UserId",
                table: "Warranties",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyAttachments_WarrantyId",
                table: "WarrantyAttachments",
                column: "WarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListDetails_FurnitureId",
                table: "WishListDetails",
                column: "FurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListDetails_WishListId",
                table: "WishListDetails",
                column: "WishListId");

            migrationBuilder.CreateIndex(
                name: "IX_WishLists_CustomerId",
                table: "WishLists",
                column: "CustomerId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CartDetails");

            migrationBuilder.DropTable(
                name: "CustomizeFurnitureAttachments");

            migrationBuilder.DropTable(
                name: "CustomizeFurnitureOrderDetails");

            migrationBuilder.DropTable(
                name: "FeedbackAttachments");

            migrationBuilder.DropTable(
                name: "FurnitureRepositories");

            migrationBuilder.DropTable(
<<<<<<<< HEAD:Migrations/20231108055151_createDb.cs
                name: "FurnitureRepositoryHistories");

            migrationBuilder.DropTable(
========
>>>>>>>> 5b0a69d7a9c81e56becf4afd48b7938d77f2faca:Migrations/20230829162124_createDb.cs
                name: "FurnitureSpecificationAttachments");

            migrationBuilder.DropTable(
                name: "ImportDetais");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MaterialRepositories");

            migrationBuilder.DropTable(
                name: "MaterialRepositoryHistories");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "PointHistories");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "WarrantyAttachments");

            migrationBuilder.DropTable(
                name: "WishListDetails");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "CustomizeFurnitures");

            migrationBuilder.DropTable(
                name: "Warranties");

            migrationBuilder.DropTable(
                name: "WishLists");

            migrationBuilder.DropTable(
                name: "FurnitureSpecifications");

            migrationBuilder.DropTable(
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "Supliers");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Furnitures");

            migrationBuilder.DropTable(
                name: "Woods");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Labels");
        }
    }
}
