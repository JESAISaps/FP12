using System.Collections;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;


public class DisableComponents : NetworkBehaviour
{
    [SerializeField]
    private Component[] componentToDisable;

    // Start is called before the first frame update
    public override void OnStartClient()

    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            foreach (MonoBehaviour item in componentToDisable)
            {
                item.enabled = false;
            }
        }
    }
}
