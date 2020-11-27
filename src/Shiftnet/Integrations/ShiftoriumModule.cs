using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Shiftnet.Shiftorium;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Debugging;
using AlkalineThunder.Pandemic.Settings;
using Newtonsoft.Json;

namespace Shiftnet.Integrations
{
    [RequiresModule(typeof(SettingsService))]
    public class ShiftoriumModule : EngineModule
    {
        public const string ApiBaseEnvironmentVariable = "SHIFTNET_SHIFTORIUM_API_BASE";
        public const string AuthTokenSetting = "shiftorium.authToken";
        public const int SkinsPerPage = 30;

        private List<Skin> _skins = new List<Skin>();
        private int _skinInfoPage = 0;
        private string _token;
        private string _apiBaseUrl;
        private User _user;

        public User User => _user;
        public bool LoggedIn => User != null;
        
        private SettingsService Settings
            => GetModule<SettingsService>();
        
        protected override void OnInitialize()
        {
            App.Logger.Info("Reading shiftorium API base from " + ApiBaseEnvironmentVariable);
            if (Environment.GetEnvironmentVariables().Contains(ApiBaseEnvironmentVariable))
            {
                _apiBaseUrl = Environment.GetEnvironmentVariable(ApiBaseEnvironmentVariable);
            }
            else
            {
                App.Logger.Warn("Environment variable not found.");
                _apiBaseUrl = "https://shiftnet.mvanoverbeek.me/shiftorium";
            }

            if (!_apiBaseUrl.EndsWith("/"))
                _apiBaseUrl += "/";
            _apiBaseUrl += "api";
            
            App.Logger.Info($"Using {_apiBaseUrl}.");

            DetectLoginStatus();
        }

        protected override void OnLoadContent()
        {
            DownloadSkinInformation();
            
            base.OnLoadContent();
        }

        private void DownloadSkinInformation()
        {
            App.Logger.Log("Downloading skin information from the Shiftorium...");

            var response = Get($"/skin/page/{_skinInfoPage}/{SkinsPerPage}");
            
            foreach (var elem in response.Payload.EnumerateArray())
            {
                _skins.Add(new Skin(elem));
            }
        }
        
        private string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new InvalidOperationException("path must not be null.");
            
            var sb = new StringBuilder();
            sb.Append(_apiBaseUrl);
            if (!path.StartsWith("/"))
                sb.Append("/");
            sb.Append(path);
            return sb.ToString();
        }

        private void TraceRequest(WebRequest request)
        {
            App.Logger.Trace($"HTTP {request.Method} {request.RequestUri}");
            foreach (string header in request.Headers)
            {
                var value = request.Headers[header];
                if (!string.IsNullOrWhiteSpace(_token))
                    value = value.Replace(_token, "xxx");
                App.Logger.Trace($"{header}: {value}");
            }
        }

        private void TraceResponse(HttpWebResponse response)
        {
            App.Logger.Trace($"HTTP {(int)response.StatusCode} {response.StatusDescription}");
            foreach (string header in response.Headers)
            {
                var value = response.Headers[header];
                if (!string.IsNullOrWhiteSpace(_token))
                    value = value.Replace(_token, "xxx");
                App.Logger.Trace($"{header}: {value}");
            }
        }
        
        private ShiftoriumResponse Get(string url)
        {
            try
            {
                var request = WebRequest.CreateHttp(GetFullPath(url));
                request.Headers.Add(HttpRequestHeader.Accept, "application/json");
                
                if (LoggedIn)
                {
                    request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {_token}");
                }

                request.Method = WebRequestMethods.Http.Get;

                TraceRequest(request);
                
                using var response = (HttpWebResponse) request.GetResponse();

                TraceResponse(response);
                
                var json = ReadResponse(response);

                var shiftoriumResponse = new ShiftoriumResponse(json, 200);
                return shiftoriumResponse;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    var json = ReadResponse(wex.Response);
                    var shiftoriumResponse =
                        new ShiftoriumResponse(json, (int) ((HttpWebResponse) wex.Response).StatusCode);

                    throw new ShiftoriumException(shiftoriumResponse, wex);
                }
                else
                {
                    throw new ShiftoriumException(null, wex);
                }
            }
            catch (Exception ex)
            {
                throw new ShiftoriumException(null, ex);
            }
        }

        private PropertySet ReadResponse(WebResponse response)
        {
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);

            var json = reader.ReadToEnd();

            using var doc = JsonDocument.Parse(json);
            var element = doc.RootElement;

            if (element.ValueKind != JsonValueKind.Object)
                throw new FormatException("API responded with malformed data.");

            var errorsProp = element.GetProperty("errors");
            var resultProperty = element.GetProperty("result").Clone();

            var errors = Array.Empty<string>();

            if (errorsProp.ValueKind == JsonValueKind.Array)
            {
                errors = new string[errorsProp.GetArrayLength()];
                var i = 0;
                foreach (var error in errorsProp.EnumerateArray())
                {
                    errors[i] = error.GetString();
                    i++;
                }
            }

            var properties = new PropertySet();

            properties.SetValue("errors", errors);
            properties.SetValue("result", resultProperty);
            
            return properties;
        }
        
        private void DetectLoginStatus()
        {
            App.Logger.Log("Detecting shiftorium login status...");

            _token = Settings.GetValue<string>(AuthTokenSetting, null);
            
            if (!string.IsNullOrWhiteSpace(_token))
            {
                try
                {
                    var user = Get("/auth/user");
                }
                catch (Exception ex)
                {
                    App.Logger.Error(ex.ToString());
                    _user = null;
                    _token = null;
                    Settings.SetValue(AuthTokenSetting, "");
                }
            }
            else
            {
                _token = null;
                _user = null;
            }
        }
    }
}