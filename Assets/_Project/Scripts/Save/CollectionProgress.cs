public class CollectionProgress
{
    public CollectionProgress(int size)
    {
        Items = new ItemProgress[size];

        for (int i = 0; i < Items.Length; i++)
            Items[i] = new ItemProgress(false);
    }

    public ItemProgress[] Items;

    public void RegisterProgress(int index, ItemProgress itemProgress)
    {
        Items[index] = itemProgress;
    }
}