using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine.InputSystem;


public class PlayerActions : NetworkBehaviour
{
    public InputAction shoot;

    [SerializeField]
    private WeaponScript weaponScript;

    [SerializeField]
    private PlayerStats playerData;

    void Update()
    {
        if (!playerData.isDead)
        {
            if (shoot.IsPressed())
            {
                RaycastHit target = weaponScript.Shoot();
                if (target.collider is not null)
                {
                    if (target.collider.CompareTag("Player"))
                    {
                        if (target.collider.name == "couille")
                        {
                            //il est mort
                        }

                        Debug.Log("Player " + target.collider.name + " has been touched");
                        Debug.Log("calling severrpc");
                        PlayerTouchedServer(target.transform.parent.gameObject, gameObject, weaponScript.weaponStats.damage);
                    }
                    else
                    {
                        Debug.Log(target.collider.name + " touched in PlayerAction");
                    }
                }


            }
        }
    }

    void OnEnable()
    {
        shoot.Enable();
    }

    void OnDisable()
    {
        shoot.Disable();
    }

    [ServerRpc]
    public void PlayerTouchedServer(GameObject target, GameObject shooter, int damage)
    {
        Debug.Log("in serverrpc");
        TakeDamage(target, damage);
    }

    [ObserversRpc]
    public void TakeDamage(GameObject player, int damage)
    {
        Debug.Log("entered observerrpc");
        player.GetComponent<PlayerStats>().Damage(damage);
    }
}
