using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarActions : MonoBehaviour {


    private Transform cam;
    private GameObject target = null;
    private GameObject Block_taken_GO = null;
    public float dist_to_Block = 5;
    private bool block_Taken = false;
    private bool canEmitSignal = true;

    public float maxDist;
    public float impulsion;
    public float signalTimer;
    public float signalRange = 5.0f;
    private Vector3 signalRangeCoord;

    public float radiusSignal = 1.0f;
    private float signalRadiusMax = 5.0f;
    private float signalRadiusMin = 1.0f;
    private float manaMax = 10.0f;
    private float mana;

	void Start () {
        cam = gameObject.GetComponent<Camera>().transform;
        mana = manaMax;
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
                Block_taken_GO.BroadcastMessage("OnHold");
                LeaveBlock();
            }
        }

        if (block_Taken)
        {
            Block_taken_GO.transform.position = transform.position + transform.forward * dist_to_Block;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ImpulseBlock();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            EmitSignal();
        }
        
    }

    private bool Pickable(out GameObject Block) // vérifie si un Block peut être saisi dans la distance maxDist
    {
        Ray r = new Ray(transform.position + cam.forward*0.5f, cam.forward);
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
            Block_taken_GO = target;
            Block_taken_GO.BroadcastMessage("OnHold");
			BanqueSons.Catch.start ();
        }
    }

    private void LeaveBlock()
    {
        block_Taken = false;
        Block_taken_GO = null;
    }

    private void ImpulseBlock()
    {
        Block_taken_GO.BroadcastMessage("OnHold");
        Block_taken_GO.GetComponent<Rigidbody>().AddForce(transform.forward * impulsion, ForceMode.Impulse);
        LeaveBlock();
		BanqueSons.Throw.start ();
    }

    private void EmitSignal()
    {
        if (canEmitSignal)
        {
            Debug.Log("Signal emitted");
            canEmitSignal = false;
            StartCoroutine("SignalTimer");
            /*Collider[] cols = Physics.OverlapSphere(transform.position, signalRange);
            foreach (Collider col in cols)
            {
                col.BroadcastMessage("OnSignal");
            }*/
            signalRangeCoord = transform.position + cam.forward * signalRange;
            Collider[] cols = Physics.OverlapCapsule(transform.position, signalRangeCoord, radiusSignal);
            foreach (Collider col in cols)
            {
                col.BroadcastMessage("OnSignal");
            }
            BanqueSons.Signal.start ();
        }
    }

    private IEnumerator SignalTimer()
    {
        yield return new WaitForSeconds(signalTimer);
        canEmitSignal = true;
    }

    /*private IEnumerator SignalSize()
    {

        yield return null;
    }*/
}
