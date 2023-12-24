﻿using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShopping.Data;
using OnlineShopping.Libraries.Services;
using OnlineShopping.Models;
using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Gallery;
using OnlineShopping.Models.Warehouse;
using OnlineShopping.ViewModels;
using OnlineShopping.ViewModels.Furniture;
using OnlineShopping.ViewModels.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopOwnerController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager; 
        private readonly ICloudinaryService _cloudinaryService;


        public ShopOwnerController(ApplicationDbContext dbContext, UserManager<User> userManager, ICloudinaryService cloudinaryService, IProjectHelper projectHelper)
        {
  
            _dbContext = dbContext;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;

        }



        //Customer request
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpGet("customer-requests/customize-furniture/search")]
        public async Task<IActionResult> SearchCustomFurniture(string? searchString)
        {
            var data = new List<CustomizeFurniture>();
            if (!searchString.IsNullOrEmpty()) { 
                data = await _dbContext.CustomizeFurnitures.Where(r => r.CustomizeFurnitureName.ToUpper().Contains(searchString.ToUpper())).ToListAsync();
                if (data.IsNullOrEmpty()) return Ok(data);
            }
            else
            {
                data = await _dbContext.CustomizeFurnitures.ToListAsync();
            }
          
            var response = data.Select(cf => new
            {
                CustomizeFurnitureId = cf.CustomizeFurnitureId,
                CustomerId = cf.CustomerId,
                CustomerName = cf.Customer.ToString(),
                CustomizeFurnitureName = cf.CustomizeFurnitureName,
                ColorId = cf.ColorId,
                Height = cf.Height,
                Width = cf.Width,
                Length = cf.Length,
                WoodId = cf.WoodId,
                Quantity = cf.Quantity,
                DesiredCompletionDate = cf.DesiredCompletionDate,
                CreationDate = cf.CreationDate,
                Images = cf.Attachments.Where(a => a.Type.Equals("Images")).Select( a => new
                {
                    AttachmentName = a.AttachmentName,
                    Path =  _cloudinaryService.GetSharedUrl(a.Path)
                }),
                Videos = cf.Attachments.Where(a => a.Type.Equals("Videos")).Select( a => new
                {
                    AttachmentName = a.AttachmentName,
                    Path =  _cloudinaryService.GetSharedUrl(a.Path)
                }),
                Status = cf.Result.Status
            });
            return Ok(response);
        }

        [Authorize(Roles = "SHOP_OWNER")]
        [HttpGet("customer-requests/customize-furniture")]
        public async Task<IActionResult> GetCustomizeFurnitureRequest(string status)
        {
            List<CustomizeFurniture> customizeFurnitures = new List<CustomizeFurniture>();
            if (status.Equals("All")) customizeFurnitures = await _dbContext.CustomizeFurnitures.OrderByDescending(cf => cf.CreationDate).ToListAsync();
            else customizeFurnitures = await _dbContext.CustomizeFurnitures.OrderByDescending(cf => cf.CreationDate).Where(cf => cf.Result.Status.Equals(status)).ToListAsync();
            if (customizeFurnitures.Count == 0) return NotFound($"There is no customize furniture with {status} status");       
            try
            {
                var response = customizeFurnitures.Select(cf => new
                {
                    CustomizeFurnitureId = cf.CustomizeFurnitureId,
                    CustomerId = cf.CustomerId,
                    CustomerName = cf.Customer.ToString(),
                    CustomizeFurnitureName = cf.CustomizeFurnitureName,
                    ColorId = cf.ColorId,
                    Height = cf.Height,
                    Width = cf.Width,
                    Length = cf.Length,
                    WoodId = cf.WoodId,
                    Quantity = cf.Quantity,
                    DesiredCompletionDate = cf.DesiredCompletionDate,
                    CreationDate = cf.CreationDate,
                    Images = cf.Attachments.Where(a => a.Type.Equals("Images")).Select( a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path =  _cloudinaryService.GetSharedUrl(a.Path)
                    }),
                    Videos = cf.Attachments.Where(a => a.Type.Equals("Videos")).Select( a => new
                    {
                        AttachmentName = a.AttachmentName,
                        Path =  _cloudinaryService.GetSharedUrl(a.Path)
                    }),
                    Status = cf.Result.Status
                }).ToList();
                return Ok(response);
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs during fetch data"));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPut("customize-requests/customize-furniture/confirm")]
        public async Task<IActionResult> ConfirmCustomizeFurnitureRequest([FromForm] ResultViewModel userInput)
        {
            CustomizeFurniture customizeFurniture = await _dbContext.CustomizeFurnitures.FindAsync(userInput.CustomizeFurnitureId);
            if (customizeFurniture == null) return NotFound("The customize furniture not found");
            if (userInput.Status.Equals(customizeFurniture.Result.Status)) return  StatusCode(StatusCodes.Status406NotAcceptable,
                    new Response("Error", $"Custom furniture status has been {userInput.Status} already"));
            Result result = customizeFurniture.Result;
            result.Status = userInput.Status;
            result.Reason = userInput.Reason;
            result.ActualCompletionDate = userInput.ActualCompletionDate;
            result.CustomizeFurnitureId = userInput.CustomizeFurnitureId;
            result.ExpectedPrice = userInput.ExpectedPrice;
            string content = userInput.Status.Equals("Accepted") ?
                "Your customize furniture status has been accepted, Now you can use it to order" :
                "Your customize furniture status has been not accepted, Go to your customize furniture to see details";
            Announcement newAnnoucement = new Announcement() 
            { 
                UserId = customizeFurniture.CustomerId, 
                Title = "Your customize furniture has been changed status",
                Content = content,
                CreationDate = DateTime.Now
            };
            await _dbContext.AddAsync(newAnnoucement);
            _dbContext.UpdateRange(customizeFurniture, result);
            await _dbContext.SaveChangesAsync();
            return Ok("Confirm customize furniture successfully");
        }

        //CRUD supplier
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpGet("shop-data/suppliers/search")]
        public async Task<IActionResult> SearchSuppliers(string searchString)
        {
            var supliers = await _dbContext.Supliers.Where(r => r.SupplierName.Contains(searchString)).ToListAsync();
            if (supliers.IsNullOrEmpty()) return Ok(new List<Supplier>());
            var response = supliers.Select( s => new
            {
                SuppllierId = s.SupplierId,
                SupplierName = s.SupplierName,
                SupplierAddress = s.Address.ToString(),
                SupplierImage =  _cloudinaryService.GetSharedUrl(s.SupplierImage),
                SupplierEmail = s.SupplierEmail,
                SupplierPhoneNum = s.SupplierPhoneNums
            }) ;
            return Ok(response);
        }
    
        [HttpGet("shop-data/suppliers")]
        public async Task<IActionResult> GetSupplier()
        {
            var suppliers = _dbContext.Supliers;
            if (suppliers.Count() == 0) return NotFound("There is no any supplier");
            var response = suppliers.Select( s => new
            {
                SupplierId = s.SupplierId,
                SupplierName = s.SupplierName,
                SupplierAddress = s.Address.ToString(),
                SuplierImage =  _cloudinaryService.GetSharedUrl(s.SupplierImage),
                SuplierEmail = s.SupplierEmail,
                SuplierPhoneNums = s.SupplierPhoneNums,
                MaterialProvided = s.Materials.Select(m => new
                {
                    MaterialId = m.MaterialId,
                    MaterialName = m.MaterialName
                })
            });
            return Ok(response);
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPost("shop-data/suppliers/add")]
        public async Task<IActionResult> AddSupplier([FromForm] SupplierViewModel userInput)
        {
            //check if supplier exists
            var supplierExist = await _dbContext.Supliers.Where(s => s.SupplierName.ToUpper().Equals(userInput.SupplierName.ToUpper()) ||
                                s.SupplierEmail.ToUpper().Equals(userInput.SuplierEmail.ToUpper()) || s.SupplierPhoneNums.Equals(userInput.SuplierPhoneNums)).ToListAsync();
            if (supplierExist.Count() > 0) return BadRequest("This supplier has existed");

            Models.Address newAddress = new Models.Address()
            {
                Street = userInput.Street,
                Ward = userInput.Ward,
                District = userInput.District,
                Provine = userInput.Provine,
                AddressOwner = "SUPPLIER"
            };

            try
            {
                await _dbContext.AddAsync(newAddress);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when adding supplier address"));
            }

            Supplier newSupplier = new Supplier()
            {
                SupplierName = userInput.SupplierName,
                SupplierAddressId = newAddress.AddressId,
                SupplierImage =  _cloudinaryService.UploadFile(userInput.SuplierImage),
                SupplierEmail = userInput.SuplierEmail,
                SupplierPhoneNums = userInput.SuplierPhoneNums
            };

            try
            {
                await _dbContext.AddAsync(newSupplier);
                await _dbContext.SaveChangesAsync();
                var response = new
                {
                    SupplierId = newSupplier.SupplierId,
                    SupplierName = newSupplier.SupplierName,
                    SupplierAddress = newAddress.ToString(),
                    SupplierImage =  _cloudinaryService.GetSharedUrl(newSupplier.SupplierImage),
                    SupplierEmail = newSupplier.SupplierEmail,
                    SupplierPhoneNums = newSupplier.SupplierPhoneNums                   
                };
                return Created("Create new supplier successfully",response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when adding new supplier"));
            }
           
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPut("shop-data/suppliers/{supplierId}/edit")]
        public async Task<IActionResult> EditSupplier([FromRoute] int supplierId, [FromForm] SupplierViewModel userInput)
        {
            Supplier supplierExist = await _dbContext.Supliers.FindAsync(supplierId);
            if (supplierExist == null) return NotFound(new Response("Error", $"The supplier with id = {supplierId} was not found"));
            
            Models.Address supplierAddress = supplierExist.Address;
            supplierAddress.Street = userInput.Street;
            supplierAddress.Ward = userInput.Ward;
            supplierAddress.District = userInput.District;
            supplierAddress.Provine = userInput.Provine;        
            supplierExist.SupplierName = userInput.SupplierName;
            supplierExist.SupplierImage = _cloudinaryService.UploadFile(userInput.SuplierImage);
            supplierExist.SupplierEmail = userInput.SuplierEmail;
            supplierExist.SupplierPhoneNums = userInput.SuplierPhoneNums;

            try
            {
                _dbContext.UpdateRange(supplierAddress, supplierExist);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", $"Update supplier with id = {supplierId} successfully"));

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"An error occurs when updating supplier with id = {supplierId}"));
            }                     
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpDelete("shop-data/suppliers/{supplierId}/remove")]
        public async Task<IActionResult> RemoveSupplier([FromRoute] int supplierId)
        {
            Supplier supplierExist = await _dbContext.Supliers.FindAsync(supplierId);
            if (supplierExist == null) return NotFound(new Response("Error", $"The supplier with id = {supplierId} was not found"));

            try
            {
                supplierExist.Materials.Clear();
                await _dbContext.SaveChangesAsync();
                _dbContext.Remove(supplierExist);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"An error occurs when removing the supplier with id = {supplierId}"));
            }
        }

        //export account csv ==> view in user controller



        //furniture/furniture specification, category, collection

        //view all and search furniture, furniture specification function redirects to customer controller
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPost("shop-data/furnitures/add")]
        public async Task<IActionResult> AddFurniture([FromForm] AddFurnitureViewModel userInput)
        {
            Category categoryExist = await _dbContext.Categories.FindAsync(userInput.CategoryId);
            if (categoryExist == null) return NotFound($"Category with {userInput.CategoryId} was not found");
            if (userInput.CollectionId.HasValue)
            {
                Collection collectionExist = await _dbContext.Collections.FindAsync(userInput.CollectionId);
                if (collectionExist == null) return NotFound($"Collection with {userInput.CollectionId} was not found");

            }
            Furniture newFurniture = new Furniture()
            {
                FurnitureName = userInput.FurnitureName,
                CategoryId = userInput.CategoryId,
                LabelId = 1,
                CollectionId = userInput.CollectionId.HasValue ? userInput.CollectionId : null,
                AppopriateRoom = userInput.AppropriateRoom
            };

            try
            {
                await _dbContext.AddAsync(newFurniture);
                await _dbContext.SaveChangesAsync();
                return Created("Create furniture successfully", new
                {
                    FurnitureId = newFurniture.FurnitureId,
                    CategoryId = newFurniture.CategoryId,
                    LabelId = newFurniture.LabelId,
                    CollectionId = newFurniture.CollectionId.HasValue ? newFurniture.CollectionId : null,
                    AppropriateRoom = newFurniture.AppopriateRoom
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when create new furniture"));
            }

        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPost("shop-data/furnitures/{furnitureId}/add")]
        public async Task<IActionResult> AddFurniturueSpecification([FromRoute] int furnitureId, [FromForm] AddFurnitureSpecificationViewModel userInput)
        {
            Furniture furnitureExist = await _dbContext.Furnitures.FindAsync(furnitureId);
            Color colorExist = await _dbContext.Colors.FindAsync(userInput.ColorId);
            Wood woodExist = await _dbContext.Woods.FindAsync(userInput.WoodId);
            if (furnitureExist == null) return NotFound($"Furniture with {furnitureId} was not found");
            if (colorExist == null) return NotFound($"Color with {userInput.ColorId} was not found");
            if (woodExist == null) return NotFound($"Wood with {userInput.WoodId} was not found");
            FurnitureSpecification newFurnitureSpecification = new FurnitureSpecification()
            {
                FurnitureSpecificationId = "FS-" + Guid.NewGuid().ToString().ToUpper(),
                FurnitureId = furnitureId,
                FurnitureSpecificationName = userInput.FurnitureSpecificationName,
                Height = userInput.Height,
                Width = userInput.Width,
                Length = userInput.Length,
                ColorId = userInput.ColorId,
                WoodId = userInput.WoodId,
                Price = userInput.Price,
                Description = userInput.Description
            };
            try
            {
                await _dbContext.AddAsync(newFurnitureSpecification);
                await _dbContext.SaveChangesAsync();

                if (userInput.UploadFiles.Count > 0)
                {
                    foreach (var file in userInput.UploadFiles)
                    {
                        var newAttachment = new FurnitureSpecificationAttachment()
                        {
                            FurnitureSpecificationId = newFurnitureSpecification.FurnitureSpecificationId,
                            AttachmentName = file.FileName,
                            Path =  _cloudinaryService.UploadFile(file),
                            Type = _cloudinaryService.ImageOrVideo(file)
                        };
                        await _dbContext.AddAsync(newAttachment);
                    }            
                }

                FurnitureRepository newFurnitureRepository = new FurnitureRepository()
                {
                    RepositoryId = 1,
                    FurnitureSpecificationId = newFurnitureSpecification.FurnitureSpecificationId,
                    Available = 0
                };
                await _dbContext.AddAsync(newFurnitureRepository);

                await _dbContext.SaveChangesAsync();
                return Created("Create furniture specification successfully", new
                {
                    FurnitureSpecificationId = newFurnitureSpecification.FurnitureSpecificationId,
                    FurnitureId = newFurnitureSpecification.FurnitureId,
                    FurnitureSpecificationName = newFurnitureSpecification.FurnitureSpecificationName,
                    Height = newFurnitureSpecification.Height,
                    Width = newFurnitureSpecification.Width,
                    Length = newFurnitureSpecification.Length,
                    ColorId = newFurnitureSpecification.ColorId,
                    WoodId = newFurnitureSpecification.WoodId,
                    Price = newFurnitureSpecification.Price,
                    Description = newFurnitureSpecification.Description,
                    Images = newFurnitureSpecification.Attachments.IsNullOrEmpty() ? new string[] { } : 
                        newFurnitureSpecification.Attachments.Where( a => a.Type.Equals("images")).Select(  a =>  _cloudinaryService.GetSharedUrl(a.Path)),
                    Videos = newFurnitureSpecification.Attachments.IsNullOrEmpty() ? new string[] {} :
                        newFurnitureSpecification.Attachments.Where( a => a.Type.Equals("videos")).Select( a =>  _cloudinaryService.GetSharedUrl(a.Path))
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when create new furniture specification"));
            }
        }

        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPut("shop-data/furnitures/edit")]
        public async Task<IActionResult> UpdateFurniturue([FromForm] EditFurnitureViewModel userInput)
        {
            Furniture furnitureExist = await _dbContext.Furnitures.FindAsync(userInput.FurnitureId);
            if (furnitureExist == null) return NotFound($"Furniture with {userInput.FurnitureId} was not found");
            if (userInput.CategoryId.HasValue)
            {
                Category categoryExist = await _dbContext.Categories.FindAsync(userInput.CategoryId);
                if (categoryExist == null) return NotFound($"Category with {userInput.CategoryId} was not found");
            }
            if (userInput.LabelId.HasValue)
            {
                Label labelExist = await _dbContext.Labels.FindAsync(userInput.LabelId);
                if (labelExist == null) return NotFound($"Label with {userInput.LabelId} was not found");

            }
            if (userInput.CollectionId.HasValue)
            {
                Collection collectionExist = await _dbContext.Collections.FindAsync(userInput.CollectionId);
                if (collectionExist == null) return NotFound($"Collection with {userInput.CollectionId} was not found");

            }

            furnitureExist.FurnitureName = userInput.FurnitureName.IsNullOrEmpty() ? furnitureExist.FurnitureName : userInput.FurnitureName;
            furnitureExist.CategoryId = userInput.CategoryId.HasValue ? userInput.CategoryId : furnitureExist.CategoryId;
            furnitureExist.LabelId = userInput.LabelId.HasValue ? userInput.LabelId : furnitureExist.LabelId;
            furnitureExist.CollectionId = userInput.CollectionId.HasValue ? userInput.CollectionId : furnitureExist.CollectionId;
            furnitureExist.AppopriateRoom = userInput.AppropriateRoom.IsNullOrEmpty() ? furnitureExist.AppopriateRoom : userInput.AppropriateRoom;

            try
            {
                _dbContext.Update(furnitureExist);
                await _dbContext.SaveChangesAsync();
                return Ok($"Update furniture with id = {userInput.FurnitureId} successfully");

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"An error occurs when updating furniture with id = {userInput.FurnitureId}"));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPut("shop-data/furnitures/{furnitureId}/edit")]
        public async Task<IActionResult> UpdateFurniturueSpecification([FromRoute] int furnitureId, [FromForm] EditFurnitureSpecificationViewModel userInput)
        {
            Furniture furnitureExist = await _dbContext.Furnitures.FindAsync(furnitureId);
            if (furnitureExist == null) NotFound($"Furniture  with {furnitureId} was not found");
            FurnitureSpecification furnitureSpecificationExist = furnitureExist.FurnitureSpecifications.FirstOrDefault(fs => fs.FurnitureSpecificationId.Equals(userInput.FurnitureSpecificationId));
            if (furnitureSpecificationExist == null) NotFound($"Furniture Specification with {userInput.FurnitureSpecificationId} was not found");
            if (userInput.ColorId.HasValue)
            {
                Color colorExist = await _dbContext.Colors.FindAsync(userInput.ColorId);
                if (colorExist == null) return NotFound($"Color with {userInput.ColorId} was not found");
            }
            if (userInput.WoodId.HasValue)
            {
                Wood woodExist = await _dbContext.Woods.FindAsync(userInput.WoodId);
                if (woodExist == null) return NotFound($"Wood with {userInput.WoodId} was not found");
            }
            furnitureSpecificationExist.FurnitureSpecificationName = userInput.FurnitureSpecificationName.IsNullOrEmpty() ? furnitureSpecificationExist.FurnitureSpecificationName : userInput.FurnitureSpecificationName;
            furnitureSpecificationExist.Height = userInput.Height.HasValue ? userInput.Height.Value : furnitureSpecificationExist.Height;
            furnitureSpecificationExist.Width = userInput.Width.HasValue ? userInput.Width.Value : furnitureSpecificationExist.Width;
            furnitureSpecificationExist.Length = userInput.Length.HasValue ? userInput.Length.Value : furnitureSpecificationExist.Length;
            furnitureSpecificationExist.ColorId = userInput.ColorId.HasValue ? userInput.ColorId.Value : furnitureSpecificationExist.ColorId;
            furnitureSpecificationExist.WoodId = userInput.WoodId.HasValue ? userInput.WoodId.Value : furnitureSpecificationExist.WoodId;
            furnitureSpecificationExist.Price = userInput.Price.HasValue ? userInput.Price.Value : furnitureSpecificationExist.Price;
            furnitureSpecificationExist.Description = userInput.Description.IsNullOrEmpty() ? furnitureSpecificationExist.Description : userInput.Description;
            if (!userInput.UploadFiles.IsNullOrEmpty())
            {
                foreach (var attachment in furnitureSpecificationExist.Attachments)
                {
                    if( _cloudinaryService.RemoveFile(attachment.Path)) _dbContext.Remove(attachment);
                };
                await _dbContext.SaveChangesAsync();
                foreach (var file in userInput.UploadFiles)
                {
                   
                    var newAttachment = new FurnitureSpecificationAttachment()
                    {
                        FurnitureSpecificationId = furnitureSpecificationExist.FurnitureSpecificationId,
                        AttachmentName = file.FileName,
                        Path =  _cloudinaryService.UploadFile(file),
                        Type = _cloudinaryService.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }
            try
            {
                _dbContext.Update(furnitureSpecificationExist);
                await _dbContext.SaveChangesAsync();
                return Ok($"Update furniture specification with id = {userInput.FurnitureSpecificationId} successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"An error occurs when updating furniture specification with id = {userInput.FurnitureSpecificationId}"));
            }

        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpDelete("shop-data/furnitures/delete/{furnitureId}")]
        public async Task<IActionResult> RemoveFurniture(int furnitureId)
        {
            Furniture furnitureExist = await _dbContext.Furnitures.FindAsync(furnitureId);
            if (furnitureExist == null) return NotFound($"The furniture with id = {furnitureId} was not found");
            if (furnitureExist.FurnitureSpecifications.Count > 0) return StatusCode(StatusCodes.Status405MethodNotAllowed,
                new Response("Error", $"Cannot remove the furniture with id = {furnitureId} because there are furniture specification using this furniture as foreign key"));
            if (furnitureExist.WishListDetails.Count > 0) return StatusCode(StatusCodes.Status405MethodNotAllowed,
                new Response("Error", $"Cannot remove the furniture with id = {furnitureId} because this furniture is placing in wish list of users"));
            _dbContext.Remove(furnitureExist);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpDelete("shop-data/furnitures/{furnitureId}/delete/{furnitureSpecificationId}")]
        public async Task<IActionResult> RemoveFurnitureSpecification(int furnitureId, string furnitureSpecificationId)
        {
            Furniture furnitureExist = await _dbContext.Furnitures.FindAsync(furnitureId);
            if (furnitureExist == null) return NotFound($"Furniture with id = {furnitureId} was not found");
            FurnitureSpecification furnitureSpecification = furnitureExist.FurnitureSpecifications.FirstOrDefault(f => f.FurnitureSpecificationId.Equals(furnitureSpecificationId));
            if (furnitureSpecification == null) return NotFound($"Furniture Specification with id = {furnitureSpecificationId} was not found");

            if (!furnitureSpecification.Attachments.IsNullOrEmpty())
            {
               
                foreach (var file in furnitureSpecification.Attachments)
                {
                     _cloudinaryService.RemoveFile(file.Path);
                }

            }
            try
            {
                furnitureSpecification.Attachments.Clear();
                furnitureSpecification.FurnitureOrderDetails.Clear();
                furnitureSpecification.FurnitureRepositories.Clear();
                furnitureSpecification.Feedbacks.Clear();
                furnitureSpecification.CartDetails.Clear();
                await _dbContext.SaveChangesAsync();
                _dbContext.Remove(furnitureSpecification);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"Cannot delete furniture specification with id = {furnitureSpecificationId} because an error occurs when removing dependency entity"));
            }
        }


        //collection
        
        [HttpGet("shop-data/collections")]
        public async Task<IActionResult> GetCollection()
        {
            var collections = await _dbContext.Collections.ToListAsync();
            if (collections.IsNullOrEmpty()) return NotFound("There is not any furniture collection");
            var response = collections.Select(c => new
            {
                CollectionId = c.CollectionId,
                CollectionName = c.CollectionName
            }).ToList();
            return Ok(response);
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPost("shop-data/collections/add")]
        public async Task<IActionResult> AddCollection([Required] string collectionName)
        {
            if (collectionName.Length < 2) return BadRequest(new Response("Error", "name must be greater than 1 character"));

            Collection collectionNameExist = await _dbContext.Collections.FirstOrDefaultAsync(c => c.CollectionName.ToUpper().Equals(collectionName.ToUpper().Trim()));
            if (collectionNameExist != null) return BadRequest(new Response("Error", "The collection already exists"));

            Collection newCollection = new Collection()
            {
                CollectionName = collectionName
            };
            try
            {
                await _dbContext.AddAsync(newCollection);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response("Success", "Create new collection successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when creating new collection"));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPut("shop-data/collections/update")]
        public async Task<IActionResult> EditCollection([Required] int collectionId, [Required] string collectionName)
        {
            Collection editCollection = await _dbContext.Collections.FindAsync(collectionId);
            if (editCollection == null) return NotFound(new Response("Error", $"The collection with id = {collectionId} not found"));
            if (collectionName.Length < 2) return BadRequest(new Response("Error", "Collection name must be greater than 1 character"));

            Collection collectionNameExist = await _dbContext.Collections.FirstOrDefaultAsync(c => c.CollectionName.ToUpper().Equals(collectionName.ToUpper().Trim()));
            if (collectionNameExist != null) return BadRequest(new Response("Error", "The collection already exists"));

            editCollection.CollectionName = collectionName;
            try
            {
                _dbContext.Update(editCollection);
                await _dbContext.SaveChangesAsync();
                return Ok($"update collection with id = {collectionId} successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when updating collection"));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpDelete("shop-data/collections/remove/{id}")]
        public async Task<IActionResult> RemoveCollection([Required] int id)
        {

            Collection deleteCollection = await _dbContext.Collections.FindAsync(id);
            if (deleteCollection == null) return NotFound("Collection not found");
            if (!deleteCollection.Furnitures.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response("Error", $"The collection with id = {id} cannot be deleted because there is furniture using this collection"));
            }
            try
            {
                _dbContext.Remove(deleteCollection);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response("Success", $"remove collection with id = {id} successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when remove collection"));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpGet("shop-data/collections/search")]
        public async Task<IActionResult> SearchCollection(string? searchString)
        {
            searchString = searchString == null ? string.Empty : searchString;
            searchString = searchString.ToUpper();
            var result = await _dbContext.Collections.Where(c => c.CollectionName.ToUpper().Contains(searchString)).ToListAsync();
            if (result.IsNullOrEmpty()) return NotFound("No results were found containing the given search string");
            var response = result.Select(c => new
            {
                CollectionId = c.CollectionId,
                CollectionName = c.CollectionName
            }).ToList();
            return Ok(response);
        }



        //Category                                       
    
        [HttpGet("shop-data/categories")]
        public async Task<IActionResult> GetCategory()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            if (categories.IsNullOrEmpty()) return NotFound("There is not any furniture category");
            var response = categories.Select(c => new
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
            return Ok(response);
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPost("shop-data/categories/add")]
        public async Task<IActionResult> AddCategory([Required] string categoryName)
        {
            if (categoryName.Length < 2) return BadRequest("Category name must be greater than 1 character");
            var categoryExist = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToUpper().Equals(categoryName.ToUpper()));
            if (categoryExist != null) return BadRequest(new Response("Error", "This category already exists"));
            Category newCategory = new Category()
            {
                CategoryName = categoryName
            };
            try
            {
                await _dbContext.AddAsync(newCategory);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, new Response("Success", "Create category successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when creating new category"));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPut("shop-data/categories/update")]
        public async Task<IActionResult> EditCategory([Required] int categoryId, [Required] string categoryName)
        {
            Category editCategory = await _dbContext.Categories.FindAsync(categoryId);
            if (editCategory == null) return NotFound("Category not found");
            if (categoryName.Length < 2) return BadRequest("Category name must be greater than 1 character");
            var categoryExist = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToUpper().Equals(categoryName.ToUpper()));
            if (categoryExist != null) return BadRequest(new Response("Error", "This category already exists"));
            editCategory.CategoryName = categoryName;
            try
            {
                _dbContext.Update(editCategory);
                await _dbContext.SaveChangesAsync();
                return Ok($"update category with id = {categoryId} successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when updating category" ));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpDelete("shop-data/categories/remove/{id}")]
        public async Task<IActionResult> RemoveCategory([Required] int id)
        {

            Category deleteCategory = await _dbContext.Categories.FindAsync(id);
            if (deleteCategory == null) return NotFound("Category not found");
            if (!deleteCategory.CustomizeFurnitures.IsNullOrEmpty() || !deleteCategory.Furnitures.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response("Error", "The category with id = {id} cannot be deleted because there is furniture using this category"));
            }
            try
            {
                _dbContext.Remove(deleteCategory);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response("Success", $"remove category with id = {id} successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when remove category"));
            }
        }
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpGet("shop-data/categories/search")]
        public async Task<IActionResult> SeachCategory(string? searchString)
        {
            searchString = searchString == null ? string.Empty : searchString;
            searchString = searchString.ToUpper();
            var result = await _dbContext.Categories.Where(c => c.CategoryName.ToUpper().Contains(searchString)).ToListAsync();
            if (result.IsNullOrEmpty()) return NotFound("No results were found containing the given search string");
            var response = result.Select(c => new
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
            return Ok(response);
        }

        //view customer order
        [Authorize(Roles = "SHOP_OWNER")]
        [HttpGet("customer-requests/orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            return RedirectToAction("GetAllOrder", "Assistant");
        }


        //add account in authentication controller

        [Authorize(Roles = "SHOP_OWNER")]
        [HttpPut("accounts/disable")]
        public async Task<IActionResult> ToggleAccountStatus(string userId)
        { 
            User userExist = await _userManager.FindByIdAsync(userId);
            if (userExist == null) return NotFound(new Response("Error", "The user was not found"));
            userExist.IsActivated = !userExist.IsActivated;
            try
            {
                _dbContext.Update(userExist);
                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", "Change status successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when updating user status"));
            }           
        }

        }
}
