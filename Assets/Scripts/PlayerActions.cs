/*using UnityEngine;
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
*/
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine.InputSystem;

public class PlayerActions : NetworkBehaviour
{
    public InputAction shoot;
    //public LayerMask playerLayer;
    float fireTimer;


    [SerializeField]
    private WeaponScript weaponScript;

    private void Update()
    {
        if (!base.IsOwner)
            return;

        if (shoot.IsPressed() | Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Got order to shoot");
            if (fireTimer <= 0)
            {
                Debug.Log("Did Shoot");
                Shoot();
                fireTimer = weaponScript.weaponStats.firerate;
            }
        }

        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;
    }

    private void Shoot()
    {
        ShootServer(weaponScript.weaponStats.damage, Camera.main.transform.position, Camera.main.transform.forward);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServer(int damageToGive, Vector3 position, Vector3 direction)
    {
        Debug.Log("Got Order server shoot");
        if (Physics.Raycast(position, direction, out RaycastHit hit, weaponScript.weaponStats.range) /*&& hit.transform.TryGetComponent(out PlayerStats enemyHealth)*/)
        {
            //enemyHealth.health -= damageToGive;
            if (hit.collider.CompareTag("Player"))
            {
                hit.transform.parent.gameObject.GetComponent<PlayerStats>().health -= damageToGive;
                Debug.Log("Gave damage");
            } 
        }
    }
}