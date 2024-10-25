using AutoMapper;
using Data;
using Data.Models;
using Logic.Dtos.Clients;
using Logic.Interfaces;
using Logic.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Logic.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IMapper mapper;
        private readonly OrionTekDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ClientRepository(IMapper _mapper, OrionTekDbContext _dbContext, IHttpContextAccessor _httpContextAccessor)
        {
            mapper = _mapper;
            dbContext = _dbContext;
            httpContextAccessor = _httpContextAccessor;
        }
        public async Task<ClientDto> Create(ClientCreateDto dto)
        {
            var client = mapper.Map<Client>(dto);
            await dbContext.Clients.AddAsync(client);
            await dbContext.SaveChangesAsync();
            return mapper.Map<ClientDto>(client);
        }

        public async Task Delete(int id)
        {
            dbContext.Clients.Remove(await GetClientOrThrow(id));
            await dbContext.SaveChangesAsync();
        }

        private async Task<Client> GetClientOrThrow(int id)
        {
            var client = await dbContext.Clients.FindAsync(id);
            if (client == null)
            {
                throw new CustomException(404, "Client not found.");
            }
            return client;
        }

        public async Task<ClientDto> Get(int id)
        {
            var client = await GetClientOrThrow(id);
            await dbContext.Entry(client)
                .Collection(c => c.Directions)
                .LoadAsync();
            return mapper.Map<ClientDto>(client);
        }

        public async Task<List<ClientDto>> GetAll(ClientFilterDto dto)
        {
            var queryable = dbContext.Clients
                .Where((client)=> (dto.Id == null? true : dto.Id == client.Id) &&
                (string.IsNullOrEmpty(dto.Name)? true : client.Name.Contains(dto.Name)))
                .AsQueryable();
            queryable = await httpContextAccessor.Paginate(queryable, dto);
            var clients = await queryable.ToListAsync();
            return mapper.Map<List<ClientDto>>(clients);
        }

        public async Task Update(int id, ClientCreateDto dto)
        {
            var client = await GetClientOrThrow(id);
            await dbContext.Entry(client)
                .Collection(c => c.Directions)
                .LoadAsync();
            dbContext.RemoveRange(client.Directions);
            mapper.Map(dto, client);
            await dbContext.SaveChangesAsync();
        }
    }
}
