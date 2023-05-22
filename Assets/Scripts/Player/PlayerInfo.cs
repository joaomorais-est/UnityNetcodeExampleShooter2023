using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    // N�o precisa de ser Network Variable pq s� vai ser usada para
    // este GameObject espec�fico. N�o queremos propagar esta variavel
    // para os restantes NetworkObjects
	[SerializeField] private ulong playerId = new ulong();

    public ulong PlayerId { get => playerId; set => playerId = value; }
    
    public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

        playerId = OwnerClientId;
    }
}
