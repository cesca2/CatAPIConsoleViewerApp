using System;
using CatAPIConsoleViewerApp.Enums;
using CatAPIConsoleViewerApp.Controllers;
using Spectre.Console;


namespace CatAPIConsoleViewerApp;

public class UserInterface : BaseController
{

    private readonly RandomImageController _randomController = new();

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
                    _randomController.ViewFavourites();
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
                _randomController.ViewImage();
                break;
            case SelectionType.Breed:
                    var breedSelectionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<BreedSelectionType>()
                        .Title("Select from this menu:")
                        .AddChoices(Enum.GetValues<BreedSelectionType>()));


                    switch (breedSelectionChoice)
                    {
                        case BreedSelectionType.List:
                            _randomController.ViewImage("List");
                            break;
                        case BreedSelectionType.Search:

                            var breedSearchPrompt = new TextPrompt<string>("Search for breed containing:").Validate(i => Validator.IsValidInputString(i), "Bad string");
                            var breedSearch = AnsiConsole.Prompt(breedSearchPrompt);
                            _randomController.ViewImage(breedSearch);
                            break;


                    }
                break;
        }
    }


}
