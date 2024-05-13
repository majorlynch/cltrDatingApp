using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userRepository.GetUsersAsync();

        var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
        
        return Ok(usersToReturn);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        //return await _userRepository.GetMemberAsync(username);
        return Ok(await _userRepository.GetUserByUsernameAsync(username));
    }
}
