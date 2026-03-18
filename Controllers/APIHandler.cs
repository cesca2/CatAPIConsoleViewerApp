using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CatAPIConsoleViewerApp.Controllers;

public class APIHandler: BaseController
{
    private readonly HttpClient _httpClient;
    private readonly string? RequestType;

    public APIHandler(string url)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(  
        new MediaTypeWithQualityHeaderValue("application/json"));

        _httpClient.BaseAddress = new Uri(url); 

        if (url.Contains("images")){
            RequestType="Image";
        }
        else if (url.Contains("breeds")){
            RequestType = "Breed";
        }
     
    }

    public async Task<byte[]> RetrieveImageBytes(CatImage image)
    {
        byte[] imageBytes = await _httpClient.GetByteArrayAsync(image.Url);
        return imageBytes;
    }
    
    public async Task<List<CatModel>> RetrieveAPIInfo(string parameters)

        {

        HttpResponseMessage response = await _httpClient.GetAsync(parameters).ConfigureAwait(false);  
       
        if (response.IsSuccessStatusCode)
        {
            switch (RequestType)
                {
                    case "Image":
                        List<CatImage>? catimages = await _httpClient.GetFromJsonAsync<List<CatImage>>(parameters);
                        return  (catimages ?? Enumerable.Empty<CatImage>())
                        .Cast<CatModel>()
                        .ToList();
                    case "Breed":
                        List<CatBreed>? catbreeds = await _httpClient.GetFromJsonAsync<List<CatBreed>>(parameters);
                        return  (catbreeds ?? Enumerable.Empty<CatBreed>())
                        .Cast<CatModel>()
                        .ToList();
            }
            
            return new List<CatModel>();
        }
        else
            {   
                DisplayMessage("Could not access API");
                return new List<CatModel>();
            } 

}}