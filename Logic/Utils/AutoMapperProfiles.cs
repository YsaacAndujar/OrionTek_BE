using AutoMapper;
using Data.Models;
using Logic.Dtos.Clients;

namespace Logic.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {
            CreateMap<Client, ClientDto>();
            CreateMap<ClientCreateDto, Client>()
                .ForMember(client => client.Directions, opt => opt.MapFrom(MapClientCreatDirectionToClient));
        }

        private List<Direction> MapClientCreatDirectionToClient(ClientCreateDto dto, Client client)
        {
            List<Direction> result = new List<Direction>();
            dto.Directions.ForEach(direction =>
            {
                result.Add(new Direction() { Name = direction });
            });
            return result;
        }
    }
}
