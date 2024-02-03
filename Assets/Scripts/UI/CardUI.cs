using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image background; // Assign in Inspector
    public TextMeshProUGUI nameText; // Assign in Inspector
    public TextMeshProUGUI suitText; // Assign in Inspector

    public void SetCardData(string name, string suit)
    {
        nameText.text = name;
        suitText.text = suit;
    }
}
