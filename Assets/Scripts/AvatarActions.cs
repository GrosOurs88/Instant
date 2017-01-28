using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarActions : MonoBehaviour {


    private Transform cam;

	void Start () {
        cam = gameObject.GetComponent<Camera>().transform;
	}
	

	void Update () {
        TakeBlock();
	}

    private bool Pickable(float maxDist) // vérifie si un Block peut être saisi dans la distance maxDist
    {
        Ray r = new Ray(transform.position, cam.forward);
        RaycastHit hit;
        int layer_mask = Physics.DefaultRaycastLayers;
        if (Physics.Raycast(r, out hit, maxDist, layer_mask))
        {
            if (hit.collider.gameObject.tag == "Block")
            {
                return true;
            }
        }
        return false;
    }

    private void TakeBlock()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Pickable(50f) == true)
            {
                
            }
        }
    }
}
