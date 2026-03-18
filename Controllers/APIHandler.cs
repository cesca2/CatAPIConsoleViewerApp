using System;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CatAPIConsoleViewerApp.Controllers;

public class APIHandler
{
    private readonly HttpClient _httpClient;

    public APIHandler(string url)
    {
        _httpClient = new();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(  
        new MediaTypeWithQualityHeaderValue("application/json"));

        _httpClient.BaseAddress = new Uri(url); 

        
    }

    public async Task<byte[]> RetrieveImageBytes(CatImage image)
    {
        byte[] imageBytes = await _httpClient.GetByteArrayAsync(image.Url);
        return imageBytes;
    }
    
    public async Task<List<CatImage>> RetrieveAPIInfo()

        {

        var parameters = $"?api_key={Consts.CATAPI_KEY}&has_breeds=true";

        HttpResponseMessage response = await _httpClient.GetAsync(parameters).ConfigureAwait(false);  
       
        // if (response.IsSuccessStatusCode)
        
        var catimages = await _httpClient.GetFromJsonAsync<List<CatImage>>(parameters);
        return catimages;
        
        
        

}
}