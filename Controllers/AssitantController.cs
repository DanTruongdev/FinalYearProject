using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Hubs;
using OnlineShopping.Models.Funiture;
using OnlineShopping.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ApplicationDbContext _dbContext;
        public AssistantController(IHubContext<SignalHub> hubContext, ApplicationDbContext dbContext)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
        }

        //Category                                       

        [HttpGet("categories")]
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

        [HttpPost("categories/add")]
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

        [HttpPut("categories/update")]
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

        [HttpDelete("categories/remove/{id}")]
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

                                                                                                //wood
        [HttpGet("woods")]
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

        [HttpPost("woods/add")]
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

        [HttpPut("woods/update")]
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

        [HttpDelete("woods/remove/{id}")]
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

                                                                                            //label
        [HttpGet("labels")]
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

        [HttpPost("labels/add")]
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

        [HttpPut("labels/update")]
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

        [HttpDelete("labels/remove/{id}")]
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

                                                                                        //collection

        [HttpGet("collections")]
        public async Task<IActionResult> GetCollectio()
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

        [HttpPost("collections/add")]
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

        [HttpPut("collections/update")]
        public async Task<IActionResult> EditCollection([Required] int collectionId, [Required] string collectionName)
        {
            Collection editCollection = await _dbContext.Collections.FindAsync(collectionId);
            if (editCollection == null) return NotFound("Collection not found");
            if (collectionName.Length < 2) return BadRequest("Wood type must be greater than 1 character");
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

        [HttpDelete("collections/remove/{id}")]
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

    }
}
