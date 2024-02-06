using Unity.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI3 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerImage;

    private const string DefaultImagePath = "Images/character_01";

    public void InitializePlayerUI(string playerName, string imagePath)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }

        string path = string.IsNullOrEmpty(imagePath) ? DefaultImagePath : imagePath;
        Sprite imageSprite = Resources.Load<Sprite>(path);
        if (playerImage != null && imageSprite != null)
        {
            playerImage.sprite = imageSprite;
        }
    }
}
