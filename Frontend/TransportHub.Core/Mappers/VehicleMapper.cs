using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Api.Dtos;
using TransportHub.Core.Models;

namespace TransportHub.Core.Mappers;

[Mapper(UseDeepCloning = true)]
public static partial class VehicleMapper
{
    public static VehicleModel ToVehicleModel(this VehicleDto dto)
    {
        var model = dto.ToVehicleModelInternal();
        model.IsDirty = false;
        return model;
    }
    private static partial VehicleModel ToVehicleModelInternal(this VehicleDto dto);

    public static IEnumerable<VehicleModel> ToVehicleModels(this IEnumerable<VehicleDto> Vehicles)
    {
        var models = Vehicles.ToVehicleModelsInternal();
        foreach (var Vehicle in models)
        {
            Vehicle.IsDirty = false;
        }
        return models;
    }
    private static partial IEnumerable<VehicleModel> ToVehicleModelsInternal(this IEnumerable<VehicleDto> Vehicles);
    public static VehicleDto ToVehicleDto(this VehicleModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name,
            Picture = model.Picture!,
            VehicleType = model.VehicleType,
            RequiredLicense = model.RequiredLicense,
            RegistrationNumber = model.RegistrationNumber!,
            Vin = model.Vin!,
            Disabled = model.Disabled,
        };
    }


    public static VehicleUpdateDto ToVehicleAddDto(this VehicleModel model)
    {
        return new()
        {
            Name = model.Name,
            Picture = model.Picture!,
            VehicleType = model.VehicleType,
            RequiredLicense = model.RequiredLicense,
            RegistrationNumber = model.RegistrationNumber!,
            Vin = model.Vin!,
            Disabled = model.Disabled,
        };
    }
}
