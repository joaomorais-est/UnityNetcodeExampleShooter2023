using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            // Get the playerId from the player that was killed
            ulong clientId = gameObject.GetComponent<PlayerInfo>().PlayerId;

            Debug.Log($"Not server, set camera active. Client Id: {clientId}");
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            EnablePlayerCameraClientRpc(clientRpcParams);
        }

        if(IsClient)
        {
            CameraManager.Instance.EnableStartCameraClientRpc(false);
        }
    }

    [ClientRpc]
    public void EnablePlayerCameraClientRpc(ClientRpcParams clientRpcParams = default)
    {
        playerCamera.gameObject.SetActive(true);
    }
}
