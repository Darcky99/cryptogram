public class CollectionProgress
{
    public CollectionProgress(int size)
    {
        _Items = new ItemProgress[size];
    }

    public ItemProgress[] Items => _Items;

    private ItemProgress[] _Items;

    public void RegisterProgress(int index, ItemProgress itemProgress)
    {
        _Items[index] = itemProgress;
    }
}