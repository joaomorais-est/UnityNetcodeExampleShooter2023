using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
	// Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
	public static PlayerManager Instance { get; private set; }

	private NetworkVariable<int> playersConnected = new NetworkVariable<int>();

	public int PlayersConnected {
		get => playersConnected.Value; set => playersConnected.Value = value;
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		playersConnected.OnValueChanged += PlayersConnectedChanged;
	}

	private void PlayersConnectedChanged(int previousValue, int newValue)
	{
		// Set the number of connected players in the Main UI
		NetworkManagerUI.Instance.SetPlayersInGameText(playersConnected.Value);
	}

	private void Awake()
	{
		// Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
		if (Instance != null && Instance != this) {
			Debug.LogWarning("Multiple PlayerManager instances are not allowed.");
			Destroy(gameObject);
		}
		else {
			Instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		NetworkManager.Singleton.OnClientConnectedCallback += (clientId) => {
			if (IsServer)
			{
				Debug.LogError($"Player {clientId} just connected.");
				PlayersConnected++;
			}

            // Set the Id of the player in the Main UI
            NetworkManagerUI.Instance.SetPlayerIdText(clientId);
        };
		NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) => {
			if (IsServer)
			{
				Debug.LogError($"Player {clientId} just disconnected.");
				PlayersConnected--;
			}
		};
	}

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (IsServer)
			{
                DisconnectServer();
                Application.Quit();
			}

			if(IsClient)
			{
                DisconnectPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
				NetworkManager.Singleton.Shutdown();
                Application.Quit();
            }

			else
			{
                Application.Quit();
            }
        }
    }

    public void PlayerKilled(GameObject player)
	{
		// If is not the Server/Host then we should early return here!
		if (!IsServer) { return; }

		// Get the playerId from the player that was killed
		ulong clientId = player.GetComponent<PlayerInfo>().PlayerId;

		Debug.LogError($"Player {clientId} just died.");

		if (NetworkManager.ConnectedClients.ContainsKey(clientId))
		{
			var clientGameObject = NetworkManager.ConnectedClients[clientId].PlayerObject.gameObject;

			// Necessario para o ClientRPC saber qual o client para enviar a mensagem
			ClientRpcParams clientRpcParams = new ClientRpcParams
			{
				Send = new ClientRpcSendParams
				{
					TargetClientIds = new ulong[] { clientId }
				}
			};

			clientGameObject.GetComponent<PlayerUi>().ShowHideOverlayMessageClientRpc(true, clientId, clientRpcParams);

		}
	}

	public void DisconnectServer()
	{
		if (IsServer)
		{
			Debug.LogError("Disconnect and destroy server");
			NetworkManager.Singleton.Shutdown();

            // Call a clientRPC with the camera and setbuttons in all clients
            NetworkManagerUI.Instance.ResetCameraAndButtonsClientRpc();
        }
    }

	[ServerRpc(RequireOwnership = false)]
	public void DisconnectPlayerServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
	{
        /*Debug.LogError($"IsServer: {IsServer}");
        Debug.LogError($"IsClient: {IsClient}");
        Debug.LogError($"IsOwner: {IsOwner}");*/

        if (IsServer)
		{
			Debug.LogError("Disconnect and destroy player - Server");
			
			// Call a clientRPC with the camera and setbuttons with the client id to
			// run the camera and set buttons on the specific client with clientId

            // Necessario para o ClientRPC saber qual o client para enviar a mensagem
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

			NetworkManagerUI.Instance.ResetCameraAndButtonsClientRpc(clientRpcParams);

            playersConnected.Value--;
            NetworkManager.Singleton.DisconnectClient(clientId);
        }
	}
}
