using System.Net.Http.Headers;
using System.Net.Http.Json;
using Spectre.Console;

//config for api 
var url = $"https://api.thecatapi.com/v1/images/search";

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(  
    new MediaTypeWithQualityHeaderValue("application/json"));

client.BaseAddress = new Uri(url); 
ProcessAsync(client).GetAwaiter().GetResult();

static async Task ProcessAsync(HttpClient client)

{
        var parameters = $"?api_key={Consts.CATAPI_KEY}&has_breeds=true";

        HttpResponseMessage response = await client.GetAsync(parameters).ConfigureAwait(false);  
       
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonString);
            var catimages = await client.GetFromJsonAsync<List<CatImage>>(parameters);

            foreach (var image in catimages ?? Enumerable.Empty<CatImage>())
                {Console.WriteLine(image.Url);

                using HttpClient imageclient = new HttpClient();
                byte[] imageBytes = await client.GetByteArrayAsync(image.Url);

                var img = new CanvasImage(imageBytes).MaxWidth(80);

                AnsiConsole.Write(new Panel(img)
                .Header($"{image.Url} preview, Random Cat")
                .Border(BoxBorder.Rounded));}
            
}}


        
