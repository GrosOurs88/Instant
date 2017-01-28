using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarActions : MonoBehaviour {


    private Transform cam;
    private GameObject target = null;
    private GameObject Block_taken = null;
    private int dist_to_Block = 10;
    private bool block_Taken = false;

    public float maxDist;

	void Start () {
        cam = gameObject.GetComponent<Camera>().transform;
	}
	

	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!block_Taken)
            {
                TakeBlock();
            }
            else
            {
                LeaveBlock();
            }
        }

        if (block_Taken)
        {
            Block_taken.transform.position = transform.position + transform.forward * dist_to_Block;
        }
        
    }

    private bool Pickable(out GameObject Block) // vérifie si un Block peut être saisi dans la distance maxDist
    {
        Ray r = new Ray(transform.position, cam.forward);
        RaycastHit hit;
        int layer_mask = Physics.DefaultRaycastLayers;
        if (Physics.Raycast(r, out hit, maxDist, layer_mask))
        {
            if (hit.collider.gameObject.tag == "Block")
            {
                Block = hit.collider.gameObject;
                return true;
            }
        }
        Block = null;
        return false;
    }

    private void TakeBlock()
    {
        if (Pickable(out target) == true)
        {
            block_Taken = true;
            Block_taken = target;
            Block_taken.BroadcastMessage("OnHold");
        }
        /*else if (Pickable(50f, out target) == false)
        {
            block_Taken = false;
            Block_taken = null;
        }*/
    }

    private void LeaveBlock()
    {
        block_Taken = false;
        Block_taken = null;
    }
}
