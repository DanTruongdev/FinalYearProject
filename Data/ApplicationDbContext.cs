using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Gallery;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;


namespace OnlineShopping.Data
{

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.ChangeTracker.AutoDetectChangesEnabled = true;
            this.ChangeTracker.LazyLoadingEnabled = true;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<CustomizeFurniture> CustomizeFurnitures { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Furniture> Furnitures { get; set; }
        public DbSet<FurnitureSpecification> FurnitureSpecifications { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Wood> Woods { get; set; }
        public DbSet<FeedbackAttachment> FeedbackAttachments { get; set; }
        public DbSet<FurnitureSpecificationAttachment> FurnitureSpecificationAttachments { get; set; }
        public DbSet<WarrantyAttachment> WarrantyAttachments { get; set; }
        public DbSet<CustomizeFurnitureAttachment> CustomizeFurnitureAttachments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<FurnitureOrderDetail> OrderDetais { get; set; }
        public DbSet<CustomizeFurnitureOrderDetail> CustomizeFurnitureOrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<FurnitureRepository> FurnitureRepositories { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<ImportDetail> ImportDetais { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialRepository> MaterialRepositories { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Suplier> Supliers { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<WishListDetail> WishListDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedUser(builder);
            SeedRole(builder);
            SeedUserRole(builder);
            SeedCategory(builder);
            SeedColor(builder);
            SeedCustomizeFurniture(builder);
            SeedFurniture(builder);
            SeedFurnitureSpecification(builder);
            SeedLabel(builder);
            SeedWood(builder);
            SeedCart(builder);
            SeedCartDetail(builder);
            SeedOrder(builder);
            SeedFurnitureOrderDetail(builder);
            SeedCustomizeFurnitureOrderDetail(builder);
            SeedPayment(builder);
            SeedPoint(builder);
            SeedWishList(builder);
            SeedWishListDetail(builder);
            SeedFurnitureRepository(builder);
            SeedImport(builder);
            SeedImportDetail(builder);
            SeedMaterial(builder);
            SeedMaterialRepository(builder);
            SeedRepository(builder);
            SeedAddress(builder);
            SeedAnnouncement(builder);
            SeedFeedback(builder);
            SeedSuplier(builder);
            SeedUserAddress(builder);
            SeedWarrantySchedule(builder);
            SeedCollection(builder);
        }


        private void SeedUser(ModelBuilder builder)
        {
            var hashed = new PasswordHasher<User>();

            var customerTest = new Models.User
            {
                Id = "1",
                FirstName = "Customer",
                LastName = "Customer",
                UserName = "customer1",
                NormalizedUserName = "customer1",
                Email = "customer1@gmail.com",
                PhoneNumber = "1234567890",
                DoB = DateTime.Parse("2002-03-11"),
                Gender = "Male",
                Avatar = "customer.jpg",
                Spent = 0,
                Debit = 0,
                CreationDate = DateTime.Now,
                IsActivated = true
            };
            var assistantTest = new User
            {
                Id = "2",
                FirstName = "Assistant",
                LastName = "Assistant",
                UserName = "assistant1",
                NormalizedUserName = "assistant1",
                Email = "assistant1@gmail.com",
                PhoneNumber = "1234567890",
                DoB = DateTime.Parse("2002-8-21"),
                Gender = "Female",
                Avatar = "assistant.jpg",
                CreationDate = DateTime.Now,
                IsActivated = true
            };
            var managerTest = new User
            {
                Id = "3",
                FirstName = "Manager",
                LastName = "Manager",
                UserName = "manager1",
                NormalizedUserName = "manager1",
                Email = "manager1@gmail.com",
                PhoneNumber = "1234567890",
                DoB = DateTime.Parse("2000-05-12"),
                Gender = "Male",
                Avatar = "manager.jpg",
                CreationDate = DateTime.Now,
                IsActivated = true
            };
            var adminTest = new User
            {
                Id = "4",
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin1",
                NormalizedUserName = "admin1",
                Email = "admin1@gmail.com",
                PhoneNumber = "1234567890",
                DoB = DateTime.Parse("2001-07-13"),
                Gender = "Male",
                Avatar = "admin.jpg",
                CreationDate = DateTime.Now,
                IsActivated = true
            };
            customerTest.PasswordHash = hashed.HashPassword(customerTest, "CustomerPass@1");
            assistantTest.PasswordHash = hashed.HashPassword(assistantTest, "AssistantPass@1");
            managerTest.PasswordHash = hashed.HashPassword(managerTest, "ManagerPass@1");
            adminTest.PasswordHash = hashed.HashPassword(adminTest, "AdminPass@1");
            builder.Entity<User>().HasData(customerTest, assistantTest, managerTest, adminTest);
        }
        private void SeedRole(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
              new IdentityRole() { Id = "1", Name = "CUSTOMER", ConcurrencyStamp = "1", NormalizedName = "CUSTOMER" },
              new IdentityRole() { Id = "2", Name = "ASSISTANT", ConcurrencyStamp = "2", NormalizedName = "ASSISTANT" },
              new IdentityRole() { Id = "3", Name = "MANAGER", ConcurrencyStamp = "3", NormalizedName = "MANAGER" },
              new IdentityRole() { Id = "4", Name = "ADMIN", ConcurrencyStamp = "4", NormalizedName = "ADMIN" }
            );
        }
        private void SeedUserRole(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
              new IdentityUserRole<string>()
              {
                  UserId = "1",
                  RoleId = "1",
              },
              new IdentityUserRole<string>()
              {
                  UserId = "2",
                  RoleId = "2",
              },
              new IdentityUserRole<string>()
              {
                  UserId = "3",
                  RoleId = "3",
              },
              new IdentityUserRole<string>()
              {
                  UserId = "4",
                  RoleId = "4",
              }
            );

        }
        private void SeedCategory(ModelBuilder builder)
        {
            builder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Chair"
                },
                 new Category
                 {
                     CategoryId = 2,
                     CategoryName = "Table"
                 },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Bed"
                }
            );
        }
        private void SeedColor(ModelBuilder builder)
        {
            builder.Entity<Color>().HasData
            (
                new Color
                {
                    ColorId = 1,
                    ColorName = "Red"
                },
                new Color
                {
                    ColorId = 2,
                    ColorName = "Green"
                },
                new Color
                {
                    ColorId = 3,
                    ColorName = "Blue"
                }
            );
        }
        private void SeedCollection(ModelBuilder builder)
        {
            builder.Entity<Collection>()
                .HasMany<Furniture>(c => c.Furnitures)
                .WithOne(f => f.Collection)
                .HasForeignKey(f => f.CollectionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Collection>().HasData(
                new Collection()
                {
                    CollectionId = 1,
                    CollectionName = "Collection 1"
                }
            );
        }
        private void SeedFurniture(ModelBuilder builder)
        {
            builder.Entity<Furniture>()
                .HasOne<Category>(f => f.Category)
                .WithMany(c => c.Furnitures)
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Furniture>()
                .HasOne<Label>(f => f.Label)
                .WithMany(l => l.Furnitures)
                .HasForeignKey(f => f.LabelId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Furniture>()
                .HasMany<FurnitureSpecification>(r => r.FurnitureSpecifications)
                .WithOne(fs => fs.Furniture)
                .HasForeignKey(fs => fs.FurnitureId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Furniture>().HasData(
               new Furniture()
               {
                   FurnitureId = 1,
                   FurnitureName = "Testing Furniture 1",
                   CategoryId = 1,
                   LabelId = 1,
                   Like = 100,
                   Sold = 100,
                   VoteStar = 5,
                   AppopriateRoom = "Living room",
                   CollectionId = 1
               },
                new Furniture()
                {
                    FurnitureId = 2,
                    FurnitureName = "Testing Furniture 2",
                    CategoryId = 2,
                    LabelId = 2,
                    Like = 10,
                    Sold = 120,
                    VoteStar = 4.5,
                    AppopriateRoom = "Bed room",
                    CollectionId = 1,
                }

           );
        }
        private void SeedFurnitureSpecification(ModelBuilder builder)
        {
            builder.Entity<FurnitureSpecification>()
                .HasOne<Color>(fs => fs.Color)
                .WithMany(c => c.FurnitureSpecifications)
                .HasForeignKey(fs => fs.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<FurnitureSpecification>()
               .HasOne<Wood>(fs => fs.Wood)
               .WithMany(w => w.FurnitureSpecification)
               .HasForeignKey(fs => fs.WoodId)
               .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<FurnitureSpecification>()
                .HasMany<FurnitureSpecificationAttachment>(fs => fs.Attachments)
                .WithOne(fsa => fsa.FurnitureSpecification)
                .HasForeignKey(fsa => fsa.FurnitureSpecificationId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<FurnitureSpecification>().HasData(
                new FurnitureSpecification()
                {
                    FurnitureSpecificationId = "FS-"+Guid.NewGuid().ToString(),
                    FurnitureSpecificationName = "Yellow",
                    FurnitureId = 1,
                    Height = 50,
                    Width = 50,
                    Length = 50,
                    ColorId = 1,
                    Price = 50,
                    WoodId = 1,
                    Description = " This is testing decription"
                },
                new FurnitureSpecification()
                {
                    FurnitureSpecificationId = "FS-"+Guid.NewGuid().ToString(),
                    FurnitureSpecificationName = "Red",
                    FurnitureId = 1,
                    Height = 60,
                    Width = 50,
                    Length = 40,
                    Price = 50,
                    ColorId = 2,
                    WoodId = 2,
                    Description = " This is testing decription"
                },
                new FurnitureSpecification()
                {
                    FurnitureSpecificationId = "FS-" + Guid.NewGuid().ToString(),
                    FurnitureSpecificationName = "Purple",
                    FurnitureId = 2,
                    Height = 30,
                    Width = 20,
                    Length = 70,
                    Price = 320,
                    ColorId = 1,
                    WoodId = 2,
                    Description = " This is testing decription"
                },
                new FurnitureSpecification()
                {
                    FurnitureSpecificationId = "FS-" + Guid.NewGuid().ToString(),
                    FurnitureSpecificationName = "Black",
                    FurnitureId = 2,
                    Height = 50,
                    Width = 60,
                    Length = 30,
                    Price = 120,
                    ColorId = 2,
                    WoodId = 1,
                    Description = " This is testing decription"
                }
            );
        }
        private void SeedLabel(ModelBuilder builder)
        {
            builder.Entity<Label>().HasData(
                new Label()
                {
                    LabelId = 1,
                    LabelName = "New"
                },
                new Label()
                {
                    LabelId = 2,
                    LabelName = "Hot Sale"
                },
                new Label()
                {
                    LabelId = 3,
                    LabelName = "Best Saller"
                }
            );
        }
        private void SeedCustomizeFurniture(ModelBuilder builder)
        {
            builder.Entity<CustomizeFurniture>()
                .HasMany<CustomizeFurnitureAttachment>(cf => cf.Attachments)
                .WithOne(a => a.CustomizeFurniture)
                .HasForeignKey(a => a.CustomizeFurnitureId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<CustomizeFurniture>()
                .HasOne<Color>(cf => cf.Color)
                .WithMany(c => c.CustomizeFurnitures)
                .HasForeignKey(cf => cf.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<CustomizeFurniture>()
                .HasOne<Wood>(cf => cf.Wood)
                .WithMany(w => w.CustomizeFurnitures)
                .HasForeignKey(cf => cf.WoodId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<CustomizeFurniture>()
                .HasOne<Category>(cf => cf.Category)
                .WithMany(c => c.CustomizeFurnitures)
                .HasForeignKey(cf => cf.WoodId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<CustomizeFurniture>()
               .HasOne<User>(cf => cf.Customer)
               .WithMany(u => u.CustomizeFurnitures)
               .HasForeignKey(cf => cf.CustomerId)
               .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<CustomizeFurniture>()
               .HasMany<CustomizeFurnitureOrderDetail>(cf => cf.CustomizeFurnitureOrderDetails)
               .WithOne(c => c.CustomizeFurniture)
               .HasForeignKey(cf => cf.CustomizeFunitureId)
               .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<CustomizeFurniture>()
                .HasOne<Result>(cf => cf.Result)
                .WithOne(r => r.CustomizeFurniture)
                .HasForeignKey<Result>(r => r.CustomizeFurnitureId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedWood(ModelBuilder builder)
        {
            builder.Entity<Wood>().HasData(
                new Wood()
                {
                    WoodId = 1,
                    WoodType = "Cherry"
                },
                new Wood()
                {
                    WoodId = 2,
                    WoodType = "Ebony"
                },
                new Wood()
                {
                    WoodId = 3,
                    WoodType = "Eucalyptus"
                }

             );
        }
        private void SeedCart(ModelBuilder builder)
        {
            builder.Entity<Cart>()
                .HasOne<User>(c => c.Customer)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedCartDetail(ModelBuilder builder)
        {
            builder.Entity<CartDetail>().HasKey(cd => cd.CartDetailId);
            builder.Entity<CartDetail>()
                 .HasOne<FurnitureSpecification>(cd => cd.FurnitureSpecifition)
                 .WithMany(fs => fs.CartDetails)
                 .HasForeignKey(cd => cd.FurnitureSpecificationId)
                 .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<CartDetail>()
                .HasOne<Cart>(cd => cd.Cart)
                .WithMany(cd => cd.CartDetails)
                .HasForeignKey(c => c.CartId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedOrder(ModelBuilder builder)
        {
            builder.Entity<Order>()
                .HasOne<User>(or => or.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(or => or.CustomerId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Order>()
                .HasMany<FurnitureOrderDetail>(o => o.FurnitureOrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Order>()
                .HasOne<Payment>(o => o.Payment)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
        private void SeedFurnitureOrderDetail(ModelBuilder builder)
        {
            builder.Entity<FurnitureOrderDetail>()
                .HasOne<FurnitureSpecification>(fod => fod.FurnitureSpecification)
                .WithMany(f => f.FurnitureOrderDetails)
                .HasForeignKey(fod => fod.FurnitureSpecificationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
        private void SeedCustomizeFurnitureOrderDetail(ModelBuilder builder)
        {
            builder.Entity<CustomizeFurnitureOrderDetail>()
                .HasOne<CustomizeFurniture>(cfod => cfod.CustomizeFurniture)
                .WithMany(cf => cf.CustomizeFurnitureOrderDetails)
                .HasForeignKey(cfod => cfod.CustomizeFunitureId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedPayment(ModelBuilder builder)
        {
            builder.Entity<Payment>().HasData(

                new Payment()
                {
                    PaymentId = 1,
                    PaymentMethod = "CASH"
                },
                new Payment()
                {
                    PaymentId = 2,
                    PaymentMethod = "VNPAYQR"
                },
                new Payment()
                {
                    PaymentId = 3,
                    PaymentMethod = "VNBANK"
                },
                new Payment()
                {
                    PaymentId = 4,
                    PaymentMethod = "INTBANK"
                }
            );
        }
        private void SeedPoint(ModelBuilder builder)
        {
            builder.Entity<Point>()
                .HasOne<User>(p => p.User)
                .WithOne(u => u.Point)
                .HasForeignKey<Point>(p => p.CustomerId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Point>().HasData(
                new Point()
                {
                    PointId = 1,
                    CustomerId = "1",
                    Description = "Create account successfully +500 points",
                    TotalPoint = 500,
                    History = DateTime.Now
                }
            );
        }
        private void SeedWishList(ModelBuilder builder)
        {
            builder.Entity<WishList>()
                .HasOne<User>(wl => wl.Customer)
                .WithOne(u => u.WishList)
                .HasForeignKey<WishList>(wl => wl.CustomerId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<WishList>()
                .HasMany<WishListDetail>(wl => wl.WishListDetails)
                .WithOne(wd => wd.WishList)
                .HasForeignKey(wd => wd.WishListId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedWishListDetail(ModelBuilder builder)
        {
            builder.Entity<WishListDetail>()
                .HasOne<Furniture>(wd => wd.Furniture)
                .WithMany(f => f.WishListDetails)
                .HasForeignKey(wd => wd.FurnitureId)
                .OnDelete(DeleteBehavior.ClientCascade);

        }
        private void SeedFurnitureRepository(ModelBuilder builder)
        {
            builder.Entity<FurnitureRepository>().HasKey(fr => new { fr.RepositoryId, fr.FurnitureSpecificationId });
            builder.Entity<FurnitureRepository>()
                .HasOne<Repository>(fr => fr.Repository)
                .WithMany(r => r.FurnitureRepositories)
                .HasForeignKey(fr => fr.RepositoryId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<FurnitureRepository>()
                .HasOne<FurnitureSpecification>(fr => fr.FurnitureSpecification)
                .WithMany(fs => fs.FurnitureRepositories)
                .HasForeignKey(fr => fr.FurnitureSpecificationId)
                .OnDelete(DeleteBehavior.ClientCascade);          
        }
        private void SeedImport(ModelBuilder builder)
        {
            builder.Entity<Import>()
                .HasOne<Repository>(i => i.Repository)
                .WithMany(r => r.Imports)
                .HasForeignKey(i => i.RepositoryId)
                .OnDelete(DeleteBehavior.ClientNoAction);
            builder.Entity<Import>()
                .HasMany<ImportDetail>(i => i.ImportDetails)
                .WithOne(id => id.Import)
                .HasForeignKey(i => i.ImportId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Import>()
                .HasOne<User>(i => i.User)
                .WithMany(u => u.Imports)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedImportDetail(ModelBuilder builder)
        {
            builder.Entity<ImportDetail>()
                .HasOne<Material>(i => i.Material)
                .WithMany(m => m.ImportDetails)
                .HasForeignKey(i => i.MaterialId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedMaterial(ModelBuilder builder)
        {

            builder.Entity<Material>()
                .HasOne<Suplier>(m => m.DefaultSuplier)
                .WithMany(s => s.Materials)
                .HasForeignKey(m => m.DefaultSuplierId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Material>().HasData(
               new Material()
               {
                   MaterialId = 1,
                   MaterialName = "Material 1",
                   MaterialPrice = 5,
                   Description = "Material Descriptint 1",
                   DefaultSuplierId = 1
               },
               new Material()
               {
                   MaterialId = 2,
                   MaterialName = "Material 2",
                   MaterialPrice = 4,
                   Description = "Material Descriptint 2",
                   DefaultSuplierId = 1
               },
               new Material()
               {
                   MaterialId = 3,
                   MaterialName = "Material 3",
                   MaterialPrice = 7,
                   Description = "Material Descriptint 3",
                   DefaultSuplierId = 1
               }

            );
        }
        private void SeedMaterialRepository(ModelBuilder builder)
        {
            builder.Entity<MaterialRepository>().HasKey(mr => new { mr.RepositoryId, mr.MaterialId });
            builder.Entity<MaterialRepository>()
                .HasOne<Repository>(mr => mr.Repository)
                .WithMany(r => r.MaterialRepositories)
                .HasForeignKey(mr => mr.RepositoryId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<MaterialRepository>()
                .HasOne<Material>(mr => mr.Material)
                .WithMany(m => m.MaterialRepositories)
                .HasForeignKey(mr => mr.MaterialId)
                .OnDelete(DeleteBehavior.ClientCascade); ;
            builder.Entity<MaterialRepository>().HasData(
                new MaterialRepository()
                {
                    MaterialId = 1,
                    RepositoryId = 1
                },
                new MaterialRepository()
                {
                    MaterialId = 2,
                    RepositoryId = 1
                },
                new MaterialRepository()
                {
                    MaterialId = 3,
                    RepositoryId = 1
                }
            );
        }
        private void SeedRepository(ModelBuilder builder)
        {
            builder.Entity<Repository>()
                .HasOne<Address>(r => r.Address)
                .WithOne(a => a.Repository)
                .HasForeignKey<Repository>(r => r.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Repository>().HasData(
                new Repository()
                {
                    RepositoryId = 1,
                    RepositoryName = "Repository 1",
                    AddressId = 1,
                    Capacity = 50,
                    CreationDate = DateTime.Now,
                }
              );
        }
        private void SeedAddress(ModelBuilder builder)
        {
            builder.Entity<Address>()
                .HasMany<UserAddress>(a => a.UserAddresses)
                .WithOne(ua => ua.Address)
                .HasForeignKey(ua => ua.AddressId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Address>().HasData(
                //user
                new Address()
                {
                    AddressId = 1,
                    Street = "Street 1",
                    Ward = "Commune 1",
                    District = "District 1",
                    Provine = "Provine 1",
                    AddressOwner = "USER"
                },
                 // repository
                 new Address()
                 {
                     AddressId = 2,
                     Street = "Street 2",
                     Ward = "Commune 2",
                     District = "District 2",
                     Provine = "Provine 2",
                     AddressOwner = "REPOSITORY"
                 },
                new Address()
                {
                    AddressId = 3,
                    Street = "Street 3",
                    Ward = "Commune 3",
                    District = "District 3",
                    Provine = "Provine 3",
                    AddressOwner = "SUPLIER"
                }
                );
        }
        private void SeedAnnouncement(ModelBuilder builder)
        {
            builder.Entity<Announcement>()
                .HasOne<User>(a => a.User)
                .WithMany(u => u.Announcements)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Announcement>().HasData(
                new Announcement()
                {
                    AnnouncementId = 1,
                    UserId = "1",
                    Title = "Welcome",
                    Content = "Welcome to the furniture shopping website!"
                }
            );
        }
        private void SeedFeedback(ModelBuilder builder)
        {
            builder.Entity<Feedback>()
                .HasOne<FurnitureSpecification>(f => f.FurnitureSpecification)
                .WithMany(fs => fs.Feedbacks)
                .HasForeignKey(f => f.FurnitureSpecificationId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Feedback>()
                .HasOne<User>(f => f.Customer)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Feedback>()
                .HasMany<FeedbackAttachment>(f => f.Attachements)
                .WithOne(fa => fa.Feedback)
                .HasForeignKey(fa => fa.FeedbackId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Feedback>()
               .HasOne<Order>(f => f.Order)
               .WithMany(o => o.Feedbacks)
               .HasForeignKey(f => f.OrderId)
               .OnDelete(DeleteBehavior.ClientCascade);
        }
        private void SeedSuplier(ModelBuilder builder)
        {

            builder.Entity<Suplier>()
                .HasOne<Address>(s => s.Address)
                .WithOne(a => a.Suplier)
                .HasForeignKey<Suplier>(s => s.SuplierAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Suplier>().HasData(
                new Suplier()
                {
                    SuplierId = 1,
                    SuplierName = "Suplier 1",
                    SuplierAddressId = 3,
                    SuplierEmail = "suplieremail1@gmail.com",
                    SuplierPhoneNums = "012334546677",
                }
            );

        }
        private void SeedUserAddress(ModelBuilder builder)
        {
            builder.Entity<UserAddress>().HasKey(ua => new { ua.UserId, ua.AddressId });
            builder.Entity<UserAddress>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAddresses)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<UserAddress>()
                .HasOne(ua => ua.Address)
                .WithMany(a => a.UserAddresses)
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<UserAddress>().HasData(
                new UserAddress()
                {
                    UserId = "1",
                    AddressId = 1,
                    AddressType = "USER"
                }
            );
        }
        private void SeedWarrantySchedule(ModelBuilder builder)
        {
            builder.Entity<Warranty>()
                .HasOne<User>(ws => ws.User)
                .WithMany(u => u.Warranties)
                .HasForeignKey(ws => ws.UserId)
                 .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Warranty>()
                .HasMany<WarrantyAttachment>(w => w.Attachments)
                .WithOne(wa => wa.Warranty)
                .HasForeignKey(wa => wa.WarrantyId)
                .OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<Warranty>()
                .HasOne<Order>(ws => ws.Order)
                .WithMany(o => o.Warranties)
                .HasForeignKey(ws => ws.OrderId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
