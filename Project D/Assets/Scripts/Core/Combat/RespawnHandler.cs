using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private WarriorPlayer playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        WarriorPlayer[] players = FindObjectsByType<WarriorPlayer>(FindObjectsSortMode.None);
        foreach (WarriorPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }
        WarriorPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        WarriorPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }  

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        WarriorPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        WarriorPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }
    private void HandlePlayerSpawned(WarriorPlayer player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    } 
    private void HandlePlayerDespawned(WarriorPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }
    private void HandlePlayerDie(WarriorPlayer player)
    {
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return null;
    
        WarriorPlayer playerInstance = Instantiate(
            playerPrefab,SpawnPoint.GetRandomSpawnPos(),Quaternion.identity);
      
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
    }
}
