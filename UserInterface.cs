using CatAPIConsoleViewerApp.Enums;
using CatAPIConsoleViewerApp.Controllers;
using Spectre.Console;


namespace CatAPIConsoleViewerApp;

public class UserInterface : BaseController
{

    private readonly CatsServiceController CatsController = new();

    internal void MainMenu()
    {
        AnsiConsole.Write(
        new FigletText("Cat API image fetcher"));
        DisplayMessage("");
        DisplayMessage("Welcome to this app!", "white");
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
                        .Title("Select from this menu:")
                        .AddChoices(Enum.GetValues<SelectionType>()));
                        ViewImage(itemTypeChoice);

                    break;
                case MenuAction.ViewFavourites:
                    CatsController.ViewFavourites();
                    break;
                case MenuAction.Exit:
                    if ( ConfirmAction("exit")) {
                        System.Environment.Exit(1);
                        }
                    break;


        }
    }
    }
    private void ViewImage(SelectionType selectionType)
    {
        switch (selectionType)
        {
            case SelectionType.Random:
                CatsController.ViewImage();
                break;
            case SelectionType.Breed:
                    var breedSelectionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<BreedSelectionType>()
                        .Title("Select from this menu:")
                        .AddChoices(Enum.GetValues<BreedSelectionType>()));


                    switch (breedSelectionChoice)
                    {
                        case BreedSelectionType.List:
                            CatsController.ViewImage("List");
                            break;
                        case BreedSelectionType.Search:

                            var breedSearchPrompt = new TextPrompt<string>("Search for breed containing:").Validate(i => Validator.IsValidInputString(i), "Bad string");
                            var breedSearch = AnsiConsole.Prompt(breedSearchPrompt);
                            CatsController.ViewImage(breedSearch);
                            break;


                    }
                break;
        }
    }


}
