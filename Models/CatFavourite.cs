public record class CatFavourite(string Image_ID, string Created_At, string Sub_ID) : CatModel<int>
{
    public DateTime CreationDate = DateTime.Parse(Created_At);
}