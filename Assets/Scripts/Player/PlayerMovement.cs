using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
	//[SerializeField] public BulletManager bulletManager;

	[SerializeField] private float moveSpeed = 5.0f;
	[SerializeField] private float rotationSpeed = 100.0f;
	//[SerializeField] private float rotationAngle = 1.0f;
	[SerializeField] private Vector2 defaultPositionRange = new Vector2(-5, 5);
	[SerializeField] private GameObject gunBulletSpawnPoint;

    // private readonly float moveDirectionValue = 1.0f;
    // private readonly float rotationAngle = 1.0f;

    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
	{
		// Position each prefab that was instantiated on a position range
		transform.position = new Vector3(
			// X - between -4 and 4
			Random.Range(defaultPositionRange.x, defaultPositionRange.y),
			// Y - on the ground
			transform.position.y,
			// Z - between -4 and 4
			Random.Range(defaultPositionRange.x, defaultPositionRange.y)
			);

        rigidBody = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	void Update()
	{
		if (!IsOwner) { return; }

		// Fire bullet
		//if (Input.GetKey(KeyCode.Space))
		if (Input.GetMouseButton(0))
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

		/* (Input.GetKey(KeyCode.W)) { moveDirection.z = moveDirectionValue; }
		if (Input.GetKey(KeyCode.S)) { moveDirection.z = -moveDirectionValue; }
		if (Input.GetKey(KeyCode.A)) {
			//moveDirection.x = -moveDirectionValue;
			rotationMovement.y = -rotationAngle;
		}
		if (Input.GetKey(KeyCode.D)) {
			//moveDirection.x = moveDirectionValue;
			rotationMovement.y = rotationAngle;
		}*/

        // Call the movement update Server Remote Procedure Call
        //UpdateMovementServerRpc(moveDirection, rotationMovement);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float mouseX = 0;

        // Only try to get the mouse value when the player camera is active
        if (!CameraManager.Instance.StartCamera.enabled)
        {
            mouseX = Input.GetAxis("Mouse X");
        }

        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        rotationMovement = new Quaternion(0, mouseX, 0, 0);

        UpdateMovementServerRpc(moveDirection, rotationMovement);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateMovementServerRpc(Vector3 moveDirection, Quaternion rotationMovement)
    {
        //rigidBody.AddForce(moveDirection.normalized * moveSpeed * Time.deltaTime, ForceMode.Force);

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationMovement.y * rotationSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HellBelow"))
        {
            if (IsServer)
            {
                PlayerManager.Instance.PlayerKilled(transform.gameObject);
            }
        }
    }
}
