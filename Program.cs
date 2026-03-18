using System.Net.Http.Headers;
using System.Net.Http.Json;
using Spectre.Console;
using CatAPIConsoleViewerApp;



// User input search for Breed 
// List breeds
// Query to display a random image of said breed https://api.thecatapi.com/v1/images/search?limit=10&breed_ids=beng&api_key=REPLACE_ME
        
UserInterface userInterface = new();
userInterface.MainMenu();