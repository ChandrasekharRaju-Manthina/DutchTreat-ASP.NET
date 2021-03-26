using System;
using System.Collections.Generic;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository dutchRepository;
        private readonly ILogger<OrdersController> logger;
        private readonly IMapper mapper;

        public OrdersController(IDutchRepository dutchRepository, ILogger<OrdersController> logger
            ,IMapper mapper)
        {
            this.dutchRepository = dutchRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get(bool includeItems = true)
        {
            try
            {
                IEnumerable<Order> orders = dutchRepository.GetAllOrders(includeItems);
                return Ok(mapper.Map<IEnumerable<OrderViewModel>>(orders));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get orders: {ex}");
                return BadRequest("Bad request ");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = dutchRepository.GetOrdeById(id);
                if( order != null) 
                {
                    return Ok(mapper.Map<Order, OrderViewModel>(order));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get order: {ex}");
                return BadRequest("Bad request ");
            }

        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = mapper.Map<Order>(model);

                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    dutchRepository.AddEntity(newOrder);

                    if (dutchRepository.SaveAll())
                    {
                        var vm = mapper.Map<OrderViewModel>(newOrder);
                        return Created($"/api/orders/{vm.OrderId}", vm);
                    }
                }else
                {
                    return BadRequest(ModelState);
                }

            } catch(Exception ex)
            {
                logger.LogError($"Failed to create order: {ex}");
            }
            return BadRequest("Failed to save new order");
        }

    }
}