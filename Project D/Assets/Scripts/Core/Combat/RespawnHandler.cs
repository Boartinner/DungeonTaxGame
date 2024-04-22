using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private WarriorPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentage;

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
        // int keptCoins = (int)(player.Wallet.TotalCoins.Value * (keptCoinPercentage / 100));

        Destroy(player.gameObject);

        // StartCoroutine(RespawnPlayer(player.OwnerClientId,keptCoins));
    }

    // private IEnumerator RespawnPlayer(ulong ownerClientId,int keptCoins)
    // {
    //     yield return null;
    //
    //     WarriorPlayer playerInstance = Instantiate(
    //         playerPrefab,SpawnPoint.GetRandomSpawnPos(),Quaternion.identity);
    //   
    //     playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
    //
    //     playerInstance.Wallet.TotalCoins.Value += keptCoins;
    // }
}
