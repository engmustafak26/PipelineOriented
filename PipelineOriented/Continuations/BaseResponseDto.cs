using Newtonsoft.Json;
using System.Collections.Generic;

namespace PipelineOriented.Continuations
{

    public class BaseResponseDto<T>
    {
        [JsonProperty]
        public bool IsSuccess { get; private set; }

        [JsonProperty]
        public int ReturnCode { get; private set; }

        [JsonProperty]
        public T Data { get; private set; }

        [JsonProperty]
        public string Message { get; private set; }

        [JsonProperty]
        public string RequestCorrelationId { get; private set; }

        [JsonProperty]
        public List<string> Errors { get; private set; }

        public BaseResponseDto()
        {
            SetSuccess(default, 0);
        }

        private void SetSuccess(T data, int returnCode, string message = default)
        {
            this.IsSuccess = true;
            this.ReturnCode = returnCode;
            this.Message = message;
            this.Data = data;
        }

        private void SetError(string message, int returnCode, List<string> errors = null)
        {
            this.IsSuccess = false;
            this.Message = message;
            this.ReturnCode = returnCode;
            this.Errors = errors;
            this.Data = default;
        }

        private void SetError(T data, string message = null, int returnCode = 1, List<string> errors = null)
        {
            this.IsSuccess = false;
            this.Message = message;
            this.ReturnCode = returnCode;
            this.Errors = errors;
            this.Data = data;
        }


        public static BaseResponseDto<TModel> Success<TModel>(TModel data, string message = default)
        {
            var response = new BaseResponseDto<TModel>();
            response.SetSuccess(data, default, message);
            return response;
        }

        public static BaseResponseDto<TModel> Error<TModel>(TModel data, string message = default, int returnCode = 1)
        {
            var response = new BaseResponseDto<TModel>();
            response.SetError(data, message, returnCode);
            return response;
        }


        public static BaseResponseDto<T> Error(string message, int returnCode = 1)
        {
            var response = new BaseResponseDto<T>();
            response.SetError(message, returnCode, new List<string>() { message });
            return response;
        }


    }

    public class BaseResponseDto<TGlobal, TLocal>
    {
        [JsonProperty]
        public BaseResponseDto<TLocal> InterMethodsResult { get; private set; }

        [JsonProperty]
        public bool IsExecutionTerminated { get; private set; }

        [JsonProperty]
        public bool? IsSuccess { get; private set; }

        [JsonProperty]
        public int ReturnCode { get; private set; }

        [JsonProperty]
        public TGlobal Data { get; private set; }

        [JsonProperty]
        public string Message { get; private set; }

        [JsonProperty]
        public string RequestCorrelationId { get; private set; }

        [JsonProperty]
        public List<string> Errors { get; private set; }


        public BaseResponseDto(TGlobal data)
        {

        }
        public BaseResponseDto()
        {

        }

        public void SetExecutionTerminationFlag()
        {
            IsExecutionTerminated = true;
        }


        public BaseResponseDto<TGlobal, TModel> InternalSuccess<TModel>(TModel data, string message = default)
        {
            var response = new BaseResponseDto<TGlobal, TModel>();
            response.IsSuccess = this.IsSuccess;
            response.Data = this.Data;
            response.Message = this.Message;
            response.RequestCorrelationId = this.RequestCorrelationId;
            response.Errors = this.Errors;
            response.ReturnCode = this.ReturnCode;
            response.InterMethodsResult = BaseResponseDto<TModel>.Success(data, message);
            return response;
        }

        public BaseResponseDto<TGlobal, TModel> InternalError<TModel>(TModel data, string message = default, int returnCode = 1)
        {
            var response = new BaseResponseDto<TGlobal, TModel>();
            response.IsSuccess = this.IsSuccess;
            response.Data = this.Data;
            response.Message = this.Message;
            response.RequestCorrelationId = this.RequestCorrelationId;
            response.Errors = this.Errors;
            response.ReturnCode = this.ReturnCode;
            response.InterMethodsResult = BaseResponseDto<TModel>.Error(data, message, returnCode);
            return response;
        }

        public BaseResponseDto<TGlobal, TModel> WithNewInterMethodResult<TModel>(TModel data = default)
        {
            var response = new BaseResponseDto<TGlobal, TModel>();
            response.IsSuccess = this.IsSuccess;
            response.Data = this.Data;
            response.Message = this.Message;
            response.RequestCorrelationId = this.RequestCorrelationId;
            response.Errors = this.Errors;
            response.ReturnCode = this.ReturnCode;
            response.InterMethodsResult = BaseResponseDto<TModel>.Success(data);
            return response;
        }

        public BaseResponseDto<TGlobal, TLocal> Error(string message = null, int returnCode = 1)
        {
            this.SetError(message, returnCode);
            return this;
        }
        public BaseResponseDto<TGlobal, TModel> Error<TModel>(TModel data, string message = null, int returnCode = 1)
        {
            var response = new BaseResponseDto<TGlobal, TModel>();
            response.IsSuccess = false;
            response.Data = this.Data;
            response.Message = message;
            response.RequestCorrelationId = this.RequestCorrelationId;
            response.Errors = this.Errors;
            response.ReturnCode = returnCode;
            response.InterMethodsResult = BaseResponseDto<TModel>.Success(data);
            return response; ;
        }

        public BaseResponseDto<TGlobal, TLocal> Success(TGlobal data, int returnCode = 0, string message = default)
        {
            this.SetSuccess(data, returnCode, message);
            return this;
        }


        public BaseResponseDto<TGlobal> ToFinal()
        {
            if (IsSuccess == true)
            {
                return BaseResponseDto<TGlobal>.Success(this.Data, this.Message);
            }

            return BaseResponseDto<TGlobal>.Error(Message, ReturnCode);

        }


        private void SetSuccess(TGlobal data, int returnCode = default, string message = default)
        {
            this.IsSuccess = true;
            this.ReturnCode = returnCode;
            this.Message = message;
            this.Data = data;
            this.InterMethodsResult = InterMethodsResult ?? new BaseResponseDto<TLocal>();
        }

        private void SetError(string message, int returnCode, List<string> errors = null)
        {
            this.IsSuccess = false;
            this.Message = message;
            this.ReturnCode = returnCode;
            this.Errors = errors;
        }

        private void SetError(TGlobal data, string message = null, int returnCode = 1, List<string> errors = null)
        {
            this.IsSuccess = false;
            this.Message = message;
            this.ReturnCode = returnCode;
            this.Errors = errors;
            this.Data = data;
        }

    }




}
