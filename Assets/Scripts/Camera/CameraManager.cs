using Unity.Netcode;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	// Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
	public static CameraManager Instance { get; private set; }
	public Camera StartCamera { get => startCamera; set => startCamera = value; }

	[SerializeField] private Camera startCamera;

	private void Awake()
	{
		// Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
		if (Instance != null && Instance != this)
		{
			Debug.LogWarning("Multiple CameraManager instances are not allowed.");
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		startCamera.enabled = true;
	}

	public void EnableStartCamera(bool enabled)
	{
		startCamera.enabled = enabled;
	}

	[ClientRpc]
	public void EnableStartCameraClientRpc(bool enabled, ClientRpcParams clientRpcParams = default)
	{
		startCamera.enabled = enabled;
	}
}
