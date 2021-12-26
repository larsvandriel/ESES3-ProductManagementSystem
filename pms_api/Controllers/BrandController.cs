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
    public class BrandController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public BrandController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetBrands([FromQuery] BrandParameters brandParameters)
        {
            try
            {
                var brands = _repository.Brand.GetAllBrands(brandParameters);

                var metadata = new
                {
                    brands.TotalCount,
                    brands.PageSize,
                    brands.CurrentPage,
                    brands.TotalPages,
                    brands.HasNext,
                    brands.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {brands.Count} brands from database.");

                var shapedBrands = brands.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedBrands);
                }

                for (var index = 0; index < brands.Count; index++)
                {
                    var brandLinks = CreateLinksForBrand(((NormalShapedEntity)brands[index]).Id, brandParameters.Fields);
                    shapedBrands[index].Add("Links", brandLinks);
                }

                var brandsWrapper = new LinkCollectionWrapper<Entity>(shapedBrands);

                return Ok(CreateLinksForBrands(brandsWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "BrandById")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetBrandById(Guid id, [FromQuery] string fields)
        {
            try
            {
                var brand = (NormalShapedEntity)_repository.Brand.GetBrandById(id, fields);

                if (brand.Id == Guid.Empty)
                {
                    _logger.LogError($"Brand with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped brand with id: {id}");
                    return Ok(brand.Entity);
                }

                brand.Entity.Add("Links", CreateLinksForBrand(brand.Id, fields));

                return Ok(brand.Entity);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wring inside GetBrandById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateBrand([FromBody] Brand brand)
        {
            try
            {
                if (brand.IsObjectNull())
                {
                    _logger.LogError("Brand object sent from client is null.");
                    return BadRequest("Brand object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid brand object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.Brand.CreateBrand(brand);
                _repository.Save();

                return CreatedAtRoute("BrandById", new { id = brand.Id }, brand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateBrand action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBrand(Guid id, [FromBody] Brand brand)
        {
            try
            {
                if (brand.IsObjectNull())
                {
                    _logger.LogError("Brand object sent from client is null.");
                    return BadRequest("Brand object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid brand object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var dbBrand = _repository.Brand.GetBrandById(id);
                if (dbBrand.IsEmptyObject())
                {
                    _logger.LogError($"Brand with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Brand.UpdateBrand(dbBrand, brand);
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
        public IActionResult DeleteBrand(Guid id)
        {
            try
            {
                var brand = _repository.Brand.GetBrandById(id);
                if (brand.IsEmptyObject())
                {
                    _logger.LogError($"Brand with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Brand.DeleteBrand(brand);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForBrand(Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetBrandById), values: new {id, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteBrand), values: new {id}), "delete_brand", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateBrand), values: new {id}), "update_brand", "PUT")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForBrands(LinkCollectionWrapper<Entity> brandsWrapper)
        {
            brandsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetBrands), values: new { }), "self", "GET"));

            return brandsWrapper;
        }
    }
}
