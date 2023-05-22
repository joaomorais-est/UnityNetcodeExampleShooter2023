using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    // Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
    public static NetworkManagerUI Instance { get; private set; }

    [SerializeField] private Button serverButton;
	[SerializeField] private Button hostButton;
	[SerializeField] private Button clientButton;
	[SerializeField] private Button disconnectButton;
	[SerializeField] private TextMeshProUGUI playersInGameText;
	[SerializeField] private TextMeshProUGUI playerId;
	[SerializeField] private GameObject deathMessageOverlay;

	private void Awake()
	{
		// Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
		if (Instance != null && Instance != this)
		{
			Debug.LogWarning("Multiple UIManager instances are not allowed.");
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		Cursor.visible = true;
	}
    private void Start()
    {
		// Set the disconnect button as non-interactable
		disconnectButton.interactable = false;
        // Hide the "You died" death message
        ShowDeathMessageOverlay(false);

        // Callbacks for the event listeners of the click event
        serverButton.onClick.AddListener(() => {
			if(NetworkManager.Singleton.StartServer()) {
				SetInteractableButtons(false);
			}
			else {
				Debug.LogError($"Server could not be started.");
			}
		});
		hostButton.onClick.AddListener(() => {
			if(NetworkManager.Singleton.StartHost()) {
				SetInteractableButtons(false);
			}
            else {
                Debug.LogError($"Host could not be started.");
            }
        });
		clientButton.onClick.AddListener(() => {
			if(NetworkManager.Singleton.StartClient()) { 
				SetInteractableButtons(false); 
			}
			else {
                Debug.LogError($"Client could not be started.");
            }
		});
		disconnectButton.onClick.AddListener(() => {
/*			Debug.LogError($"IsServer: {IsServer}");
			Debug.LogError($"IsClient: {IsClient}");
			Debug.LogError($"IsOwner: {IsOwner}");*/

			if(IsClient)
			{
				PlayerManager.Instance.DisconnectPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
			}

			if(IsServer)
			{
				PlayerManager.Instance.DisconnectServer();
                ResetCameraAndButtons();
            }

        });

	}

    public void SetInteractableButtons(bool interactable)
	{
		serverButton.interactable = interactable;
		hostButton.interactable = interactable;
		clientButton.interactable = interactable;
		disconnectButton.interactable = !interactable;
	}

	public void ShowDeathMessageOverlay(bool show)
	{
		deathMessageOverlay.SetActive(show);
	}

	public void SetPlayerIdText(ulong clientId)
	{
		if(!IsServer)
		{
			playerId.enabled = true;
			playerId.text = $"Player ID: {clientId}";
		}
    }

	public void SetPlayersInGameText(int numPlayersInGame)
	{
        playersInGameText.text = $"Players In Game: {numPlayersInGame}";
    }

	// Local version
	public void ResetCameraAndButtons()
	{
        CameraManager.Instance.EnableStartCameraClientRpc(true);
        SetInteractableButtons(true);
    }

	// Client RPC version called by the server when a client disconnects
	[ClientRpc]
	public void ResetCameraAndButtonsClientRpc(ClientRpcParams clientRpcParams = default)
	{
        // Create a clientRPC to put this two lines (camera and set buttons)
        CameraManager.Instance.EnableStartCameraClientRpc(true);
        SetInteractableButtons(true);
    }
}
