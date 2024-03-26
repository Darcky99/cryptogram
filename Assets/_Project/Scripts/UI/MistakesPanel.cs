using UnityEngine;

public class MistakesPanel : Singleton<MistakesPanel>
{
    [SerializeField] private GameObject[] _Crosses;

    public void DisplayMistakeCount(int count)
    {
        for(int i = 0; i < _Crosses.Length; i++)
            _Crosses[i].SetActive(count > i);
    }
}