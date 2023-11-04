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
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: true),
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
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    Capacity = table.Column<double>(type: "float", nullable: false),
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
                    SuplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuplierName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SuplierAddressId = table.Column<int>(type: "int", maxLength: 60, nullable: true),
                    SuplierImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuplierEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuplierPhoneNums = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supliers", x => x.SuplierId);
                    table.ForeignKey(
                        name: "FK_Supliers_Addresses_SuplierAddressId",
                        column: x => x.SuplierAddressId,
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
                name: "Points",
                columns: table => new
                {
                    PointId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalPoint = table.Column<int>(type: "int", nullable: false),
                    History = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.PointId);
                    table.ForeignKey(
                        name: "FK_Points_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
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
                    CustomizeFurnitureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        principalColumn: "SuplierId");
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
                    CustomizeFurnitureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    CustomizeFurnitureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                name: "FurnitureSpecificationAttachments",
                columns: table => new
                {
                    FurnitureSpecificationAttachemnetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                name: "OrderDetais",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    FurnitureSpecificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetais", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetais_FurnitureSpecifications_FurnitureSpecificationId",
                        column: x => x.FurnitureSpecificationId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                    table.ForeignKey(
                        name: "FK_OrderDetais_Orders_OrderId",
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
                    { "3", "3", "MANAGER", "MANAGER" },
                    { "4", "4", "ADMIN", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreationDate", "Debit", "Discriminator", "DoB", "Email", "EmailConfirmed", "FirstName", "Gender", "IsActivated", "LastName", "LatestUpdate", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Spent", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "customer.jpg", "d585fc5e-d47c-4ae8-8e37-1efa4a1a7032", new DateTime(2023, 9, 6, 20, 16, 31, 268, DateTimeKind.Local).AddTicks(3034), 0.0, "User", new DateTime(2002, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer1@gmail.com", false, "Customer", "Male", true, "Customer", null, false, null, null, "customer1", "AQAAAAEAACcQAAAAEO4S3AJb6ODWG4WKvamyTCq0WQmCmUS3FP9LkL92TX+CRyr7nknxa9BrHf1xys8fWQ==", "1234567890", false, "51e5a1d7-1ff9-414d-8b11-ccf668728a23", 0.0, false, "customer1" },
                    { "2", 0, "assistant.jpg", "2684aaa7-c180-49d8-a209-6d12c8a2de94", new DateTime(2023, 9, 6, 20, 16, 31, 268, DateTimeKind.Local).AddTicks(3069), 0.0, "User", new DateTime(2002, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "assistant1@gmail.com", false, "Assistant", "Female", true, "Assistant", null, false, null, null, "assistant1", "AQAAAAEAACcQAAAAEAtZcRGQ1aUbfQC5jiGHrm55/uSaF05p6/9DZTrG4Qf2Csn83ZAN1LVhFhmkYkZw0A==", "1234567890", false, "5f0d22b2-09d4-45d3-b5ba-31ffb986d5be", 0.0, false, "assistant1" },
                    { "3", 0, "manager.jpg", "9c194a00-3813-42ee-b23e-9173e545b0af", new DateTime(2023, 9, 6, 20, 16, 31, 268, DateTimeKind.Local).AddTicks(3088), 0.0, "User", new DateTime(2000, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager1@gmail.com", false, "Manager", "Male", true, "Manager", null, false, null, null, "manager1", "AQAAAAEAACcQAAAAEGqaLQDXDNAqRrZa6z73FDmlMVkh0L9iBjHSybo8aarO7EXuy8MiQ+eBXcxkWE0/CQ==", "1234567890", false, "e1beb15e-637a-4df1-9509-5366686a56c1", 0.0, false, "manager1" },
                    { "4", 0, "admin.jpg", "2546034d-1cd7-465f-800b-33d6ab257d5a", new DateTime(2023, 9, 6, 20, 16, 31, 268, DateTimeKind.Local).AddTicks(3103), 0.0, "User", new DateTime(2001, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin1@gmail.com", false, "Admin", "Male", true, "Admin", null, false, null, null, "admin1", "AQAAAAEAACcQAAAAEF1LG0ipiSOI4uLnfLISzsbxIuM7TvCnLY9SDidT6LDIH2xfOhrgDOQjATCJcnixAQ==", "1234567890", false, "3b070941-a9f1-4ca8-a427-e30a7e5647ab", 0.0, false, "admin1" }
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
                    { "3", "3" },
                    { "4", "4" }
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
                table: "Points",
                columns: new[] { "PointId", "CustomerId", "Description", "History", "TotalPoint" },
                values: new object[] { 1, "1", "Create account successfully +500 points", new DateTime(2023, 9, 6, 20, 16, 31, 300, DateTimeKind.Local).AddTicks(8303), 500 });

            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "RepositoryId", "AddressId", "Capacity", "CreationDate", "RepositoryName" },
                values: new object[] { 1, 1, 50.0, new DateTime(2023, 9, 6, 20, 16, 31, 302, DateTimeKind.Local).AddTicks(8734), "Repository 1" });

            migrationBuilder.InsertData(
                table: "Supliers",
                columns: new[] { "SuplierId", "SuplierAddressId", "SuplierEmail", "SuplierImage", "SuplierName", "SuplierPhoneNums" },
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
                    { "FS-1bb8d3c2-1e2d-4dd0-b124-e3c903af6733", 1, " This is testing decription", 1, "Yellow", 50.0, 50.0, 50.0, 50.0, 1 },
                    { "FS-3d976d64-5105-4f3b-b587-7ee1280f89c0", 1, " This is testing decription", 2, "Purple", 30.0, 70.0, 320.0, 20.0, 2 },
                    { "FS-9c73f5b6-067f-4622-aaab-a3c9d23f97ca", 2, " This is testing decription", 1, "Red", 60.0, 40.0, 50.0, 50.0, 2 },
                    { "FS-cf13db72-b214-4744-8265-4cea4d613ab1", 2, " This is testing decription", 2, "Black", 50.0, 30.0, 120.0, 60.0, 1 }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "MaterialId", "DefaultSuplierId", "Description", "MaterialImage", "MaterialName", "MaterialPrice" },
                values: new object[,]
                {
                    { 1, 1, "Material Descriptint 1", null, "Material 1", 5 },
                    { 2, 1, "Material Descriptint 2", null, "Material 2", 4 },
                    { 3, 1, "Material Descriptint 3", null, "Material 3", 7 }
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
                name: "IX_MaterialRepositories_MaterialId",
                table: "MaterialRepositories",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_DefaultSuplierId",
                table: "Materials",
                column: "DefaultSuplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetais_FurnitureSpecificationId",
                table: "OrderDetais",
                column: "FurnitureSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetais_OrderId",
                table: "OrderDetais",
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
                name: "IX_Points_CustomerId",
                table: "Points",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_AddressId",
                table: "Repositories",
                column: "AddressId",
                unique: true,
                filter: "[AddressId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Results_CustomizeFurnitureId",
                table: "Results",
                column: "CustomizeFurnitureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supliers_SuplierAddressId",
                table: "Supliers",
                column: "SuplierAddressId",
                unique: true,
                filter: "[SuplierAddressId] IS NOT NULL");

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
                name: "FurnitureSpecificationAttachments");

            migrationBuilder.DropTable(
                name: "ImportDetais");

            migrationBuilder.DropTable(
                name: "MaterialRepositories");

            migrationBuilder.DropTable(
                name: "OrderDetais");

            migrationBuilder.DropTable(
                name: "Points");

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
