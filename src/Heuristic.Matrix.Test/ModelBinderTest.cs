using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Heuristic.Matrix.Test
{
    using AspNetCore;

    public class ModelBinderTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ModelBinderTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("2,[3-4,6];[2,4-6],1;1,5")]
        [InlineData("2,[1,3-4,6];[4-6],1;1,5")]
        [InlineData("2,[3,4,6];[2,4,5,6],1;1,5")]
        public async Task EndpointsGetReturnSuccessTest(string value)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("api/values/" + value);

            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var expected = new[] { "(2, 3)", "(2, 4)", "(2, 6)", "(2, 1)", "(2, 1)", "(4, 1)", "(5, 1)", "(6, 1)", "(1, 5)" };
            var actual = await response.Content.ReadAsAsync<string[]>();

            Assert.Equal(expected.ToHashSet(), actual.ToHashSet());
        }
    }
}