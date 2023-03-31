using GameRPGAPI.DTOs.Weapon;
using GameRPGAPI.Models;

namespace GameRPGAPI.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}
