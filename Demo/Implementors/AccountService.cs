using Demo.Abstractions;
using Demo.DTO.Requests;
using Demo.DTO.Responses;
using Demo.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PipelineOriented.Continuations;
using PipelineOriented.PipeLine;

namespace Demo.Implementors
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _dataContext;
        private readonly IMemoryCache _memoryCache;

        public AccountService(DataContext dataContext, IMemoryCache memoryCache)
        {
            _dataContext = dataContext;
            _memoryCache = memoryCache;
        }

        public async Task<BaseResponseDto<LoginResponse>> LoginAsync(BaseRequestDto<LoginRequest> request)
        {
            return await BaseResponseDto<LoginResponse>.Success(new LoginResponse())
                          .Start()                              
                               .Handle("check if user exist in db", async (result) =>
                               {
                                   var user = _dataContext.Users.Include(x => x.Role)
                                                          .FirstOrDefault(x => x.Email == request.Data.Email);
                                   if (user is null)
                                   {
                                       return result.Error(user, "Email Not exists");
                                   }
                                   return result.InternalSuccess(user);

                               })
                               .Handle("return if user is soft deleted", async (result) =>
                               {

                                   var model = result.InterMethodsResult.Data;
                                   if (model == null || model.IsDeleted)
                                   {
                                       return result.Error(model, "User is deleted");
                                   }
                                   return result.InternalSuccess(model);
                               })
                               .Handle("return if user has not any active role", async (result) =>
                                  {

                                      var model = result.InterMethodsResult.Data;
                                      if (!model.IsAdmin && ((model?.Role?.IsDeleted ?? true)))
                                      {
                                          return result.Error(model, "User has no access rights");
                                      }
                                      return result.InternalSuccess(model);
                                  })
                               .Handle("return user can login", async (result) =>
                                 {

                                     var model = result.InterMethodsResult.Data;

                                     return result.Success(new LoginResponse()
                                     {
                                         Name = model?.Name,
                                         Role = model?.Role?.Name,
                                     });

                                 })
                          .End("finish", async (result) =>
                          {
                             
                          });
        }
    }

}
