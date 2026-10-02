using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDTO;

namespace FreeLanceTracker.MappingProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Clients
        CreateMap<CreateClientDto, Client>();
        CreateMap<Client, CreateClientDto>();

        CreateMap<GetAllClientDto, Client>();
        CreateMap<Client, GetAllClientDto>();

        CreateMap<GetClientDto, Client>();
        CreateMap<Client, GetClientDto>();

        CreateMap<UpdateClientDto, Client>();
        CreateMap<Client, UpdateClientDto>();
    }
}