using AutoMapper;
using FreeLanceTracker.Data;
using FreeLanceTracker.DTOs.ClientDtos;
using FreeLanceTracker.DTOs.InvoiceDtos;
using FreeLanceTracker.DTOs.LineItemDto;
using FreeLanceTracker.DTOs.ProjectDtos;
using FreeLanceTracker.DTOs.TimeEntryDtos;

namespace FreeLanceTracker.MappingProfile;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Clients.
        // Added exceptions to ignore when making the mapping
        CreateMap<CreateClientDto, Client>()
            .ForMember(dest => dest.ClientId, opt => opt.Ignore())
            .ForMember(dest => dest.IsArchived, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Projects, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        CreateMap<Client, CreateClientDto>();

        CreateMap<GetAllClientDto, Client>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Projects, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        CreateMap<Client, GetAllClientDto>();

        CreateMap<ClientDto, Client>()
            .ForMember(dest => dest.IsArchived, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Projects, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        CreateMap<Client, ClientDto>();

        CreateMap<UpdateClientDto, Client>()
            .ForMember(dest => dest.IsArchived, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Projects, opt => opt.Ignore())
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        CreateMap<Client, UpdateClientDto>();


        //Invoice
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<InvoiceDto, Invoice>()
            .ForMember(dest => dest.Client, opt => opt.Ignore());

        CreateMap<UpdateInvoiceStatusDto, Invoice>()
            .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
            .ForMember(dest => dest.IssueDate, opt => opt.Ignore())
            .ForMember(dest => dest.DueDate, opt => opt.Ignore())
            .ForMember(dest => dest.ClientId, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .ForMember(dest => dest.LineItems, opt => opt.Ignore());
        
        CreateMap<Invoice, UpdateInvoiceStatusDto>();

        //InvoiceLineItem
        CreateMap<InvoiceLineItem, InvoiceLineItemDto>();
        CreateMap<InvoiceLineItemDto, InvoiceLineItem>();

        //TimeEntry
        CreateMap<TimeEntry, TimeEntryDto>();
        CreateMap<TimeEntryDto, TimeEntry>();

        //Project
        CreateMap<Project, ProjectDto>();
        CreateMap<ProjectDto, Project>();
    }
}