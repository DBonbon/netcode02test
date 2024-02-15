using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI suitText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Image iconImage;
    [SerializeField] private List<TextMeshProUGUI> siblingTexts;

    private Sprite[] cardIcons;

    private void Awake()
    {
        cardIcons = Resources.LoadAll<Sprite>("Images/character_01"); // Adjust the path as needed
    }

    public void UpdateCardUIWithCardData(CardData cardData)
    {
        string matchingSiblingName = cardData.cardName; // For highlighting
        UpdateUI(cardData.cardName, cardData.suit, cardData.hint, cardData.siblings, matchingSiblingName);
    }

    public void UpdateUI(string cardName, string suit, string hint, List<string> siblings, string matchingSiblingName)
    {
        cardNameText.text = cardName;
        suitText.text = suit;
        hintText.text = hint;

        if (iconImage != null && cardIcons.Length > 0)
        {
            iconImage.sprite = cardIcons[Random.Range(0, cardIcons.Length)];
        }

        for (int i = 0; i < siblingTexts.Count; i++)
        {
            if (i < siblings.Count)
            {
                siblingTexts[i].gameObject.SetActive(true);
                siblingTexts[i].text = siblings[i];
                siblingTexts[i].color = siblings[i] == matchingSiblingName ? Color.red : Color.white;
            }
            else
            {
                siblingTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void ResetUI()
    {
        cardNameText.text = "";
        suitText.text = "";
        hintText.text = "";
        iconImage.sprite = null;
        foreach (var siblingText in siblingTexts)
        {
            siblingText.gameObject.SetActive(false);
            siblingText.text = "";
            siblingText.color = Color.white;
        }
    }
}
