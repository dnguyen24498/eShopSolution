namespace eShopSolution.ViewModels.Common
{
    public class ApiSuccessResult<T>:ApiResult<T>
    {
        public ApiSuccessResult(T resultObj)
        {
            IsSuccess = true;
            ResultObj = resultObj;
        }

        public ApiSuccessResult()
        {
            IsSuccess = true;
        }
    }
}