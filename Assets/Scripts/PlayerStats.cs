using System.Collections;
using UnityEngine;
using FishNet.Object.Synchronizing;
using FishNet.Object;
using TMPro;
 
public class PlayerStats : NetworkBehaviour
{
    [SyncVar] public int health = 10;
    private TextMeshProUGUI healthText;

    private PlayerController playerController;

    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        healthText.text = health.ToString();

        if (health <= 0)
        {
            playerController.DetachCamera();
            Despawn();
            //KillPlayerServer(this, gameObject, changeOnDeath, 3);
        }
    }

    public void ReceiveDamage(int amount)
    {
        health -= amount;
        healthText.text = health.ToString();

        if (health <= 0)
        {

            Despawn();
            //KillPlayerServer(this, gameObject, changeOnDeath, 3);
        }
    }
    /*

    [ServerRpc]
    public void KillPlayerServer(PlayerStats script, GameObject player, Behaviour[] toChange, int timeToWait, bool isRespawning=false)
    {
        script.KillPlayer(toChange);

        if(!isRespawning)
            script.StartCoroutine(RespawnTimer(script, player, toChange, timeToWait));
    }

    [ObserversRpc]
    public void KillPlayer(Behaviour[] toChange)
    {
        foreach (Behaviour item in toChange)
        {
            item.enabled = !item.enabled;
        }
    }

    public IEnumerator RespawnTimer(PlayerStats script, GameObject player, Behaviour[] toChange, int timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        KillPlayerServer(script, player, toChange, timeToWait, true);
    }*/
}