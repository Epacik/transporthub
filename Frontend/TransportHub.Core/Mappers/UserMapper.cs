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
public static partial class UserMapper
{
    public static UserModel ToUserModel(this UserDto dto)
    {
        var model = dto.ToUserModelInternal();
        model.IsDirty = false;
        return model;
    }
    private static partial UserModel ToUserModelInternal(this UserDto dto);

    public static IEnumerable<UserModel> ToUserModels(this IEnumerable<UserDto> users)
    {
        var models = users.ToUserModelsInternal();
        foreach (var user in models)
        {
            user.IsDirty = false;
        }
        return models;
    }
    private static partial IEnumerable<UserModel> ToUserModelsInternal(this IEnumerable<UserDto> users);
    public static UserDto ToUserDto(this UserModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name,
            Picture = model.Picture,
            PasswordExpirationDate = model.PasswordExpirationDate,
            UserType = model.UserType,
            MultiLogin = model.MultiLogin,
            Disabled = model.Disabled,
        };
    }

    public static UserAdminUpdateDto ToUserAdminUpdateDto(this UserModel model)
    {
        return new()
        {
            Name = model.Name,
            Picture = model.Picture,
            PasswordExpirationDate = model.PasswordExpirationDate,
            UserType = model.UserType,
            MultiLogin = model.MultiLogin,
            Disabled = model.Disabled,
        };
    }

    public static UserAddDto ToUserAddDto(this UserModel model)
    {
        return new()
        {
            Name = model.Name,
            Picture = model.Picture,
            PasswordExpirationDate = model.PasswordExpirationDate,
            UserType = model.UserType,
            MultiLogin = model.MultiLogin,
        };
    }

    public static UserUpdateDto ToUserUpdateDto(this UserModel model)
    {
        return new()
        {
            Name = model.Name,
            Picture = model.Picture,
        };
    }
}
