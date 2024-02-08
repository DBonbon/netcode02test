using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI suitText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Image iconImage; // Assign in the inspector

    private Card card;
    private Sprite[] cardIcons;

    private void Start()
    {
        card = GetComponent<Card>();
        // Load all card icon sprites from the Resources folder
        cardIcons = Resources.LoadAll<Sprite>("Images");
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        if (card != null)
        {
            if (cardNameText != null)
                cardNameText.text = card.CardName.Value.ToString(); // Convert FixedString32Bytes to string

            if (suitText != null)
                suitText.text = card.Suit.Value.ToString(); // Convert FixedString32Bytes to string

            if (hintText != null)
                hintText.text = card.Hint.Value.ToString(); // Convert FixedString32Bytes to string

            // Assign a random icon to the card
            if (iconImage != null && cardIcons.Length > 0)
            {
                int iconIndex = Random.Range(0, cardIcons.Length);
                iconImage.sprite = cardIcons[iconIndex];
            }

            // Update iconImage sprite here if necessary
        }
    }

    // Optionally, subscribe to card's NetworkVariable changes to update UI in real-time
}
