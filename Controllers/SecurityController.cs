using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using PassCodeManager.DTO.RequestObjects;
using PassCodeManager.Services.Abstract;

namespace PassCodeManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpPost("AddPasscode")]
        public async Task<ApiResponse> AddPassCode([FromBody]AddPasscodeObject passcodeObject)
        {
            try
            {
                var result = await _securityService.AddPasscode(passcodeObject);
                return new ApiResponse("Passcode Saved Successfully!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpGet("GetPasscode/{Mobile}")]
        public async Task<ApiResponse> GetPassCodesByMobile(string Mobile)
        {
            try
            {
                var result = await _securityService.GetPassCodesByMobile(Mobile);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpPut("UpdatePasscode")]
        public async Task<ApiResponse> UpdatePassCode([FromBody]UpdatePasscodeObject request)
        {
            try
            {
                var result = await _securityService.UpdatePassCode(request);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}