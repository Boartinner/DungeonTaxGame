using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class EnemyDetect : NetworkBehaviour
{
    public float speed = 5f;
    public float stoppingDistance = 1f;
    public int damageAmount = 5;
    public Transform target;
    public Health playerHealth;
    private bool isFindingTarget;
    private WaitForSeconds waitTimeToFindNewPlayer = new WaitForSeconds(1);
    void Update()
    {
        if (!IsServer) return; // Only run on the server

        if (target != null)
        {
            // Move towards the target
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            
            // Check if close enough to stop moving
            if (Vector2.Distance(transform.position, target.position) <= stoppingDistance)
            {
                AttackPlayer();
            }
        }

        if (target == null)
        {
            if (!isFindingTarget)
            {
                StartCoroutine(Start());
            }
        }
        if (playerHealth != null)
        {
            if (playerHealth.CurrentHealth.Value == 0)
            {
                target = null;
                playerHealth = null;
               
            }
        }
        
    }

    private IEnumerator Start()
    {
        isFindingTarget = true;
        while (true)
        {
           // yield return new WaitForSeconds(1);
            
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length == 0)
            { 
                yield return  waitTimeToFindNewPlayer;
                 continue;
            }
            
            GameObject selectedPlayer = player[Random.Range(0, player.Length)];
            if (selectedPlayer != null)
            {
                if (selectedPlayer.TryGetComponent<Health>(out Health playerHealth))
                {
                    this.playerHealth = playerHealth;
                }

                SetTarget(selectedPlayer.transform);
                isFindingTarget = false;
                break;
                
            }
        
           
        }
        
    }
    
    void AttackPlayer()
    {
        if (playerHealth != null)
        {
            // Reduce player's health
            playerHealth.TakeDamage(damageAmount);
        }
    }

    // This method is called by the player when it enters the enemy's detection range
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        SyncTargetPositionServerRpc(target.position);
    }

    // Synchronize target position over the network
    [ServerRpc]
    void SyncTargetPositionServerRpc(Vector2 position)
    {
        SyncTargetPositionClientRpc(position);
    }

    [ClientRpc]
    void SyncTargetPositionClientRpc(Vector2 position)
    {
        if (!IsServer)
        {
            target.position = position;
        }
    }
}