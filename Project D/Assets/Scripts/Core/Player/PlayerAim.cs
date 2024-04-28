using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform playerAimTransform;

    private void LateUpdate()
    {
        if (!IsOwner) { return; }
    
        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);
    
        playerAimTransform.up = new Vector2(
            aimWorldPosition.x - playerAimTransform.position.x,
            aimWorldPosition.y - playerAimTransform.position.y);
    }
    
}
