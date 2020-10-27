using AutoMapper;
using BlModels = TechnikumDirekt.BusinessLogic.Models;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class DalMapperProfile : Profile
    {
        public DalMapperProfile()
        {
            CreateMap<BlModels.Hop, DalModels.Hop>().ReverseMap();
            CreateMap<BlModels.Warehouse, DalModels.Warehouse>().ReverseMap();
            CreateMap<BlModels.Truck, DalModels.Truck>().ReverseMap();
            CreateMap<BlModels.Transferwarehouse, DalModels.Transferwarehouse>().ReverseMap();
            CreateMap<BlModels.HopArrival, DalModels.HopArrival>().ReverseMap();
            CreateMap<BlModels.Parcel, DalModels.Parcel>().ReverseMap();
            CreateMap<BlModels.Recipient, DalModels.Recipient>().ReverseMap();
        }
    }
}