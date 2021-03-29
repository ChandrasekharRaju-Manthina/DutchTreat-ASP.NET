using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Controllers
{
    [Route("/api/orders/{orderid}/items")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderItemsController : Controller
    {
        private readonly IDutchRepository dutchRepository;
        private readonly ILogger<OrderItemsController> logger;
        private readonly IMapper mapper;

        public OrderItemsController(IDutchRepository dutchRepository, ILogger<OrderItemsController> logger
            , IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dutchRepository = dutchRepository;
        }

        [HttpGet]
        public IActionResult Get(int orderId)
        {
            var order = dutchRepository.GetOrdeById(User.Identity.Name, orderId);
            if (order != null)
            {
                return Ok(mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemViewModel>>(order.Items));
            }
            return NotFound("not found");
        }

        [HttpGet("{id}")]
        public IActionResult Get(int orderId, int id)
        {

            var order = dutchRepository.GetOrdeById(User.Identity.Name, orderId);
            if (order != null)
            {
                var item = order.Items.Where(i => i.Id == id).FirstOrDefault();
                return Ok(mapper.Map<OrderItem, OrderItemViewModel>(item));
            }
            return NotFound();
        }


    }
}
