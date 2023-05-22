using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementPreClass : NetworkBehaviour
{
    [SerializeField] public float moveSpeed = 5.0f;
    [SerializeField] public float rotationSpeed = 100.0f;

    private float moveDirectionValue = 1.0f;
    private float rotationAngle = 1.0f;

    // Create a new NetworkVariable. Must be initialized here in order to work
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Use the lambda expression to get the value that was changed
        randomNumber.OnValueChanged += ValueChanged;
    }

    private void ValueChanged(int previousValue, int newValue)
    {
        Debug.Log($"{OwnerClientId}; randomNumber: {randomNumber.Value}");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("Open console on build!");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{OwnerClientId}; randomNumber: {randomNumber.Value}");

        if (!IsOwner) { return; }

        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);

            //randomNumber.Value = Random.Range(0, 100);
            //TestServerRpc("Hello World Multiplayer!");
            //TestClientRpc();

        }

        // Use the Y key to despawn a Network Object
        if (Input.GetKeyDown(KeyCode.Y))
        {
            spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
        }

        // Non Authoritative Player Movement
        //UpdateMovement();

        // Authoritative Player Movement 
        // (Only the server can order the client to move)
        UpdateMovementServerAuth();
    }

    private void UpdateMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        Quaternion rotationMovement = Quaternion.identity;

        if (Input.GetKey(KeyCode.W)) { moveDirection.z = moveDirectionValue; }
        if (Input.GetKey(KeyCode.S)) { moveDirection.z = -moveDirectionValue; }
        if (Input.GetKey(KeyCode.Q)) { moveDirection.x = -moveDirectionValue; }
        if (Input.GetKey(KeyCode.E)) { moveDirection.x = moveDirectionValue; }
        if (Input.GetKey(KeyCode.A)) { rotationMovement.y = -rotationAngle; }
        if (Input.GetKey(KeyCode.D)) { rotationMovement.y = rotationAngle; }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationMovement.y * rotationSpeed * Time.deltaTime);
    }

    private void UpdateMovementServerAuth()
    {
        Vector3 moveDirection = Vector3.zero;
        Quaternion rotationMovement = Quaternion.identity;

        if (Input.GetKey(KeyCode.W)) { moveDirection.z = moveDirectionValue; }
        if (Input.GetKey(KeyCode.S)) { moveDirection.z = -moveDirectionValue; }
        if (Input.GetKey(KeyCode.Q)) { moveDirection.x = -moveDirectionValue; }
        if (Input.GetKey(KeyCode.E)) { moveDirection.x = moveDirectionValue; }
        if (Input.GetKey(KeyCode.A)) { rotationMovement.y = -rotationAngle; }
        if (Input.GetKey(KeyCode.D)) { rotationMovement.y = rotationAngle; }

        // Call the movement update Server Remote Procedure Call
        UpdateMovementServerRpc(moveDirection, rotationMovement);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateMovementServerRpc(Vector3 moveDirection, Quaternion rotationMovement)
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationMovement.y * rotationSpeed * Time.deltaTime);
    }


    [ServerRpc]
    public void TestServerRpc(string message)
    {
        Debug.Log($"TestServerRpc: {OwnerClientId}; Message: {message}");
    }

    [ClientRpc]
    public void TestClientRpc()
    {
        Debug.Log($"TestClientRpc");
    }


}
