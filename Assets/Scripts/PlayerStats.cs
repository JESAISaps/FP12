/*using UnityEngine;
using System.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SyncVar] public int health = 100;

    [SyncVar] public bool isDead = false;
    [SerializeField]
    private int deathTimer = 3;

    [SerializeField]
    private GameObject body;

    private void Start()
    {
        //isDead = false;
        InitializePlayer(this);
    }

    private void InitializePlayer(PlayerStats script)
    {
        SetHealth(this, maxHealth);
    }

    public void Damage(int amount)
    {
        ChangeHealth(this, -amount);   
        
        if (health <= 0)
        {
            DieOrRespawnServer(this);
        }
        Debug.Log(health);
    }

    [ServerRpc (RequireOwnership = false)]
    public void ChangeHealth(PlayerStats script, int amount)
    {
        script.health += amount;
    }

    [ServerRpc (RequireOwnership = false)]
    public void SetHealth(PlayerStats script, int amount)
    {
        script.health = amount;
    }

    [ServerRpc (RequireOwnership =false)]
    public void DieOrRespawnServer(PlayerStats script)
    {
        DieOrRespawn(script);
    }

    [ObserversRpc]
    private void DieOrRespawn(PlayerStats script)
    {
        script.isDead = !script.isDead;
        script.GetComponentInChildren<Collider>().enabled = !script.GetComponentInChildren<Collider>().enabled;
        script.body.GetComponentInChildren<Renderer>().enabled = !script.body.GetComponentInChildren<Renderer>().enabled;

        if (script.isDead)
            script.StartCoroutine(RespawnTimer(deathTimer));
    }

    private IEnumerator RespawnTimer(int timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        DieOrRespawnServer(this);
        InitializePlayer(this);
    }

}*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using FishNet.Object;
using TMPro;
 
public class PlayerStats : NetworkBehaviour
{
    [SyncVar] public int health = 10;
    private TextMeshProUGUI healthText;

    private void Start()
    {
        healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        healthText.text = health.ToString();
    }
}