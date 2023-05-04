using AuthServer.RequestModels;
using AuthServer.ResponseModels;
using System.Net;
using Newtonsoft.Json;

namespace AuthServer.Services
{
    public class PictureUploadService
    {
        private readonly IConfiguration _configuration;

        public PictureUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadImage(PictureUpload pic)
        {
            const string imgExpiration = "15552000";

            try
			{
                using (var ms = new MemoryStream())
                {
                    await pic.files.CopyToAsync(ms);
                    var imageBytes = ms.ToArray();
                    string byteString = Convert.ToBase64String(imageBytes);
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://api.imgbb.com");
                        var content = new[]
                        {
                                new KeyValuePair<string, string>("expiration", imgExpiration),
                                new KeyValuePair<string, string>("key", _configuration["ImageCDN:APIKey"]),
                                new KeyValuePair<string, string>("image", byteString)
                            };

                        var encodedItems = content.Select(i => WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
                        var encodedContent = new StringContent(string.Join("&", encodedItems), null, "application/x-www-form-urlencoded");

                        var result = await client.PostAsync("/1/upload", encodedContent);
                        var response = await result.Content.ReadAsStreamAsync();

                        using var sr = new StreamReader(response);
                        using var jr = new JsonTextReader(sr);
                        JsonSerializer serializer = new JsonSerializer();

                        dynamic jsonResponse = serializer.Deserialize(jr);
                        var imageUrlDynamic = jsonResponse.data.url;
                        var imageUrlString = Convert.ToString(imageUrlDynamic);

                        return imageUrlString;
                    }
                }
            }
			catch (JsonReaderException jsonEx)
			{
                throw jsonEx;
            }
        }
    }
}
