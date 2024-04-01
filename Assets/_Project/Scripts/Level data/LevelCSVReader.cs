using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class LevelCSVReader : MonoBehaviour
{
	private const string _LANGUAJE_ENGLISH = "English";
	private const string _LANGUAJE_SPANISH = "Spanish";

	private const string _FILENAME_HCLEVELS_PREFIX = "HC Levels - ";

	private const string _TXTS_PATH = "Assets/_Project/Documents/Levels/";
	private const string _TXT_FORMAT_PREXIX = ".txt";

	private const string _SCRIPTABLES_PATH = "Assets/_Project/Resources/HC Levels/";
	private const string _ASSET_FORMAT_PREXIX = ".asset";

	private LevelData[] readCSV(string languaje)
	{
		string[] lines = File.ReadAllLines(_TXTS_PATH + _FILENAME_HCLEVELS_PREFIX + languaje + _TXT_FORMAT_PREXIX);

		if (lines == null || lines.Length == 0)
			Debug.LogError("Not valid file");

		LevelData[] levels = new LevelData[lines.Length - 1];

		for (int i = 1; i < lines.Length; i++)
		{
			string[] values = lines[i].Split(';');

			string autor = values[0];
			string phrase = values[1];
			string partiallyHiden = values.Length > 2 ? values[2] : "";
			string hiden = values.Length > 3 ? values[3] : "";

			LevelData level = new LevelData(autor, phrase, partiallyHiden, hiden);
			levels[i - 1] = level;
		}
		return levels;
	}
	private LevelsData_Scriptable createScriptable(LevelData[] levels)
    {
		LevelsData_Scriptable levelsScriptable = ScriptableObject.CreateInstance<LevelsData_Scriptable>();
		levelsScriptable.SetLevels(levels);
		return levelsScriptable;
	}
	private void createScriptableAsset(LevelsData_Scriptable scriptable, string languaje)
    {
		AssetDatabase.CreateAsset(scriptable, _SCRIPTABLES_PATH + _FILENAME_HCLEVELS_PREFIX + languaje + _ASSET_FORMAT_PREXIX);
	}

	private void convertCSV(string languaje)
    {
		LevelData[] levelsData = readCSV(languaje);
		LevelsData_Scriptable levelsData_Scriptable = createScriptable(levelsData);
		createScriptableAsset(levelsData_Scriptable, languaje);
	}

	private void convertAllFiles()
    {
		convertCSV(_LANGUAJE_ENGLISH);
		convertCSV(_LANGUAJE_SPANISH);
	}

	public void ConvertAllFiles() => convertAllFiles();
}

[CustomEditor(typeof(LevelCSVReader))]
public class LevelCSVReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		LevelCSVReader csvReader = (LevelCSVReader)target;

		if (GUILayout.Button("Create Scriptable"))
			csvReader.ConvertAllFiles();
	}
}