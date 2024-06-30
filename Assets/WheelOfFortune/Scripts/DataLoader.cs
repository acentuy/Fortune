using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DataLoader : MonoBehaviour
{
    public RewardData LoadData(string filePath)
    {
        TextAsset dataFile = Resources.Load<TextAsset>(filePath);
        if (dataFile)
        {
            string json = dataFile.text;
            return JsonConvert.DeserializeObject<RewardData>(json);
        }
        else
        {
            Debug.LogError("JSON file not found");
            return null;
        }
    }
}