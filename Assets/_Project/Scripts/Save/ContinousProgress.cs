public class ContinousProgress
{
    public ContinousProgress(int levelIndex, LevelContinue levelContinue)
    {
        LevelIndex = levelIndex;
        LevelContinue = levelContinue;
    }
    public LevelContinue LevelContinue;
    public int LevelIndex;

    public void IncreaseLevelIndex() => LevelIndex++;
}