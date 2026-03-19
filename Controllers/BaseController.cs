using Spectre.Console;

namespace CatAPIConsoleViewerApp;
public abstract class BaseController
{
    protected void DisplayMessage(string message, string color = "yellow")
    {
        AnsiConsole.MarkupLine($"[{color}]{message}[/]");
    }

    protected void DisplayImage(string message, byte[] bytes, string description = "")
    {
        var img = new CanvasImage(bytes).MaxWidth(80);
        var header = string.Join(" ", description, message);

        AnsiConsole.Write(new Panel(img)
            .Header($"{description}, {message}")
            .Border(BoxBorder.Rounded));
    }
    
    protected bool ConfirmAction(string itemName)
    {
        var confirm = AnsiConsole.Confirm($"Are you sure you want to [red]{itemName}[/]?");

        return confirm;
    }

    protected bool OfferAction(string itemName)
    {
        var confirm = AnsiConsole.Confirm($"Would you like to {itemName}?");

        return confirm;
    }
}
