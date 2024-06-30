using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelDivision : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private Image divisionImage;
    [SerializeField] private Image itemImage;
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
        probability = reward.probability; 
    }
}