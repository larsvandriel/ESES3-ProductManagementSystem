using LoggingService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ProductManagementSystem.API.Filters;
using ProductManagementSystem.Contracts;
using ProductManagementSystem.Entities.Extensions;
using ProductManagementSystem.Entities.Models;
using ProductManagementSystem.Entities.Parameters;
using ProductManagementSystem.Entities.ShapedEntities;

namespace ProductManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public CategoryController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetCategories([FromQuery] CategoryParameters categoryParameters)
        {
            try
            {
                var categories = _repository.Category.GetAllCategories(categoryParameters);

                var metadata = new
                {
                    categories.TotalCount,
                    categories.PageSize,
                    categories.CurrentPage,
                    categories.TotalPages,
                    categories.HasNext,
                    categories.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {categories.Count} categories from database.");

                var shapedCategories = categories.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedCategories);
                }

                for (var index = 0; index < categories.Count; index++)
                {
                    var categoryLinks = CreateLinksForCategory(((NormalShapedEntity)categories[index]).Id, categoryParameters.Fields);
                    shapedCategories[index].Add("Links", categoryLinks);
                }

                var categoriesWrapper = new LinkCollectionWrapper<Entity>(shapedCategories);

                return Ok(CreateLinksForCategories(categoriesWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "CategoryById")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetCategoryById(Guid id, [FromQuery] string fields)
        {
            try
            {
                var category = (NormalShapedEntity)_repository.Category.GetCategoryById(id, fields);

                if (category.Id == Guid.Empty)
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped category with id: {id}");
                    return Ok(category.Entity);
                }

                category.Entity.Add("Links", CreateLinksForCategory(category.Id, fields));

                return Ok(category.Entity);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wring inside GetCategoryById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            try
            {
                if (category.IsObjectNull())
                {
                    _logger.LogError("Category object sent from client is null.");
                    return BadRequest("Category object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid category object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.Category.CreateCategory(category);
                _repository.Save();

                return CreatedAtRoute("CategoryById", new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCategory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(Guid id, [FromBody] Category category)
        {
            try
            {
                if (category.IsObjectNull())
                {
                    _logger.LogError("Category object sent from client is null.");
                    return BadRequest("Category object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid category object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var dbCategory = _repository.Category.GetCategoryById(id);
                if (dbCategory.IsEmptyObject())
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Category.UpdateCategory(dbCategory, category);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(Guid id)
        {
            try
            {
                var category = _repository.Category.GetCategoryById(id);
                if (category.IsEmptyObject())
                {
                    _logger.LogError($"Category with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Category.DeleteCategory(category);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForCategory(Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetCategoryById), values: new {id, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteCategory), values: new {id}), "delete_category", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateCategory), values: new {id}), "update_category", "PUT")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForCategories(LinkCollectionWrapper<Entity> categoriesWrapper)
        {
            categoriesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetCategories), values: new { }), "self", "GET"));

            return categoriesWrapper;
        }
    }
}
