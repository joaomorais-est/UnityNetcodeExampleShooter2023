using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
	//[SerializeField] public BulletManager bulletManager;

	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float rotationSpeed = 100.0f;
	[SerializeField] private Vector2 defaultPositionRange = new Vector2(-5, 5);
	[SerializeField] private GameObject gunBulletSpawnPoint;

	private readonly float moveDirectionValue = 1.0f;
	private readonly float rotationAngle = 1.0f;

	// Start is called before the first frame update
	void Start()
	{
		transform.position = new Vector3(
			// X - between -4 and 4
			Random.Range(defaultPositionRange.x, defaultPositionRange.y),
			// Y - on the ground
			transform.position.y,
			// Z - between -4 and 4
			Random.Range(defaultPositionRange.x, defaultPositionRange.y)
			);
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsOwner) { return; }

		// Fire bullet
		if (Input.GetKey(KeyCode.Space))
		{
			BulletManager.Instance.SpawnBulletServerRpc(gunBulletSpawnPoint.transform.position, gunBulletSpawnPoint.transform.rotation);
		}

		// Authoritative Player Movement 
		// (Only the server can order the client to move)
		UpdateMovementServerAuth();
	}

	private void UpdateMovementServerAuth()
	{
		Vector3 moveDirection = Vector3.zero;
		Quaternion rotationMovement = Quaternion.identity;

		if (Input.GetKey(KeyCode.W)) { moveDirection.z = moveDirectionValue; }
		if (Input.GetKey(KeyCode.S)) { moveDirection.z = -moveDirectionValue; }
		if (Input.GetKey(KeyCode.A)) {
			//moveDirection.x = -moveDirectionValue;
			rotationMovement.y = -rotationAngle;
		}
		if (Input.GetKey(KeyCode.D)) {
			//moveDirection.x = moveDirectionValue;
			rotationMovement.y = rotationAngle;
		}

		// Call the movement update Server Remote Procedure Call
		UpdateMovementServerRpc(moveDirection, rotationMovement);
	}

	[ServerRpc(RequireOwnership = false)]
	private void UpdateMovementServerRpc(Vector3 moveDirection, Quaternion rotationMovement)
	{
		transform.position += moveDirection * moveSpeed * Time.deltaTime;
		transform.Rotate(Vector3.up, rotationMovement.y * rotationSpeed * Time.deltaTime);
	}
}
