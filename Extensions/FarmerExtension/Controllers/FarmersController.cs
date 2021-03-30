using Common;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace FarmerExtension.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FarmersController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IPersonService _personService;

        //public FarmersController()
        //{
            
        //}
        public FarmersController(IDataService dataService, IPersonService personService)
        {
            _dataService = dataService;
            _personService = personService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            //return Ok("Farmer");
            return Ok($"This is a some Farmers DataService:{_dataService.GetDataText()}  PersonService: {_personService.GetData()}");
        }
    }
}
