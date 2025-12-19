using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class ScoreEntry
{
    public string playerID;
    public string levelID;
    public int score;
}

[System.Serializable]
public class ScoreEntryList
{
    public ScoreEntry[] entries;
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    public async Task<ScoreEntryList> GetScoresAsync(string level)
    {
        string url = $"https://y88cl5cvw5.execute-api.us-east-2.amazonaws.com/getTopScores?levelID={level}";

        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        string response = request.downloadHandler.text;
        request.Dispose();

        string wrappedJson = "{\"entries\":" + response + "}";
        return JsonUtility.FromJson<ScoreEntryList>(wrappedJson);
    }

    public async Task<string> PostScoreAsync(string player, string level, int new_score)
    {
        string url = "https://y88cl5cvw5.execute-api.us-east-2.amazonaws.com/submitScore";

        ScoreEntry upload = new ScoreEntry
        {
            playerID = player,
            levelID = level,
            score = new_score
        };

        string json = JsonUtility.ToJson(upload);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        string response = request.downloadHandler.text;
        request.Dispose();

        return response;
    }
}

