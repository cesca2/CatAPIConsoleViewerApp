// model
public record class CatModel<TID>: BaseModel
{
    // use auto-properties
    public required TID ID {get; set;}
    public override object IDValue => ID!;

}