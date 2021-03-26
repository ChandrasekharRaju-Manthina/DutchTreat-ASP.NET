using System;
using System.Collections.Generic;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Produces("applicaiton/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IDutchRepository dutchRepository;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IDutchRepository dutchRepository, ILogger<ProductsController> logger)
        {
            this.dutchRepository = dutchRepository;
            this.logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Product>> Get()
        {
            try
            {
                return Ok(dutchRepository.GetAllProducts());
            } catch (Exception ex)
            {
                logger.LogError($"Failed to get products: {ex}");
                return BadRequest("Bad request");
            }
        }
          
    }
}
