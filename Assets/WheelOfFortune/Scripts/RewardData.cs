using System.Collections.Generic;
[System.Serializable]
public class RewardData
{
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