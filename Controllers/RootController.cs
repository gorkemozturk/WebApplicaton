using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        [ProducesResponseType(200)]
        public IActionResult GetRoot()
        {
            var response = new RootResponse {
                Self = Link.To(nameof(GetRoot)),
                Rooms = Link.ToCollection(nameof(RoomsController.GetRooms)),
                Info = Link.To(nameof(GetInfo))
            };

            return Ok(response);
        }

        [HttpGet("Info", Name = "GetInfo")]
        [ProducesResponseType(200)]
        public ActionResult<Hotel> GetInfo()
        {
            var hotel = new Hotel
            {
                Href = Url.Link(nameof(GetRoot), null),
                Title = "Premium Dexul Hotel",
                Tagline = "It's a quite powerfull premium hotel",
                Email = "info@premiumhotel.com",
                Website = "www.premiumhotel.com",
                Address = new Address
                {
                    Country = "Turkey",
                    City = "Antalya",
                    Region = "Alanya"
                }
            };

            return hotel;
        }
    }
}