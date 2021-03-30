using Common;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace DriverExtension.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IPersonService _personService;

        public DriversController()
        {
            
        }
        public DriversController(IDataService dataService, IPersonService personService)
        {
            _dataService = dataService;
            _personService = personService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            //return Ok("Driver");
             return Ok($"This is a some Drivers DataService:{_dataService.GetDataText()}  PersonService: {_personService.GetData()}");
        }
    }
}
