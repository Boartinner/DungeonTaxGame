using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Portal : NetworkBehaviour
{
    public Transform destinationPortal;
    public float teleportDelay = 0.5f;

    private bool isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || isTeleporting) return; // Only perform teleportation on the server
        if (!other.TryGetComponent<NetworkObject>(out var networkObject)) return;

        if (networkObject.IsOwner)
        {
            isTeleporting = true;
            StartCoroutine(TeleportWithDelay()); // Start coroutine for delayed teleportation
        }
    }

    private IEnumerator TeleportWithDelay()
    {
        yield return new WaitForSeconds(teleportDelay);

        // Invoke the teleport method on all clients
        TeleportClientRpc();
    }

    [ClientRpc]
    private void TeleportClientRpc()
    {
        if (destinationPortal != null)
        {
            var netObject = GetComponent<NetworkObject>();
            if (netObject.IsOwner)
            {
                Vector3 relativePosition = transform.InverseTransformPoint(destinationPortal.position);
                relativePosition.z = 0;

                netObject.transform.position = destinationPortal.TransformPoint(relativePosition);
            }
        }
        isTeleporting = false;
    }
}