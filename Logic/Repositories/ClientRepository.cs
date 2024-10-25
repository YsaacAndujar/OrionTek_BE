using AutoMapper;
using Data;
using Logic.Dtos.Clients;
using Logic.Interfaces;

namespace Logic.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IMapper mapper;
        private readonly OrionTekDbContext dbContext;

        public ClientRepository(IMapper _mapper, OrionTekDbContext _dbContext)
        {
            mapper = _mapper;
            dbContext = _dbContext;
        }
        public Task<ClientDto> Create(ClientCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ClientDto> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClientDto>> GetAll(ClientFilterDto dto)
        {
            throw new NotImplementedException();
        }

        public Task Update(int id, ClientCreateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
