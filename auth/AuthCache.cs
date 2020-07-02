using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// todo: 持久化cache
namespace CloudBase
{
    class AuthCache
    {
        public readonly string AccessTokenKey;
        public readonly string AccessTokenExpireKey;
        public readonly string RefreshTokenKey;
        public readonly string AnonymousUuidKey;
        public readonly string LoginTypeKey;

        private Dictionary<string, string> MCache;

        public AuthCache(Core core)
        {
            string envId = core.Env;

            this.AccessTokenKey = $"{envId}_{AuthCacheKey.ACCESS_TOKEN}";
            this.AccessTokenExpireKey = $"{envId}_{AuthCacheKey.ACCESS_TOKEN_EXPIRE}";
            this.RefreshTokenKey = $"{envId}_{AuthCacheKey.REFRESH_TOKEN}";
            this.AnonymousUuidKey = $"{envId}_{AuthCacheKey.ANONYMOUS_UUID}";
            this.LoginTypeKey = $"{envId}_{AuthCacheKey.LOGIN_TYPE}";

            this.MCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public Task<string> GetStoreAsync(string key)
        {
            var val = GetStore(key);
            return Task.FromResult(val);
        }

        public Task SetStoreAsync(string key, string value)
        {
            SetStore(key, value);
            return Task.CompletedTask;
        }

        public Task RemoveStoreAsync(string key)
        {
            RemoveStore(key);
            return Task.CompletedTask;
        }

        public string GetStore(string key)
        {

            this.MCache.TryGetValue(key, out string value);
            return value;
        }

        public void SetStore(string key, string value)
        {
            this.MCache[key] = value;
        }

        public void RemoveStore(string key)
        {
            this.MCache.Remove(key);
        }
    }
}