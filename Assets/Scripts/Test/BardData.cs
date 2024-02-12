using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]

public class BardData
{
    public int id;
    public string suit;
    public string bardName;
    public string hint;
    public string level;
    public string bardImage;
    public List<string> biblings; // List to store bibling bard names
    

    public void PopulateBiblings(List<BardData> allBards)
    {
        // Find bards with the same suit and add their names to the bibling list
        biblings = allBards
            .Where(bard => bard.suit == suit)
            .Select(bard => bard.bardName)
            .ToList();

        // Debug log to check the populated biblings
        Debug.Log($"Biblings for bard {bardName}: {string.Join(", ", biblings)}");
    }

}