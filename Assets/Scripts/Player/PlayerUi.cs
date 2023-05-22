using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerUi : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>();

    [SerializeField] private TextMeshProUGUI localPlayerName;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playerName.OnValueChanged += PlayerNameChanged;

        if (IsServer)
        {
            playerName.Value = $"Player {OwnerClientId}";
        }

        if(IsClient)
        {
            // Set the name of the player in the overlay
            SetNameOverlay(playerName.Value.ToString());
            NetworkManagerUI.Instance.ShowDeathMessageOverlay(false);
        }
    }

    public void SetNameOverlay(string playerName)
    {
        localPlayerName.text = playerName;
    }

    private void PlayerNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        Debug.LogError($"Player Name changed from: {previousValue} to: {newValue}");
        // Set the name of the player in the overlay
        SetNameOverlay(playerName.Value.ToString());
    }

    [ClientRpc]
    public void ShowHideOverlayMessageClientRpc(bool show, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        NetworkManagerUI.Instance.ShowDeathMessageOverlay(show);

        PlayerManager.Instance.DisconnectPlayerServerRpc(clientId);

        NetworkManagerUI.Instance.ResetCameraAndButtons();

        //CameraManager.Instance.EnableStartCameraClientRpc(true);
        //NetworkManagerUI.Instance.SetInteractableButtons(true);
    }
}
