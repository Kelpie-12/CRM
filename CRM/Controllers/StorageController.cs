using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StorageController : Controller
    {
        private readonly IStorageServices _storageServices;
        public StorageController(IStorageServices storageServices)
        {
            _storageServices = storageServices;
        }

        [HttpPost]
        [Route("CreateItem")]
        public async Task<IActionResult> CreateItem(string name, string data)
        {
            //придумать строку которая будет создаватся динамически для каждого айтема
            //то есть если пользователь хочет создать новый товар, услугу и тд
            //на этапе создания он устанавливает все параметры

            return Ok();
        }
        [HttpPost]
        [Route("WriteOffItem")]
        public async Task<IActionResult> WriteOffItem(int id, int count)
        {
            return Ok();
        }

        [HttpPost]
        [Route("ArrivalItem")]
        public async Task<IActionResult> ArrivalItem(int id, string data)
        {
            return Ok();
        }
        [HttpPost]
        [Route("EditingItem")]
        public async Task<IActionResult> EditingItem(int id, string data)
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetInfoItem")]
        public async Task<string> GetInfoItem(int id)
        {
            return "Info item";
        }

        [HttpGet]
        [Route("GetInfoWriteOffItem")]
        public async Task<string> GetInfoWriteOffItem(int id)
        {
            return "Info WriteOffItem item";
        }

        [HttpGet]
        [Route("GetInfoArrivalItem")]
        public async Task<string> GetInfoArrivalItem(int id)
        {
            return "Info ArrivalItem item";
        }

        [HttpGet]
        [Route("GetInfoGroupItem")]
        public async Task<string> GetInfoGroupItem(string nameGroup)
        {
            return "Info GetInfoGroupItem item";
        }

    }
}
