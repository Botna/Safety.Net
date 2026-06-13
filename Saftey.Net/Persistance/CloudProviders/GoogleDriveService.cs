using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

//https://dev.to/adiamante/net-maui-google-drive-oauth-on-windows-and-android-4lm4

namespace Safety.Net.Persistance.CloudProviders
{
    public class GoogleDriveService : ICloudProviderService
    {
        readonly string _androidClientId = "825838237223-gq0pdk3hdnvfjflbmef5u7sml44djn0m.apps.googleusercontent.com";  // Android client

        Oauth2Service? _oauth2Service;
        DriveService? _driveService;
        GoogleCredential? _credential;
        string? _email;

        //temp shitty counter
        private int counter = 0;

        private readonly string DriveFolderLocation = "Safety.net Recordings";

        public bool IsSignedIn => _credential != null;
        public string? Email => _email;

        #region Auth Methods
        public async Task Init()
        {
            var hasRefreshToken = await SecureStorage.GetAsync("refresh_token") is not null;
            if (!IsSignedIn && hasRefreshToken)
            {
                await SignIn();
            }
        }

        public async Task SignIn()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var expiresIn = Preferences.Get("access_token_epires_in", 0L);
            var isExpired = now - 10 > expiresIn;   // 10 second buffer
            var hasRefreshToken = await SecureStorage.GetAsync("refresh_token") is not null;

            if (isExpired && hasRefreshToken)
            {
                Debug.WriteLine("Using refresh token");
                await RefreshToken();
            }
            else if (isExpired)     // No refresh token
            {
                Debug.WriteLine("Starting auth code flow");
                if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                {
                    await DoAuthCodeFlowAndroid();
                }
                else
                {
                    throw new NotImplementedException($"Auth flow for platform {DeviceInfo.Current.Platform} not implemented.");
                }
            }

            var accesToken = await SecureStorage.GetAsync("access_token");
            _credential = GoogleCredential.FromAccessToken(accesToken);
            _oauth2Service = new Oauth2Service(new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
                ApplicationName = "yeetmedia3"
            });
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
                ApplicationName = "yeetmedia3"
            });
            var userInfo = await _oauth2Service.Userinfo.Get().ExecuteAsync();
            _email = userInfo.Email;
        }

        public async Task SignOut()
        {
            await RevokeTokens();
        }

        private async Task DoAuthCodeFlowAndroid()
        {
            var authUrl = "https://accounts.google.com/o/oauth2/v2/auth";
            var clientId = _androidClientId;
            var redirectUri = "com.dbstudios.safety.net://";  // requires a period: https://developers.google.com/identity/protocols/oauth2/native-app#android
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var parameters = GenerateAuthParameters(redirectUri, clientId, codeChallenge);
            var queryString = string.Join("&", parameters.Select(param => $"{param.Key}={param.Value}"));
            var fullAuthUrl = $"{authUrl}?{queryString}";


            //TODO move off of CustomURI Scheme whatever that means
#pragma warning disable CA1416
            var authCodeResponse = await WebAuthenticator.AuthenticateAsync(new Uri(fullAuthUrl), new Uri("com.dbstudios.safety.net://"));
#pragma warning restore CA1416
            var authorizationCode = authCodeResponse.Properties["code"];

            await GetInitialToken(authorizationCode, redirectUri, clientId, codeVerifier);
        }

        private static Dictionary<string, string> GenerateAuthParameters(string redirectUri, string clientId, string codeChallenge)
        {
            return new Dictionary<string, string>
        {
            //{ "scope", "https://www.googleapis.com/auth/drive https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/drive.appdata" },
            { "scope", string.Join(' ', [Oauth2Service.Scope.UserinfoProfile, Oauth2Service.Scope.UserinfoEmail, DriveService.Scope.Drive, DriveService.Scope.DriveFile, DriveService.Scope.DriveAppdata]) },
            { "access_type", "offline" },
            { "include_granted_scopes", "true" },
            { "response_type", "code" },
            //{ "state", "state_parameter_passthrough_value" },
            { "redirect_uri", redirectUri },
            { "client_id", clientId },
            { "code_challenge_method", "S256" },
            { "code_challenge", codeChallenge },
            //{ "prompt", "consent" }
        };
        }

        private static async Task GetInitialToken(string authorizationCode, string redirectUri, string clientId, string codeVerifier)
        {
            var tokenEndpoint = "https://oauth2.googleapis.com/token";
            var client = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
            {
                Content = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", authorizationCode),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("code_verifier", codeVerifier)
                ])
            };

            var response = await client.SendAsync(tokenRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) throw new Exception($"Error requesting token: {responseBody}");

            Debug.WriteLine($"Access token: {responseBody}");
            var jsonToken = JsonObject.Parse(responseBody);
            var accessToken = jsonToken!["access_token"]!.ToString();
            var refreshToken = jsonToken!["refresh_token"]!.ToString();
            var accessTokenExpiresIn = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + int.Parse(jsonToken!["expires_in"]!.ToString());
            await SecureStorage.SetAsync("access_token", accessToken);
            await SecureStorage.SetAsync("refresh_token", refreshToken);
            Preferences.Set("access_token_epires_in", accessTokenExpiresIn);
        }

        private async Task RefreshToken()
        {
            var clientId = _androidClientId;
            var tokenEndpoint = "https://oauth2.googleapis.com/token";
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            var client = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
            {
                Content = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken!)
                    ]
                )
            };

            var response = await client.SendAsync(tokenRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) throw new Exception($"Error requesting token: {responseBody}");

            Debug.WriteLine($"Refresh token: {responseBody}");
            var jsonToken = JsonObject.Parse(responseBody);
            var accessToken = jsonToken!["access_token"]!.ToString();
            var accessTokenExpiresIn = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + int.Parse(jsonToken!["expires_in"]!.ToString());
            await SecureStorage.SetAsync("access_token", accessToken);
            Preferences.Set("access_token_epires_in", accessTokenExpiresIn);
        }

        private async Task RevokeTokens()
        {
            var revokeEndpoint = "https://oauth2.googleapis.com/revoke";
            var access_token = await SecureStorage.GetAsync("access_token");
            var client = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, revokeEndpoint)
            {
                Content = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("token", access_token!),
                ]
                )
            };

            var response = await client.SendAsync(tokenRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) throw new Exception($"Error revoking token: {responseBody}");

            Debug.WriteLine($"Revoke token: {responseBody}");
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
            Preferences.Remove("access_token_epires_in");

            _credential = null;
            _oauth2Service = null;
            _driveService = null;
        }

        private static string GenerateCodeVerifier()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32]; // Length can vary, e.g., 43-128 characters
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
            return Convert.ToBase64String(hash)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        #endregion

        public async Task UploadBytesToDrive(byte[] data, string filename)
        {
            var folderId = await GetFolderId();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = $"{filename}_{counter}",
                // Optional: Specify a parent folder ID
                Parents = new List<string> { folderId }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new MemoryStream(data))
            {
                // service is your authorized DriveService instance
                request = _driveService.Files.Create(fileMetadata, stream, "video/mp4");
                request.Fields = "id";
                await request.UploadAsync();
            }
            counter++;
        }

        private async Task<string> GetFolderId()
        {
            string query = $"mimeType = 'application/vnd.google-apps.folder' and name = '{DriveFolderLocation}' and trashed = false";

            var listRequest = _driveService.Files.List();
            listRequest.Q = query;
            listRequest.Fields = "files(id, name)";
            listRequest.PageSize = 1; // We only need to know if at least one exists

            var result = await listRequest.ExecuteAsync();
            if (result.Files != null && result.Files.Count > 0)
            {
                return result.Files[0].Id; // Returns the ID of the first match
            }
            else
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = "Safety.Net Recordings",
                    MimeType = "application/vnd.google-apps.folder"
                };

                var createRequest = _driveService.Files.Create(fileMetadata);
                createRequest.Fields = "id";
                var file = createRequest.Execute();
                return file.Id;
            }
        }



        public Task UploadContentToCloud(Stream fileStream, string contentName)
        {
            throw new NotImplementedException();
        }

        public Task AppendContentToCloud(byte[] bytes, string contentName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAzureConnectionString()
        {
            throw new NotImplementedException();
        }

        public Task SetAzureConnectionString(string connstr)
        {
            throw new NotImplementedException();
        }
    }
}