using UnityEngine;
using System.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SyncVar] public int health = 100;
    public bool isDead;

    [SerializeField]
    private int respawnTime;

    private void Start()
    {
        isDead = false;
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        SetHealth(this, maxHealth);
    }

    public void Damage(int amount)
    {
        ChangeHealth(this, -amount);

        if (health <= 0)
            Die();

        Debug.Log(health);
    }

    [ServerRpc]
    public void ChangeHealth(PlayerStats script, int amount)
    {
        script.health += amount;
    }

    [ServerRpc]
    public void SetHealth(PlayerStats script, int amount)
    {
        script.health = amount;
    }

    //[ServerRpc]
    public void KillOrRevivePlayerServer(PlayerStats script)
    {
        KillOrRevivePlayerObserver(script);
    }

    //[ObserversRpc]
    public void KillOrRevivePlayerObserver(PlayerStats script)
    {
        script.isDead = !script.isDead;
        script.gameObject.GetComponentInChildren<Collider>().enabled = !script.gameObject.GetComponentInChildren<Collider>().enabled;
        if (script.isDead == false)
            InitializePlayer();
    }

    private void Die()
    {
        KillOrRevivePlayerServer(this);

        StartCoroutine(WaitForRevive(respawnTime));
    }

    private IEnumerator WaitForRevive(int time)
    {
        yield return new  WaitForSeconds(time);

        KillOrRevivePlayerServer(this);
    }
}
