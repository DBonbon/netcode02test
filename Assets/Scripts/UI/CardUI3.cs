using TMPro;
using UnityEngine;

public class CardUI3 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText; // Assign in the inspector

    private Card3 card;

    private void Start()
    {
        card = GetComponent<Card3>();
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        if (card != null && cardNameText != null)
        {
            cardNameText.text = card.CardName.Value.ToString(); // Convert FixedString32Bytes to string
        }
    }

    // Optionally, subscribe to card's NetworkVariable changes to update UI in real-time
}
