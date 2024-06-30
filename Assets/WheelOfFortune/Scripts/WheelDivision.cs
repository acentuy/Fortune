using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WheelDivision : MonoBehaviour
{
    public TextMeshProUGUI multiplierText;
    public Image divisionImage;
    public Image itemImage;
    public float probability;

    public void SetupDivision(RewardItem reward, Sprite itemSprite)
    {
        if (reward.multiplier > 0)
        {
            multiplierText.text = reward.multiplier.ToString();
        }
        else
        {
            multiplierText.text = reward.item;
        }
        if (ColorUtility.TryParseHtmlString(reward.color, out Color color))
        {
            divisionImage.color = color;
        }
        if (itemSprite)
        {
            itemImage.sprite = itemSprite;
        }
        probability = reward.probability; // Установка вероятности для сравнения
    }
}