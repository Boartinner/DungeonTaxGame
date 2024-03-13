using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine;

public class PlayerCtrl : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private InputReader inputReader;

    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4f;
    private float speedX, speedY;

    private Vector2 previousMovementInput;
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) {  return; }
        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.MoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }

    private void Update()
    {
        previousMovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }
    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
        rb.velocity =  previousMovementInput * moveSpeed ;
    }




}
