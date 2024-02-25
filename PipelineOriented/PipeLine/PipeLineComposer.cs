using PipelineOriented.Continuations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineOriented.PipeLine
{
    public static class PipeLineComposer
    {
        public static async Task<BaseResponseDto<TGlobalIn, TOut>> Handle<TGlobalIn, TLocalIn, TOut>(this Task<BaseResponseDto<TGlobalIn, TLocalIn>> input, string name, Func<BaseResponseDto<TGlobalIn, TLocalIn>, Task<BaseResponseDto<TGlobalIn, TOut>>> func)
        {
            var result = await input;
            if (result.IsSuccess.HasValue)
            {
                result.SetExecutionTerminationFlag();
                return result.WithNewInterMethodResult(default(TOut));
            }
            return await func(await input);
        }

        public static async Task<BaseResponseDto<TGlobalIn, TOut>> Handle<TGlobalIn, TLocalIn, TOut>(this Task<BaseResponseDto<TGlobalIn, TLocalIn>> input, string name, bool toggleOff, Func<BaseResponseDto<TGlobalIn, TLocalIn>, Task<BaseResponseDto<TGlobalIn, TOut>>> func)
        {
            var result = await input;

            if (toggleOff)
            {
                if (result.InterMethodsResult != null && result.InterMethodsResult.Data is TOut internalResult)
                {
                    return result.WithNewInterMethodResult(internalResult);
                }
                else
                {
                    return result.WithNewInterMethodResult(default(TOut));
                }
            }

            if (result.IsSuccess.HasValue)
            {
                result.SetExecutionTerminationFlag();
                return result.WithNewInterMethodResult(default(TOut));
            }
            return await func(await input);
        }
        public static async Task<BaseResponseDto<TGlobalIn>> End<TGlobalIn, TLocalIn>(this Task<BaseResponseDto<TGlobalIn, TLocalIn>> input, string name, Func<BaseResponseDto<TGlobalIn, TLocalIn>, Task> func)
        {
            var result = await input;
            await func(result);
            return result.ToFinal();

        }

        public static async Task<BaseResponseDto<TIn, TIn>> Start<TIn>(this BaseResponseDto<TIn> input, string name = null)
        {
            return new BaseResponseDto<TIn, TIn>(input.Data);
        }
    }
}
