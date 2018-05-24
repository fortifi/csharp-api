using System.Net.Http.Headers;
using System.Diagnostics;
using System;

namespace FortifiAPI
{
    public partial class Client
    {
        private const string userAgent = "Fortifi/1.0 (CSharp API client)";
        private const string bearerTokenScheme = "Bearer ";
        private const string fortifiOrgHeader = "x-fortifi-org";
        private const string fortifiAuthHeader = "authorization";
        private const string fortifiUserAgentHeader = "user-agent";
        private const int fortifiExpiryBuffer = 14400;

        // Active token expiry epoch
        private long activeTokenExpiry = 0;
        private string user = "";
        private string key = "";

        // Active token currently in use by API
        public string activeToken { get; set; }

        // FID of Organisation that this client is for
        public string organisationFid { get; set; }

        public Client(String orgFid, String user, String key)
        {
            organisationFid = orgFid;
            this.user = user;
            this.key = key;
            _settings = new Lazy<Newtonsoft.Json.JsonSerializerSettings>(() =>
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                UpdateJsonSerializerSettings(settings);
                return settings;
            });
        }

        private void GetNewToken()
        {
            var payload = new ServiceAccountCredentialsPayload();
            payload.Id = user;
            payload.Key = key;
            var response = GetServiceAuthTokenAsync(payload).Result;
            activeTokenExpiry = Convert.ToInt64(response.Data.Expiry);
            activeToken = response.Data.Token;
        }

        private long TimeNowUnix()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            if ((TimeNowUnix() > (activeTokenExpiry - fortifiExpiryBuffer) || String.IsNullOrEmpty(this.activeToken)) && request.RequestUri.LocalPath != "/v1/svcauth/verify")
            {
                GetNewToken();
            }

            request.Headers.Add(fortifiOrgHeader, organisationFid);
            request.Headers.Add(fortifiAuthHeader, bearerTokenScheme + activeToken);
            request.Headers.Add(fortifiUserAgentHeader, userAgent);
        }
    }
}
