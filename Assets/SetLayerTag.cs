using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLayerTag : MonoBehaviour
{
    [SerializeField]
    private int layer;

    private void Start()
    {
        SetLayerRecursively(gameObject, layer);
	}

    void SetLayerRecursively(GameObject graphic, int layer)
    {
        foreach (Transform child in graphic.transform)
        {
            child.gameObject.layer = layer;

            if (child.GetComponentInChildren<Transform>())
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
