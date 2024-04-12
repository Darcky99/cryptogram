public class ContinousProgress
{
    public ContinousProgress(int levelIndex, LevelContinue levelContinue)
    {
        LevelIndex = levelIndex;
        LevelContinue = levelContinue;
    }


    public int LevelIndex;
    public LevelContinue LevelContinue;

    public void IncreaseLevelIndex() => LevelIndex++;
}