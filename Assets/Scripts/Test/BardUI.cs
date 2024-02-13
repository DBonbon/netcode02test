using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode; // Ensure this namespace is included
using System.Collections.Generic;

public class BardUI : MonoBehaviour
{
    /*[SerializeField] private TextMeshProUGUI bardNameText;
    [SerializeField] private TextMeshProUGUI suitText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI biblingText1;
    [SerializeField] private TextMeshProUGUI biblingText2;
    [SerializeField] private TextMeshProUGUI biblingText3;
    [SerializeField] private TextMeshProUGUI biblingText4;

    private Bard bard;
    private Sprite[] bardIcons;

    private void Start()
    {
        bard = GetComponent<Bard>();
        bardIcons = Resources.LoadAll<Sprite>("Images");
        if (bard.BiblingNames != null)
        {
            // Ensure we correctly subscribe to the NetworkList's event
            bard.BiblingNames.OnListChanged += HandleBiblingsChanged;
        }
        UpdateBardUI();
    }

    private void HandleBiblingsChanged(NetworkListEvent<BiblingName> changeEvent)
    {
        UpdateBardUI();
    }

    private void UpdateBardUI()
    {
        if (bard != null)
        {
            bardNameText.text = bard.BardName.Value.ToString();
            suitText.text = bard.Suit.Value.ToString();
            hintText.text = bard.Hint.Value.ToString();

            // Update the icon if applicable
            if (iconImage != null && bardIcons.Length > 0)
            {
                iconImage.sprite = bardIcons[Random.Range(0, bardIcons.Length)];
            }

            // Update bibling texts based on the NetworkList
            UpdateBiblingText(biblingText1, 0);
            UpdateBiblingText(biblingText2, 1);
            UpdateBiblingText(biblingText3, 2);
            UpdateBiblingText(biblingText4, 3);
        }
    }

    private void UpdateBiblingText(TextMeshProUGUI biblingText, int index)
    {
        if (bard.BiblingNames.Count > index)
        {
            biblingText.gameObject.SetActive(true);
            BiblingName biblingName = bard.BiblingNames[index];
            biblingText.text = biblingName.Name;
            biblingText.color = biblingName.Name == bard.BardName.Value.ToString() ? Color.red : Color.white;
        }
        else
        {
            biblingText.gameObject.SetActive(false);
        }
    }*/
}