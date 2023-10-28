using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField]
    public WeaponData weaponStats;

    private Transform shootCameraTransform;

    private void Start()
    {
        shootCameraTransform = Camera.main.transform;
        SetWeaponLayerRecursively(transform);
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

    private void SetWeaponLayerRecursively(Transform weaponPart)
    {
        weaponPart.gameObject.layer = 3; // met la layer 3 qui est celle de weapon

        if (weaponPart.childCount == 0)
            return;
        foreach (Transform child in transform)
        {
            SetWeaponLayerRecursively(child.transform);
        }
    }
}
