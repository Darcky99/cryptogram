

public class LevelProgress
{
    public LevelProgress(eLevelsCollection levels, int index, bool[] progress, int mistakeCount)
    {
        ContinueLevelType = levels;
        ContinueLevelIndex = index;
        ContinueLevelProgress = progress;
        MistakeCount = mistakeCount;
    }

    public eLevelsCollection ContinueLevelType;
    public int ContinueLevelIndex;
    public bool[] ContinueLevelProgress;
    public int MistakeCount;

    public override string ToString()
    {
        return $"Collection: {ContinueLevelType}, Level Index: {ContinueLevelIndex}, Progress: {ContinueLevelProgress}, Mistake: {MistakeCount}";
    }
}