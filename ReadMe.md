# Cat Image Retriever

.NET console app to retrieve images from the CatAPI. 

Retrieving images includes providing the URL for the image and providing an image preview in the console. 


## Demo

![Demo Animation](../demo/cat_demo.gif?raw=true)


## Features

* Display cat images
    * Find random image
    * Filter images by breed, user can select from complete list of breeds and filter this list by a keyword search
    * Select from images that have been added to favourites
* Favourite images, save images for future reference taking user input for naming
* View count provided for each image (updated for each individual image each time it is displayed)

## Pre-requisites 

### Dependencies 

* .NET 10.0 installation

### Environment Variables

To run this project, you will need to add the following environment variables to your .env file:

* `MY_API_KEY` - A personal API key for the Cat API, which can be obtained for free through their website https://thecatapi.com/. 


## Run Locally

Clone the project

```bash
  git clone https://github.com/cesca2/CatAPIConsoleViewerApp.git
```

Go to the project directory

```bash
  cd CatAPIConsoleViewerApp
```


Run the application

```bash
  chmod +x ./setup.sh
  ./setup.sh
```


## Acknowledgements
 - [The Cat API](https://thecatapi.com/)
 - [Project inspiration](https://www.thecsharpacademy.com/project/15/drink) - This app fulfils the use cases (adapted for the different API choice) detailed in the C# academy project.
 - [Http requests - Microsoft learn](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient)

