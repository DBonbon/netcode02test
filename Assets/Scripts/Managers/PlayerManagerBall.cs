using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManagerBall : NetworkBehaviour
{
    public GameObject playerPrefab;
    public GameObject ballPrefab;
    public Transform canvasTransform;
    private List<GameObject> players = new List<GameObject>();
    private GameObject ball;
    private int totalPlayers = 2; // Set your total players
    private float moveInterval = 5.0f; // Interval for moving the ball



    private void Start()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnPlayers());
        }
    }

    System.Collections.IEnumerator SpawnPlayers()
    {
        for (int i = 0; i < totalPlayers; i++)
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(0, i * 200, 0), Quaternion.identity);
            NetworkObject playerNetObj = player.GetComponent<NetworkObject>();
            playerNetObj.Spawn();
            players.Add(player);
            yield return new WaitForSeconds(1); // Wait for a second before spawning the next player
        }

        SpawnBall();
    }

    void SpawnBall()
    {
        ball = Instantiate(ballPrefab, players[0].transform.position, Quaternion.identity);
        NetworkObject ballNetObj = ball.GetComponent<NetworkObject>();
        ballNetObj.Spawn();
        StartCoroutine(MoveBall());
    }

    System.Collections.IEnumerator MoveBall()
    {
        int currentPlayerIndex = 0;

        while (true)
        {
            yield return new WaitForSeconds(moveInterval);
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            ball.transform.SetParent(players[currentPlayerIndex].transform);
            ball.transform.localPosition = Vector3.zero; // Reset position to the center of the parent
        }
    }
}
