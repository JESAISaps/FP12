using FishNet.Object;
using UnityEngine;

public class SetLayerTag : NetworkBehaviour
{
    [SerializeField]
    private int layer;

    private void Start()
    {
        SetLayerRecursively(gameObject, layer);
	}

    void SetLayerRecursively(GameObject graphic, int layer)
    {
        if (base.Owner.IsLocalClient)
        {
            graphic.layer = layer;

            foreach (Transform child in graphic.transform)
            {
                //child.gameObject.layer = layer;

                if (child.GetComponentInChildren<Transform>())
                {
                    SetLayerRecursively(child.gameObject, layer);
                }
            }
        }
    }
}
