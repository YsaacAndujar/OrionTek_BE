using Logic.Dtos.Clients;
using Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrionTek.Middlewares;

namespace OrionTek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthCustom]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository clientRepository;

        public ClientsController(IClientRepository _clientRepository)
        {
            clientRepository = _clientRepository;
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> Create([FromBody] ClientCreateDto dto)
        {
            return Ok(await clientRepository.Create(dto));
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> Update(int id,[FromBody] ClientCreateDto dto)
        {
            await clientRepository.Update(id, dto);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<ClientDto>>> GetAll([FromQuery] ClientFilterDto dto)
        {
            return Ok(await clientRepository.GetAll(dto));
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> Get(int id)
        {
            return Ok(await clientRepository.Get(id));
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<ClientDto>> Delete(int id)
        {
            await clientRepository.Delete(id);
            return Ok();
        }
    }
}
