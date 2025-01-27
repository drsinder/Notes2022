﻿using Microsoft.AspNetCore.Mvc;
using Notes2022.Server.Services;
using System.Web;

namespace Notes2022.Server.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/{uri}")]
    [ApiController]
    public class LinkTestController : ControllerBase
    {
        public LinkTestController()
        {
        }

        [HttpGet]
        public async Task<bool> Get(string uri)
        {
            string urireal = HttpUtility.UrlDecode(uri);

            LinkProcessor lp = new LinkProcessor(null);
            return await lp.Test(urireal);
        }
    }
}
