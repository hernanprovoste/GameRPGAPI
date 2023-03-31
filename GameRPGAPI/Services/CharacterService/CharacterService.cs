using AutoMapper;
using GameRPGAPI.Data;
using GameRPGAPI.Models;
using System.Security.Claims;

namespace GameRPGAPI.Services.CharacterService
{
    public class CharacterService : ICharacterService { 

        private static List<Character> characters = new List<Character>
        {
            new Character(),
            new Character {Name = "Sam"}
        };
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;

        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public IHttpContextAccessor HttpContextAccessor { get; }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddNewCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
   
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Characters.Where(c => c.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());

                if (character is null) throw new Exception($"Character with id '{id}' not found.");

                _context.Characters.Remove(character);

                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters.Where(c => c.User!.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.Include(c => c.Weapon).Include(c => c.Skills).Where(c => c.User!.Id == GetUserId()).ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters.Include(c => c.Weapon).Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try {
                var character = await _context.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);

                if (character is null || character.User!.Id != GetUserId()) throw new Exception($"Character with id '{updateCharacter.Id}' not found.");

                _mapper.Map(updateCharacter, character);

                character.Name = updateCharacter.Name;
                character.HitPoints = updateCharacter.HitPoints;
                character.Strength = updateCharacter.Strength;
                character.Defence = updateCharacter.Defence;
                character.Intelligence = updateCharacter.Intelligence;
                character.Class = updateCharacter.Class;

                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            } catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharracterSkill)
        {
            var response = new ServiceResponse<GetCharacterDto>();

            try
            {
                var character = await _context.Characters.Include(c => c.Weapon).Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == newCharracterSkill.CharacterId && c.User!.Id == GetUserId());

                if (character is null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharracterSkill.SkillId);

                if (skill is null)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                character.Skills!.Add(skill);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
