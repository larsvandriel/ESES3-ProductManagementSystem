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
    public class SupplierController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly LinkGenerator _linkGenerator;

        public SupplierController(ILoggerManager logger, IRepositoryWrapper repository, LinkGenerator linkGenerator)
        {
            _logger = logger;
            _repository = repository;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetSuppliers([FromQuery] SupplierParameters supplierParameters)
        {
            try
            {
                var suppliers = _repository.Supplier.GetAllSuppliers(supplierParameters);

                var metadata = new
                {
                    suppliers.TotalCount,
                    suppliers.PageSize,
                    suppliers.CurrentPage,
                    suppliers.TotalPages,
                    suppliers.HasNext,
                    suppliers.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned {suppliers.Count} suppliers from database.");

                var shapedSuppliers = suppliers.Select(i => i.Entity).ToList();

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Ok(shapedSuppliers);
                }

                for (var index = 0; index < suppliers.Count; index++)
                {
                    var supplierLinks = CreateLinksForSupplier((Guid)((NormalShapedEntity)suppliers[index]).Id, supplierParameters.Fields);
                    shapedSuppliers[index].Add("Links", supplierLinks);
                }

                var suppliersWrapper = new LinkCollectionWrapper<Entity>(shapedSuppliers);

                return Ok(CreateLinksForSuppliers(suppliersWrapper));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "SupplierById")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public IActionResult GetSupplierById(Guid id, [FromQuery] string fields)
        {
            try
            {
                var supplier = (NormalShapedEntity)_repository.Supplier.GetSupplierById(id, fields);

                if (supplier.Id == Guid.Empty)
                {
                    _logger.LogError($"Supplier with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                var mediaType = (MediaTypeHeaderValue)HttpContext.Items["AcceptHeaderMediaType"];

                if (!mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogInfo($"Returned shaped supplier with id: {id}");
                    return Ok(supplier.Entity);
                }

                supplier.Entity.Add("Links", CreateLinksForSupplier(supplier.Id, fields));

                return Ok(supplier.Entity);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wring inside GetSupplierById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateSupplier([FromBody] Supplier supplier)
        {
            try
            {
                if (supplier.IsObjectNull())
                {
                    _logger.LogError("Supplier object sent from client is null.");
                    return BadRequest("Supplier object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid supplier object sent from client.");
                    return BadRequest("Invalid model object");
                }

                _repository.Supplier.CreateSupplier(supplier);
                _repository.Save();

                return CreatedAtRoute("SupplierById", new { id = supplier.Id }, supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateSupplier action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSupplier(Guid id, [FromBody] Supplier supplier)
        {
            try
            {
                if (supplier.IsObjectNull())
                {
                    _logger.LogError("Supplier object sent from client is null.");
                    return BadRequest("Supplier object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid supplier object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var dbSupplier = _repository.Supplier.GetSupplierById(id);
                if (dbSupplier.IsEmptyObject())
                {
                    _logger.LogError($"Supplier with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Supplier.UpdateSupplier(dbSupplier, supplier);
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
        public IActionResult DeleteSupplier(Guid id)
        {
            try
            {
                var supplier = _repository.Supplier.GetSupplierById(id);
                if (supplier.IsEmptyObject())
                {
                    _logger.LogError($"Supplier with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.Supplier.DeleteSupplier(supplier);
                _repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private IEnumerable<Link> CreateLinksForSupplier(Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetSupplierById), values: new {id, fields}), "self", "GET"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(DeleteSupplier), values: new {id}), "delete_supplier", "DELETE"),
                new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(UpdateSupplier), values: new {id}), "update_supplier", "PUT")
            };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForSuppliers(LinkCollectionWrapper<Entity> suppliersWrapper)
        {
            suppliersWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(HttpContext, nameof(GetSuppliers), values: new { }), "self", "GET"));

            return suppliersWrapper;
        }
    }
}
