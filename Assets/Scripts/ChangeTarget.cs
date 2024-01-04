using UnityEngine;
using UnityEngine.Animations.Rigging;
using FishNet.Object;
using FishNet.Component.Animating;


public class ChangeTarget : NetworkBehaviour
{

    [SerializeField]
    private RigBuilder rig;

    [ServerRpc]
    public void ChangeHandleTarget(int weaponIndex)
    {
        ChangeHandleTargetObserver(weaponIndex);
    }

    [ObserversRpc]
    private void ChangeHandleTargetObserver(int weaponIndex)
    {
        rig.layers[1 - weaponIndex].active = false;
        rig.layers[weaponIndex].active = true;

        rig.Build();
    }
    
}
