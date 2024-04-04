using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine;
using System;
using System.Collections.Generic;

public class RemoteDataManager : Singleton<RemoteDataManager>
{
    #region Unity
    protected override void OnAwakeEvent()
    {
        base.OnAwakeEvent();
    }
    #endregion

    private const string s_ConnectionString = "mongodb+srv://darcking99:bf7hqE98rOntwJ08@sandbox.g2woihu.mongodb.net/?retryWrites=true&w=majority&appName=Sandbox";

    private LevelData.Level_JSON[] getCollection(string dataBaseName, string collectionName)
    {
        LevelData.Level_JSON[] documents = null;
        try
        {
            MongoClient client = new MongoClient(s_ConnectionString);
            var database = client.GetDatabase(dataBaseName);
            var collection = database.GetCollection<LevelData.Level_JSON>(collectionName);
            documents = collection.Find(FilterDefinition<LevelData.Level_JSON>.Empty).ToList().ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
        return documents;
    }


    #region Level Collections

    #region Test
    private const string _THEMED_LEVELS_DATABASE = "ThemeLevels";
    private const string _TEST_COLLECTION = "MyTest";

    public LevelData[] GetTestLevels()
    {
        LevelData.Level_JSON[] documents = getCollection(_THEMED_LEVELS_DATABASE, _TEST_COLLECTION);
        LevelData[] levels = new LevelData[documents.Length];
        for (int i = 0; i < levels.Length; i++)
            levels[i] = new LevelData(documents[i]);
        return levels;
    }
    #endregion

    #region DailyChallenges
    private const string _DAILY_CHALLENGE_DATABASE = "DailyChallenges";

    private Dictionary<int, LevelData[]> _LevelsByMonth;

    public LevelData[] GetDailyChallengeLevels(int month)
    {
        if (_LevelsByMonth == null)
            _LevelsByMonth = new Dictionary<int, LevelData[]>();
        if (_LevelsByMonth.ContainsKey(month))
            return _LevelsByMonth[month];

        LevelData.Level_JSON[] documents = getCollection(_DAILY_CHALLENGE_DATABASE, month.ToString());
        LevelData[] levels = new LevelData[documents.Length];
        for (int i = 0; i < levels.Length; i++)
            levels[i] = new LevelData(documents[i]);

        _LevelsByMonth[month] = levels;
        return levels;
    }

    #endregion

    #endregion
}