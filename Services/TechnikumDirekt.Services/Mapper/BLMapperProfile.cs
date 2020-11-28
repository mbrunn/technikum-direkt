using System.Collections.Generic;
using AutoMapper;
using NetTopologySuite.Geometries;
using BlModels = TechnikumDirekt.BusinessLogic.Models;
using SvcModels = TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Mapper
{
    /// <summary>
    /// Configures Mapperprofile between BL-Layer and SVC-Layer
    /// </summary>
    public class BlMapperProfile : Profile
    {
        public BlMapperProfile()
        {
            CreateMap<SvcModels.Hop, BlModels.Hop>()
                .ForMember(destMemb => destMemb.LocationCoordinates,
                    destMemb =>
                        destMemb.ConvertUsing<PointConverter, SvcModels.GeoCoordinate>(p => p.LocationCoordinates))
                .Include<SvcModels.Warehouse, BlModels.Warehouse>()
                .Include<SvcModels.Truck, BlModels.Truck>()
                .Include<SvcModels.Transferwarehouse, BlModels.Transferwarehouse>();

            CreateMap<BlModels.Hop, SvcModels.Hop>()
                .ForMember(destMemb => destMemb.LocationCoordinates,
                    destMemb =>
                        destMemb.ConvertUsing<PointConverter, Point>(p => p.LocationCoordinates))
                .Include<BlModels.Warehouse, SvcModels.Warehouse>()
                .Include<BlModels.Truck, SvcModels.Truck>()
                .Include<BlModels.Transferwarehouse, SvcModels.Transferwarehouse>();

            CreateMap<SvcModels.Truck, BlModels.Truck>()
                .ForMember(destMemb => destMemb.RegionGeometry,
                    destMemb =>
                        destMemb.ConvertUsing<GeoJsonConverter, string>(p => p.RegionGeoJson));

            CreateMap<BlModels.Truck, SvcModels.Truck>()
                .ForMember(destMemb => destMemb.RegionGeoJson,
                    destMemb => destMemb.ConvertUsing<GeoJsonConverter, Geometry>(p => p.RegionGeometry))
                .ForMember(destMemb => destMemb.HopType, destMemb => destMemb.MapFrom(src => "Truck"));

            CreateMap<SvcModels.Transferwarehouse, BlModels.Transferwarehouse>()
                .ForMember(destMemb => destMemb.RegionGeometry,
                    destMemb =>
                        destMemb.ConvertUsing<GeoJsonConverter, string>(p => p.RegionGeoJson));

            CreateMap<BlModels.Transferwarehouse, SvcModels.Transferwarehouse>()
                .ForMember(destMemb => destMemb.RegionGeoJson,
                    destMemb => destMemb.ConvertUsing<GeoJsonConverter, Geometry>(p => p.RegionGeometry))
                .ForMember(destMemb => destMemb.HopType, destMemb => destMemb.MapFrom(src => "TransferWarehouse"));

            CreateMap<BlModels.Warehouse, SvcModels.Warehouse>()
                .ForMember(destMemb => destMemb.HopType, destMemb => destMemb.MapFrom(src => "Warehouse"));

            CreateMap<SvcModels.Warehouse, BlModels.Warehouse>().ReverseMap();

            CreateMap<SvcModels.WarehouseNextHops, BlModels.WarehouseNextHops>().ReverseMap();

            CreateMap<SvcModels.HopArrival, BlModels.HopArrival>().ReverseMap();

            CreateMap<BlModels.Parcel, SvcModels.TrackingInformation>();

            CreateMap<BlModels.HopArrival, SvcModels.HopArrival>()
                .ForMember(dest => dest.DateTime,
                    dest => dest.MapFrom(src => src.HopArrivalTime));

            CreateMap<BlModels.Parcel, SvcModels.NewParcelInfo>();

            CreateMap<SvcModels.Parcel, BlModels.Parcel>()
                .AfterMap((_, parcel) =>
                {
                    parcel.VisitedHops ??= new List<BlModels.HopArrival>();
                    parcel.FutureHops ??= new List<BlModels.HopArrival>();
                });

            CreateMap<BlModels.Parcel, SvcModels.Parcel>();
            
            CreateMap<SvcModels.Recipient, BlModels.Recipient>().ReverseMap();
        }
    }
}