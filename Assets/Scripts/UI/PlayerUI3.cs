using TMPro;
using UnityEngine;

public class PlayerUI3 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void UpdatePlayerNameUI(string playerName)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
    }
}
