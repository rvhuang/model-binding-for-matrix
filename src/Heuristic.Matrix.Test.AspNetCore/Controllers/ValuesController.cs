using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Heuristic.Matrix.Test.AspNetCore.Controllers
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
        
        [HttpPost]
        public MatrixIndicator Post([FromBody]int[][] array)
        {
            var selectFromArray = from j in Enumerable.Range(0, array.Length)
                                  from i in Enumerable.Range(0, array[j].Length)
                                  where array[j][i] > 0
                                  select (I: i, J: j);
            return MatrixIndicator.Create(selectFromArray.ToArray(), t => t.I, t => t.J);
        }
    }
}