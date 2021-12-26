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

namespace pms_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierOfferController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public SupplierOfferController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetAllSupplierOffers([FromQuery] SupplierOfferParameters supplierOfferParameters)
        {
            try
            {
                var supplierOffers = _repository.SupplierOffer.GetAllSupplierOffersBySupplier(supplierOfferParameters);

                var metadata = new
                {
                    supplierOffers.TotalCount,
                    supplierOffers.PageSize,
                    supplierOffers.CurrentPage,
                    supplierOffers.TotalPages,
                    supplierOffers.HasNext,
                    supplierOffers.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {supplierOffers.Count} supplierOffers from database.");

                var shapedSupplierOffers = supplierOffers.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedSupplierOffers);
                }

                for (var index = 0; index < supplierOffers.Count; index++)
                {
                    var supplierLinks = CreateLinksForSupplierOffer(((ShapedSupplierOfferEntity)supplierOffers[index]).SupplierId, ((ShapedSupplierOfferEntity)supplierOffers[index]).ProductId, ((ShapedSupplierOfferEntity)supplierOffers[index]).Amount, supplierOfferParameters.Fields);
                    shapedSupplierOffers[index].Add("Links", supplierLinks);
                }

                var suppliersWrapper = new LinkCollectionWrapper<Entity>(shapedSupplierOffers);

                return Ok(CreateLinksForSupplierOffers(suppliersWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllSupplierOffers action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateSupplierOffer([FromBody] SupplierOffer supplierOffer)
        {
            try
            {
                if (supplierOffer.IsObjectNull())
                {
                    _logger.LogError("SupplierOffer object sent from client is null.");
                    return BadRequest("SupplierOffer object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid supplierOffer object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.SupplierOffer.CreateSupplierOffer(supplierOffer);
                _repository.Save();

                return CreatedAtRoute("GetSupplierOfferBySupplierProductAndAmount", new { supplierId = supplierOffer.SupplierId, productId = supplierOffer.ProductId, amount = supplierOffer.Amount }, supplierOffer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{supplierId}/{productId}/{amount}")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetSupplierOfferBySupplierProductAndAmount(Guid supplierId, Guid productId, int amount, [FromQuery] string fields)
        {
            try
            {
                var supplierOffer = (ShapedSupplierOfferEntity)_repository.SupplierOffer.GetSupplierOfferBySupplierProductAndAmount(supplierId, productId, amount, fields);

                if (supplierOffer.SupplierId == Guid.Empty)
                {
                    _logger.LogError($"SupplierOffer with SupplierId: {supplierId}, hasn't been found in db.");
                    return NotFound();
                }

                if (supplierOffer.ProductId == Guid.Empty)
                {
                    _logger.LogError($"SupplierOffer with SupplierId: {supplierId} and ProductId: {productId}, hasn't been found in db.");
                    return NotFound();
                }

                if (supplierOffer.Amount <= 0)
                {
                    _logger.LogError($"SupplierOffer with SupplierId: {supplierId}, ProductId: {productId} and an amount of {amount}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped supplierOffer with supplierId: {supplierId}, productId: {productId} and an amount of {amount}");
                    return Ok(supplierOffer.Entity);
                }

                supplierOffer.Entity.Add("Links", CreateLinksForSupplierOffer(supplierOffer.SupplierId, supplierOffer.ProductId, supplierOffer.Amount, fields));

                return Ok(supplierOffer.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSupplierOfferBySupplierProductAndAmount action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{supplierId}/{productId}/{amount}")]
        public IActionResult UpdateSupplierOffer(Guid supplierId, Guid productId, int amount, [FromBody] SupplierOffer supplierOffer)
        {
            try
            {
                if (supplierOffer.IsObjectNull())
                {
                    _logger.LogError("SupplierOffer object sent from client is null.");
                    return BadRequest("SupplierOffer object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid supplierOffer object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var dbSupplierOffer = _repository.SupplierOffer.GetSupplierOfferBySupplierProductAndAmount(supplierId, productId, amount);
                if (dbSupplierOffer.IsEmptyObject())
                {
                    _logger.LogError($"SupplierOffer with supplierId: {supplierId}, productId: {productId} and an amount of {amount}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.SupplierOffer.UpdateSupplierOffer(dbSupplierOffer, supplierOffer);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSupplierOffer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{supplierId}/{productId}/{amount}")]
        public IActionResult DeleteSupplierOffer(Guid supplierId, Guid productId, int amount)
        {
            try
            {
                var supplierOffer = _repository.SupplierOffer.GetSupplierOfferBySupplierProductAndAmount(supplierId, productId, amount);
                if (supplierOffer.IsEmptyObject())
                {
                    _logger.LogError($"SupplierOffer with supplierId: {supplierId}, productId {productId} and an amount of {amount}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.SupplierOffer.DeleteSupplierOffer(supplierOffer);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteSupplierOffer action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForSupplierOffer(Guid supplierId, Guid productId, int amount, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetSupplierOfferBySupplierProductAndAmount), values: new {supplierId, productId, amount, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteSupplierOffer), values: new {supplierId, productId, amount}), "delete_supplierOffer", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateSupplierOffer), values: new {supplierId, productId, amount}), "update_supplierOffer", "PUT")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForSupplierOffers(LinkCollectionWrapper<Entity> supplierOffersWrapper)
        {
            supplierOffersWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetAllSupplierOffers), values: new { }), "self", "GET"));

            return supplierOffersWrapper;
        }
    }
}
