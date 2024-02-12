using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode; // Ensure this namespace is included
using System.Collections.Generic;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI suitText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI siblingText1;
    [SerializeField] private TextMeshProUGUI siblingText2;
    [SerializeField] private TextMeshProUGUI siblingText3;
    [SerializeField] private TextMeshProUGUI siblingText4;

    private Card card;
    private Sprite[] cardIcons;

    private void Start()
    {
        card = GetComponent<Card>();
        cardIcons = Resources.LoadAll<Sprite>("Images");
        if (card.SiblingNames != null)
        {
            // Ensure we correctly subscribe to the NetworkList's event
            card.SiblingNames.OnListChanged += HandleSiblingsChanged;
        }
        UpdateCardUI();
    }

    private void HandleSiblingsChanged(NetworkListEvent<SiblingName> changeEvent)
    {
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        if (card != null)
        {
            cardNameText.text = card.CardName.Value.ToString();
            suitText.text = card.Suit.Value.ToString();
            hintText.text = card.Hint.Value.ToString();

            // Update the icon if applicable
            if (iconImage != null && cardIcons.Length > 0)
            {
                iconImage.sprite = cardIcons[Random.Range(0, cardIcons.Length)];
            }

            // Update sibling texts based on the NetworkList
            UpdateSiblingText(siblingText1, 0);
            UpdateSiblingText(siblingText2, 1);
            UpdateSiblingText(siblingText3, 2);
            UpdateSiblingText(siblingText4, 3);
        }
    }

    private void UpdateSiblingText(TextMeshProUGUI siblingText, int index)
    {
        if (card.SiblingNames.Count > index)
        {
            siblingText.gameObject.SetActive(true);
            SiblingName siblingName = card.SiblingNames[index];
            siblingText.text = siblingName.Name;
            siblingText.color = siblingName.Name == card.CardName.Value.ToString() ? Color.red : Color.white;
        }
        else
        {
            siblingText.gameObject.SetActive(false);
        }
    }
}