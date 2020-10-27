using System.Collections.Generic;
using System.Linq;
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
            CreateMap<BlModels.Warehouse, DalModels.Warehouse>()
                .ForMember(w => w.NextHops,
                    opt => opt.MapFrom<DalHopNexthopResolver>())
                .ReverseMap();
            CreateMap<BlModels.Truck, DalModels.Truck>().ReverseMap();
            CreateMap<BlModels.Transferwarehouse, DalModels.Transferwarehouse>().ReverseMap();
            CreateMap<BlModels.HopArrival, DalModels.HopArrival>().ReverseMap();
            CreateMap<BlModels.Parcel, DalModels.Parcel>().ReverseMap();
            CreateMap<BlModels.Recipient, DalModels.Recipient>().ReverseMap();
        }
    }

    public class DalHopNexthopResolver : IValueResolver<BlModels.Warehouse, DalModels.Warehouse, List<DalModels.Hop>>
    {
        public List<DalModels.Hop> Resolve(BlModels.Warehouse source, DalModels.Warehouse destination, List<DalModels.Hop> destMember, ResolutionContext context)
        {
            // return source.NextHops.Select(wnh => wnh.Hop).ToList();
            return new List<DalModels.Hop>();
        }
    }
}