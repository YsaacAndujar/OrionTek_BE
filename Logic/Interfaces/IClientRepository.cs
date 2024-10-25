using Logic.Dtos.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IClientRepository
    {
        Task<ClientDto> Create(ClientCreateDto dto);
        Task<List<ClientDto>> GetAll(ClientFilterDto dto);
        Task Update(int id, ClientCreateDto dto);
        Task<ClientDto> Get(int id);
        Task Delete(int id);
    }
}
