using Unity.Netcode;
using UnityEngine;

public class PlayerPositioner : NetworkBehaviour
{
    private float xOffset = 2f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ulong clientId = GetComponent<NetworkObject>().NetworkObjectId;
            float xPosition = xOffset * (clientId - 1);
            transform.position = new Vector3(xPosition, 0, 0);
            Debug.Log("Player is Spawned");

            if (ServerStartSpawner.SpawnedObject != null)
            {
                Debug.Log("Found non-player object. Attempting to parent.");
                ParentNonPlayerObject(ServerStartSpawner.SpawnedObject);
            }
            else
            {
                Debug.LogError("Non-player object not found.");
            }
        }
    }

    private void ParentNonPlayerObject(GameObject nonPlayerObject)
    {
        nonPlayerObject.transform.SetParent(transform, false);
        Debug.Log("Non-player object parented to player.");
    }
}
