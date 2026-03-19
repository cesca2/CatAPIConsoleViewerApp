using Spectre.Console;
namespace CatAPIConsoleViewerApp.Controllers;

public class CatsServiceController : BaseController
{
    public void PostFavourite(CatImage Image, string Description)
    { 
        var post = new CatFavouritePost() { image_id= Image.ID, sub_id = Description };
        var url = $"{Consts.CATAPI_ENDPOINT}/v1/favourites";
        var parameters = $"?api_key={Consts.CATAPI_KEY}"; 
        
        APIHandler apiHandler = new APIHandler(url);
        var success = apiHandler.PostAPIInfo(post, parameters).GetAwaiter().GetResult();

    }

    public void UpdateViews(CatImage Image, int CurrentViews)
    { 
        var Description = "Views";
        var post = new CatVotePost(CurrentViews) { image_id= Image.ID, sub_id = Description };
        var url = $"{Consts.CATAPI_ENDPOINT}/v1/votes";
        var parameters = $"?api_key={Consts.CATAPI_KEY}"; 
        
        APIHandler apiHandler = new APIHandler(url);
        var success = apiHandler.PostAPIInfo(post, parameters).GetAwaiter().GetResult();

    }

    public int GetViews(CatImage Image)
    { 
        var url = $"{Consts.CATAPI_ENDPOINT}/v1/votes";
        var parameters = $"?api_key={Consts.CATAPI_KEY}&sub_id=Views"; 
        
        APIHandler apiHandler = new APIHandler(url);
        var results = apiHandler.RetrieveAPIInfo(parameters).GetAwaiter().GetResult();
        var viewresults = results.OfType<CatVote>();
        foreach (var vote in viewresults ?? Enumerable.Empty<CatVote>())
            {
                // Console.WriteLine(vote.Image_ID);
                if (Image.ID==vote.Image_ID)
                {
                    return vote.Value;
                }

        
            }
        return 0;
    }

    public void DisplayImage(string message, CatImage image, byte[] bytes, string description = "")
    {
        var img = new CanvasImage(bytes).MaxWidth(80);
        var header = string.Join(" ", description, message);

        AnsiConsole.Write(new Panel(img)
            .Header($"{description}, {message}")
            .Border(BoxBorder.Rounded));
        var views = GetViews(image)+1; // displaying image now so add 1 to views
        DisplayMessage($"Views: {views}");
        UpdateViews(image, views);
    }

           
    public void AddFavourite(CatImage image)
    {
        if ( OfferAction("Add to favourites?")) {
            var infoPrompt = new TextPrompt<string>("Enter name for favourite:").Validate(i => Validator.IsValidInputString(i), "Bad string");
            var info = AnsiConsole.Prompt(infoPrompt);

            PostFavourite(image, info);
            
    }
    }

    public void ViewImage(string BreedChoice = "", string IDChoice = "")
    {
        
        //config for api 
        if (string.IsNullOrEmpty(IDChoice))
        {var url = $"{Consts.CATAPI_ENDPOINT}/v1/images/search";
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
           
            DisplayImage($"Linked Image preview", image, imageBytes, $"{breedSelection.Name} Cat");
            AddFavourite(image);
        
        }}
        else
        {
            var url = $"{Consts.CATAPI_ENDPOINT}/v1/images/{IDChoice}";
            var parameters = $"?api_key={Consts.CATAPI_KEY}";  

            APIHandler apiHandler = new APIHandler(url);
            var results = apiHandler.RetrieveAPIInfo(parameters).GetAwaiter().GetResult();
            var catimages = results.OfType<CatImage>();
            foreach (var image in catimages ?? Enumerable.Empty<CatImage>())
                {
                DisplayMessage("Here is your image: " +$"[link={image.Url}]Linked Image[/]" + $" ({image.Url})");

                var imageBytes = apiHandler.RetrieveImageBytes(image).GetAwaiter().GetResult();
            
                DisplayImage($"Linked Image preview", image, imageBytes, $"Cat");


        }}

        
    
    DisplayMessage("Press Any Key to Continue.");
    Console.ReadKey();
    }

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

    public void ViewFavourites()
    {
        var url = $"{Consts.CATAPI_ENDPOINT}/v1/favourites";
        var parameters = $"?api_key={Consts.CATAPI_KEY}";  

        APIHandler apiHandler = new APIHandler(url);
        var results = apiHandler.RetrieveAPIInfo(parameters).GetAwaiter().GetResult();
        var cats = results.Cast<CatFavourite>().ToList();

        if (cats.Count>0) 
        {
        var catSelection = AnsiConsole.Prompt(
            new SelectionPrompt<CatFavourite>()
            .Title("Select a favourite:")
            .UseConverter(m => $"{m.Sub_ID, -15} {m.CreationDate}")
            .AddChoices(cats.OrderByDescending(i=>i.CreationDate)));
        
        ViewImage("", catSelection.Image_ID);   

    }
    DisplayMessage("Press Any Key to Continue.");
    Console.ReadKey();
    }
}

