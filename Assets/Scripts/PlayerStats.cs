using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerStats : NetworkBehaviour
{
    [SyncVar] public int health = 100;
    public void Damage(int amount)
    {
        ChangeHealth(this, -amount);
        Debug.Log(health);
    }

    [ServerRpc]
    public void ChangeHealth(PlayerStats script, int amount)
    {
        script.health += amount;
    }
}
