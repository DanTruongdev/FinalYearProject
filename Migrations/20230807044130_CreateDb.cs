using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShopping.Migrations
{
    public partial class CreateDb : Migration
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
                    Commune = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
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
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "CustomizeFurnitures",
                columns: table => new
                {
                    CustomizeFurnitureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpectedPrice = table.Column<double>(type: "float", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizeFurnitures", x => x.CustomizeFurnitureId);
                    table.ForeignKey(
                        name: "FK_CustomizeFurnitures_AspNetUsers_CustomerId",
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
                    PaymentId = table.Column<int>(type: "int", nullable: true),
                    UsedPoint = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "Requirements",
                columns: table => new
                {
                    RequirementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomizeFurnitureId = table.Column<int>(type: "int", nullable: false),
                    CustomizeFurnitureName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Length = table.Column<double>(type: "float", nullable: false),
                    WoodId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DesiredCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.RequirementId);
                    table.ForeignKey(
                        name: "FK_Requirements_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_Requirements_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "ColorId");
                    table.ForeignKey(
                        name: "FK_Requirements_CustomizeFurnitures_CustomizeFurnitureId",
                        column: x => x.CustomizeFurnitureId,
                        principalTable: "CustomizeFurnitures",
                        principalColumn: "CustomizeFurnitureId");
                    table.ForeignKey(
                        name: "FK_Requirements_Woods_WoodId",
                        column: x => x.WoodId,
                        principalTable: "Woods",
                        principalColumn: "WoodId");
                });

            migrationBuilder.CreateTable(
                name: "FurnitureSpecifications",
                columns: table => new
                {
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "CustomizeFurnitureOrderDetails",
                columns: table => new
                {
                    CustomizeFurnitureOrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomizeFunitureId = table.Column<int>(type: "int", nullable: false)
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
                name: "WarrantySchedules",
                columns: table => new
                {
                    WarrantyScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    WarrantyReasons = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstimatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantySchedules", x => x.WarrantyScheduleId);
                    table.ForeignKey(
                        name: "FK_WarrantySchedules_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarrantySchedules_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
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
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false),
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
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "FurnitureRepositories",
                columns: table => new
                {
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false),
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
                name: "OrderDetais",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    FurnitureSpecificationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
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
                name: "Attachments",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LikedItemId = table.Column<int>(type: "int", nullable: false),
                    AttachmentFor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedbackId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_Attachments_Feedbacks_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedbacks",
                        principalColumn: "FeedbackId");
                    table.ForeignKey(
                        name: "FK_Attachments_FurnitureSpecifications_LikedItemId",
                        column: x => x.LikedItemId,
                        principalTable: "FurnitureSpecifications",
                        principalColumn: "FurnitureSpecificationId");
                    table.ForeignKey(
                        name: "FK_Attachments_Materials_LikedItemId",
                        column: x => x.LikedItemId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId");
                    table.ForeignKey(
                        name: "FK_Attachments_Requirements_LikedItemId",
                        column: x => x.LikedItemId,
                        principalTable: "Requirements",
                        principalColumn: "RequirementId");
                    table.ForeignKey(
                        name: "FK_Attachments_Supliers_LikedItemId",
                        column: x => x.LikedItemId,
                        principalTable: "Supliers",
                        principalColumn: "SuplierId");
                    table.ForeignKey(
                        name: "FK_Attachments_WarrantySchedules_LikedItemId",
                        column: x => x.LikedItemId,
                        principalTable: "WarrantySchedules",
                        principalColumn: "WarrantyScheduleId");
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressId", "AddressOwner", "Commune", "District", "Provine", "Street" },
                values: new object[,]
                {
                    { 1, "USER", "Commune 1", "District 1", "Provine 1", "Street 1" },
                    { 2, "REPOSITORY", "Commune 2", "District 2", "Provine 2", "Street 2" },
                    { 3, "SUPLIER", "Commune 3", "District 3", "Provine 3", "Street 3" }
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
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreationDate", "Debit", "Discriminator", "DoB", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Spent", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1", 0, "customer.jpg", "2ea440e8-c041-43b0-b26a-e0f7e74f3a42", new DateTime(2023, 8, 7, 11, 41, 30, 100, DateTimeKind.Local).AddTicks(2189), 0.0, "User", new DateTime(2002, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "customer1@gmail.com", false, "Customer", "Male", "Customer", false, null, null, "customer1", "AQAAAAEAACcQAAAAEMZJItrhCWGGyKAhZ2i1WAix5ifyUvIDyNObcsmyL6KvMPc8CFrd9ddBNv8I80r8OA==", "1234567890", false, "f75e657d-901a-47d9-8ea2-31f6f97dbd26", 0.0, "Activated", false, "customer1" },
                    { "2", 0, "assistant.jpg", "e4056902-215e-4ed0-beba-4ff5aa7a1236", new DateTime(2023, 8, 7, 11, 41, 30, 100, DateTimeKind.Local).AddTicks(2212), 0.0, "User", new DateTime(2002, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "assistant1@gmail.com", false, "Assistant", "Female", "Assistant", false, null, null, "assistant1", "AQAAAAEAACcQAAAAEIVtvmLBn4iEjrXE+JHudsJW6l16TceH6JqanhaQb3nTXwaeHIk1Ki+yWwCKNkk44Q==", "1234567890", false, "71b2cf87-f3af-4acc-8588-7d2bbf8c6045", 0.0, "Activated", false, "assistant1" },
                    { "3", 0, "manager.jpg", "08d78758-5813-4300-a305-4dacc273194d", new DateTime(2023, 8, 7, 11, 41, 30, 100, DateTimeKind.Local).AddTicks(2226), 0.0, "User", new DateTime(2000, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager1@gmail.com", false, "Manager", "Male", "Manager", false, null, null, "manager1", "AQAAAAEAACcQAAAAEL8nSILZMst9x0BpOx6GmBRhHuELXBEjArfM0eDn88NTLFkrbk1MXMG+L6Hrz8tBkQ==", "1234567890", false, "952736d7-7669-4513-b699-4f0ff114e7b3", 0.0, "Activated", false, "manager1" },
                    { "4", 0, "admin.jpg", "6b07bb44-2f1b-4871-aa0a-a7343fcab539", new DateTime(2023, 8, 7, 11, 41, 30, 100, DateTimeKind.Local).AddTicks(2236), 0.0, "User", new DateTime(2001, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin1@gmail.com", false, "Admin", "Male", "Admin", false, null, null, "admin1", "AQAAAAEAACcQAAAAEPGlh+pkIEn2LyAt0MCdcmjBmjvnf/1kC4IGtumklyZ3GcHpjMCwlEKb170IpvvJ2g==", "1234567890", false, "0c7bb796-9e6a-4c77-adf6-7a8a9b2e6803", 0.0, "Activated", false, "admin1" }
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
                    { 1, "Tranfer" },
                    { 2, "Credit" },
                    { 3, "Payment on delivery" }
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
                values: new object[] { 1, "1", "Create account successfully +500 points", new DateTime(2023, 8, 7, 11, 41, 30, 126, DateTimeKind.Local).AddTicks(3205), 500 });

            migrationBuilder.InsertData(
                table: "Repositories",
                columns: new[] { "RepositoryId", "AddressId", "Capacity", "CreationDate", "RepositoryName" },
                values: new object[] { 1, 1, 50.0, new DateTime(2023, 8, 7, 11, 41, 30, 128, DateTimeKind.Local).AddTicks(3286), "Repository 1" });

            migrationBuilder.InsertData(
                table: "Supliers",
                columns: new[] { "SuplierId", "SuplierAddressId", "SuplierEmail", "SuplierName", "SuplierPhoneNums" },
                values: new object[] { 1, 3, "suplieremail1@gmail.com", "Suplier 1", "012334546677" });

            migrationBuilder.InsertData(
                table: "UserAddresses",
                columns: new[] { "AddressId", "UserId", "AddressType" },
                values: new object[] { 1, "1", "USER" });

            migrationBuilder.InsertData(
                table: "FurnitureSpecifications",
                columns: new[] { "FurnitureSpecificationId", "ColorId", "Description", "FurnitureId", "FurnitureSpecificationName", "Height", "Length", "Price", "Width", "WoodId" },
                values: new object[,]
                {
                    { 1, 1, " This is testing decription", 1, "Yellow", 50.0, 50.0, 50.0, 50.0, 1 },
                    { 2, 2, " This is testing decription", 1, "Red", 60.0, 40.0, 50.0, 50.0, 2 },
                    { 3, 1, " This is testing decription", 2, "Purple", 30.0, 70.0, 320.0, 20.0, 2 },
                    { 4, 2, " This is testing decription", 2, "Black", 50.0, 30.0, 120.0, 60.0, 1 }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "MaterialId", "DefaultSuplierId", "Description", "MaterialName", "MaterialPrice" },
                values: new object[,]
                {
                    { 1, 1, "Material Descriptint 1", "Material 1", 5 },
                    { 2, 1, "Material Descriptint 2", "Material 2", 4 },
                    { 3, 1, "Material Descriptint 3", "Material 3", 7 }
                });

            migrationBuilder.InsertData(
                table: "Feedbacks",
                columns: new[] { "FeedbackId", "Anonymous", "Content", "CreationDate", "CustomerId", "FurnitureSpecificationId", "VoteStar" },
                values: new object[] { 1, false, "This is the testing feedback", new DateTime(2023, 8, 7, 11, 41, 30, 128, DateTimeKind.Local).AddTicks(7224), "1", 1, 5 });

            migrationBuilder.InsertData(
                table: "FurnitureRepositories",
                columns: new[] { "FurnitureSpecificationId", "RepositoryId", "Available" },
                values: new object[,]
                {
                    { 1, 1, 0 },
                    { 2, 1, 0 }
                });

            migrationBuilder.InsertData(
                table: "MaterialRepositories",
                columns: new[] { "MaterialId", "RepositoryId", "Available" },
                values: new object[,]
                {
                    { 1, 1, 0 },
                    { 2, 1, 0 },
                    { 3, 1, 0 }
                });

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
                name: "IX_Attachments_FeedbackId",
                table: "Attachments",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_LikedItemId",
                table: "Attachments",
                column: "LikedItemId");

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
                name: "IX_CustomizeFurnitureOrderDetails_CustomizeFunitureId",
                table: "CustomizeFurnitureOrderDetails",
                column: "CustomizeFunitureId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitureOrderDetails_OrderId",
                table: "CustomizeFurnitureOrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizeFurnitures_CustomerId",
                table: "CustomizeFurnitures",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CustomerId",
                table: "Feedbacks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FurnitureSpecificationId",
                table: "Feedbacks",
                column: "FurnitureSpecificationId");

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
                name: "IX_Requirements_CategoryId",
                table: "Requirements",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_ColorId",
                table: "Requirements",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_CustomizeFurnitureId",
                table: "Requirements",
                column: "CustomizeFurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_WoodId",
                table: "Requirements",
                column: "WoodId");

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
                name: "IX_WarrantySchedules_OrderId",
                table: "WarrantySchedules",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantySchedules_UserId",
                table: "WarrantySchedules",
                column: "UserId");

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
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "CartDetails");

            migrationBuilder.DropTable(
                name: "CustomizeFurnitureOrderDetails");

            migrationBuilder.DropTable(
                name: "FurnitureRepositories");

            migrationBuilder.DropTable(
                name: "ImportDetais");

            migrationBuilder.DropTable(
                name: "MaterialRepositories");

            migrationBuilder.DropTable(
                name: "OrderDetais");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "WishListDetails");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Requirements");

            migrationBuilder.DropTable(
                name: "WarrantySchedules");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "WishLists");

            migrationBuilder.DropTable(
                name: "FurnitureSpecifications");

            migrationBuilder.DropTable(
                name: "CustomizeFurnitures");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "Supliers");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Furnitures");

            migrationBuilder.DropTable(
                name: "Woods");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Labels");
        }
    }
}
