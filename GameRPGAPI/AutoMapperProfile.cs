using AutoMapper;
using GameRPGAPI.DTOs.Fight;
using GameRPGAPI.DTOs.Skill;
using GameRPGAPI.DTOs.Weapon;
using GameRPGAPI.Models;

namespace GameRPGAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<UpdateCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
            CreateMap<Character, HighscoreDto>();
        }
    }
}
