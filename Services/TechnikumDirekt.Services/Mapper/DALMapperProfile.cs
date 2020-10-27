using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;
using BlModels = TechnikumDirekt.BusinessLogic.Models;
using DalModels = TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class DalMapperProfile : Profile
    {
        public DalMapperProfile()
        {
            CreateMap<BlModels.Hop, DalModels.Hop>()
                .Include<BlModels.Warehouse, DalModels.Warehouse>()
                .Include<BlModels.Truck, DalModels.Truck>()
                .Include<BlModels.Transferwarehouse, DalModels.Transferwarehouse>();

            CreateMap<BlModels.Warehouse, DalModels.Warehouse>()
                .ForMember(dest => dest.NextHops,
                    dest =>
                        dest.MapFrom(src => src.NextHops.Select(wnh => wnh.Hop)))
                .ForMember(dest => dest.Level,
                    dest =>
                        dest.MapFrom(wh => wh.Level))
                .AfterMap((src, dest, context) =>
                {
                    for (int i = 0; i < src.NextHops.Count; i++)
                    {
                        dest.NextHops[i].ParentTraveltimeMins = src.NextHops[i].TraveltimeMins;
                    }
                }).ReverseMap();

            CreateMap<DalModels.Warehouse, BlModels.Warehouse>()
                .ForMember(dest => dest.NextHops,
                    dest =>
                        dest.MapFrom(src => src.NextHops));

            CreateMap<DalModels.Warehouse, BlModels.WarehouseNextHops>()
                .AfterMap((src, dest, context) =>
                {
                    dest = new BlModels.WarehouseNextHops()
                    {
                        //TraveltimeMins = src.NextHops.Find(x => x.Code == dest.Hop.Code).ParentTraveltimeMins
                        //WTF dude
                    };
                });

            CreateMap<BlModels.Truck, DalModels.Truck>().ReverseMap();
            CreateMap<BlModels.Transferwarehouse, DalModels.Transferwarehouse>().ReverseMap();
            CreateMap<BlModels.HopArrival, DalModels.HopArrival>().ReverseMap();
            CreateMap<BlModels.Parcel, DalModels.Parcel>().ReverseMap();
            CreateMap<BlModels.Recipient, DalModels.Recipient>().ReverseMap();
        }
    }
}