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
    public class ProductController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public ProductController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetProducts([FromQuery] ProductParameters productParameters)
        {
            try
            {
                var products = _repository.Product.GetAllProducts(productParameters);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.TotalPages,
                    products.HasNext,
                    products.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {products.Count} products from database.");

                var shapedProducts = products.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedProducts);
                }

                for (var index = 0; index < products.Count; index++)
                {
                    var productLinks = CreateLinksForProduct(((NormalShapedEntity)products[index]).Id, productParameters.Fields);
                    shapedProducts[index].Add("Links", productLinks);
                }

                var productsWrapper = new LinkCollectionWrapper<Entity>(shapedProducts);

                return Ok(CreateLinksForProducts(productsWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "ProductById")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetProductById(Guid id, [FromQuery] string fields)
        {
            try
            {
                var product = (NormalShapedEntity)_repository.Product.GetProductById(id, fields);

                if (product.Id == Guid.Empty)
                {
                    _logger.LogError($"Product with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped product with id: {id}");
                    return Ok(product.Entity);
                }

                product.Entity.Add("Links", CreateLinksForProduct(product.Id, fields));

                return Ok(product.Entity);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wring inside GetProductById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            try
            {
                if (product.IsObjectNull())
                {
                    _logger.LogError("Product object sent from client is null.");
                    return BadRequest("Product object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid product object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.Product.CreateProduct(product);
                _repository.Save();

                return CreatedAtRoute("ProductById", new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateProduct action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(Guid id, [FromBody] Product product)
        {
            try
            {
                if (product.IsObjectNull())
                {
                    _logger.LogError("Product object sent from client is null.");
                    return BadRequest("Product object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid product object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var dbProduct = _repository.Product.GetProductById(id);
                if (dbProduct.IsEmptyObject())
                {
                    _logger.LogError($"Product with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Product.UpdateProduct(dbProduct, product);
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
        public IActionResult DeleteProduct(Guid id)
        {
            try
            {
                var product = _repository.Product.GetProductById(id);
                if (product.IsEmptyObject())
                {
                    _logger.LogError($"Product with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Product.DeleteProduct(product);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForProduct(Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetProductById), values: new {id, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteProduct), values: new {id}), "delete_product", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateProduct), values: new {id}), "update_product", "PUT")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForProducts(LinkCollectionWrapper<Entity> productsWrapper)
        {
            productsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetProducts), values: new { }), "self", "GET"));

            return productsWrapper;
        }
    }
}
