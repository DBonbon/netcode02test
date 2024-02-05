using UnityEngine;
using Unity.Netcode;

public class PlayerP : NetworkBehaviour
{
    private static int playerCount = 0;
    private float xOffset = 20f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Adjust the position based on the number of players
            transform.position = new Vector3(xOffset * playerCount, 0, 0);
            playerCount++;
        }
    }
}
