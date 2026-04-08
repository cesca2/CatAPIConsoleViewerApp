using CatAPIConsoleViewerApp.Controllers;
using CatAPIConsoleViewerApp.Enums;
using SixLabors.ImageSharp;
using Spectre.Console;
using System.Security.Cryptography.X509Certificates;


namespace CatAPIConsoleViewerApp;

public class UserInterface : BaseController
{

    private readonly CatsServiceController _catsController = new();

    internal async Task MainMenu()
    {
        AnsiConsole.Write(
        new FigletText("Cat API image fetcher"));
        DisplayMessage("");
        DisplayMessage("");
        Console.ReadKey();
        while (true)
        {
            Console.Clear();

            var actionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuAction>()
                .Title("What do you want to do next?")
                .UseConverter(e => System.Text.RegularExpressions.Regex.Replace(e.ToString(), "([a-z])([A-Z])", "$1 $2"))
                .AddChoices(Enum.GetValues<MenuAction>()));


            switch (actionChoice)
            {
                case MenuAction.ViewImage:
                    var itemTypeChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<SelectionType>()
                        .Title("Select type of image to fetch:")
                        .AddChoices(Enum.GetValues<SelectionType>()));
                    await ViewImage(itemTypeChoice);

                    DisplayMessage("Press Any Key to Continue");
                    Console.ReadKey();

                    break;

                case MenuAction.Exit:
                    if (ConfirmAction("exit"))
                    {
                        System.Environment.Exit(1);
                    }
                    break;


            }
        }
    }
    private async Task ViewImage(SelectionType selectionType)
    {
        List<CatImage> imageChoice = new List<CatImage>();
        bool favouriteOption = true;
        string selectionTypeString = "";

        switch (selectionType)
        {
            case SelectionType.Favourite:
                var favs = await _catsController.GetFavourites();

                var catSelection = AnsiConsole.Prompt(
                new SelectionPrompt<CatFavourite>()
                    .Title("Select an image from your saved favourites:")
                    .UseConverter(m => $"{m.Sub_ID,-15} {m.CreationDate}")
                    .AddChoices(favs.OrderByDescending(i => i.CreationDate)));

                Console.WriteLine($"Selected {catSelection.Sub_ID}");

                imageChoice = await _catsController.GetImage(new CatBreed("Random") { ID = "Random" }, catSelection.Image_ID);
                favouriteOption = false; // can't refavourite a favourite
                selectionTypeString = "Favourite";
                break;


            case SelectionType.Random:
                imageChoice = await _catsController.GetImage(new CatBreed("Random") { ID = "Random" });
                selectionTypeString = "Random";
                break;

            case SelectionType.Breed:

                List<CatBreed> breedOptions = new List<CatBreed>();

                var breedSelectionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<BreedSelectionType>()
                    .Title("Either see available list of breeds or search by phrase:")
                    .AddChoices(Enum.GetValues<BreedSelectionType>()));


                switch (breedSelectionChoice)
                {
                    case BreedSelectionType.List:
                        breedOptions = await _catsController.GetBreeds("List");
                        break;
                    case BreedSelectionType.Search:
                        int retries = 0;
                        while (!breedOptions.Any())
                        {
                            if (retries > 0)
                            {
                                DisplayMessage("No breed matching your query, please try again");
                            }
                            var breedSearchPrompt = new TextPrompt<string>("Search for breed containing:").Validate(i => Validator.IsValidInputString(i), "Bad string");
                            var breedSearch = AnsiConsole.Prompt(breedSearchPrompt);
                            breedOptions = await _catsController.GetBreeds(breedSearch);
                            if (!breedOptions.Any())
                            {
                                retries += 1;

                            }

                        }

                        break;

                }
                var breedSelection = AnsiConsole.Prompt(
                    new SelectionPrompt<CatBreed>()
                    .Title("Select a breed:")
                    .UseConverter(m => $"{m.Name}")
                    .AddChoices(breedOptions));

                DisplayMessage($"Selected {breedSelection.Name}");
                imageChoice = await _catsController.GetImage(breedSelection);
                selectionTypeString = breedSelection.Name;



                break;
        }

        foreach (var image in imageChoice)
        {
            DisplayMessage($"Requested image available at: {image.Url}");
            DisplayImage($"Preview {image.Url}", await _catsController.GetImageBytes(image), $"{selectionTypeString} Cat");
            var viewSuccess = await _catsController.HandleViews(image);
            if (!viewSuccess)
            {
                DisplayMessage("Warning the views did not update successfully for this image", "red");
            }
            if (favouriteOption)
            {
                bool favourited = await AddFavourite(image);
                if (favourited) { DisplayMessage("Favourited successfully", "green"); }
                ;
            }


        }
    }

    private void DisplayImage(string message, byte[] bytes, string description = "")
    {
        var img = new CanvasImage(bytes).MaxWidth(80);

        AnsiConsole.Write(new Panel(img)
            .Header($"{description}, {message}")
            .Border(BoxBorder.Rounded));
    }

    private async Task<bool> AddFavourite(CatImage image)
    {
        if (OfferAction("Add to favourites?"))
        {
            var infoPrompt = new TextPrompt<string>("Enter name for favourite:").Validate(i => Validator.IsValidInputString(i), "Bad string");
            var info = AnsiConsole.Prompt(infoPrompt);

            var success = await _catsController.PostFavourite(image, info);

            return success;
        }
        else
        {
            return false;
        }
    }



}