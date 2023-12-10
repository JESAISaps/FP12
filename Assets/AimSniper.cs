using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed class AimSniper : View
{
    [SerializeField]
    private GameObject scopePanel;

    private Camera weaponCamera;

    bool didSetWeaponCamera = false;

    // Update is called once per frame
    private void Update()
    {/*
        if (!Initialized)
        {
            Debug.Log("Not Initialized in aim sniper");
            return;
        }*/

        // le test de Initialized ne marche pas pour certaines raisons, donc on doit etre plus archaique
        Player player = Player.Instance;
        if (player == null)
        {
            return;
        }

        if (!didSetWeaponCamera)
        {
            SetWeaponCamera(player);
            didSetWeaponCamera = true;
        }

        if (player.controlledPawn.weaponScript.currentWeapon == 1)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (scopePanel != null)
                    scopePanel.SetActive(true);
                // cette ligne va chercher la camera dans le stack de la camera principale
                weaponCamera.enabled = false;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (scopePanel != null)
                    scopePanel.SetActive(false);
                weaponCamera.enabled = true;
            }
        }
    }

    // fonction appelé une fois dans update puis plus jamais (j'espere car elle est lourde)
    void SetWeaponCamera(Player player)
    {
        weaponCamera = player.controlledPawn.weaponScript.shootCamera.GetComponent<Camera>().GetUniversalAdditionalCameraData().cameraStack[0].gameObject.GetComponent<Camera>();
    }
}
