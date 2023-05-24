using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BulletManager : NetworkBehaviour
{
    // Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
    public static BulletManager Instance { get; private set; }

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int maxNumberOfBullets = 1;
    [SerializeField] private float secondsBeforeNewFire = 0.5f;
    [SerializeField] private float secondsBeforeKillBullet = 3.0f;

    private int numberOfBulletsSpawned = 0;

    void OnEnable()
    {
        // Padrao de desenvolvimento Singleton. Apenas permite criar uma instancia deste elemento
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple BulletManager instances are not allowed.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnBulletServerRpc(Vector3 gunCurrentPosition, Quaternion gunCurrentRotation)
    {
        // If the number of bullets already spawned is the same of the
        // maximum bumber of bullets, we just return and don't spawn a new bullet
        if (numberOfBulletsSpawned == maxNumberOfBullets) { return; }

        // Spawn one bullet for each maxNumberOfBullets
        // Examples: shotgun (multiple bullets simultaneously)
        //          standard gun (only one bullet at a time)
        for (int i = 0; i < maxNumberOfBullets; i++)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                gunCurrentPosition,
                gunCurrentRotation
                );

            bullet.GetComponent<NetworkObject>().Spawn(true);

            numberOfBulletsSpawned++;

            // Esperar X segundos e limpar a variavel de controlo do numero de balas
            StartCoroutine(WaitToClearNumberOfBullets());
            // Esperar X segundos e destruir a bala
            StartCoroutine(WaitToKillBullets(bullet));
        }
    }

    private IEnumerator WaitToClearNumberOfBullets()
    {
        yield return new WaitForSeconds(secondsBeforeNewFire);

        numberOfBulletsSpawned = 0;
    }

    private IEnumerator WaitToKillBullets(GameObject bullet)
    {
        yield return new WaitForSeconds(secondsBeforeKillBullet);

        if (bullet != null)
        {
            Destroy(bullet);
            bullet.GetComponent<NetworkObject>().Despawn(true);
        }
    }   
}
