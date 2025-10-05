
using Application.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class CompanyMapping : Profile
    {
        public CompanyMapping()
        {
            CreateMap<Company, CompanyDto>();
        }
    }
}