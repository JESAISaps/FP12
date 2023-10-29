using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField]
    private WeaponData[] weaponStats;
    // important que l'arme et sont ScriptableObject soient en meme indice
    [SerializeField]
    private GameObject[] weapons;

    [HideInInspector]
    public WeaponData currentWeaponData;

    private Transform shootCameraTransform;

    private void Start()
    {
        shootCameraTransform = Camera.main.transform;


        foreach (GameObject weapon in weapons)
        {
            SetWeaponLayerRecursively(weapon, 3);
        }

        currentWeaponData = weaponStats[0];
    }
    /*
    public RaycastHit Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(shootCameraTransform.position, shootCameraTransform.TransformDirection(Vector3.forward), out hit, weaponStats.range))
        {
            //Debug.Log(hit + " touched");
            //if hit.name == "Player"{
            Debug.Log(hit.collider.name);
        }

        return hit;
    }*/
    /*
    private void Update()
    {
        // temporaire pour les tirs
        if (Input.GetButtonDown("Fire1")){
            Shoot();
        }
    }*/
    private void SetWeaponLayerRecursively(GameObject weapon, int layer)
    {
        weapon.layer = layer; // met la layer 3 qui est celle de weapon

        // applique la layer de maniere recursive a tous les enfants
        foreach(Transform child in weapon.transform)
        {
            child.gameObject.layer = layer;

            if (child.GetComponentInChildren<Transform>())
            {
                SetWeaponLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
