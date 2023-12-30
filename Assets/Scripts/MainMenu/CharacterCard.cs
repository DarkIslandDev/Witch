using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    [SerializeField] private LocalizedText nameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Image abilityImage;
    [SerializeField] private LocalizedText buttonText;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Color selectColor, buyColor;
    private CharacterSelector characterSelector;
    private CharacterBlueprint characterBlueprint;
    private CoinDisplay coinDisplay;

    public void Init(CharacterSelector characterSelector, CharacterBlueprint characterBlueprint, CoinDisplay coinDisplay)
    {
        this.characterSelector = characterSelector;
        this.characterBlueprint = characterBlueprint;
        this.coinDisplay = coinDisplay;

        characterImage.sprite = characterBlueprint.characterSprite;
        abilityImage.sprite = characterBlueprint.startingAbilities[0].GetComponentInChildren<SpriteRenderer>().sprite;
        nameText.Localize("", characterBlueprint.name);

        if (characterBlueprint.owned)
        {
            buttonText.Localize("slcbtn_Key");
        }
        else
        {
            buttonText.Localize("", characterBlueprint.cost + "$");
        }
        
        buttonImage.color = characterBlueprint.owned ? selectColor : buyColor;
    }

    public void SelectedToStart()
    {
        if (!characterBlueprint.owned)
        {
            int coinCount = PlayerPrefs.GetInt("Coins");
            if (coinCount >= characterBlueprint.cost)
            {
                PlayerPrefs.SetInt("Coins", coinCount - characterBlueprint.cost);
                characterBlueprint.owned = true;
                buttonText.Localize("slcbtn_Key");
                buttonImage.color = selectColor;
                coinDisplay.UpdateDisplay();
            }
        }
        else
        {
            characterSelector.StartGame(characterBlueprint);
        }
    }

    public void SelectedToWatchStats()
    {
        characterSelector.characterStats.Init(characterBlueprint);
    }
}