using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using System;
using Random = UnityEngine.Random;

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
public class DataLoader : MonoBehaviour
{
   private List<RewardItem> rewardList;
   private List<RewardItem> shuffledList;
   public WheelDivision[] wheelDivisions;
   public GameObject spinningWheel;
   public float wheelSpinTime = 2f;
   private float elapsedTime = 0f;
   private bool isRotating = false;
   private float targetAngle;
   private const float initialOffset = 45f;

   // Rewards
   private string rewardTextValue = "";
   private RewardItem selectedReward; // Сохранение выбранной награды

   // Mana Manager
   private ManaManager manaManager;

   // UI
   public Button spinButton;
   public TextMeshProUGUI rewardText;
   public Image coinLogo;

   // Data
   private TextAsset dataFile;
   private RewardData jsonData;
  
   public List<Sprite> itemSprites;
   private Dictionary<string, Sprite> itemSpriteDictionary;

   private void Start()
   {
       Init();
   }

   private void Update()
   {
       manaManager.UpdateManaUI();
   }

   private void Init()
   {
       ResetUI();
       LoadData();
       InitializeItemSpriteMap();
       manaManager = GetComponent<ManaManager>(); 
       if (jsonData != null)
       {
           rewardList = jsonData.rewards;
           PopulateWheelData();
       }
   }

   private void ResetUI()
   {
       rewardText.text = "";
       coinLogo.sprite = null;
       spinButton.interactable = true;
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

   private void LoadData()
   {
       dataFile = Resources.Load<TextAsset>("data");
       if (dataFile)
       {
           string json = dataFile.text;
           jsonData = JsonConvert.DeserializeObject<RewardData>(json);
       }
       else
       {
           Debug.LogError("JSON file not found");
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
           int k = UnityEngine.Random.Range(0, n + 1);
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
           float randomProbability = UnityEngine.Random.Range(0.01f, 1f);
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
               rewardTextValue = selectedReward.multiplier > 0 ? selectedReward.multiplier.ToString() : selectedReward.item;
               for (int i = 0; i < wheelDivisions.Length; i++)
               {
                   if (Mathf.Approximately(wheelDivisions[i].probability, selectedReward.probability))
                   {
                       int randomRotationCycles = UnityEngine.Random.Range(2, 4);
                       targetAngle = (randomRotationCycles * 360) + (i * (360 / wheelDivisions.Length)) - initialOffset;
                       if (!isRotating)
                       {
                           StartCoroutine(SpinToTargetAngle());
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

   private IEnumerator SpinToTargetAngle()
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
       StartCoroutine(ShowRewards());
   }

   private IEnumerator ShowRewards()
   {
       rewardText.text = rewardTextValue;
       coinLogo.sprite = itemSpriteDictionary[selectedReward.item];
       yield return new WaitForSeconds(4f);
       ResetUI();
       PopulateWheelData();
   }
}