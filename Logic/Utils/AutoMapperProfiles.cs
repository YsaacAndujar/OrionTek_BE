using AutoMapper;
using Data.Models;
using Logic.Dtos.Clients;
using Logic.Dtos.Users;

namespace Logic.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {
            CreateMap<User, UserDto>();

            CreateMap<Client, ClientDto>()
                .ForMember(dto => dto.Directions, opt => opt.MapFrom(MapClientDirectionToClientDto));

            CreateMap<ClientCreateDto, Client>()
                .ForMember(client => client.Directions, opt => opt.MapFrom(MapClientCreateDirectionToClient));
        }

        private List<Direction> MapClientCreateDirectionToClient(ClientCreateDto dto, Client client)
        {
            List<Direction> result = new List<Direction>();
            dto.Directions.ForEach(direction =>
            {
                result.Add(new Direction() { Name = direction });
            });
            return result;
        }
        private List<string> MapClientDirectionToClientDto(Client client, ClientDto dto)
        {
            List<string> result = new List<string>();
            client.Directions?.ForEach(direction =>
            {
                result.Add(direction.Name);
            });
            return result;
        }
    }
}
