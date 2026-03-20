// designed for interaction with CATAPI calls generic APIHandler, performs GET, POST, etc 
namespace CatAPIConsoleViewerApp.Controllers;

public class CatsServiceController : BaseController
{
    private APIHandler apiHandler = new APIHandler(Consts.CATAPI_ENDPOINT, Consts.CATAPI_KEY);
    
    public async Task<bool> PostFavourite(CatImage Image, string Description)
    { 
        var post = new CatFavouritePost() { image_id= Image.ID, sub_id = Description };
        var url = $"favourites";
        
        var success = await apiHandler.PostAPIInfo(post, url);
        
        return success;

    }
    
    // to use in UI 
    public async Task<bool> HandleViews(CatImage Image)
    {
        // receive number of views stored in API and add one since it's being viewed now
        var currentviews = await GetViews(Image)+1;
        DisplayMessage($"Current views: {currentviews}");
        var success =  await PostViews(Image,currentviews);
        return success;

    }

    private async Task<bool> PostViews(CatImage Image, int CurrentViews)
    { 
        var Description = Consts.CATAPI_VIEWS_KEYWORD; // should match sub_id used in parameters in GetViews() method
        var post = new CatVotePost(CurrentViews) { image_id= Image.ID, sub_id = Description };
        var url = $"votes";
        var success = await apiHandler.PostAPIInfo(post, url);
        
        return success;

    }

    private async Task<int> GetViews(CatImage Image)
    { 
        var url = $"votes";
        // unfortunately the API doesn't support image_id query directly in votes
        var parameters = $"?sub_id={Consts.CATAPI_VIEWS_KEYWORD}"; 

        var results = await apiHandler.RetrieveAPIInfo<CatVote>(url+parameters);
        foreach (var vote in results ?? Enumerable.Empty<CatVote>())
            {
                if (Image.ID==vote.Image_ID)
                {
                    return vote.Value;
                }

        
            }
        return 0;
    }


    public async Task<List<CatImage>> GetImage(CatBreed BreedChoice, string IDChoice = "")
    {
        
        List<CatImage> results; 
        string url;
        string parameters;
        
        if (string.IsNullOrEmpty(IDChoice))
            {
                url = "images/search";
                parameters = ""; 
                if (BreedChoice.ID != "Random")
                {
                    parameters += $"?breed_ids={BreedChoice.ID}";
                }
            }
        
        else
        {
            url = $"images/{IDChoice}";
            parameters = ""; 
            
        }
   

       results = await apiHandler.RetrieveAPIInfo<CatImage>(url+parameters);
  

    return results ?? [];
    }
    
    public async Task<byte[]> GetImageBytes(CatImage Image)
    {
        var url = Image.Url;

        return await apiHandler.RetrieveImageBytes(url);


    }

    public async Task<List<CatBreed>> GetBreeds(string query = "")
    {
        var url = "breeds";
        var parameters = "";
        List<CatBreed> results = new List<CatBreed>();
        
        if (string.IsNullOrEmpty(query))
        {
            results.Add( new CatBreed("Random") { ID = "Random" });

            return results;
        }
        else if (query != "List") 
        {
            url +="/search"; parameters+=$"?q={query}";
        }
        
        var info = await apiHandler.RetrieveAPIInfo<CatBreed>(url+parameters);

        return info;
        

    }

    public async Task<List<CatFavourite>> GetFavourites()
    {
        var url = $"favourites";

        var results = await apiHandler.RetrieveAPIInfo<CatFavourite>(url);

    return results;
    }
}

