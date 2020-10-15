using System.ComponentModel;
using AutoMapper;
using GeoCoordinate = GeoCoordinatePortable.GeoCoordinate;
using BlModels = TechnikumDirekt.BusinessLogic.Models;
using SvcModels = TechnikumDirekt.Services.Models;

namespace TechnikumDirekt.Services.Mapper
{
    public class BlMapperProfile : Profile
    {
        public BlMapperProfile()
        {
            CreateMap<SvcModels.Hop, BlModels.Hop>()
                .ForMember(destMemb => destMemb.LocationCoordinates,
                    destMemb =>
                        destMemb.ConvertUsing<GeoCoordinateConverter, SvcModels.GeoCoordinate>(p => p.LocationCoordinates))
                .Include<SvcModels.Warehouse, BlModels.Warehouse>()
                .Include<SvcModels.Truck, BlModels.Truck>()
                .Include<SvcModels.Transferwarehouse, BlModels.Transferwarehouse>();

            CreateMap<BlModels.Hop, SvcModels.Hop>()
                .ForMember(destMemb => destMemb.LocationCoordinates,
                    destMemb =>
                        destMemb.ConvertUsing<GeoCoordinateConverter, GeoCoordinate>(p => p.LocationCoordinates))
                .Include<BlModels.Warehouse, SvcModels.Warehouse>()
                .Include<BlModels.Truck, SvcModels.Truck>()
                .Include<BlModels.Transferwarehouse, SvcModels.Transferwarehouse>();
            
            CreateMap<SvcModels.Truck, BlModels.Truck>()
                .ForMember(destMemb => destMemb.RegionGeometry,
                    destMemb =>
                        destMemb.ConvertUsing<GeoJsonConverter, string>(p => p.RegionGeoJson));
            
            CreateMap<BlModels.Truck, SvcModels.Truck>()
                .ForMember(destMemb => destMemb.RegionGeoJson, destMemb => destMemb.ConvertUsing<GeoJsonConverter, GeoCoordinate>(p => p.RegionGeometry))
                .ForMember(destMemb => destMemb.HopType, destMemb => destMemb.MapFrom(src => "Truck")); //TODO !!!!!!!!!!!
            
            CreateMap<SvcModels.Transferwarehouse, BlModels.Transferwarehouse>()
                .ForMember(destMemb => destMemb.RegionGeometry,
                    destMemb =>
                        destMemb.ConvertUsing<GeoJsonConverter, string>(p => p.RegionGeoJson));
            
            CreateMap<BlModels.Transferwarehouse, SvcModels.Transferwarehouse>()
                .ForMember(destMemb => destMemb.RegionGeoJson, destMemb => destMemb.ConvertUsing<GeoJsonConverter, GeoCoordinate>(p => p.RegionGeometry))
                .ForMember(destMemb => destMemb.HopType, destMemb => destMemb.MapFrom(src => "Transferwarehouse")); //TODO !!!!!!!!!!!

            CreateMap<BlModels.Warehouse, SvcModels.Warehouse>()
                .ForMember(destMemb => destMemb.HopType, destMemb => destMemb.MapFrom(src => "Warehouse")); //TODO !!!!!!!!!!!
            
            CreateMap<SvcModels.Warehouse,BlModels.Warehouse>().ReverseMap();
            
            CreateMap<SvcModels.WarehouseNextHops,BlModels.WarehouseNextHops>().ReverseMap();
            
            CreateMap<SvcModels.HopArrival, BlModels.HopArrival>().ReverseMap();

            CreateMap<BlModels.Parcel, SvcModels.TrackingInformation>();

            CreateMap<BlModels.Parcel, SvcModels.NewParcelInfo>();
            
            CreateMap<SvcModels.Parcel, BlModels.Parcel>();

            CreateMap<SvcModels.Recipient, BlModels.Recipient>().ReverseMap();
        }
    }
}