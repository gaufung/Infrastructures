﻿using Aiursoft.Developer.SDK.Models.ApiAddressModels;
using Aiursoft.Developer.SDK.Models.ApiViewModels;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.SDKTools.Attributes;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Developer.SDK.Services.ToDeveloperServer
{
    public class DeveloperApiService : IScopedDependency
    {
        private readonly DeveloperLocator _serviceLocation;
        private readonly HTTPService _http;
        private readonly AiurCache _cache;

        public DeveloperApiService(
            DeveloperLocator serviceLocation,
            HTTPService http,
            AiurCache cache)
        {
            _serviceLocation = serviceLocation;
            _http = http;
            _cache = cache;
        }

        public virtual Task<bool> IsValidAppAsync(string appId, string appSecret)
        {
            if (!new IsGuidOrEmpty().IsValid(appId))
            {
                return Task.FromResult(false);
            }
            if (!new IsGuidOrEmpty().IsValid(appSecret))
            {
                return Task.FromResult(false);
            }
            return _cache.GetAndCache($"ValidAppWithId-{appId}-Secret-{appSecret}", () => IsValidAppWithoutCacheAsync(appId, appSecret));
        }

        public Task<AppInfoViewModel> AppInfoAsync(string appId)
        {
            if (!new IsGuidOrEmpty().IsValid(appId))
            {
                throw new AiurAPIModelException(ErrorType.NotFound, "Invalid app Id!");
            }
            return _cache.GetAndCache($"app-info-cache-{appId}", () => AppInfoWithoutCacheAsync(appId));
        }

        private async Task<bool> IsValidAppWithoutCacheAsync(string appId, string appSecret)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "api", "IsValidApp", new IsValidateAppAddressModel
            {
                AppId = appId,
                AppSecret = appSecret
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            return jResult.Code == ErrorType.Success;
        }

        private async Task<AppInfoViewModel> AppInfoWithoutCacheAsync(string appId)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "api", "AppInfo", new AppInfoAddressModel
            {
                AppId = appId
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AppInfoViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexpectedResponse(jResult);
            return jResult;
        }
    }
}
