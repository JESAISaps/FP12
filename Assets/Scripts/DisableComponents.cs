using System.Collections;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;


public class DisableComponents : NetworkBehaviour
{
    [SerializeField]
    private Component[] toDisable;

    // Start is called before the first frame update
    public override void OnStartClient()

    {
        if (!base.IsOwner)
        {
            foreach (MonoBehaviour item in toDisable)
            {
                item.enabled = false;
            }
        }
    }
}
