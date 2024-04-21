using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float meleeSpeed;
    [SerializeField] private int damage;

    private float timeUntilMelee;

    [SerializeField] private InputReader inputReader; // Reference to the InputReader script

    private void Start()
    {
        if (IsLocalPlayer && inputReader != null)
        {
            inputReader.AttackEvent += OnAttackInput;
        }
    }

    private void OnDestroy()
    {
        if (IsLocalPlayer && inputReader != null)
        {
            inputReader.AttackEvent -= OnAttackInput;
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer)
            return;

        if (timeUntilMelee <= 0f)
        {
            if (inputReader != null && inputReader.AttackInput)
            {
                AttackServerRpc();
            }
        }
        else
        {
            timeUntilMelee -= Time.deltaTime;
        }
    }

    [ServerRpc]
    private void AttackServerRpc()
    {
        AttackClientRpc();
        timeUntilMelee = meleeSpeed;
    }

    [ClientRpc]
    private void AttackClientRpc()
    {
        anim.SetTrigger("Attack");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer)
            return;

        if (col.CompareTag("Enemy"))
        {
            // Get the Health component of the enemy
            Health enemyHealth = col.GetComponent<Health>();
        
            // Check if the enemy has a Health component
            if (enemyHealth != null)
            {
                // Apply damage to the enemy
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Debug.Log("Enemy does not have a Health component.");
            }
        }
    }

    private void OnAttackInput(bool isAttacking)
    {
        if (isAttacking && IsLocalPlayer)
        {
            AttackServerRpc();
        }
    }
}