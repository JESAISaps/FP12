using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class SetTag : NetworkBehaviour
{
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (base.Owner.IsLocalClient)
        {
            gameObject.tag = "MainCamera";
        }
    }
}
