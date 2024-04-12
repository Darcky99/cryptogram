public class ItemProgress
{
    public ItemProgress(bool isComplete, LevelContinue levelContinue = null)
    {
        LevelContinue = levelContinue;
        IsCompleted = isComplete;
    }
    public LevelContinue LevelContinue;
    public bool IsCompleted = false;
}
