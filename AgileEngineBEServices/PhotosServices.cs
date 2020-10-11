using AgileEngineBEServices.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AgileEngineBEServices
{
    public class PhotosServices
    {
        public PhotosServices() { }

        public string GetToken(string apikey)
        {
            using (var httpClient = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(new AuthDto { apiKey = apikey});
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var response = httpClient.PostAsync("http://interview.agileengine.com/auth", byteContent).Result)
                {
                    string apiResponse = response.Content.ReadAsStringAsync().Result;
                    var tokenDto = JsonConvert.DeserializeObject<TokenDto>(apiResponse);
                    return tokenDto.token;
                }
            }

        }

        public List<PictureDto> GetImages(int page, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = httpClient.GetAsync("http://interview.agileengine.com/images?page=" + page).Result)
                {
                    string apiResponse = response.Content.ReadAsStringAsync().Result;
                    var picturesDto = JsonConvert.DeserializeObject<PicturesDto>(apiResponse);
                    return picturesDto.pictures;
                }
            }

        }

        public PictureDetailsDto GetImageDetail(string id, string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = httpClient.GetAsync("http://interview.agileengine.com/images/" + id).Result)
                {
                    string apiResponse = response.Content.ReadAsStringAsync().Result;
                    var pictureDetailDto = JsonConvert.DeserializeObject<PictureDetailsDto>(apiResponse);
                    return pictureDetailDto;
                }
            }

        }
    }
}
