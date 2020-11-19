using System.Linq;
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
                    if (src?.NextHops == null) return;

                    for (int i = 0; i < src.NextHops.Count; i++)
                    {
                        dest.NextHops[i].ParentTraveltimeMins = src.NextHops[i].TraveltimeMins;
                    }
                });

            CreateMap<DalModels.Warehouse, BlModels.Warehouse>()
                .AfterMap((src, dest, context) =>
                {
                    if (src.NextHops == null) return;

                    for (int i = 0; i < src.NextHops.Count; i++)
                    {
                        dest.NextHops[i].TraveltimeMins = src.NextHops[i].ParentTraveltimeMins;
                    }
                });

            CreateMap<DalModels.Hop, BlModels.WarehouseNextHops>()
                .ForMember(dest => dest.Hop,
                    opt =>
                        opt.MapFrom(src => src))
                .Include<DalModels.Warehouse, BlModels.WarehouseNextHops>()
                .Include<DalModels.Truck, BlModels.WarehouseNextHops>()
                .Include<DalModels.Transferwarehouse, BlModels.WarehouseNextHops>();

            CreateMap<DalModels.Warehouse, BlModels.WarehouseNextHops>()
                .ForMember(dest => dest.Hop,
                    opt =>
                        opt.MapFrom(src => src));
            CreateMap<DalModels.Truck, BlModels.WarehouseNextHops>()
                .ForMember(dest => dest.Hop,
                    opt =>
                        opt.MapFrom(src => src));
            CreateMap<DalModels.Transferwarehouse, BlModels.WarehouseNextHops>()
                .ForMember(dest => dest.Hop,
                    opt =>
                        opt.MapFrom(src => src));

            CreateMap<DalModels.Warehouse, BlModels.Hop>()
                .As<BlModels.Warehouse>();

            CreateMap<DalModels.Truck, BlModels.Hop>()
                .As<BlModels.Truck>();

            CreateMap<DalModels.Transferwarehouse, BlModels.Hop>()
                .As<BlModels.Transferwarehouse>();

            CreateMap<BlModels.Truck, DalModels.Truck>().ReverseMap();
            CreateMap<BlModels.Transferwarehouse, DalModels.Transferwarehouse>().ReverseMap();
            CreateMap<BlModels.HopArrival, DalModels.HopArrival>()
                .ForMember(dest => dest.HopCode,
                    opt => opt.MapFrom(src => src.Code))
                .ReverseMap();

            CreateMap<BlModels.Parcel, DalModels.Parcel>().ReverseMap();

            CreateMap<BlModels.Parcel, DalModels.Parcel>()
                .BeforeMap((src, dest) => { src.VisitedHops.AddRange(src.FutureHops); })
                .ForMember(dest => dest.HopArrivals,
                    opt => opt.MapFrom(src => src.VisitedHops));

            CreateMap<DalModels.Parcel, BlModels.Parcel>()
                .ForMember(dest => dest.FutureHops,
                    opt => opt.MapFrom(src => src.HopArrivals.Where(ha => ha.HopArrivalTime == null)))
                .ForMember(dest => dest.VisitedHops,
                    opt => opt.MapFrom(src =>
                        src.HopArrivals.Where(ha => ha.HopArrivalTime != null)
                            .OrderBy(ha => ha.HopArrivalTime)))
                .ForMember(dest => dest.FutureHops,
                    opt =>
                    {
                        opt.MapFrom(src =>
                            src.HopArrivals.Where(ha => ha.HopArrivalTime == null)
                                .OrderBy(ha => ha.Order));
                    })
                .AfterMap((src, dest, context) =>
                {
                    foreach (var destFutureHop in dest.FutureHops)
                    {
                        destFutureHop.Description = src.HopArrivals
                            .FirstOrDefault(ha => ha.HopCode == destFutureHop.Code)
                            ?.Hop.Description;
                    }
                    
                    foreach (var visitedHop in dest.VisitedHops)
                    {
                        visitedHop.Description = src.HopArrivals
                            .FirstOrDefault(ha => ha.HopCode == visitedHop.Code)
                            ?.Hop.Description;
                    }

                    /*
                    var allWh = src.HopArrivals.FindAll(ha => ha.Hop.HopType == DalModels.HopType.Warehouse)
                        .Select(ha => ha.Hop).Cast<BlModels.Warehouse>().ToList();

                    var topMostWhLevel = allWh.Min(wh => wh.Level);
                    
                    var topMostWh = allWh.FirstOrDefault(wh => wh.Level == topMostWhLevel);
                    */
                });
            
            CreateMap<BlModels.Recipient, DalModels.Recipient>().ReverseMap();
        }
    }
}