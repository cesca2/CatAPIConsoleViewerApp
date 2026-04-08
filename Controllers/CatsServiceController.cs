// designed for interaction with CATAPI calls generic APIHandler, performs GET, POST, etc 
namespace CatAPIConsoleViewerApp.Controllers;

public class CatsServiceController : BaseController
{
    private APIHandler _apiHandler = new APIHandler(Consts.CATAPI_ENDPOINT, Consts.CATAPI_KEY);

    public async Task<bool> PostFavourite(CatImage image, string description)
    {
        var post = new CatFavouritePost() { Image_id = image.ID, Sub_id = description };
        var url = $"favourites";

        var success = await _apiHandler.PostAPIInfo(post, url);

        return success;

    }

    // to use in UI 
    public async Task<bool> HandleViews(CatImage image)
    {
        // receive number of views stored in API and add one since it's being viewed now
        var currentviews = await GetViews(image) + 1;
        DisplayMessage($"Current views: {currentviews}");
        var success = await PostViews(image, currentviews);
        return success;

    }

    private async Task<bool> PostViews(CatImage image, int currentViews)
    {
        var description = Consts.CATAPI_VIEWS_KEYWORD; // should match sub_id used in parameters in GetViews() method
        var post = new CatVotePost(currentViews) { Image_id = image.ID, Sub_id = description };
        var url = $"votes";
        var success = await _apiHandler.PostAPIInfo(post, url);

        return success;

    }

    private async Task<int> GetViews(CatImage image)
    {
        var url = $"votes";
        // unfortunately the API doesn't support image_id query directly in votes
        var parameters = $"?sub_id={Consts.CATAPI_VIEWS_KEYWORD}";

        var results = await _apiHandler.RetrieveAPIInfo<CatVote>(url + parameters);
        foreach (var vote in results ?? Enumerable.Empty<CatVote>())
        {
            if (image.ID == vote.Image_ID)
            {
                return vote.Value;
            }


        }
        return 0;
    }


    public async Task<List<CatImage>> GetImage(CatBreed breedChoice, string idChoice = "")
    {

        List<CatImage> results;
        string url;
        string parameters;

        if (string.IsNullOrEmpty(idChoice))
        {
            url = "images/search";
            parameters = "";
            if (breedChoice.ID != "Random")
            {
                parameters += $"?breed_ids={breedChoice.ID}";
            }
        }

        else
        {
            url = $"images/{idChoice}";
            parameters = "";

        }


        results = await _apiHandler.RetrieveAPIInfo<CatImage>(url + parameters);


        return results ?? [];
    }

    public async Task<byte[]> GetImageBytes(CatImage image)
    {
        var url = image.Url;

        return await _apiHandler.RetrieveImageBytes(url);


    }

    public async Task<List<CatBreed>> GetBreeds(string query = "")
    {
        var url = "breeds";
        var parameters = "";
        List<CatBreed> results = new List<CatBreed>();

        if (string.IsNullOrEmpty(query))
        {
            results.Add(new CatBreed("Random") { ID = "Random" });

            return results;
        }
        else if (query != "List")
        {
            url += "/search"; parameters += $"?q={query}";
        }

        var info = await _apiHandler.RetrieveAPIInfo<CatBreed>(url + parameters);

        return info;


    }

    public async Task<List<CatFavourite>> GetFavourites()
    {
        var url = $"favourites";

        var results = await _apiHandler.RetrieveAPIInfo<CatFavourite>(url);

        return results;
    }
}