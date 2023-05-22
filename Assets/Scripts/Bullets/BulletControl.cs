using Unity.Netcode;
using UnityEngine;

public class BulletControl : NetworkBehaviour
{
    [SerializeField] private float velocityMultiplier = 10.0f;

    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidBody.AddForce(transform.forward * velocityMultiplier, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && collision.collider.gameObject != gameObject)
        {
            // Destroy the bullet
            Destroy(gameObject);
            GetComponent<NetworkObject>().Despawn(true);
            
            // Inform the server that a player was killed
            PlayerManager.Instance.PlayerKilled(collision.collider.transform.gameObject);
            //PlayerManager.Instance.PlayerKilledServerRpc();
        }
    }
}
