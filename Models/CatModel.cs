public record class CatModel<TID> : BaseModel
{
    public required TID ID { get; set; }
    public override object IDValue => ID!;

}