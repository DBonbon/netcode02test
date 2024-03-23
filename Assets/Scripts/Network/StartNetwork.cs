using Unity.Netcode;
using UnityEngine;

/// <summary>
/// a script for the Start Button for the MatchMaker Client script
/// </summary>
public class StartNetwork : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

}
