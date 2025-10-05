
using Application.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class StoreMapping : Profile
    {
        public StoreMapping()
        {
            CreateMap<Store, StoreDto>();
            CreateMap<Store, DetailedStoreDto>();
        }
    }
}