using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class WarriorPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [field:SerializeField] public Health Health {  get; private set; }
    // [field:SerializeField] public CoinWallet Wallet {  get; private set; }

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;

    [SerializeField] private Color ownerColorOnMap;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    // public NetworkVariable<int> PlayerColorIndex = new NetworkVariable<int>();

    public static event Action<WarriorPlayer> OnPlayerSpawned;
    public static event Action<WarriorPlayer> OnPlayerDespawned;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            PlayerName.Value = userData.userName;
            // PlayerColorIndex.Value = userData.userColorIndex;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }
    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
