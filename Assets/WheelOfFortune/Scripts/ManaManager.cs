using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour
{
    public int MaxMana { get; private set; } = 10;
    public int CurrentMana { get; private set; }
    public float ManaRegenTime { get; private set; } = 60f;

    private float manaRegenTimer = 0f;
    private int manaMove = 1;
    private bool isRegeneratingMana = false;

    // UI
    public TextMeshProUGUI manaText;
    public Slider manaProgressBar;

    private void Start()
    {
        ResetMana();
    }

    private void Update()
    {
        if (isRegeneratingMana)
        {
            RegenerateMana();
        }
    }

    public void UseMana()
    {
        if (CurrentMana >= manaMove)
        {
            CurrentMana -= manaMove;
            UpdateManaUI();
        }
        else
        {
            Debug.Log("Not enough mana!");
        }
    }

    private void RegenerateMana()
    {
        manaRegenTimer += Time.deltaTime;
        if (manaRegenTimer >= ManaRegenTime)
        {
            if (CurrentMana < MaxMana)
            {
                CurrentMana++;
                UpdateManaUI();
            }
            manaRegenTimer = 0f;
        }
        UpdateManaProgressBar(); 
    }

    public void StartManaRegeneration()
    {
        isRegeneratingMana = true;
    }

    public void UpdateManaUI()
    {
        manaText.text = "Mana: " + CurrentMana + "/" + MaxMana;
        manaProgressBar.value = (float)CurrentMana / MaxMana;
    }

    private void UpdateManaProgressBar()
    {
        manaProgressBar.value = (float)CurrentMana / MaxMana;
    }

    public void ResetMana()
    {
        CurrentMana = MaxMana;
        UpdateManaUI();
        UpdateManaProgressBar();
        StartManaRegeneration();
    }
}