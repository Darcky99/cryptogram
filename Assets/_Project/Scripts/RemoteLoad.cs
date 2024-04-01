using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine;
using System;

public static class RemoteLoad
{
    private static string s_ConnectionString = "mongodb+srv://darcking99:bf7hqE98rOntwJ08@sandbox.g2woihu.mongodb.net/?retryWrites=true&w=majority&appName=Sandbox";
    private const string s_DatabaseName = "ThemeLevels";
    private const string s_CollectionName = "MyTest";

    private static LevelData.Level_JSON[] getCollection(string dataBaseName, string collectionName)
    {
        LevelData.Level_JSON[] documents = null;
        try
        {
            MongoClient client = new MongoClient(s_ConnectionString);
            var database = client.GetDatabase(dataBaseName);
            var collection = database.GetCollection<LevelData.Level_JSON>(collectionName);
            documents = collection.Find(FilterDefinition<LevelData.Level_JSON>.Empty).ToList().ToArray();
            //documents = collection.Find(FilterDefinition<BsonDocument>.Empty).ToList().ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}");
        }
        return documents;
    }

    public static LevelData[] GetTestLevels()
    {
        LevelData.Level_JSON[] documents = getCollection(s_DatabaseName, s_CollectionName);
        LevelData[] levels = new LevelData[documents.Length];
        for (int i = 0; i < levels.Length; i++)
            levels[i] = new LevelData(documents[i]);
        return levels;
    }
}

public enum LevelsAvailable
{
    HC, Chrismas, Europe, Food, anotherone, Military, anothertwo, etc
}