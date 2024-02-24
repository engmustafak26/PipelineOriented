using Demo.DTO.Requests;
using Demo.DTO.Responses;
using PipelineOriented.Continuations;

namespace Demo.Abstractions
{
    public interface IAccountService
    {
        public Task<BaseResponseDto<LoginResponse>> LoginAsync(BaseRequestDto<LoginRequest> request);
    }
}
