using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDtos;
using FreeLanceTracker.DTOs.InvoiceDtos;
using FreeLanceTracker.DTOs.LineItemDto;
using FreeLanceTracker.DTOs.ProjectDtos;

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

        CreateMap<ClientDto, Client>();
        CreateMap<Client, ClientDto>();

        CreateMap<UpdateClientDto, Client>();
        CreateMap<Client, UpdateClientDto>();
        
        //Invoice
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<InvoiceDto, Invoice>();
        
        CreateMap<UpdateInvoiceStatusDto, Invoice>();
        CreateMap<Invoice, UpdateInvoiceStatusDto>();
        
        //InvoiceLineItem
        CreateMap<InvoiceLineItem, InvoiceLineItemDto>();
        CreateMap<InvoiceLineItemDto, InvoiceLineItem>();
        
        //TimeEntry
        CreateMap<TimeEntry, InvoiceLineItemDto>();
        CreateMap<InvoiceLineItemDto, TimeEntry>();
        
        //Project
        CreateMap<Project, ProjectDto>();
        CreateMap<ProjectDto, Project>();
    }
}