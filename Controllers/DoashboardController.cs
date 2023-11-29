using Bogus;
using Bogus.DataSets;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShopping.Data;
using OnlineShopping.Libraries.Services;
using OnlineShopping.Models;
using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Warehouse;
using OnlineShopping.ViewModels.Dashboard;

namespace OnlineShopping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {



        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;    
        private readonly IProjectHelper _projectHelper;     
        private int currentMonth = DateTime.Now.Month;
        private int currentYear = DateTime.Now.Year;

        public DashboardController(ApplicationDbContext dbContext, UserManager<User> userManager, IProjectHelper projectHelper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _projectHelper = projectHelper;
        }



        //Assistant dashboard
        [HttpGet("assistant/statics")]
        public async Task<IActionResult>AssistantDashboardStatics()
        {
         
            int materialTotal = _dbContext.MaterialRepositories.Sum(m => m.Available);
            int furnitureTotal = _dbContext.FurnitureRepositories.Sum(f => f.Available);
            double spent = _dbContext.ImportDetais.Where(id => id.Import.CreationDate.Month == currentMonth && id.Import.CreationDate.Year == currentYear).Sum(id => (id.Material.MaterialPrice * id.Quantity));
            int materialExportTime = _dbContext.MaterialRepositoryHistories.Where(m => m.CreationDate.Month == currentMonth && m.CreationDate.Year == currentYear).Count();
            int furnitureExportTime = _dbContext.FurnitureRepositoryHistories.Where(f => f.CreationDate.Month == currentMonth && f.CreationDate.Year == currentYear).Count();
            var response = new
            {
                MaterialTotal = materialTotal,
                FurnitureTotal = furnitureTotal,
                Spent = spent,
                MaterialExportTime = materialExportTime,
                FurnitureExportTime = furnitureExportTime
            };
            return Ok(response);
        }

        [HttpGet("assistant/charts")]
        public async Task<IActionResult> AssistantDashboardChart()
        {
            List<int> materialImportTimeList = new List<int>();
            List<int> furnitureImportTimeList = new List<int>();       
            for (int i = 1; i <= 12; i++)
            {
                var materialImportByMonth = await _dbContext.MaterialRepositoryHistories.Where(m => m.CreationDate.Month == i && m.CreationDate.Year == currentYear).ToListAsync();
                var furnitureImportByMonth = await _dbContext.FurnitureRepositoryHistories.Where(f => f.CreationDate.Month == i && f.CreationDate.Year == currentMonth).ToListAsync();
                if (!materialImportByMonth.IsNullOrEmpty())
                {
                    materialImportTimeList.Add(materialImportByMonth.Count());                  
                }
                if (!furnitureImportByMonth.IsNullOrEmpty())
                {
                    furnitureImportTimeList.Add(furnitureImportByMonth.Count());
                    continue;
                }
                materialImportTimeList.Add(0);
                furnitureImportTimeList.Add(0);

            }
            


            List<DashboardData<int>> supplierData = new List<DashboardData<int>>();
            var suppliers = await _dbContext.ImportDetais.Where(id => id.Import.CreationDate.Month == currentMonth && id.Import.CreationDate.Year == currentYear).Select(id => id.Material.DefaultSuplier).ToListAsync();
            var groupSupplier = suppliers.GroupBy(s => s.SupplierId);
            foreach ( var group in groupSupplier)
            {
                supplierData.Add(new DashboardData<int>()
                {
                    Name = group.FirstOrDefault().SupplierName,
                    Data = group.Count()
                });
                
            }

            supplierData = supplierData.OrderByDescending(s => s.Data).ToList();
            if(supplierData.Count > 4) 
            {
               var tempData = supplierData.Skip(3).ToList();             
               supplierData = supplierData.Take(3).ToList();
               supplierData.Add(new DashboardData<int>()
               {
                   Name = "Other",
                   Data = tempData.Sum(temp => temp.Data)
               });
            }
            int importTotalTime = supplierData.Sum(s => s.Data);          
            List<DashboardData<double>> supplierPercents = new List<DashboardData<double>>();
            foreach (var supplier in supplierData)
            {
                supplierPercents.Add(new DashboardData<double>
                {
                    Name = supplier.Name,
                    Data = Math.Round((double) supplier.Data / importTotalTime * 100, 2)
                });              
            }

            
            

            var orders = _dbContext.Orders.Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear);
           
            var orderStatus = new
            {
                Processing = orders.Where(o => o.Status.Equals("Processing")).Count(),
                Preparing = orders.Where(o => o.Status.Equals("Preparing")).Count(),
                Delivering = orders.Where(o => o.Status.Equals("Delivering")).Count(),
                Delivered = orders.Where(o => o.Status.Equals("Delivered")).Count(),
                Canceled = orders.Where(o => o.Status.Equals("Canceled")).Count()
            };

            var response = new
            {
                materialImportTimeAnnually = materialImportTimeList,
                furnitureImportTimeAnnually = furnitureImportTimeList,
                TopSupplier = supplierPercents,
                orderStatus = orderStatus

            };
            return Ok(response);
        }

        [HttpGet("shop-owner/statics")]
        public async Task<IActionResult> ShopOwnerDashboardStatics()
        {          
            var orders = _dbContext.Orders;
            double spent = _dbContext.ImportDetais.Where(id => id.Import.CreationDate.Month == currentMonth && id.Import.CreationDate.Year == currentYear) == null ? 0 :
                            _dbContext.ImportDetais.Where(id => id.Import.CreationDate.Month == currentMonth && id.Import.CreationDate.Year == currentYear).Sum(id => (id.Material.MaterialPrice * id.Quantity));
            double income = orders.Where(o => o.Status.Equals("Delivered") && o.IsPaid && o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear) == null ? 0 :
                            orders.Where(o => o.Status.Equals("Delivered") && o.IsPaid && o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear).Sum(o => o.TotalCost);
            double cancel = orders.Where(o => o.Status.Equals("Canceled") && o.IsPaid && o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear) == null ? 0 :
                            orders.Where(o => o.Status.Equals("Canceled") && o.IsPaid && o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear).Sum(o => o.TotalCost);
            double profit = income - (spent + cancel);
            int totalFurniture = _dbContext.Furnitures.Count();
            int orderInCurrentMonth = orders.Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear) == null ? 0 : orders.Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear).Count();
            int customerAccount =  _userManager.GetUsersInRoleAsync("CUSTOMER").Result.Where(u => u.IsActivated).Count();

            var response = new
            {
                Profit = profit,
                TotalFurniture = totalFurniture,
                OrderInCurrentMonth = orderInCurrentMonth,
                CustomerAccount = customerAccount
            };
            return Ok(response);
        }

        [HttpGet("shop-owner/charts")]
        public async Task<IActionResult> ShopOwnerDashboardCharts()
        {
            var spentInYear = new List<double>();
            var incomeInYear = new List<double>();
            var newAccountInYear = new List<int>();
            var importDetails = _dbContext.ImportDetais;
            var orders = _dbContext.Orders;
            var users = await _userManager.GetUsersInRoleAsync("CUSTOMER");
         
            for (int i = 1 ; i <= 12; i++) 
            {
                int registerAccount = users.Where(u => u.CreationDate.Month == i && u.CreationDate.Year == currentYear) == null ? 0:
                                      users.Where(u => u.CreationDate.Month == i && u.CreationDate.Year == currentYear).Count();
                double spent = _dbContext.ImportDetais.Where(id => id.Import.CreationDate.Month == i) == null ? 0 :
                           _dbContext.ImportDetais.Where(id => id.Import.CreationDate.Month == i).Sum(id => (id.Material.MaterialPrice * id.Quantity));
                double income = orders.Where(o => o.Status.Equals("Delivered") && o.IsPaid && o.OrderDate.Month == i && o.OrderDate.Year == currentYear) == null ? 0 :
                                orders.Where(o => o.Status.Equals("Delivered") && o.IsPaid && o.OrderDate.Month == i && o.OrderDate.Year == currentYear).Sum(o => o.TotalCost);
                double cancel = orders.Where(o => o.Status.Equals("Canceled") && o.IsPaid && o.OrderDate.Month == i && o.OrderDate.Year == currentYear) == null ? 0 :
                                orders.Where(o => o.Status.Equals("Canceled") && o.IsPaid && o.OrderDate.Month == i && o.OrderDate.Year == currentYear).Sum(o => o.TotalCost);
                spentInYear.Add(spent);
                incomeInYear.Add(income - cancel);
                newAccountInYear.Add(registerAccount);                
            }

            var categoryData = new List<DashboardData<int>>();
            var categories = await _dbContext.OrderDetails.Where(od => od.Order.OrderDate.Month == currentMonth  && od.Order.OrderDate.Year == currentYear && od.Order.IsPaid).Select(od => od.FurnitureSpecification.Furniture.Category).ToListAsync();
            if (!categories.IsNullOrEmpty())
            {
                var groupCategory = categories.GroupBy(c => c.CategoryId);
                foreach (var category in groupCategory)
                {
                    categoryData.Add(new DashboardData<int>
                    {
                        Name = category.First().CategoryName,
                        Data = category.Count()
                    });
                }
                
                if (categoryData.Count > 4)
                {
                    var tempList = categoryData.Skip(3).ToList();
                    categoryData = categoryData.Take(3).ToList();
                    categoryData.Add(new DashboardData<int>
                    {
                        Name = "Other",
                        Data = tempList.Sum(t => t.Data)
                    }) ;
                }
               
            }

            var woodPercent = new List<DashboardData<double>>();
            var woods = await _dbContext.OrderDetails.Where(od => od.Order.OrderDate.Month == currentMonth && od.Order.OrderDate.Year == currentYear && od.Order.IsPaid).Select(od => od.FurnitureSpecification.Wood).ToListAsync();
            if (!woods.IsNullOrEmpty())
            {
                var woodData = new List<DashboardData<int>>();
                var groupWood = woods.GroupBy(w => w.WoodId);
                foreach (var wood in groupWood)
                {
                    woodData.Add(new DashboardData<int>()
                    {
                        Name = wood.First().WoodType,
                        Data = wood.Count()
                    });
                }

                if (woodData.Count > 4)
                {
                    var tempList = woodData.Skip(3).ToList();
                    woodData = woodData.Take(3).ToList();
                    woodData.Add(new DashboardData<int>()
                    {
                        Name = "Other",
                        Data = tempList.Sum(t => t.Data)
                    });
                }

                var sumWood = woodData.Sum(w => w.Data);              
                foreach (var wood in woodData)
                {
                    woodPercent.Add(new DashboardData<double>()
                    {
                        Name = wood.Name,
                        Data = Math.Round((double) wood.Data / sumWood * 100, 2)
                    });
                }
            }

            var response = new 
            {
                SpentInYear = spentInYear,
                Income = incomeInYear,
                Categories = categoryData,
                WoodPercent = woodPercent,
                NewAccountInYear = newAccountInYear
            
            };
            return Ok(response);

        }

        [HttpGet("shop-owner/tables")]
        public async Task<IActionResult> ShopOwnerDashboardTables()
        {
            var topSaleFurniture = new List<DashboardData<int>>();
            var orderDetails = await _dbContext.OrderDetails.Where(od => od.Order.OrderDate.Month == currentMonth && od.Order.OrderDate.Year == currentYear && od.Order.IsPaid && od.Order.Status.Equals("Delivered")).ToListAsync();
            if (!orderDetails.IsNullOrEmpty())
            {
                var furnitures = orderDetails.Select(od => od.FurnitureSpecification);
                var groupFurniture = furnitures.GroupBy(f => f.FurnitureId);
                foreach (var furniture in groupFurniture)
                {
                    var newData = new DashboardData<int>()
                    {
                        Name = furniture.First().FurnitureSpecificationName,
                        Data = furniture.Count() // sold
                    };
                    topSaleFurniture.Add(newData);
                }
            }
            
            var topFavoriteFurniture = new List<DashboardData<int>>();
            var wishList = await _dbContext.WishListDetails.ToListAsync();
            if (!wishList.IsNullOrEmpty())
            {
                var furnitures = wishList.Select(w => w.Furniture);
                var groupFurniture = furnitures.GroupBy(f => f.FurnitureId);
                foreach (var furniture in groupFurniture)
                {
                    var newData = new DashboardData<int>()
                    {
                        Name = furniture.First().FurnitureName,
                        Data = furniture.Count() // like
                    };
                    topSaleFurniture.Add(newData);
                }
            }

            topFavoriteFurniture = topFavoriteFurniture.OrderByDescending(list => list.Data).ToList();
            topSaleFurniture = topSaleFurniture.OrderByDescending(list => list.Data).ToList();
            var response = new
            {
                topSaleFurniture, topFavoriteFurniture
            };
            return Ok(response);

        }
        


    }
}
