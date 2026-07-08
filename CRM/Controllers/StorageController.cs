using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;
using CRM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{

    [Route("api/v1/[controller]")]
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
        [Authorize(Policy = Permissions.Create)]
        [Route("Create")]
        public async Task<IActionResult> CreateItem([FromBody] ItemModel item)
        {
            CreateItemResponse response = await _storageServices.CreateItem(item);
            if (response.Item.Id != -1)
            {
                return StatusCode(StatusCodes.Status201Created, response);//new { message = response.Message, Item = response.Item,Error=response.Error });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest, response);//new { message = "Created was fail", error = "error_create_item_fail" });
            //придумать строку которая будет создаватся динамически для каждого айтема
            //то есть если пользователь хочет создать новый товар, услугу и тд
            //на этапе создания он устанавливает все параметры


        }
        [HttpPut]
        [Route("Update")]
        [Authorize(Policy = Permissions.Update)]
        public async Task<IActionResult> UpdateItem([FromBody] ItemModel item)
        {
            Item res = new Item();
            res = await _storageServices.UpdateItem(item);
            if (res.Id != 1)
            {
                return StatusCode(StatusCodes.Status201Created, new { message = "Item was created", Item = res });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Created was fail", error = "error_create_item_fail" });


        }
        [HttpDelete]
        [Route("Delete")]
        [Authorize(Policy = Permissions.Delete)]
        public async Task<IActionResult> DeleteItem([FromBody] ItemModel item)
        {

            if (await _storageServices.DeleteItem(item))
            {
                return StatusCode(StatusCodes.Status200OK, new { message = "Item was deleted" });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Deleted was fail", error = "error_create_item_fail" });
        }
        [HttpPut]
        [Authorize(Policy = Permissions.Update)]
        [Route("WriteOff")]
        public async Task<IActionResult> WriteOffItem(int id, int count)
        {
            return Ok();
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Update)]
        [Route("Arrival")]
        public async Task<IActionResult> ArrivalItem(int id, string data)
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetAll/{companyId}/{companyName}")]
        [Authorize(Policy = Permissions.Read)]
        public async Task<IActionResult> GetAll(int companyId, string companyName)
        {
            ICollection<Item> items = await _storageServices.GetAll(new CompanyDto { Name = companyName, Id = companyId });
            return StatusCode(StatusCodes.Status200OK, items);

            //return  Ok("GETALLL");
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
