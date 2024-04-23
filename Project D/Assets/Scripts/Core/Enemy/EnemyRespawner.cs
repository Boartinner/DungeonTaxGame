using System.Collections;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class EnemyRespawner : NetworkBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] public GameObject enemyPrefab; // Prefab of the enemy to spawn
    public int maxEnemies = 10; // Maximum number of enemies allowed on the map
    

    [Header("Respawn Settings")]
    public float respawnDelay = 10f; // Delay before respawning the enemy

    private List<GameObject> spawnedEnemies = new List<GameObject>(); // List to store spawned enemies

    private void Start()
    {
        if (!IsServer) return; // Only run on the server
        SpawnInitialEnemies();
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    private void Update()
    {
        if (!IsServer) return; // Only run on the server

        // Check if we need to spawn a new enemy
        if (spawnedEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (EnemySpawnpoint.enemySpawnpoints.Count == 0)
        {
            return;
        }

        // Choose a random spawn point
        Vector3 spawnPoint = EnemySpawnpoint.GetRandomSpawnPos();

        // Instantiate the enemy prefab at the chosen spawn point
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);

        // Spawn the new enemy using the NetworkManager instance associated with this object
        NetworkObject networkObject = newEnemy.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }

        // Attach the Health component and set up respawn logic
        Health enemyHealth = newEnemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            // Subscribe to the OnDie event of the enemyHealth component
            enemyHealth.OnDie += HandleEnemyDie;
        }
    }

    private void HandleEnemyDie(Health enemyHealth)
    {
        // Get the GameObject associated with the Health component
        GameObject enemy = enemyHealth.gameObject;
    
        // Call the RespawnEnemy method when the enemy dies
        RespawnEnemy(enemy);
    }

    private void RespawnEnemy(GameObject enemy)
    {
        StartCoroutine(RespawnCoroutine(enemy));
    }

    private IEnumerator RespawnCoroutine(GameObject enemy)
    {
        // Wait for the respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Remove the enemy from the list of spawned enemies
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }

        // Spawn a new enemy
        SpawnEnemy();
    }
}
