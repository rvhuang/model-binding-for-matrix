using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Heuristic.Matrix.Examples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values/2,[1,3-4,6];[4-6],1;1,5
        [HttpGet("{value}")]
        public IEnumerable<string> Get(MatrixIndicator value)
        {
            return value.AsEnumerable((i, j) => ValueTuple.Create(i, j).ToString());
        }
    }
}
