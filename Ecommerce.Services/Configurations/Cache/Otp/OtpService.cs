using TaskManager.Services.Configurations.Cache.CacheServices;
using TaskManager.Services.Configurations.Cache.Security;

namespace TaskManager.Services.Configurations.Cache.Otp
{
    public class OtpService : IOtpService
    {
        private readonly ICacheService _cacheService;
        private TimeSpan OtpValidity = TimeSpan.FromMinutes(5);


        public OtpService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }


        public async Task<string> GenerateOtpAsync(string userId, OtpOperation operation)
        {
            string cacheKey = CacheKeySelector.OtpCodeCacheKey(userId, operation);
            OtpCodeDto? otpCode = await _cacheService.ReadFromCache<OtpCodeDto>(cacheKey);

            if (otpCode != null)
                otpCode.Otp = CacheKeySelector.GenerateToken();
            
            
            otpCode = new(CacheKeySelector.GenerateToken());
            
           await _cacheService.WriteToCache(cacheKey, otpCode, null, OtpValidity);
            return otpCode.Otp;
        }


        public async Task<bool> VerifyOtpAsync(string userId, string otp, OtpOperation operation)
        {
            string cacheKey = CacheKeySelector.OtpCodeCacheKey(userId, operation);
            OtpCodeDto? otpCode = await _cacheService.ReadFromCache<OtpCodeDto>(cacheKey);

            if (otpCode == null)
                return false;

            ++otpCode.Attempts;

            if (otpCode.Attempts >= 3)
            { 
                await _cacheService.ClearFromCache(cacheKey);
                return false;
            }

            if (otpCode.Otp != otp)
                return false;
           
            else return true;
        }


        public async Task<string> GenerateUniqueOtpAsync(string userId, OtpOperation operation)
        {
            string cacheKeyandvalue = CacheKeySelector.GenerateUniqueOtpCacheKey(userId, operation);
            OtpCodeDto? otpCode = await _cacheService.ReadFromCache<OtpCodeDto>(cacheKeyandvalue);

            if (otpCode != null)          
                otpCode.Otp = cacheKeyandvalue;
          
            otpCode = new(cacheKeyandvalue);           
            await _cacheService.WriteToCache(cacheKeyandvalue, otpCode, null, OtpValidity);
            return otpCode.Otp;
        }


        public async Task<bool> VerifyUniqueOtpAsync(string userId, string otp, OtpOperation operation)
        {
            string cacheKey = CacheKeySelector.GenerateUniqueOtpCacheKey(userId, operation);
            OtpCodeDto? otpCode = await _cacheService.ReadFromCache<OtpCodeDto>(cacheKey);

            if (otpCode == null)
                return false;
            
            if (otpCode.Otp != otp)
                return false;
            
            await _cacheService.ClearFromCache(cacheKey);
            return true;
        }
    }
}
