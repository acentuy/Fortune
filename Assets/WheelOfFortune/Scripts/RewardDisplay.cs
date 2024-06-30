using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class RewardDisplay : MonoBehaviour
{
    private const float displayTime = 2f; 
    
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Image coinLogo;
    [SerializeField] private Button spinButton;

    public void DisplayReward(string reward, Sprite sprite)
    {
        StartCoroutine(DisplayRewardCoroutine(reward, sprite));
    }

    private IEnumerator DisplayRewardCoroutine(string reward, Sprite sprite)
    {
        coinLogo.gameObject.SetActive(true);
        coinLogo.sprite = sprite;
        rewardText.text = reward;
        yield return new WaitForSeconds(displayTime);
        ResetRewardUI();
    }

    private void ResetRewardUI()
    {
        rewardText.text = "";
        coinLogo.gameObject.SetActive(false);
        spinButton.interactable = true; 
    }
}