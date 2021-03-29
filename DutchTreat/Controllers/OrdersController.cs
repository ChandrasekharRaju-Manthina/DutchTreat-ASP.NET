using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository dutchRepository;
        private readonly ILogger<OrdersController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<StoreUser> userManager;

        public OrdersController(IDutchRepository dutchRepository, ILogger<OrdersController> logger
            , IMapper mapper, UserManager<StoreUser> userManager)
        {
            this.dutchRepository = dutchRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get(bool includeItems = true)
        {
            try
            {
                var username = User.Identity.Name;

                IEnumerable<Order> orders = dutchRepository.GetAllOrdersByUser(username, includeItems);
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
                var order = dutchRepository.GetOrdeById(User.Identity.Name, id);
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
        public async Task<IActionResult> Post([FromBody] OrderViewModel model)
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

                    var currentUser = await userManager.FindByNameAsync(User.Identity.Name);
                    newOrder.User = currentUser;

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