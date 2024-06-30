using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardData
{
    public int coins;
    public List<RewardItem> rewards;
}

[System.Serializable]
public class RewardItem
{
    public int multiplier;
    public float probability;
    public string item;
    public string color;
}