using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections; // Required for FixedString32Bytes

public class Player3 : NetworkBehaviour
{   
    //[SerializeField] private TextMeshProUGUI playerNameText;
    //private float xOffset = 2f;
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> PlayerDbId = new NetworkVariable<int>();
    public NetworkVariable<FixedString128Bytes> PlayerImagePath = new NetworkVariable<FixedString128Bytes>();

    private PlayerUI3 playerUI;

    public override void OnNetworkSpawn()
    {
        playerUI = GetComponent<PlayerUI3>();
    }

    public void InitializePlayer(string name, int dbId, string imagePath)
    {
        PlayerName.Value = new FixedString32Bytes(name);
        PlayerDbId.Value = dbId;
        PlayerImagePath.Value = new FixedString128Bytes(imagePath);
        
        if (playerUI != null)
        {
            playerUI.InitializePlayerUI(PlayerName.Value.ToString(), PlayerImagePath.Value.ToString());
        }
    }

    public void ParentCard(GameObject card)
    {
        if (card != null)
        {
            card.transform.SetParent(transform, false);
            card.transform.localPosition = Vector3.zero; // Reset local position
            Debug.Log($"Card {card.name} parented to player {PlayerName.Value}.");
        }
    }

}
