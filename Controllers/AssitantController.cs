using Bogus.Extensions.Extras;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Hubs;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Libraries.Services;

using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Gallery;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;
using OnlineShopping.ViewModels;
using OnlineShopping.ViewModels.Furniture;
using OnlineShopping.ViewModels.Warehouse;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ApplicationDbContext _dbContext;
        private readonly ISMSService _smsService;
        private readonly IEmailService _emailService;
        public AssistantController(IHubContext<SignalHub> hubContext, ApplicationDbContext dbContext, ISMSService smsService, IEmailService emailService)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
            _smsService = smsService;
            _emailService = emailService;
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

        [HttpPost("shop-data/categories/add")]
        public async Task<IActionResult> AddCategory([Required] string categoryName)
        {
            if (categoryName.Length < 2) return BadRequest("Category name must be greater than 1 character");
            Category newCategory = new Category()
            {
                CategoryName = categoryName
            };
            try
            {
                await _dbContext.AddAsync(newCategory);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response() { Status = "Success", Message = "Create category successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when creating new category" });
            }
        }

        [HttpPut("shop-data/categories/update")]
        public async Task<IActionResult> EditCategory([Required] int categoryId, [Required] string categoryName)
        {
            Category editCategory = await _dbContext.Categories.FindAsync(categoryId);
            if (editCategory == null) return NotFound("Category not found");
            if (categoryName.Length < 2) return BadRequest("Category name must be greater than 1 character");
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
                    new Response() { Status = "Error", Message = "An error occurs when updating category" });
            }
        }

        [HttpDelete("shop-data/categories/remove/{id}")]
        public async Task<IActionResult> RemoveCategory([Required] int id)
        {

            Category deleteCategory = await _dbContext.Categories.FindAsync(id);
            if (deleteCategory == null) return NotFound("Category not found");
            if (!deleteCategory.CustomizeFurnitures.IsNullOrEmpty() || !deleteCategory.Furnitures.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response() { Status = "Error", Message = "The category with id = {id} cannot be deleted because there is furniture using this category" });
            }
            try
            {
                _dbContext.Remove(deleteCategory);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response() { Status = "Success", Message = $"remove category with id = {id} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when remove category" });
            }
        }

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
                                                                                                //wood
        [HttpGet("shop-data/woods")]
        public async Task<IActionResult> GetWood()
        {
            var woods = await _dbContext.Woods.ToListAsync();
            if (woods.IsNullOrEmpty()) return NotFound("There is not any furniture category");
            var response = woods.Select(w => new
            {
                CategoryId = w.WoodId,
                CategoryName = w.WoodType
            }).ToList();
            return Ok(response);
        }

        [HttpPost("shop-data/woods/add")]
        public async Task<IActionResult> AddWood([Required] string woodType)
        {
            if (woodType.Length < 2) return BadRequest("Wood type must be greater than 1 character");
            Wood newWood = new Wood()
            {
                WoodType = woodType
            };
            try
            {
                await _dbContext.AddAsync(newWood);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response() { Status = "Success", Message = "Create wood successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when creating new wood" });
            }
        }

        [HttpPut("shop-data/woods/update")]
        public async Task<IActionResult> EditWood([Required] int woodId, [Required] string woodType)
        {
            Wood editWood = await _dbContext.Woods.FindAsync(woodId);
            if (editWood == null) return NotFound("Wood not found");
            if (woodType.Length < 2) return BadRequest("Wood type must be greater than 1 character");
            editWood.WoodType = woodType;
            try
            {
                _dbContext.Update(editWood);
                await _dbContext.SaveChangesAsync();
                return Ok($"update wood with id = {woodId} successfully");
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when updating wood" });
            }
        }

        [HttpDelete("shop-data/woods/remove/{id}")]
        public async Task<IActionResult> RemoveWood([Required] int id)
        {

            Wood deleteWood = await _dbContext.Woods.FindAsync(id);
            if (deleteWood == null) return NotFound("Wood not found");
            if (!deleteWood.CustomizeFurnitures.IsNullOrEmpty() || !deleteWood.FurnitureSpecification.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response() { Status = "Error", Message = $"The wood with id = {id} cannot be deleted because there is furniture using this wood" });
            }
            try
            {
                _dbContext.Remove(deleteWood);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response() { Status = "Success", Message = $"remove wood with id = {id} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when remove wood" });
            }
        }

        [HttpGet("shop-data/woods/search")]
        public async Task<IActionResult> SearchWood(string? searchString)
        {
            searchString = searchString == null ? string.Empty : searchString;
            searchString = searchString.ToUpper();
            var result = await _dbContext.Woods.Where(c => c.WoodType.ToUpper().Contains(searchString)).ToListAsync();
            if (result.IsNullOrEmpty()) return NotFound("No results were found containing the given search string");
            var response = result.Select(w => new
            {
                WoodId = w.WoodId,
                WoodType = w.WoodType
            }).ToList();
            return Ok(response);
        }

                                                                                            //label
        [HttpGet("shop-data/labels")]
        public async Task<IActionResult> GetLabel()
        {
            var labels = await _dbContext.Labels.ToListAsync();
            if (labels.IsNullOrEmpty()) return NotFound("There is not any furniture category");
            var response = labels.Select(l => new
            {
                LabelId = l.LabelId,
                LabelName = l.LabelName
            }).ToList();
            return Ok(response);
        }

        [HttpPost("shop-data/labels/add")]
        public async Task<IActionResult> Addlabel([Required] string labelName)
        {
            if (labelName.Length < 2) return BadRequest("Label name must be greater than 1 character");
            Label newLabel = new Label()
            {
                LabelName = labelName
            };
            try
            {
                await _dbContext.AddAsync(newLabel);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response() { Status = "Success", Message = "Create new label successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when creating new label" });
            }
        }

        [HttpPut("shop-data/labels/update")]
        public async Task<IActionResult> EditLabel([Required] int labelId, [Required] string labelName)
        {
            Label editLabel = await _dbContext.Labels.FindAsync(labelId);
            if (editLabel == null) return NotFound("Label not found");
            if (labelName.Length < 2) return BadRequest("Wood type must be greater than 1 character");
            editLabel.LabelName = labelName;
            try
            {
                _dbContext.Update(editLabel);
                await _dbContext.SaveChangesAsync();
                return Ok($"update label with id = {labelId} successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when updating label" });
            }
        }

        [HttpDelete("shop-data/labels/remove/{id}")]
        public async Task<IActionResult> RemoveLabel([Required] int id)
        {

            Label deleteLabel = await _dbContext.Labels.FindAsync(id);
            if (deleteLabel == null) return NotFound("Label not found");
            if (!deleteLabel.Furnitures.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response() { Status = "Error", Message = $"The label with id = {id} cannot be deleted because there is furniture using this label" });
            }
            try
            {
                _dbContext.Remove(deleteLabel);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response() { Status = "Success", Message = $"remove label with id = {id} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when remove label" });
            }
        }

        [HttpGet("shop-data/labels/search")]
        public async Task<IActionResult> SearchLabel(string? searchString)
        {
            searchString = searchString == null ? string.Empty : searchString;
            searchString = searchString.ToUpper();
            var result = await _dbContext.Labels.Where(c => c.LabelName.ToUpper().Contains(searchString)).ToListAsync();
            if (result.IsNullOrEmpty()) return NotFound("No results were found containing the given search string");
            var response = result.Select(l => new
            {
                LabelId = l.LabelId,
                LabelName = l.LabelName
            }).ToList();
            return Ok(response);
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

        [HttpPost("shop-data/collections/add")]
        public async Task<IActionResult> AddCollection([Required] string collectionName)
        {
            if (collectionName.Length < 2) return BadRequest("Collection name must be greater than 1 character");
            Collection newCollection = new Collection()
            {
                CollectionName = collectionName
            };
            try
            {
                await _dbContext.AddAsync(newCollection);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response() { Status = "Success", Message = "Create new collection successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when creating new collection" });
            }
        }

        [HttpPut("shop-data/collections/update")]
        public async Task<IActionResult> EditCollection([Required] int collectionId, [Required] string collectionName)
        {
            Collection editCollection = await _dbContext.Collections.FindAsync(collectionId);
            if (editCollection == null) return NotFound("Collection not found");
            if (collectionName.Length < 2) return BadRequest("Collection name must be greater than 1 character");
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
                    new Response() { Status = "Error", Message = "An error occurs when updating collection" });
            }
        }

        [HttpDelete("shop-data/collections/remove/{id}")]
        public async Task<IActionResult> RemoveCollection([Required] int id)
        {

            Collection deleteCollection = await _dbContext.Collections.FindAsync(id);
            if (deleteCollection == null) return NotFound("Collection not found");
            if (!deleteCollection.Furnitures.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response() { Status = "Error", Message = $"The collection with id = {id} cannot be deleted because there is furniture using this collection" });
            }
            try
            {
                _dbContext.Remove(deleteCollection);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response() { Status = "Success", Message = $"remove collection with id = {id} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when remove collection" });
            }
        }

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


        //color

        [HttpGet("shop-data/colors")]
        public async Task<IActionResult> GetColor()
        {
            var colors = await _dbContext.Colors.ToListAsync();
            if (colors.IsNullOrEmpty()) return NotFound("There is not any furniture color");
            var response = colors.Select(c => new
            {
                ColorsId = c.ColorId,
                ColorName = c.ColorName
            }).ToList();
            return Ok(response);
        }

        [HttpPost("shop-data/colors/add")]
        public async Task<IActionResult> AddColor([Required] string colorName)
        {
            if (colorName.Length < 2) return BadRequest("Color name must be greater than 1 character");
            Color newColor = new Color()
            {
                ColorName = colorName
            };
            try
            {
                await _dbContext.AddAsync(newColor);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response() { Status = "Success", Message = "Create new color successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when creating new color" });
            }
        }

        [HttpPut("shop-data/colors/update")]
        public async Task<IActionResult> EditColor([Required] int colorId, [Required] string colorName)
        {
            Color editColor = await _dbContext.Colors.FindAsync(colorId);
            if (editColor == null) return NotFound("Color not found");
            if (colorName.Length < 2) return BadRequest("Color name must be greater than 1 character");
            editColor.ColorName = colorName;
            try
            {
                _dbContext.Update(editColor);
                await _dbContext.SaveChangesAsync();
                return Ok($"update color with id = {colorId} successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when updating color" });
            }
        }

        [HttpDelete("shop-data/colors/remove/{id}")]
        public async Task<IActionResult> RemoveColors([Required] int id)
        {

            Color deleteColor = await _dbContext.Colors.FindAsync(id);
            if (deleteColor == null) return NotFound("Color not found");
            if (!deleteColor.FurnitureSpecifications.IsNullOrEmpty() || !deleteColor.CustomizeFurnitures.IsNullOrEmpty())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response() { Status = "Error", Message = $"The color with id = {id} cannot be deleted because there is furniture using this color" });
            }
            try
            {
                _dbContext.Remove(deleteColor);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response() { Status = "Success", Message = $"remove color with id = {id} successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when remove color" });
            }
        }

        [HttpGet("shop-data/colors/search")]
        public async Task<IActionResult> SearchColor(string? searchString)
        {
            searchString = searchString == null ? string.Empty : searchString;
            searchString = searchString.ToUpper();
            var result = await _dbContext.Colors.Where(c => c.ColorName.ToUpper().Contains(searchString)).ToListAsync();
            if (result.IsNullOrEmpty()) return NotFound("No results were found containing the given search string");
            var response = result.Select(c => new
            {
                ColorId = c.ColorId,
                ColorName = c.ColorName
            }).ToList();
            return Ok(response);
        }

        //                                                                      Furniture /Specification Furniture
        
        [HttpGet("shop-data/furniures")]
        public async Task<IActionResult> GetAllFurnitures()
        {
            return RedirectToAction("GetAllFurniture", "Customer");
        }
        [HttpGet("shop-data/furniures/{furnitureId}")]
        public async Task<IActionResult> GetFurnitureSpecificationById([FromRoute]int furnitureId)
        {
            return RedirectToAction("GetFurnitureSpecificationById", "Customer", new {id = furnitureId});
        }

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
                    new Response() { Status = "Error", Message = "An error occurs when create new furniture" });
            }
            
        }

        [HttpPost("shop-data/furnitures/{furnitureId}/add")]
        public async Task<IActionResult> AddFurniturueSpecification([FromRoute]int furnitureId, [FromForm] AddFurnitureSpecificationViewModel userInput)
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

            if (userInput.UploadFiles.IsNullOrEmpty())
            {
                var fileHandle = new FileHandleService();
                foreach (var file in userInput.UploadFiles)
                {
                    var result = fileHandle.UploadFile("FurnitureSpecificaiton", file);
                    if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs during upload file" });
                    var newAttachment = new FurnitureSpecificationAttachment()
                    {
                        FurnitureSpecificationId = newFurnitureSpecification.FurnitureSpecificationId,
                        AttachmentName = file.FileName,
                        Path = result,
                        Type = fileHandle.ImageOrVideo(file)
                    };
                    await _dbContext.AddAsync(newAttachment);
                }
                await _dbContext.SaveChangesAsync();
            }

            try
            {
                await _dbContext.AddAsync(newFurnitureSpecification);
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
                    Description = newFurnitureSpecification.Description
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when create new furniture specification" });
            }          
        }

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
                    new Response() { Status = "Error", Message = $"An error occurs when updating furniture with id = {userInput.FurnitureId}" });
            }
        }

        [HttpPut("shop-data/furnitures/{furnitureId}/edit")]
        public async Task<IActionResult> UpdateFurniturueSpecification([FromRoute] int furnitureId, [FromForm] EditFurnitureSpecificationViewModel userInput)
        {
            Furniture furnitureExist= await _dbContext.Furnitures.FindAsync(furnitureId);
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
                var fileHandle = new FileHandleService();
                foreach (var attachment in furnitureSpecificationExist.Attachments)
                {
                    fileHandle.DeleteFile(attachment.Path);
                    _dbContext.Remove(attachment);
                };
                await _dbContext.SaveChangesAsync();
                foreach (var file in userInput.UploadFiles)
                {
                    var result = fileHandle.UploadFile("FurnitureSpecificaiton", file);
                    if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs during upload file" });
                    var newAttachment = new FurnitureSpecificationAttachment()
                    {
                        FurnitureSpecificationId = furnitureSpecificationExist.FurnitureSpecificationId,
                        AttachmentName = file.FileName,
                        Path = result,
                        Type = fileHandle.ImageOrVideo(file)
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
                    new Response() { Status = "Error", Message = $"An error occurs when updating furniture specification with id = {userInput.FurnitureSpecificationId}" });
            }

        }

        [HttpDelete("shop-data/furnitures/delete/{furnitureId}")]
        public async Task<IActionResult> RemoveFurniture(int furnitureId)
        {
            Furniture furnitureExist = await _dbContext.Furnitures.FindAsync(furnitureId);
            if (furnitureExist == null) return NotFound($"The furniture with id = {furnitureId} was not found");
            if (furnitureExist.FurnitureSpecifications.Count > 0) return StatusCode(StatusCodes.Status405MethodNotAllowed,
                new Response() { Status = "Error", Message = $"Cannot remove the furniture with id = {furnitureId} because there are furniture specification using this furniture as foreign key"});
            if (furnitureExist.WishListDetails.Count > 0) return StatusCode(StatusCodes.Status405MethodNotAllowed,
                new Response() { Status = "Error", Message = $"Cannot remove the furniture with id = {furnitureId} because this furniture is placing in wish list of users" });
            _dbContext.Remove(furnitureExist);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("shop-data/furnitures/{furnitureId}/delete/{furnitureSpecificationId}")]
        public async Task<IActionResult> RemoveFurnitureSpecification(int furnitureId, string furnitureSpecificationId)
        {
            Furniture furnitureExist = await _dbContext.Furnitures.FindAsync(furnitureId);
            if (furnitureExist == null) return NotFound($"Furniture with id = {furnitureId} was not found");
            FurnitureSpecification furnitureSpecification = furnitureExist.FurnitureSpecifications.FirstOrDefault(f => f.FurnitureSpecificationId.Equals(furnitureSpecificationId));
            if (furnitureSpecification == null) return NotFound($"Furniture Specification with id = {furnitureSpecificationId} was not found");

            if (!furnitureSpecification.Attachments.IsNullOrEmpty())
            {
                var fileHandler = new FileHandleService();
                foreach (var file in furnitureSpecification.Attachments)
                {
                    fileHandler.DeleteFile(file.Path);
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
                    new Response() { Status = "Error", Message = $"Cannot delete furniture specification with id = {furnitureSpecificationId} because an error occurs when removing dependency entity" });
            }
        }

        //Material
        [HttpGet("shop-data/materials")]
        public async Task<IActionResult> GetMaterial() 
        {
            var materials = await _dbContext.Materials.ToListAsync();
            var response = materials.Select(m => new
            {
                MaterialId = m.MaterialId,
                MaterialName = m.MaterialName,
                MaterialPrice = m.MaterialPrice,
                MaterialImage = m.MaterialImage,
                Description = m.Description,
                DefaultSuplierId = m.DefaultSuplierId,
                Available = m.MaterialRepositories.Select(mr => new
                {
                    RepositoryId = mr.Repository.RepositoryId,
                    RepositoryName = mr.Repository.RepositoryName,
                    Address = mr.Repository.Address.ToString(),
                    Available = mr.Available
                })
            });
            return Ok(response);           
        }

        [HttpPost("shop-data/materials/add")]
        public async Task<IActionResult> AddMaterial([FromForm] MaterialViewModel userInput)
        {
            Suplier SupplierExist = await _dbContext.Supliers.FindAsync(userInput.DefaultSuplierId);
            if (SupplierExist == null) return NotFound($"The supplier with id = {userInput.DefaultSuplierId} was not found");
            Material newMaterial = new Material()
            {
                MaterialName = userInput.MaterialName,
                MaterialPrice = userInput.MaterialPrice,
                Description = userInput.Description.IsNullOrEmpty() ? "" : userInput.Description,
                DefaultSuplierId = userInput.DefaultSuplierId
            };
         
            var fileHandler = new FileHandleService();
            var result = fileHandler.UploadFile("Material", userInput.UploadImage);
            if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                new Response() { Status = "Error", Message = "An error occurs during upload file" });
            newMaterial.MaterialImage = result;
            
            try
            {
                await _dbContext.AddAsync(newMaterial);
                await _dbContext.SaveChangesAsync();
                return Created("Create new material successfully", new
                {
                    MaterialId = newMaterial.MaterialId,
                    MaterialName = newMaterial.MaterialName,
                    MaterialPrice = newMaterial.MaterialPrice,
                    Description = newMaterial.Description,
                    MaterialImage = newMaterial.MaterialImage,
                    DefaultSuplierId = newMaterial.DefaultSuplierId

                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occur when adding new material" });
            }

        }


        [HttpPut("shop-data/materials/edit")]
        public async Task<IActionResult> EditMaterial([FromForm] EditMaterialViewModel userInput)
        {
            Material materialExist = await _dbContext.Materials.FindAsync(userInput.MaterialId);
            if (materialExist == null) return NotFound($"The material with id = {userInput.MaterialId} was not found");
            if (userInput.DefaultSuplierId.HasValue)
            {
                Suplier supplierExist = await _dbContext.Supliers.FindAsync(userInput.DefaultSuplierId);
                if (supplierExist == null) return NotFound($"The supplier with id = {userInput.DefaultSuplierId} was not found");
            }
            materialExist.MaterialName = userInput.MaterialName.IsNullOrEmpty() ? materialExist.MaterialName : userInput.MaterialName;
            materialExist.MaterialPrice = userInput.MaterialPrice.HasValue ? userInput.MaterialPrice.Value : materialExist.MaterialPrice;
            materialExist.Description = userInput.Description.IsNullOrEmpty() ? materialExist.Description : userInput.Description;
            materialExist.DefaultSuplierId = userInput.DefaultSuplierId.HasValue ? userInput.DefaultSuplierId.Value :  materialExist.DefaultSuplierId;
            if (userInput.UploadImage != null)
            {
                var fileHandler = new FileHandleService();
                fileHandler.DeleteFile(materialExist.MaterialImage);
                var result = fileHandler.UploadFile("Material", userInput.UploadImage);
                if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs during upload file" });
                materialExist.MaterialImage = result;
            }
            try
            {
                _dbContext.Update(materialExist);
                await _dbContext.SaveChangesAsync();
                return Ok($"Update the material with id = {materialExist.MaterialId} successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = $"An error occur when updating material with id = {materialExist.MaterialId}" });
            }
        }

        [HttpPut("shop-data/materials/delete/{materialId}")]
        public async Task<IActionResult> RemoveMaterial([FromRoute] int materialId)
        {
            Material materialExist = await _dbContext.Materials.FindAsync(materialId);
            if (materialExist == null) return NotFound($"The material with id = {materialId} was not found");
            try
            {
                materialExist.MaterialRepositories.Clear();
                materialExist.ImportDetails.Clear();
                await _dbContext.SaveChangesAsync();
                _dbContext.Remove(materialExist);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = $"An error occur when removing material with id = {materialId}"});
            }
          
        }

        // Import
        [HttpGet("warehouse/material/import")]
        public async Task<IActionResult> GetImports()
        {
            var imports = await _dbContext.Imports.ToListAsync();
            if (imports.Count < 0) return NotFound("There are not any import orders");
            var response = imports.Select(i => new
            {
                ImportId = i.ImportId,
                CreatedBy = i.User.FirstName + " " + i.User.LastName,
                RepositoryId = i.RepositoryId,
                RepositoryAddress = i.Repository.Address.ToString(),
                TotalCost = i.ImportDetails.Sum(id => (id.Material.MaterialPrice * id.Quantity)),
                ImportItems = i.ImportDetails.Select(item => new
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Note = item.Note.IsNullOrEmpty() ? "" : item.Note
                }),
                BillImage = i.BillImage,
                CreationDate = i.CreationDate,
                DeliveryDate = i.DeliveryDate,
                Status = i.Status
            });
            return Ok(response);
        }

        [HttpPost("warehouse/material/imports/create")]
        public async Task<IActionResult> CreateImports(ImportViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Loggined user not found ");
            var logginedUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            Repository respositoryExist = await _dbContext.Repositories.FindAsync(userInput.RepositoryId);
            if (respositoryExist == null) return NotFound($"The repository with id = {userInput.RepositoryId} was not found");
            Import newImport = new Import()
            {
                UserId = logginedUser.Id,
                RepositoryId = userInput.RepositoryId,
                CreationDate = DateTime.Now,
                BillImage = "",
                Status = "Processing"
            };
            try
            {
                await _dbContext.AddAsync(newImport);
                await _dbContext.SaveChangesAsync();
            } 
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when adding new import" });
            }

            foreach(var item in userInput.Items)
            {
                Material materialExist = await _dbContext.Materials.FindAsync(item.MaterialId);
                if (materialExist == null) NotFound($"Material with id = {item.MaterialId} was not found");
                if (materialExist.DefaultSuplier == null) return NotFound($"Default supplier of material with id = {item.MaterialId} was not found");
                ImportDetail newItem = new ImportDetail()
                {
                    ImportId = newImport.ImportId,
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Note = item.Note.IsNullOrEmpty() ? "" : item.Note
                };
              
                try
                {
                    await _dbContext.AddAsync(newItem);
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when adding new import item" });
                }
            }
            return Created("Create new import successfully", new
            {
                ImportId = newImport.ImportId,
                CrearetedBy = logginedUser.FirstName + " " + logginedUser.LastName,
                RepositoryId = userInput.RepositoryId,           
                ImportItems = newImport.ImportDetails.Select(item => new
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Note = item.Note.IsNullOrEmpty() ? "" : item.Note
                }),
                Status = newImport.Status
            });

        }

        [HttpPut("warehouse/material/imports/confirm/{importId}")]
        public async Task<IActionResult> ConfirmImports([FromRoute] int importId,  [FromForm] ConfirmImportViewModel userInput)
        {
            Import importExist = await _dbContext.Imports.FindAsync(importId);
            if (importExist == null) return NotFound($"The import with id = {importId} was not found");
            importExist.DeliveryDate = userInput.DeliveryDate;
            importExist.Status = "Deliveried";
            var fileHandler = new FileHandleService();
            var result = fileHandler.UploadFile("Import", userInput.BillImage);
            if (result.Equals("Error")) return StatusCode(StatusCodes.Status500InternalServerError,
                new Response() { Status = "Error", Message = "An error occurs during upload file" });
            importExist.BillImage = result;
            List<ImportDetail> materialList = importExist.ImportDetails.ToList();
            //add material to repository
            foreach(var material in materialList)
            {
                MaterialRepository materialRepositoryExist = await _dbContext.MaterialRepositories.FirstOrDefaultAsync(m => m.RepositoryId == importExist.RepositoryId &&
                                                               m.MaterialId == material.MaterialId);
                
                if(materialRepositoryExist == null)
                {
                    MaterialRepository newMaterialRepository = new MaterialRepository()
                    {
                        RepositoryId = importExist.RepositoryId,
                        MaterialId = material.MaterialId,
                        Available = material.Quantity
                    };
                    try
                    {
                        await _dbContext.AddAsync(newMaterialRepository);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response() { Status = "Error", Message = "An error occurs when adding material to repository"});
                    }
                }
                else
                {
                    materialRepositoryExist.Available += material.Quantity;
                }
                
            }
            try
            {
                
                _dbContext.Update(importExist);
                await _dbContext.SaveChangesAsync();
                return Ok("Confirm import successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
               new Response() { Status = "Error", Message = "An error occurs when confirm import" });
            }
            
        }


        [HttpDelete("warehouse/material/imports/remove/{importId}")] 
        public async Task<IActionResult> RemoveImports([FromRoute] int importId)
        {
            Import importExist = await _dbContext.Imports.FindAsync(importId);
            if (importExist == null) return NotFound($"The import with id = {importId} was not found");
            if (importExist.Status.Equals("Deliveried")) return BadRequest("Cannot remove the confirmed import");
            try
            {
                importExist.ImportDetails.Clear();
                await _dbContext.SaveChangesAsync();
                _dbContext.Remove(importExist);
                await _dbContext.SaveChangesAsync();                    
                return Ok($"Remove import with  id = {importId} successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response() { Status = "Error", Message = "An error occurs when removing import" });
            }
           
        }

        //Repository 
       



        //Supplier 
        




    }
}
