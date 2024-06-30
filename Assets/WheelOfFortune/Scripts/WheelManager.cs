using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour
{
    private const float initialOffset = 45f;
    [SerializeField] private WheelDivision[] wheelDivisions;
    [SerializeField] private GameObject spinningWheel;
    [SerializeField] private float wheelSpinTime = 2f;

    private List<RewardItem> rewardList;
    private List<RewardItem> shuffledList;
    private float elapsedTime = 0f;
    private bool isRotating = false;
    private float targetAngle;

    // Dependencies
    private ManaManager manaManager;
    private RewardDisplay rewardDisplayManager;

    private Dictionary<string, Sprite> itemSpriteDictionary;

    // UI
    [SerializeField] private Button spinButton;
    [SerializeField] private List<Sprite> itemSprites;

    private RewardItem selectedReward;

    private void Start()
    {
        InitializeItemSpriteMap();
        manaManager = GetComponent<ManaManager>();
        rewardDisplayManager = GetComponent<RewardDisplay>();
        DataLoader dataLoader = GetComponent<DataLoader>();
        RewardData jsonData = dataLoader.LoadData("data");

        if (jsonData != null)
        {
            rewardList = jsonData.rewards;
            PopulateWheelData();
        }

        spinButton.onClick.AddListener(Spin);
        ResetUI();
    }

    private void InitializeItemSpriteMap()
    {
        itemSpriteDictionary = new Dictionary<string, Sprite>();
        foreach (var image in itemSprites)
        {
            string itemName = image.name;
            itemSpriteDictionary[itemName] = image;
        }
    }

    private void PopulateWheelData()
    {
        shuffledList = ShuffleList(rewardList);
        for (int i = 0; i < shuffledList.Count; i++)
        {
            wheelDivisions[i].SetupDivision(shuffledList[i], itemSpriteDictionary[shuffledList[i].item]);
        }
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        List<T> shuffledList = new List<T>(list);
        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = shuffledList[k];
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }

        return shuffledList;
    }

    public void Spin()
    {
        if (manaManager.CurrentMana > 0)
        {
            manaManager.UseMana();
            spinButton.interactable = false;
            float randomProbability = Random.Range(0.01f, 1f);
            float cumulativeProbability = 0;

            selectedReward = null;
            foreach (RewardItem reward in rewardList)
            {
                cumulativeProbability += reward.probability;
                if (randomProbability <= cumulativeProbability)
                {
                    selectedReward = reward;
                    break;
                }
            }

            if (selectedReward != null)
            {
                string rewardTextValue = selectedReward.multiplier > 0
                    ? selectedReward.multiplier.ToString()
                    : selectedReward.item;
                for (int i = 0; i < wheelDivisions.Length; i++)
                {
                    if (Mathf.Approximately(wheelDivisions[i].probability, selectedReward.probability))
                    {
                        int randomRotationCycles = Random.Range(2, 4);
                        targetAngle = (randomRotationCycles * 360) + (i * (360 / wheelDivisions.Length)) -
                                      initialOffset;
                        if (!isRotating)
                        {
                            StartCoroutine(SpinToTargetAngle(rewardTextValue));
                        }

                        break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Not enough mana to spin the wheel!");
        }
    }

    private IEnumerator SpinToTargetAngle(string rewardTextValue)
    {
        isRotating = true;
        float initialRotation = transform.eulerAngles.z;
        while (elapsedTime < wheelSpinTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / wheelSpinTime);
            t = 1 - Mathf.Pow(1 - t, 2);
            float currentAngle = Mathf.Lerp(initialRotation, targetAngle, t);
            spinningWheel.transform.eulerAngles = new Vector3(0, 0, currentAngle);
            yield return null;
        }

        spinningWheel.transform.eulerAngles = new Vector3(0, 0, targetAngle);
        isRotating = false;
        elapsedTime = 0f;
        rewardDisplayManager.DisplayReward(rewardTextValue, itemSpriteDictionary[selectedReward.item]);
    }

    private void ResetUI()
    {
        spinButton.interactable = true;
    }
}