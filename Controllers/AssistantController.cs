using Castle.Core.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Hubs;
using OnlineShopping.Libraries.Services;
using OnlineShopping.Models;
using OnlineShopping.Models.Customer;
using OnlineShopping.Models.Funiture;
using OnlineShopping.Models.Purchase;
using OnlineShopping.Models.Warehouse;
using OnlineShopping.ViewModels;
using OnlineShopping.ViewModels.Post;
using OnlineShopping.ViewModels.Warehouse;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        private readonly IHubContext<SignalHub> _hubContext;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ISMSService _smsService;
        private readonly IEmailService _emailService;
        private readonly IFirebaseService _firebaseService;
        private readonly IProjectHelper _projectHelper;
        public AssistantController(IHubContext<SignalHub> hubContext, ApplicationDbContext dbContext, ISMSService smsService, IEmailService emailService, 
            UserManager<User> userManager, IFirebaseService firebaseService, IProjectHelper projectHelper)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
            _smsService = smsService;
            _emailService = emailService;
            _userManager = userManager;
            _firebaseService = firebaseService;
            _projectHelper = projectHelper;
        }



        //wood


        [HttpGet("shop-data/woods")]
        public async Task<IActionResult> GetWood()
        {
            var woods = await _dbContext.Woods.ToListAsync();
            if (woods.IsNullOrEmpty()) return NotFound("There is not any furniture category");
            var response = woods.Select(w => new
            {
                WoodId = w.WoodId,
                WoodType = w.WoodType
            }).ToList();
            return Ok(response);
        }

        [HttpPost("shop-data/woods/add")]
        public async Task<IActionResult> AddWood([Required] string woodType)
        {
            if (woodType.Length < 2) return BadRequest(new Response("Error", "Wood type must be greater than 1 character"));

            Wood woodTypeExist = await _dbContext.Woods.FirstOrDefaultAsync(w => w.WoodType.ToUpper().Equals(woodType.ToUpper().Trim()));
            if (woodTypeExist != null) return BadRequest(new Response("Error", "The wood type already exists")); 

            Wood newWood = new Wood()
            {
                WoodType = woodType
            };
            try
            {
                await _dbContext.AddAsync(newWood);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response("Success", "Create wood successfully"));
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when creating new wood"));
            }
        }

        [HttpPut("shop-data/woods/update")]
        public async Task<IActionResult> EditWood([Required] int woodId, [Required] string woodType)
        {
            Wood editWood = await _dbContext.Woods.FindAsync(woodId);
            if (editWood == null) return NotFound(new Response("Error", $"The wood with id = {woodId}  not found"));

            Wood woodTypeExist = await _dbContext.Woods.FirstOrDefaultAsync(w => w.WoodType.ToUpper().Equals(woodType.ToUpper().Trim()));
            if (woodTypeExist != null) return BadRequest(new Response("Error", "The wood type already exists"));

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
                    new Response("Error", "An error occurs when updating wood"));
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
                    new Response("Error", $"The wood with id = {id} cannot be deleted because there is furniture using this wood"));
            }
            try
            {
                _dbContext.Remove(deleteWood);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response("Success", $"remove wood with id = {id} successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when remove wood"));
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
        public async Task<IActionResult> AddLabel([Required] string labelName)
        {
            if (labelName.Length < 2) return BadRequest("Label name must be greater than 1 character");
            Label lableNameExist = await _dbContext.Labels.FirstOrDefaultAsync(l =>  l.LabelName.ToUpper().Equals(labelName.ToUpper()));
            if (lableNameExist != null) return BadRequest(new Response("Error", "The label already exists")); 
            Label newLabel = new Label()
            {
                LabelName = labelName
            };
            try
            {
                await _dbContext.AddAsync(newLabel);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response("Success", "Create new label successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when creating new label"));
            }
        }

        [HttpPut("shop-data/labels/update")]
        public async Task<IActionResult> EditLabel([Required] int labelId, [Required] string labelName)
        {
            Label editLabel = await _dbContext.Labels.FindAsync(labelId);
            if (editLabel == null) return NotFound(new Response("Error", $"The label with id = {labelId} not found"));

            if (labelName.Length < 2) return BadRequest(new Response("Error", "The label must be greater than 1 character"));

            Label lableNameExist = await _dbContext.Labels.FirstOrDefaultAsync(l => l.LabelName.ToUpper().Equals(labelName.ToUpper()));
            if (lableNameExist != null) return BadRequest(new Response("Error", "The label already exists"));

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
                    new Response("Error", "An error occurs when updating label"));
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
                    new Response("Error", $"The label with id = {id} cannot be deleted because there is furniture using this label"));
            }
            try
            {
                _dbContext.Remove(deleteLabel);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response("Success", $"remove label with id = {id} successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when remove label"));
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
            if (colorName.Length < 2) return BadRequest(new Response("Error", "Color name must be greater than 1 character"));
            

            Color colorExist = await _dbContext.Colors.FirstOrDefaultAsync(c => c.ColorName.ToUpper().Equals(colorName.Trim().ToUpper()));
            if (colorExist != null) return BadRequest(new Response("Error", "Color name already exists"));   
            
            Color newColor = new Color()
            {
                ColorName = colorName
            };
            try
            {
                await _dbContext.AddAsync(newColor);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created,
                    new Response("Success", "Create new color successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when creating new color"));
            }
        }

        [HttpPut("shop-data/colors/update")]
        public async Task<IActionResult> EditColor([Required] int colorId, [Required] string colorName)
        {
            Color editColor = await _dbContext.Colors.FindAsync(colorId);
            if (editColor == null) return NotFound(new Response("Error", $"The color with {colorId} was not found"));

            Color colorExist = await _dbContext.Colors.FirstOrDefaultAsync(c => c.ColorName.ToUpper().Equals(colorName.Trim().ToUpper()));
            if (colorExist != null) return BadRequest(new Response("Error", "Color name already exists"));

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
                    new Response("Error", "An error occurs when updating color"));
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
                    new Response("Error", $"The color with id = {id} cannot be deleted because there is furniture using this color"));
            }
            try
            {
                _dbContext.Remove(deleteColor);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status204NoContent,
                    new Response("Success", $"remove color with id = {id} successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occurs when remove color"));
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



        //Material
        [HttpGet("shop-data/materials/search")]
        public async Task<IActionResult> SearchMaterial(string? searchString)
        {
            
            searchString = searchString.IsNullOrEmpty() ? String.Empty : searchString.Trim().ToUpper();
            var result = await _dbContext.Materials.Where(f => f.MaterialName.ToUpper().Contains(searchString)).ToListAsync();
            if (result.IsNullOrEmpty()) return Ok(new List<Material>());
            var response = result.Select(m => new
            {
                MaterialId = m.MaterialId,
                MaterialName = m.MaterialName,
                MaterialPrice = m.MaterialPrice,
                MaterialImage = _firebaseService.GetDownloadUrl(m.MaterialImage),
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
        [HttpGet("shop-data/materials")]
        public async Task<IActionResult> GetMaterial() 
        {
            var materials = await _dbContext.Materials.ToListAsync();
            var response = materials.Select(m => new
            {
                MaterialId = m.MaterialId,
                MaterialName = m.MaterialName,
                MaterialPrice = m.MaterialPrice,
                MaterialImage = _firebaseService.GetDownloadUrl(m.MaterialImage),
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
            Supplier SupplierExist = await _dbContext.Supliers.FindAsync(userInput.DefaultSuplierId);
            if (SupplierExist == null) return NotFound(new Response("Error", $"The supplier with id = {userInput.DefaultSuplierId} was not found"));
           

            Material materialExist = await _dbContext.Materials.FirstOrDefaultAsync(m => m.MaterialName.ToUpper().Equals(userInput.MaterialName.ToUpper()));
            if (materialExist != null) return BadRequest(new Response("Error", "The material already exists"));
           

            Material newMaterial = new Material()
            {
                MaterialName = userInput.MaterialName,
                MaterialPrice = userInput.MaterialPrice,
                Description = userInput.Description.IsNullOrEmpty() ? "" : userInput.Description,
                DefaultSuplierId = userInput.DefaultSuplierId,
                MaterialImage = _firebaseService.UploadFile(userInput.UploadImage)
            };
         
           
           
            
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
                    MaterialImage = _firebaseService.GetDownloadUrl(newMaterial.MaterialImage),
                    DefaultSuplierId = newMaterial.DefaultSuplierId

                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occur when adding new material"));
            }

        }


        [HttpPut("shop-data/materials/edit")]
        public async Task<IActionResult> EditMaterial([FromForm] EditMaterialViewModel userInput)
        {
            Material materialExist = await _dbContext.Materials.FindAsync(userInput.MaterialId);
            if (materialExist == null) return NotFound(new Response("Error", $"The material with id = {userInput.MaterialId} was not found"));
          

            if (userInput.DefaultSuplierId.HasValue)
            {
                Supplier supplierExist = await _dbContext.Supliers.FindAsync(userInput.DefaultSuplierId);
                if (supplierExist == null) return NotFound($"The supplier with id = {userInput.DefaultSuplierId} was not found");
            }

            if (!userInput.MaterialName.IsNullOrEmpty())
            {
                Material materialNameExistName = await _dbContext.Materials.FirstOrDefaultAsync(m => m.MaterialName.ToUpper().Equals(userInput.MaterialName.Trim().ToUpper()));
                if (materialNameExistName != null && materialNameExistName.MaterialName.Equals(materialExist.MaterialName)) return BadRequest(new Response("Error", "The material already exists"));
            }
           
         
            materialExist.MaterialName = userInput.MaterialName.IsNullOrEmpty() ? materialExist.MaterialName : userInput.MaterialName;
            materialExist.MaterialPrice = userInput.MaterialPrice.HasValue ? userInput.MaterialPrice.Value : materialExist.MaterialPrice;
            materialExist.Description = userInput.Description.IsNullOrEmpty() ? materialExist.Description : userInput.Description;
            materialExist.DefaultSuplierId = userInput.DefaultSuplierId.HasValue ? userInput.DefaultSuplierId.Value :  materialExist.DefaultSuplierId;
            if (userInput.UploadImage != null)
            {
                var removeResult = _firebaseService.RemoveFile(materialExist.MaterialImage);
                if(!removeResult) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"An error occur when updating material with id = {materialExist.MaterialId}"));
                materialExist.MaterialImage = _firebaseService.UploadFile(userInput.UploadImage);
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
                    new Response("Error", $"An error occur when updating material with id = {materialExist.MaterialId}"));
            }
        }

        [HttpDelete("shop-data/materials/delete/{materialId}")]
        public async Task<IActionResult> RemoveMaterial([FromRoute] int materialId)
        {
            Material materialExist = await _dbContext.Materials.FindAsync(materialId);
            if (materialExist == null) return NotFound($"The material with id = {materialId} was not found");
            if (!materialExist.MaterialImage.IsNullOrEmpty())
            {
                var removeResult = _firebaseService.RemoveFile(materialExist.MaterialImage);

                if (!removeResult) return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", $"An error occur when removing material with id = {materialId}"));
            }
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
                    new Response("Error", $"An error occur when removing material with id = {materialId}"));
            }
          
        }

        // Import material from supplier 
        [HttpGet("warehouse/material/imports")]
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
                BillImage = _firebaseService.GetDownloadUrl(i.BillImage),
                CreationDate = i.CreationDate,
                DeliveryDate = i.DeliveryDate,
                Status = i.Status
            });
            return Ok(response);
        }

        [HttpPost("warehouse/material/imports/create")]
        public async Task<IActionResult> CreateImports([FromBody] ImportMaterialViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found ");
            var loggedInUser = await _dbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email.Equals(email));
            Repository respositoryExist = await _dbContext.Repositories.FindAsync(userInput.RepositoryId);
            if (respositoryExist == null) return NotFound($"The repository with id = {userInput.RepositoryId} was not found");
            if (respositoryExist.IsFull) return BadRequest(new Response("Error", "Cannot import material into full repository"));
            foreach (var item in userInput.Items)
            {
                Material materialExist = await _dbContext.Materials.FindAsync(item.MaterialId);
                if (materialExist == null) return NotFound($"Material with id = {item.MaterialId} was not found");
                if (materialExist.DefaultSuplier == null) return NotFound($"Default supplier of material with id = {item.MaterialId} was not found");               
            }

            Import newImport = new Import()
            {
                UserId = loggedInUser.Id,
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
                    new Response("Error", "An error occurs when adding new import"));
            }

            foreach (var item in userInput.Items)
            {              
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
                    new Response("Error", "An error occurs when adding new import item"));
                }
            }

            return Created("Create new import successfully", new
            {
                ImportId = newImport.ImportId,
                CrearetedBy = loggedInUser.ToString(),
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

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            Import importExist = await _dbContext.Imports.FindAsync(importId);
            if (importExist == null) return NotFound($"The import with id = {importId} was not found");
            if (userInput.DeliveryDate.Date < importExist.CreationDate.Date) return BadRequest(new Response("Error", "Delivery date must be greater than or equal creation date"));
            importExist.DeliveryDate = userInput.DeliveryDate;
            importExist.Status = "Delivered";         
            importExist.BillImage = _firebaseService.UploadFile(userInput.BillImage);
            List<ImportDetail> importDetails = importExist.ImportDetails.ToList();
            //add material to repository
            foreach(var import in importDetails)
            {
                MaterialRepository materialRepositoryExist = await _dbContext.MaterialRepositories.FirstOrDefaultAsync(m => m.RepositoryId == importExist.RepositoryId &&
                                                               m.MaterialId == import.MaterialId);
                
                if(materialRepositoryExist == null)
                {
                    MaterialRepository newMaterialRepository = new MaterialRepository()
                    {
                        RepositoryId = importExist.RepositoryId,
                        MaterialId = import.MaterialId,
                        Available = import.Quantity
                    };
                    await _dbContext.AddAsync(newMaterialRepository);
                   
                }
                else
                {
                    materialRepositoryExist.Available += import.Quantity;
                    _dbContext.Update(materialRepositoryExist); 
                }

                MaterialRepositoryHistory newHistory = new MaterialRepositoryHistory()
                {
                    RepositoryId = importExist.RepositoryId,
                    Type = "IMPORT",
                    AssistantId = loggedInUser.Id,
                    MaterialId = import.MaterialId,
                    Quantity = import.Quantity,
                    Description = $"Import from  \"{import.Material.DefaultSuplier.SupplierName}\" supplier",
                    CreationDate = DateTime.Now
                };
                await _dbContext.AddAsync(newHistory);

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
               new Response("Error", "An error occurs when confirm import"));
            }
            
        }


        [HttpDelete("warehouse/material/imports/remove/{importId}")] 
        public async Task<IActionResult> RemoveImports([FromRoute] int importId)
        {
            Import importExist = await _dbContext.Imports.FindAsync(importId);
            if (importExist == null) return NotFound($"The import with id = {importId} was not found");
            if (importExist.Status.Equals("Delivered")) return BadRequest("Cannot remove the confirmed import");                  
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
                    new Response("Error", "An error occurs when removing import"));
            }
           
        }

        //lich su nhap material tu supplier -> csv 
        [HttpGet("warehouse/material/import-history/to-csv")]
        public async Task<IActionResult> GetImportMaterialHistoryCSV()
        {
            var data = await _dbContext.Imports.ToListAsync();
            var csv = new StringBuilder();
            string heading = "ImportId,Import Creator,Creation Date,Delivery Date,Status,Material Name,Supplier Name,Quantity,Cost (Thousand VND),Note";
            csv.AppendLine(heading);
            foreach (var import in data)
            {
                foreach (var  row in import.ImportDetails)
                {
                    var newRow = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                             row.ImportId,
                             row.Import.User.ToString(),
                             row.Import.CreationDate.ToString(),
                             row.Import.DeliveryDate == null ? "(Empty)" : row.Import.DeliveryDate.ToString(),
                             row.Import.Status,
                             row.Material.MaterialName,
                             row.Material.DefaultSuplier.SupplierName,
                             row.Quantity,
                             row.Material.MaterialPrice * row.Quantity,
                             row.Note.IsNullOrEmpty() ? "(Empty)" : row.Note
                           );
                    csv.AppendLine(newRow);
                }
             
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "Import.csv");
        }

        //Repository 
        [HttpGet("warehouse/repositories/search")]
        public async Task<IActionResult> SearchRepositories(string searchString)
        {
            var repositories = await _dbContext.Repositories.Where(r => r.RepositoryName.Contains(searchString)).ToListAsync();
            if (repositories.IsNullOrEmpty()) return Ok(new List<Repository>());
            var response = repositories.Select(r => new
            {
                RepositoryId = r.RepositoryId,
                RepositoryName = r.RepositoryName,
                Address = r.Address.ToString(),
                Capacity = r.Capacity,
                IsFull = r.IsFull,
                CreationDate = r.CreationDate
            });
            return Ok(response);

        }
        [HttpGet("warehouse/repositories")]
        public async Task<IActionResult> GetRepositories()
        {
            var repo = await _dbContext.Repositories.ToListAsync();
            if (repo.Count == 0) return NotFound("There is no any repository");
            var response = repo.Select(r => new
            {
                RepositoryId = r.RepositoryId,
                RepositoryName = r.RepositoryName,
                AddressId = r.AddressId,
                Capacity = r.Capacity,
                IsFull = r.IsFull,
                CreationDate = r.CreationDate
            });
            return Ok(response);
        }

        [HttpGet("warehouse/repositories/{repoId}")]
        public async Task<IActionResult> GetRepositoryDetail([FromRoute] int repoId)
        {
            Repository repo = await _dbContext.Repositories.FindAsync(repoId);
            if (repo == null) return NotFound($"The repository with id = {repoId} was not found");
            return Ok(new
            {
                RepositoryId = repo.RepositoryId,
                RepositoryName = repo.RepositoryName,
                Address = new
                {
                    Street = repo.Address.Street,
                    Ward = repo.Address.Ward,
                    District = repo.Address.District,
                    Province = repo.Address.Provine
                },
                Capacity = repo.Capacity,
                IsFull = repo.IsFull,
                CreationDate = repo.CreationDate,
                FurnitureRepository = repo.FurnitureRepositories.Select(f => new
                {
                    FurnitureSpecificationId = f.FurnitureSpecificationId,
                    FurnitureSpecificationName = f.FurnitureSpecification.FurnitureSpecificationName,
                    Available = f.Available
                }),
                MaterialRepository = repo.MaterialRepositories.Select(m => new
                {
                    MaterialId = m.MaterialId,
                    MaterialName = m.Material.MaterialName,
                    Available = m.Available
                })
            });
        }

        [HttpPost("warehouse/repositories/add")]
        public async Task<IActionResult> AddRepository([FromForm] RepositoryViewModel userInput)
        {
            Models.Address newAddress = new Models.Address()
            {
                Street = userInput.Street,
                Ward = userInput.Ward,
                District = userInput.District,
                Provine = userInput.Province,
                AddressOwner = "Repository",
            };
            try
            {
                await _dbContext.AddAsync(newAddress);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occur when adding repository address"));
            }
            Repository newRepo = new Repository()
            {
                RepositoryName = userInput.RepositoryName,
                AddressId = newAddress.AddressId,
                Capacity = userInput.Capacity,
                IsFull = false,
                CreationDate = DateTime.Now
            };
            try
            {
                await _dbContext.AddAsync(newRepo);
                await _dbContext.SaveChangesAsync();
                return Created("Create new repository successfully", new
                {
                    RepositoryName = newRepo.RepositoryName,
                    AddressId = newRepo.AddressId,
                    Capacity = newRepo.Capacity,
                    IsFull = newRepo.IsFull,
                    CreationDate = newRepo.CreationDate
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response("Error", "An error occur when adding new repository"));
            }
        }

        [HttpPut("warehouse/repositories/{repoId}/edit")]
        public async Task<IActionResult> EditRepository([FromRoute] int repoId, [FromForm] EditRepositoryViewModel userInput)
        {
            Repository repoExist = await _dbContext.Repositories.FindAsync(repoId);
            if (repoExist == null) return NotFound($"The repository with id = {repoId} was not found");
            Models.Address repoAddress = repoExist.Address;
            repoExist.RepositoryName = userInput.RepositoryName.IsNullOrEmpty() ? repoExist.RepositoryName : userInput.RepositoryName;
            repoExist.Capacity = userInput.Capacity.HasValue ? userInput.Capacity.Value : repoExist.Capacity;
            repoExist.IsFull = userInput.IsFull == null ? repoExist.IsFull : userInput.IsFull.Value; 
            repoAddress.Street = userInput.Street.IsNullOrEmpty() ? repoAddress.Street : userInput.Street;
            repoAddress.Ward = userInput.Ward.IsNullOrEmpty() ? repoAddress.Ward : userInput.Ward;
            repoAddress.District = userInput.District.IsNullOrEmpty() ? repoAddress.District : userInput.District;
            repoAddress.Provine = userInput.Province.IsNullOrEmpty() ? repoAddress.Provine : userInput.Province;
            try
            {
                _dbContext.UpdateRange(repoExist, repoAddress);
                _dbContext.SaveChangesAsync();
                return Ok($"Update repository with id = {repoId} successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response("Error", $"An error occurs when import furnitures into repository with id = {repoId}"));
            }
        }

        [HttpDelete("warehouse/repositories/{repoId}/remove")]
        public async Task<IActionResult> RemoveRepository([FromRoute] int repoId, [Required] string password) // yeu cau nhap password de xoa
        {
            if (repoId == 1) return BadRequest(new Response("Error", "Not allow to remove main repository with id = 1"));
            Repository repoExist = await _dbContext.Repositories.FindAsync(repoId);
            if (repoExist != null) return NotFound($"The repository with id = {repoId} was not found");
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            bool passwordChecker = await _userManager.CheckPasswordAsync(loggedInUser, password);
            if (!passwordChecker) return Unauthorized("Incorrect password");
            try
            {
                repoExist.FurnitureRepositories.Clear();
                repoExist.MaterialRepositories.Clear();
                await _dbContext.SaveChangesAsync();
                _dbContext.Remove(repoExist);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response("Error", $"An error occurs when remove repository with id = {repoId}"));
            }
        }

        //Import items into Repository 
        [HttpPost("warehouse/repositories/{repoId}/import-material")]
        public async Task<IActionResult> ImportMaterialIntoRepository([FromRoute] int repoId, [FromBody]  ImportViewModel<int> userInput)
        {

            Repository repoExist = await _dbContext.Repositories.FindAsync(repoId);
            if (repoExist == null) return NotFound($"The repository with id = {repoId} was not found");
            if (repoExist.IsFull) return BadRequest(new Response("Error", "Cannot import materials into the repository in full status"));
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            var addedItemList = new List<Material>();
            foreach (var item in userInput.Items)
            {
                Material materialExist = await _dbContext.Materials.FindAsync(item.Id);
                if (materialExist == null) return NotFound($"The material with id = {item.Id} was not found");
                addedItemList.Add(materialExist);
            }

            for (int i = 0; i < addedItemList.Count; i++)
            {

                MaterialRepository materialRepositoryExist = addedItemList[i].MaterialRepositories.FirstOrDefault(f => f.RepositoryId == repoExist.RepositoryId);
                if (materialRepositoryExist == null)
                {
                    MaterialRepository newMaterialRepository = new MaterialRepository()
                    {
                        RepositoryId = repoExist.RepositoryId,
                        MaterialId = addedItemList[i].MaterialId,
                        Available = userInput.Items[i].Quantity
                    };

                    await _dbContext.AddAsync(newMaterialRepository);
                }
                else
                {
                    materialRepositoryExist.Available += userInput.Items[i].Quantity;
                    _dbContext.Update(materialRepositoryExist);
                }
                MaterialRepositoryHistory newHistory = new MaterialRepositoryHistory()
                {
                    RepositoryId = repoId,
                    Type = "IMPORT",
                    AssistantId = loggedInUser.Id,
                    MaterialId = addedItemList[i].MaterialId,
                    Quantity = userInput.Items[i].Quantity,
                    Description = userInput.ImportReason,
                    CreationDate = userInput.ImportDate
                };
                await _dbContext.AddAsync(newHistory);
            }

            try
            {

                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", $"Import materials to repository with id = {repoId} successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response("Error", $"An error occurs when import furnitures to repository with id = {repoId}"));
            }
        }

        [HttpPost("warehouse/repositories/{repoId}/import-furniture")]
        public async Task<IActionResult> ImportFurnitureIntoRepository([FromRoute] int repoId, [FromBody] ImportViewModel<string> userInput)
        {
            Repository repoExist = await _dbContext.Repositories.FindAsync(repoId);
            if (repoExist == null) return NotFound($"The repository with id = {repoId} was not found");
            if (repoExist.IsFull) return BadRequest(new Response("Error", "Cannot import furniture into the repository in full status"));
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            var addedItemList = new List<FurnitureSpecification>();
            foreach(var item in userInput.Items)
            {
                FurnitureSpecification furnitureSpecificationExist = await _dbContext.FurnitureSpecifications.FindAsync(item.Id);
                if (furnitureSpecificationExist == null) return NotFound($"The specification with id = {item.Id} was not found");
                addedItemList.Add(furnitureSpecificationExist);
            }

            for (int i = 0; i < addedItemList.Count; i++ )
            {
                
                FurnitureRepository furnitureRepositoryExist = addedItemList[i].FurnitureRepositories.FirstOrDefault(f => f.RepositoryId == repoExist.RepositoryId);
                if (furnitureRepositoryExist == null)
                {
                    FurnitureRepository newFurnitureRepository = new FurnitureRepository()
                    {
                        RepositoryId = repoExist.RepositoryId,
                        FurnitureSpecificationId = addedItemList[i].FurnitureSpecificationId,
                        Available = userInput.Items[i].Quantity
                    };
                  
                    await _dbContext.AddAsync(newFurnitureRepository);                   
                } else
                {
                    furnitureRepositoryExist.Available += userInput.Items[i].Quantity;
                    _dbContext.Update(furnitureRepositoryExist);
                }

                FurnitureRepositoryHistory newHistory = new FurnitureRepositoryHistory()
                {
                    RepositoryId = repoId,
                    Type = "IMPORT",
                    AssistantId = loggedInUser.Id,
                    FurnitureSpecificationId = addedItemList[i].FurnitureSpecificationId,
                    Quantity = userInput.Items[i].Quantity,
                    Description = userInput.ImportReason,
                    CreationDate = userInput.ImportDate
                };
                await _dbContext.AddAsync(newHistory);


            }
            try
            {
              
                await _dbContext.SaveChangesAsync();
                return Ok(new Response("Success", $"Import furnitures to repository with id = {repoId} successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response("Error", $"An error occurs when import furnitures to repository with id = {repoId}"));
            }
          
        }

        //Export item from Repository 
        [HttpPost("warehouse/repositories/{fromRepoId}/export-material")]
        public async Task<IActionResult> ExportMaterialFromRepository([FromRoute] int fromRepoId, [FromBody] ExportViewModel<int> userInput)
        {
            Repository repoExist = await _dbContext.Repositories.FindAsync(fromRepoId);
            if (repoExist == null) return NotFound($"The repository with id = {fromRepoId} was not found");
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (userInput.Items.Distinct().Count() != userInput.Items.Count()) return BadRequest("Contain duplicate item in the list of export items");
            foreach (var item in userInput.Items)
            {
                MaterialRepository MaterialRepositoryExist = repoExist.MaterialRepositories.FirstOrDefault(mr => mr.MaterialId == item.Id);
                if (MaterialRepositoryExist == null) return NotFound(new Response("Error", $"Material with id  = {item.Id} was not found"));
                if (MaterialRepositoryExist.Available < item.Quantity) return BadRequest(new Response("Error", $"The quantity of material with id = {MaterialRepositoryExist.MaterialId} to be exported is greater than the quantity available"));
            }

            try
            {
                foreach (var item in userInput.Items) 
                {
                    MaterialRepositoryHistory newHistory = new MaterialRepositoryHistory()
                    {
                        RepositoryId = fromRepoId,
                        Type = "EXPORT",
                        AssistantId = loggedInUser.Id,
                        MaterialId = item.Id,
                        Quantity = item.Quantity,
                        Description = userInput.ExportReason,
                        CreationDate = userInput.ExportDate
                    };
               
                    var materialRepositoryExist = repoExist.MaterialRepositories.FirstOrDefault(r => r.MaterialId == item.Id);
                    materialRepositoryExist.Available -= item.Quantity;
                    if (materialRepositoryExist.Available == 0) await _projectHelper.CreateAnnouncementAsync(loggedInUser,
                                                                        $"Warning: {materialRepositoryExist.Material.MaterialName} is out of stock",
                                                                        $"{materialRepositoryExist.Material.MaterialName} is out of stock. Please import more");
                    else if (materialRepositoryExist.Available < 4) await _projectHelper.CreateAnnouncementAsync(loggedInUser,
                                                                        $"Warning: {materialRepositoryExist.Material.MaterialName} is running out",
                                                                        $"The number of furniture left in repository is only {materialRepositoryExist.Available}.Please import more to avoid running out of stock");
                    _dbContext.Update(materialRepositoryExist);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.AddAsync(newHistory);
                }               
                await _dbContext.SaveChangesAsync();
                _projectHelper.CreateLogAsync(loggedInUser.Id, "You has exported materials successfully");
                return Created("Create new export successfully",new Response("Success", "Export materials successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  new Response("Error", "An error occurs when exporting materials"));
            }
        }

        [HttpPost("warehouse/repositories/{fromRepoId}/export-furniture")]
        public async Task<IActionResult> ExportFurnitureFromRepository([FromRoute] int fromRepoId, [FromForm] ExportViewModel<string> userInput)
        {
            Repository repoExist = await _dbContext.Repositories.FindAsync(fromRepoId);
            if (repoExist == null) return NotFound($"The repository with id = {fromRepoId} was not found");
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (userInput.Items.Distinct().Count() != userInput.Items.Count()) return BadRequest("Contain duplicate item in the list of export items");
            foreach (var item in userInput.Items)
            {
                FurnitureRepository FurnitureRepositoryExist = repoExist.FurnitureRepositories.FirstOrDefault(fr => fr.FurnitureSpecificationId.Equals(item.Id));
                if (FurnitureRepositoryExist == null) return NotFound(new Response("Error", $"Material with id  = {item.Id} was not found"));
                if (FurnitureRepositoryExist.Available < item.Quantity) return BadRequest(new Response("Error", $"The quantity of furniture specification with id = {FurnitureRepositoryExist.FurnitureSpecificationId} to be exported is greater than the quantity available"));
            }

            try
            {
                foreach (var item in userInput.Items)
                {
                    FurnitureRepositoryHistory newHistory = new FurnitureRepositoryHistory()
                    {
                        RepositoryId = fromRepoId,
                        Type = "EXPORT",
                        AssistantId = loggedInUser.Id,
                        FurnitureSpecificationId = item.Id,
                        Quantity = item.Quantity,
                        Description = userInput.ExportReason,
                        CreationDate = userInput.ExportDate

                    };

                    var furnitureRepositoryExist = repoExist.FurnitureRepositories.FirstOrDefault(fr => fr.FurnitureSpecificationId.Equals(item.Id));
                    furnitureRepositoryExist.Available -= item.Quantity;
                    if (furnitureRepositoryExist.Available == 0) await _projectHelper.CreateAnnouncementAsync(loggedInUser,
                                                                        $"Warning: {furnitureRepositoryExist.FurnitureSpecification.FurnitureSpecificationName} is out of stock",
                                                                        $"{furnitureRepositoryExist.FurnitureSpecification.FurnitureSpecificationName} is out of stock. Please import more");
                    else if (furnitureRepositoryExist.Available < 4) await _projectHelper.CreateAnnouncementAsync(loggedInUser,
                                                                        $"Warning: {furnitureRepositoryExist.FurnitureSpecification.FurnitureSpecificationName} is running out",
                                                                        $"The number of furniture left in repository is only {furnitureRepositoryExist.Available}.Please import more to avoid running out of stock");
                    _dbContext.Update(furnitureRepositoryExist);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.AddAsync(newHistory);
                }
                await _dbContext.SaveChangesAsync();
                _projectHelper.CreateLogAsync(loggedInUser.Id, "You has exported materials successfully");
                return Created("Create new export successfully", new Response("Success", "Export materials successfully"));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  new Response("Error", "An error occurs when exporting materials"));
            }
        }
  

        //Transfer item from a repository to other 
        [HttpPut("warehouse/repositories/{fromRepoId}/material/transfer/{toRepoId}")]
        public async Task<IActionResult> TransferMaterial([FromRoute] int fromRepoId, [FromRoute] int toRepoId, List<ItemViewModel<int>> tranferItemList)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (fromRepoId == toRepoId) return BadRequest(new Response("Error",$"\"From Repository Id = {fromRepoId}\" is not allowed to be the same as \"To Repository Id = {toRepoId}\""));
            Repository fromRepoExist = await _dbContext.Repositories.FindAsync(fromRepoId);
            if (fromRepoExist == null) return NotFound(new Response("Error", $"The repository with id = {fromRepoId} was not found"));
            Repository toRepoExist = await _dbContext.Repositories.FindAsync(toRepoId);
            if (toRepoExist == null) return NotFound(new Response("Error", $"The repository with id = {toRepoId} was not found"));
            if (toRepoExist.IsFull) return BadRequest(new Response("Error", $"The repository with id = {toRepoId} cannot receive materials which transferred from the repository with id = {fromRepoId} when it is in full status"));
            
            if (tranferItemList.Distinct().Count() != tranferItemList.Count()) return BadRequest(new Response("Error", "Contain duplicate item in the list of transfer items"));
            List<MaterialRepository> fromMaterialRepositories = new List<MaterialRepository>();
            foreach (var item in tranferItemList)
            {
                MaterialRepository materialRepositoryExist = fromRepoExist.MaterialRepositories.FirstOrDefault(mr => mr.MaterialId == item.Id);
                if (materialRepositoryExist == null) return NotFound(new Response("Error", $"Material with id  = {item.Id} was not found in the repository"));
                if (materialRepositoryExist.Available < item.Quantity) return BadRequest(new Response("Error", $"The quantity of material with id = {item.Id} to be transferred is greater than the quantity available"));
                fromMaterialRepositories.Add(materialRepositoryExist);
            }

            for (int i = 0; i < tranferItemList.Count(); i++) 
            {
                var toMaterialRepository = toRepoExist.MaterialRepositories.FirstOrDefault(r => r.MaterialId == fromMaterialRepositories[i].MaterialId);
                if (toMaterialRepository == null)
                {
                    MaterialRepository newMaterialRepository = new MaterialRepository()
                    {
                        RepositoryId = toRepoExist.RepositoryId,
                        MaterialId = fromMaterialRepositories[i].MaterialId,
                        Available = tranferItemList[i].Quantity
                    };
                    try
                    {
                        await _dbContext.AddAsync(newMaterialRepository);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response("Error", $"An error occurs when transfer material from the repository with id {fromRepoId} to the repository with id {toRepoId}"));
                    }

                }
                else
                {
                    toMaterialRepository.Available += tranferItemList[i].Quantity;
                }
                _dbContext.Update(toMaterialRepository);
                MaterialRepositoryHistory newHistory1 = new MaterialRepositoryHistory()
                {
                    RepositoryId = toRepoId,
                    Type = "TRANFER",
                    AssistantId = loggedInUser.Id,
                    MaterialId = tranferItemList[i].Id,
                    Quantity = tranferItemList[i].Quantity,
                    Description = $"Receive material which transferred from repository with id = {fromRepoId}",
                    CreationDate = DateTime.Now

                };
                await _dbContext.AddAsync(newHistory1);
                await _dbContext.SaveChangesAsync();

                MaterialRepositoryHistory newHistory2 = new MaterialRepositoryHistory()
                {
                    RepositoryId = fromRepoId,
                    Type = "TRANFER",
                    AssistantId = loggedInUser.Id,
                    MaterialId = tranferItemList[i].Id,
                    Quantity = tranferItemList[i].Quantity,
                    Description = $"Transfer material to repository with id = {toRepoId}",
                    CreationDate = DateTime.Now

                };

                fromMaterialRepositories[i].Available -= tranferItemList[i].Quantity;             
                _dbContext.Update(fromMaterialRepositories[i]);
                
                await _dbContext.AddAsync(newHistory2);
                await _dbContext.SaveChangesAsync();
            }
            try
            {
                await _dbContext.SaveChangesAsync();
                _projectHelper.CreateLogAsync(loggedInUser.Id, $"You has tranfferd materials from the repository with id {fromRepoId} to the repository with id {toRepoId} successfully");
                return Ok(new Response("Success", $"Transfer materials from the repository with id {fromRepoId} to the repository with id {toRepoId} successfully"));

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response("Error", $"An error occurs when transfer material from the repository with id {fromRepoId} to the repository with id {toRepoId}"));
            }

        }

        [HttpPut("warehouse/repositories/{fromRepoId}/furniture/transfer/{toRepoId}")]
        public async Task<IActionResult> TransferFurniture([FromRoute] int fromRepoId, [FromRoute] int toRepoId, List<ItemViewModel<string>> tranferItemList)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound("Logged in user not found");
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (fromRepoId == toRepoId) return BadRequest("\"From Repository Id\" is not allowed to be the same as \"To Repository Id\"");
            Repository fromRepoExist = await _dbContext.Repositories.FindAsync(fromRepoId);
            if (fromRepoExist == null) return NotFound($"The repository with id = {fromRepoId} was not found");
            Repository toRepoExist = await _dbContext.Repositories.FindAsync(toRepoId);
            if (toRepoExist == null) return NotFound($"The repository with id = {toRepoId} was not found");
            if (toRepoExist.IsFull) return BadRequest(new Response("Error", $"The repository with id = {toRepoId} cannot receive furnitures which transferred from the repository with id = {fromRepoId} when it is in full status"));
            if (tranferItemList.Distinct().Count() != tranferItemList.Count()) return BadRequest("Contain duplicate item in the list of transfer items");
            List<FurnitureRepository> fromFurnitureRepositories = new List<FurnitureRepository>();
            foreach (var item in tranferItemList)
            {
                FurnitureRepository furnitureRepositoryExist = fromRepoExist.FurnitureRepositories.FirstOrDefault(fr => fr.FurnitureSpecificationId.Equals(item.Id));
                if (furnitureRepositoryExist == null) return NotFound($"Furniture specification with id  = {item.Id} was not found in the repository");
                if (furnitureRepositoryExist.Available < item.Quantity) return BadRequest($"The quantity of furniture specification with id = {item.Id} to be transferred is greater than the quantity available");
                fromFurnitureRepositories.Add(furnitureRepositoryExist);
            }

            for (int i = 0; i < tranferItemList.Count(); i++)
            {
                var toFurnitureRepository = toRepoExist.FurnitureRepositories.FirstOrDefault(r => r.FurnitureSpecificationId.Equals(fromFurnitureRepositories[i].FurnitureSpecificationId));
                if (toFurnitureRepository == null)
                {
                    FurnitureRepository newFurnitureRepository = new FurnitureRepository()
                    {
                        RepositoryId = toRepoExist.RepositoryId,
                        FurnitureSpecificationId = fromFurnitureRepositories[i].FurnitureSpecificationId,
                        Available = tranferItemList[i].Quantity
                    };
                    try
                    {
                        await _dbContext.AddAsync(newFurnitureRepository);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response("Error", $"An error occurs when transfer furnitures from the repository with id {fromRepoExist.RepositoryId} to the repository with id {toRepoExist.RepositoryId}"));
                    }

                }
                else
                {
                    toFurnitureRepository.Available += tranferItemList[i].Quantity;
                    _dbContext.Update(toFurnitureRepository);

                }

                FurnitureRepositoryHistory newHistory1 = new FurnitureRepositoryHistory()
                {
                    RepositoryId = toRepoId,
                    Type = "TRANSFER",
                    AssistantId = loggedInUser.Id,
                    FurnitureSpecificationId = tranferItemList[i].Id,
                    Quantity = tranferItemList[i].Quantity,
                    Description = $"Receive material which transferred from repository with id = {fromRepoId}",
                    CreationDate = DateTime.Now

                };

                FurnitureRepositoryHistory newHistory2 = new FurnitureRepositoryHistory()
                {
                    RepositoryId = fromRepoId,
                    Type = "TRANSFER",
                    AssistantId = loggedInUser.Id,
                    FurnitureSpecificationId = tranferItemList[i].Id,
                    Quantity = tranferItemList[i].Quantity,
                    Description = $"Transfer material to repository with id = {toRepoId}",
                    CreationDate = DateTime.Now

                };

                fromFurnitureRepositories[i].Available -= tranferItemList[i].Quantity;
                _dbContext.Update(fromFurnitureRepositories[i]);
                await _dbContext.AddRangeAsync(newHistory1, newHistory2);


            }
            try
            {
                await _dbContext.SaveChangesAsync();
                _projectHelper.CreateLogAsync(loggedInUser.Id, $"You has tranfferd furnitures from the repository with id {fromRepoId} to the repository with id {toRepoId} successfully");
                return Ok($"Transfer materials from the repository with id {fromRepoId} to the repository with id {toRepoId} successfully");

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response("Error", $"An error occurs when transfer furnitures from the repository with id {fromRepoId} to the repository with id {toRepoId}"));
            }

        }

        //View repo history
        [HttpGet("warehouse/repositories/{repoId}/material-repository-history")]
        public async Task<IActionResult> MaterialRepositoryHistory([FromRoute] int repoId, string? type)
        {
            string historyType = type.IsNullOrEmpty() ? "ALL" : type;
            if (!historyType.Equals("IMPORT") && !historyType.Equals("EXPORT") && !historyType.Equals("TRANSFER") && !historyType.Equals("ALL")) 
                return BadRequest(new Response("Error", "Type of repository history must be \"IMPORT\",  \"EXPORT\",  \"TRANSFER\" or \"ALL\""));
            Repository historyExist = await _dbContext.Repositories.FindAsync(repoId);
            if (historyExist == null) return NotFound(new Response("Error", $"The repository with id = {repoId} was not found"));
            var data = historyType.Equals("ALL") ? historyExist.MaterialRepositoryHistories.ToList() : historyExist.MaterialRepositoryHistories.Where(h => h.Type.Equals(historyType)).ToList();
            var response = data.Select(d => new
            {
                MaterialRepositoryHistoryId = d.MaterialRepositoryHistoryId,
                RepositoryId = d.RepositoryId,
                Type = d.Type,
                AssistantId = d.AssistantId,
                MaterialName = d.Material.MaterialName,
                Quantity = d.Quantity,
                Descrition = d.Description,
                CreationDate = d.CreationDate
            });
            return Ok(response);
        }

        [HttpGet("warehouse/repositories/{repoId}/furniture-repository-history")]
        public async Task<IActionResult> FuritureRepositoryHistory([FromRoute] int repoId, string? type)
        {
            string historyType = type.IsNullOrEmpty() ? "ALL" : type;
            if (!historyType.Equals("IMPORT") && !historyType.Equals("EXPORT") && !historyType.Equals("TRANSFER") && !historyType.Equals("ALL"))
                return BadRequest(new Response("Error", "Type of repository history must be \"IMPORT\",  \"EXPORT\",  \"TRANSFER\" or \"ALL\""));
            Repository historyExist = await _dbContext.Repositories.FindAsync(repoId);
            if (historyExist == null) return NotFound(new Response("Error", $"The repository with id = {repoId} was not found"));
            var data = historyType.Equals("ALL") ? historyExist.FurnitureRepositoryHistories.ToList() : historyExist.FurnitureRepositoryHistories.Where(h => h.Type.Equals(historyType)).ToList();
            var response = data.Select(d => new
            {
                FuritureRepositoryHistoryId = d.FurnitureRepositoryHistoryId,
                RepositoryId = d.RepositoryId,
                Type = d.Type,
                AssistantId = d.AssistantId,
                FurnitureSpecificationName = d.FurnitureSpecification.FurnitureSpecificationName,
                Quantity = d.Quantity,
                Descrition = d.Description,
                CreationDate = d.CreationDate
            });
            return Ok(response);
        }


        //xuat lich su xuat/nhap kho -> csv
        [HttpGet("warehouse/repositories/furniture-repository-history/to-csv")]
        public async Task<IActionResult> GetFurnitureRepositoryHistoryCSV()
        {
            var data = await _dbContext.FurnitureRepositoryHistories.ToListAsync();
            var csv = new StringBuilder();
            string heading = "RepositoryId,Repository Name,Type,Creator,Furniture Specification Name,Quantity,Description,CreationDate";
            csv.AppendLine(heading);
            foreach (var row in data)
            {              
                    var newRow = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                             row.RepositoryId,
                             row.Repository.RepositoryName,
                             row.Type,
                             row.Assistant.ToString(),
                             row.FurnitureSpecification.FurnitureSpecificationName,
                             row.Quantity,
                             row.Description,                          
                             row.CreationDate.ToString()
                           );
                    csv.AppendLine(newRow);               
           }
            byte[] bytes = Encoding.ASCII.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "Furniture_repository_history.csv");
        }

        [HttpGet("warehouse/repositories/material-repository-history/to-csv" )]
        public async Task<IActionResult> GetMaterialRepositoryHistoryCSV()
        {
            var data = await _dbContext.MaterialRepositoryHistories.ToListAsync();
            var csv = new StringBuilder();
            string heading = "RepositoryId,Repository Name,Type,Creator,Material Name,Quantity,Description,CreationDate";
            csv.AppendLine(heading);
            foreach (var row in data)
            {
                var newRow = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                         row.RepositoryId,
                         row.Repository.RepositoryName,
                         row.Type,
                         row.Assistant.ToString(),
                         row.Material.MaterialName,
                         row.Quantity,
                         row.Description,
                         row.CreationDate.ToString()
                       );
                csv.AppendLine(newRow);
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "Material_repository_history.csv");
        }




        //view feedback 
        [HttpGet("feedbacks")]
        public async Task<IActionResult> GetAllFeedback()
        {
            var feedbacks = await _dbContext.Feedbacks.ToListAsync();
            if (feedbacks.IsNullOrEmpty()) return Ok(new List<Feedback>());
            var response = feedbacks.Select(fb => new
            {
                FeedbackId = fb.FeedbackId,
                CustomerId = fb.CustomerId,
                CustomerName = fb.Anonymous ? "Anonymous" : fb.Customer.ToString(),
                OrderId = fb.OrderId,
                FurnitureSpecificationId = fb.FurnitureSpecificationId,
                FurnitureSpecificationName = fb.FurnitureSpecification.FurnitureSpecificationName,
                Content = fb.Content,
                VoteStar = fb.VoteStar,
                CreationDate = fb.CreationDate
            });
            return Ok(response);
        }

        [HttpDelete("feedbacks/remove/{feedbackId}")]
        public async Task<IActionResult> RemoveFeedback([FromRoute] int feedbackId)
        {
            var feedbackExist = await _dbContext.Feedbacks.FindAsync(feedbackId);
            if (feedbackExist == null) return NotFound(new Response("Error",$"The feedback with id = {feedbackId} was not found"));
            try
            {
                if (!feedbackExist.Attachements.IsNullOrEmpty())
                {                
                    foreach (var attachment in feedbackExist.Attachements)
                    {
                        _firebaseService.RemoveFile(attachment.Path);
                    }
                    feedbackExist.Attachements.Clear();
                }

                _dbContext.Remove(feedbackExist);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            } catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when removing the feedback"));
            }
            
        }

        //view furniture order
        [HttpGet("customer-requests/orders")]
        public async Task<IActionResult> GetAllOrder()
        {
            var furnitureOrder = _dbContext.Orders;
            if (furnitureOrder.Count() == 0) return NotFound("There is no any order");
          
            var response = furnitureOrder.Select(o => new
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.ToString(),
                PaymentMethod = o.Payment.PaymentMethod,
                DeliveryAddress = o.DeliveryAddress,
                UserPoint = o.UsedPoint,
                Note = o.Note,
                Status = o.Status,
                IsPaid = o.IsPaid,
                OrderDate = o.OrderDate,
                TotalCost = o.TotalCost,
                FurnitureOrderItems = o.FurnitureOrderDetails.Select(od => new {
                    FurnitureSpecificationId = od.FurnitureSpecificationId,
                    Quantity = od.Quantity,
                    Cost = od.Cost
                
                }),
                CustomizeFurnitureOrderItems = o.CustomizeFurnitureOrderDetails.Select(od => new
                {
                    CustomizeFunitureId = od.CustomizeFunitureId,

                })
            });

            return Ok(response);
        }

        //update order status
        [HttpPut("customer-requests/orders/{orderId}/change-status")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, string status)
        {
            Order orderExist = await _dbContext.Orders.FindAsync(orderId);
            if (orderExist == null) return NotFound($"The order with id = {orderId} was not found");
            List<string> statusList = new List<string>() { "Pending", "Canceled", "Preparing", "Delivering", "Delivered" };
            if (!statusList.Contains(status)) return BadRequest($"Status must be {statusList[0]}, {statusList[1]}, {statusList[2]}, {statusList[3]} or {statusList[4]}");
            orderExist.Status = status;
            orderExist.IsPaid = true;
            _projectHelper.CreateAnnouncementAsync(orderExist.Customer, "Order status has been changed", $"Your order is in {status} status");
            try
            {
               if (status.Equals("Delivered"))
                {
                    var result = await _projectHelper.UpdateCustomerSpentAsync(orderExist);
                    //update sold for furniture
                    if (!orderExist.FurnitureOrderDetails.IsNullOrEmpty())
                    {
                        foreach (var item in orderExist.FurnitureOrderDetails)
                        {
                            var furniture = item.FurnitureSpecification.Furniture;
                            furniture.Sold += item.Quantity;
                            _dbContext.Update(furniture);
                        }
                    }
                    if (!result) throw new Exception(); 
                }
                _dbContext.Update(orderExist);
                await _dbContext.SaveChangesAsync();
                return Ok($"Update the order with id = {orderId} successfully");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                             new Response("Error", $"An error occurs when updating the status of order with id = {orderId}"));
            }       
        }

        //CRUD post
        [HttpGet("shop-data/posts")]
        public async Task<IActionResult> GetPost([FromQuery]string? type)
        {
            if (type.IsNullOrEmpty()) type = "ALL";
            else if (!type.Equals("TIP") || !type.Equals("NEW")) return BadRequest(new Response("Error", "Invalid post type. it must be \"TIP\" or \"NEW\""));
            var posts =  type.Equals("ALL")? await _dbContext.Posts.ToListAsync() : await _dbContext.Posts.Where(p => p.Type.Equals(type)).ToListAsync();
            if (posts.IsNullOrEmpty()) return Ok(new List<Post>());
            var response = posts.Select(p => new
            {
               PostId = p.PostId,
               Author = p.Author.ToString(),
               PostTitle = p.Title,
               PosContent = p.Content,
               PostImage = _firebaseService.GetDownloadUrl(p.Image),
               CreationDate = p.CreationDate,
               LatestUpdate = p.LatestUpdate == null ? p.CreationDate : p.LatestUpdate
            });
            return Ok(response);
        }

        [HttpPost("shop-data/posts/add")]
        public async Task<IActionResult> AddPost([FromForm] PostViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return Unauthorized(new Response("Error", "Logged in user not found "));
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (!userInput.Type.Equals("TIP") && !userInput.Type.Equals("NEW")) return BadRequest(new Response("Error", "Invalid post type. It must be \"TIP\" or \"NEW\""));

            Post newPost = new Post()
            {
                AuthorId = loggedInUser.Id,
                Title = userInput.Title,
                Content = userInput.Title,
                Type = userInput.Type,
                Image = _firebaseService.UploadFile(userInput.Image),
                CreationDate = DateTime.Now,
                LatestUpdate = DateTime.Now
            };
            try
            {
                await _dbContext.AddAsync(newPost);
                await _dbContext.SaveChangesAsync();
                var response = new
                {
                    PostId = newPost.PostId,
                    Author = newPost.Author.ToString(),
                    Title = newPost.Title,
                    Content = newPost.Title,
                    Type = newPost.Type,
                    Image = _firebaseService.GetDownloadUrl(newPost.Image),
                    CreationDate = newPost.CreationDate,
                    LatestUpdate = newPost.CreationDate
                };
                return Created($"Create new post with type = {userInput.Type} successfully", response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "An error occurs when adding new post"));
            }
        }

        [HttpPut("shop-data/posts/{postId}/edit")]
        public async Task<IActionResult> EditPost([FromRoute] int postId, [FromForm] PostViewModel userInput)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return Unauthorized(new Response("Error", "Logged in user not found "));
            var loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            Post postExist = await _dbContext.Posts.FindAsync(postId);
            if (postExist == null) return NotFound(new Response("Error", $"The post with id = {postId} was not found"));
            if (!userInput.Type.Equals("TIP") || !userInput.Type.Equals("NEW")) return BadRequest(new Response("Error", "Invalid post type. It must be \"TIP\" or \"NEW\""));
            
            postExist.Title = userInput.Title;
            postExist.Content = userInput.Title;
            postExist.Type = userInput.Type;
            postExist.Image = _firebaseService.UploadFile(userInput.Image);
            postExist.LatestUpdate = DateTime.Now;

            try
            {
                _dbContext.Update(postExist);
                await _dbContext.SaveChangesAsync();
                var response = new
                {
                    PostId = postExist.PostId,
                    Author = postExist.Author.ToString(),
                    Title = postExist.Title,
                    Content = postExist.Title,
                    Type = postExist.Type,
                    Image = _firebaseService.GetDownloadUrl(postExist.Image),
                    CreationDate = postExist.CreationDate,
                    LatestUpdate = postExist.LatestUpdate
                };
                return Ok(response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", $"An error occurs when upload the post with id = {postId}"));
            }
        }

        [HttpDelete("shop-data/posts/{postId}/remove")]
        public async Task<IActionResult> RemovePost([FromRoute] int postId)
        {
            Post postExist = await _dbContext.Posts.FindAsync(postId);
            if (postExist == null) return NotFound(new Response("Error", $"The post with id = {postId} was not found"));
            try
            {
                _dbContext.Remove(postExist);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", $"An error occurs when removing the post with id = {postId}"));
            }
        }





        //get log
        [HttpGet("logs")]
        public async Task<IActionResult> GetLog()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return NotFound(new Response("Error","Logged in user not found"));
            User loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            
            var logs = loggedInUser.Logs;
            if (logs.IsNullOrEmpty()) return Ok(new List<Log>());
            var response = logs.Select(l => new
            {
                Activity = l.Activity,
                Date = l.Date
            });
            return Ok(response);
        }

        //get announcement

        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncement()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null) return Unauthorized(new Response("Error", "Logged in user not found"));
            User loggedInUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            var announcements = loggedInUser.Announcements;
            if (announcements.IsNullOrEmpty()) return Ok(new List<Announcement>());
            var response = announcements.Select(a => new
            {
                Title = a.Title,
                Content = a.Title,
                CreationDate = a.CreationDate
            });
            return Ok(response);
        }



    }
}
