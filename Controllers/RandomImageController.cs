using System.Data.Common;
using CatAPIConsoleViewerApp.Enums;
using Spectre.Console;
namespace CatAPIConsoleViewerApp.Controllers;

public class RandomImageController : BaseController
{

    public void ViewImage(string BreedChoice = "")
    {
        //config for api 

        var url = $"{Consts.CATAPI_ENDPOINT}/v1/images/search";
        var parameters = $"?api_key={Consts.CATAPI_KEY}";  
        
        CatBreed breedSelection = SelectBreeds(BreedChoice);
       
        if (breedSelection.ID != "Random"){
            parameters += $"&breed_ids={breedSelection.ID}";
        }

        APIHandler apiHandler = new APIHandler(url);
        var results = apiHandler.RetrieveAPIInfo(parameters).GetAwaiter().GetResult();
        var catimages = results.OfType<CatImage>();
        foreach (var image in catimages ?? Enumerable.Empty<CatImage>())
            {
            DisplayMessage("Here is your image: " +$"[link={image.Url}]Linked Image[/]" + $" ({image.Url})");

            var imageBytes = apiHandler.RetrieveImageBytes(image).GetAwaiter().GetResult();
           
            DisplayImage($"Linked Image preview", imageBytes, $"{breedSelection.Name} Cat");
        
    
    DisplayMessage("Press Any Key to Continue.");
    Console.ReadKey();
    }}

    public CatBreed SelectBreeds(string query = "")
    {
        // return breed 
        var url = $"{Consts.CATAPI_ENDPOINT}/v1/breeds";
        var parameters = $"?api_key={Consts.CATAPI_KEY}";
        
        if (string.IsNullOrEmpty(query))
        {

            return new CatBreed("Random") { ID = "Random" };
        }
        else if (query != "List") 
        {url +="/search"; parameters+=$"&q={query}";}
        
        APIHandler apiHandler = new APIHandler(url);
        var info = apiHandler.RetrieveAPIInfo(parameters).GetAwaiter().GetResult();
        var catbreeds = info.Cast<CatBreed>().ToList();
        
        if (catbreeds.Count>0) 
        {
        var breedSelection = AnsiConsole.Prompt(
            new SelectionPrompt<CatBreed>()
            .Title("Select a breed:")
            .UseConverter(m => $"{m.Name}")
            .AddChoices(catbreeds));
        
        DisplayMessage($"Selected {breedSelection.Name}");
        return breedSelection;
        }

        else
        {
            DisplayMessage("No matching breed found, please try again", "red");
            var breedSearchPrompt = new TextPrompt<string>("Search for breed containing:").Validate(i => Validator.IsValidInputString(i), "[red]Invalid input string[/]");
            var breedSearch = AnsiConsole.Prompt(breedSearchPrompt);
            return SelectBreeds(breedSearch);

        }
        

    

    }

}

