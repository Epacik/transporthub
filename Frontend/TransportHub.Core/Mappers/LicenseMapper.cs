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
public static partial class LicenseTypeMapper
{
    public static LicenseTypeModel ToLicenseTypeModel(this LicenseTypeDto dto)
    {
        var model = dto.ToLicenseTypeModelInternal();
        model.IsDirty = false;
        return model;
    }
    private static partial LicenseTypeModel ToLicenseTypeModelInternal(this LicenseTypeDto dto);

    public static IEnumerable<LicenseTypeModel> ToLicenseTypeModels(this IEnumerable<LicenseTypeDto> LicenseTypes)
    {
        var models = LicenseTypes.ToLicenseTypeModelsInternal();
        foreach (var LicenseType in models)
        {
            LicenseType.IsDirty = false;
        }
        return models;
    }
    private static partial IEnumerable<LicenseTypeModel> ToLicenseTypeModelsInternal(this IEnumerable<LicenseTypeDto> LicenseTypes);
    public static LicenseTypeDto ToLicenseTypeDto(this LicenseTypeModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            MinimalAgeOfHolder = model.MinimalAgeOfHolder,
            AlternativeMinimalAgeOfHolder = model.AlternativeMinimalAgeOfHolder,
            ConditionForAlternativeMinimalAge = model.ConditionForAlternativeMinimalAge,
            Disabled = model.Disabled,
        };
    }


    public static LicenseTypeUpdateDto ToLicenseTypeAddDto(this LicenseTypeModel model)
    {
        return new()
        {
            Name = model.Name,
            Description = model.Description,
            MinimalAgeOfHolder = model.MinimalAgeOfHolder,
            AlternativeMinimalAgeOfHolder = model.AlternativeMinimalAgeOfHolder,
            ConditionForAlternativeMinimalAge = model.ConditionForAlternativeMinimalAge,
            Disabled = model.Disabled,
        };
    }
}
