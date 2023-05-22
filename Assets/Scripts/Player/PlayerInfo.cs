using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    // Não precisa de ser Network Variable pq só vai ser usada para
    // este GameObject específico. Não queremos propagar esta variavel
    // para os restantes NetworkObjects
	[SerializeField] private ulong playerId = new ulong();

    public ulong PlayerId { get => playerId; set => playerId = value; }
    
    public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

        playerId = OwnerClientId;
    }
}
