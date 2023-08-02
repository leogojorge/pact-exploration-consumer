using Microsoft.AspNetCore.Mvc;
using PactExploration.Provider;
using Provider.Models;

namespace Provider.Controllers
{
    [Route("api/some-data")]
    [ApiController]
    public class IWillProvideSomeDataController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<GetSomeDataByIdResponse> Get()
        {
            return FakeDatabase.OurFakeData;
        }

        [HttpGet("{id}")]
        public GetSomeDataByIdResponse Get(string id)
        {
            bool isValidId = Guid.TryParse(id, out Guid idAsGuid);
            if (!isValidId)
                return null;

            return FakeDatabase.OurFakeData.FirstOrDefault(x => x.Id == idAsGuid);
        }

        [HttpPost]
        public GetSomeDataByIdResponse Post([FromBody] PostDataRequest request)
        {
            var response = new GetSomeDataByIdResponse()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                LastName = request.LastName,
                Age = request.Age,
            };

            FakeDatabase.OurFakeData.Add(response);

            return response;
        }

        [HttpDelete("{id}")]
        public GetSomeDataByIdResponse Delete([FromRoute] string id)
        {
            bool isValidId = Guid.TryParse(id, out Guid idAsGuid);
            if (!isValidId)
                return null;

            var entity = FakeDatabase.OurFakeData.FirstOrDefault(x => x.Id == idAsGuid);

            if (entity is null)
                return null;

            FakeDatabase.OurFakeData.Remove(entity);

            return entity;
        }
    }
}