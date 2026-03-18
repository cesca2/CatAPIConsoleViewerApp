using System;
using Spectre.Console;


namespace CatAPIConsoleViewerApp.Controllers;

public class RandomImageController : BaseController, IBaseController
{

    public void ViewImage()
    {
        //config for api 
        var url = $"{Consts.CATAPI_ENDPOINT}/v1/images/search";

        APIHandler apiHandler = new APIHandler(url);
        var catimages = apiHandler.RetrieveAPIInfo().GetAwaiter().GetResult();

        foreach (var image in catimages ?? Enumerable.Empty<CatImage>())
            {Console.WriteLine(image.Url);

            var imageBytes = apiHandler.RetrieveImageBytes(image).GetAwaiter().GetResult();
           
            DisplayImage(image.Url+" preview", imageBytes, "Random Cat");
        
    
    DisplayMessage("Press Any Key to Continue.");
    Console.ReadKey();
    }
    }

}
