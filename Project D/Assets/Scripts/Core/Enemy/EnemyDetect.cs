using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyDetect : NetworkBehaviour
{
    public float speed = 5f;
    public float stoppingDistance = 1f;
    public int damageAmount = 5;
    public Transform target;
    public Health playerHealth;

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