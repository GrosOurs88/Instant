using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarActions : MonoBehaviour {

    [Header("Block Manipulation")]
    private Transform cam;
    private GameObject target = null;
    private GameObject Block_taken_GO = null;
    private GameObject[] Blocks_taken;
    public float dist_to_Block = 5.0f;
    private float dist_to_BlockMax = 7.0f;
    private float dist_to_BlockMin = 3.0f;
    private bool block_Taken = false;
    public float maxDist;
    public float impulsion;
    private float radiusSelection = 1.0f;
    private float radiusSelectionMax = 7.0f;
    private float radiusSelected = 1.0f;
    public Image SelectionFdbck;


    [Header("Signal Parameters")]
    private bool canEmitSignal = true;
    public float signalTimer;
    public float signalRange = 5.0f;
    private Vector3 signalRangeCoord;
    public float radiusSignal = 1.0f;
    private float signalRadiusMax = 5.0f;
    private float signalRadiusMin = 1.0f;
    public float signalEvolve = 1.0f;
    public Image SignalFdbck; 

    [Header("Resources")]
    private float manaMax = 10.0f;
    private float mana;

	void Start () {
        cam = gameObject.GetComponent<Camera>().transform;
        mana = manaMax;
	}
	

	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            radiusSelection += 1 * Time.deltaTime; 
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Debug.Log(radiusSelection);
            radiusSelected = Mathf.Lerp(radiusSelected, radiusSelection, 0.1f);
            SelectionFdbck.rectTransform.localScale = new Vector3(radiusSelected, radiusSelected, 1);
            if (radiusSelected < 1.2f)
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
            if (radiusSelected > 1.2f)
            {
                TakeSeveralBlock();
            }
        }

        if (block_Taken)
        {
            if (Input.GetAxis("Mouse ScrollWheel") <0) // Bas
            {
                dist_to_Block -= 0.5f;
                if (dist_to_Block < dist_to_BlockMin)
                {
                    dist_to_Block = dist_to_BlockMin;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") >0) //Haut
            {
                dist_to_Block += 0.5f;
                if (dist_to_Block > dist_to_BlockMax)
                {
                    dist_to_Block = dist_to_BlockMax;
                }
            }
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

        if (!block_Taken)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Bas
            {
                signalEvolve -= 0.1f;
                radiusSignal = Mathf.Lerp(signalRadiusMin, radiusSignal, signalEvolve);
                SignalFdbck.rectTransform.localScale = new Vector3(radiusSignal * 0.75f, radiusSignal * 0.75f, 1);
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // Haut
            {
                signalEvolve += 0.1f;
                radiusSignal = Mathf.Lerp(radiusSignal, signalRadiusMax, signalEvolve);
                SignalFdbck.rectTransform.localScale = new Vector3(radiusSignal * 0.75f, radiusSignal * 0.75f, 1);
            }
        }
    }

    private bool Pickable(out GameObject Block) // Vérifie si un Block peut être saisi dans la distance maxDist
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

    private void TakeSeveralBlock()
    {
        Vector3 maxDistVector = transform.position + cam.forward * maxDist;
        Collider[] cols = Physics.OverlapCapsule(transform.position, maxDistVector, radiusSelection);
        for (int i =0; i < cols.Length; i++)
        {
            Blocks_taken[i] = cols[i].gameObject;
        }
        foreach (GameObject a in Blocks_taken)
        {
            a.transform.position = transform.position + transform.forward * dist_to_Block;
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

}
