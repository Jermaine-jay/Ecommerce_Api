using Ecommerce.Services.Configurations.Cache.CacheServices;

namespace Ecommerce.Services.Configurations.Cache.Security
{
    public class LoginAttempt : ILoginAttempt
    {
        private readonly ICacheService _cacheService;


        public LoginAttempt(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<string> LoginAttemptAsync(string userId)
        {
            string cacheKey = CacheKeySelector.AccountLockoutCacheKey(userId);
            var numberofattempts = await _cacheService.ReadFromCache<AttemptDto>(cacheKey);

            if (numberofattempts != null)
                numberofattempts.Attempts = numberofattempts.Attempts;
            else
                numberofattempts = new AttemptDto();

            await _cacheService.WriteToCache(cacheKey, numberofattempts, null, TimeSpan.FromDays(365));
            return cacheKey;
        }

        public async Task<AttemptDto> CheckLoginAttemptAsync(string userId)
        {
            string cacheKey = CacheKeySelector.AccountLockoutCacheKey(userId);
            AttemptDto? numberofattempts = await _cacheService.ReadFromCache<AttemptDto>(cacheKey);

            return numberofattempts;
        }

        public async Task ResetLoginAttemptAsync(string userId)
        {
            string cacheKey = CacheKeySelector.AccountLockoutCacheKey(userId);
            await _cacheService.ClearFromCache(cacheKey);
        }
    }

    public class AttemptDto
    {
        public int Attempts { get; set; }

        public AttemptDto()
        {
            Attempts = 0;
        }
    }
}
