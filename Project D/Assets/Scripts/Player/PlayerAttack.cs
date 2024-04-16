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
    [SerializeField] private float damage;

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
        if (IsServer && col.CompareTag("Enemy"))
        {
            Debug.Log("Hit");
            // Apply damage or any other logic here
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