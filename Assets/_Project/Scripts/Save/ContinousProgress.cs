public class ContinousProgress
{
    public ContinousProgress(int levelIndex, LevelContinue levelContinue)
    {
        _LevelIndex = levelIndex;
        _LevelContinue = levelContinue;
    }

    public int LevelIndex => _LevelIndex;
    public LevelContinue LevelContinue => _LevelContinue;

    private int _LevelIndex;
    private LevelContinue _LevelContinue;

    public void IncreaseLevelIndex() => _LevelIndex++;
}