﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarActions : MonoBehaviour {

    [Header("Block Manipulation")]
    private Transform cam;
    private GameObject target = null;
    private GameObject Block_taken_GO = null;
    public List<GameObject> Blocks_taken = new List<GameObject>();
    private List<GameObject> Blocks = new List<GameObject>();
    public float dist_to_Block = 5.0f;
    private float dist_to_BlockMax = 7.0f;
    private float dist_to_BlockMin = 3.0f;
    private bool block_Taken = false;
    private bool Many_block_taken = false;
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
    public GameObject[] All_Blocks = new GameObject[10];
    private float timerE = 1.5f;

    [Header("Resources")]
    private int manaMax = 100;
    private int manaCurrent;
    private int manaWorth = 0;
    private int manaUsed = 0;
    public Text manaC;
    public Text manaW;
    public Text manaM;

	void Start () {
        cam = gameObject.GetComponent<Camera>().transform;
        manaCurrent = manaMax;
        All_Blocks = GameObject.FindGameObjectsWithTag("Block");
	}
	

	void Update () {
        ManaDistribution();

        if (manaWorth > manaCurrent)
        {
            canEmitSignal = false;
        }

        if (Input.GetKey(KeyCode.E))
        {
            timerE -= Time.deltaTime;
            if (timerE < 0.0f)
            {
                foreach (GameObject e in All_Blocks)
                {
                    e.BroadcastMessage("OnDesactivationSignal");
                }
                timerE = 1.5f;
                manaCurrent = manaMax;
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            radiusSelection *= 1.01f;
            RadiusSelectionGrowUp();
            ManaDistribution();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (radiusSelected < 2.0f)
            {
                if (!block_Taken)
                {
                    TakeBlock();
                    radiusSelection = 1.0f;
                    radiusSelected = radiusSelection;
                }
                else
                {
                    Block_taken_GO.BroadcastMessage("OnHold");
                    LeaveBlock();
                }
            }
            if (radiusSelected > 2.0f)
            {
                if (!Many_block_taken)
                {
                    TakeSeveralBlock();
                    radiusSelection = 1.0f;
                    radiusSelected = radiusSelection;
                }
                else
                {
                    foreach (GameObject c in Blocks_taken)
                    {
                        c.BroadcastMessage("OnHold");
                        LeaveBlock();
                    }
                       
                }
            }
        }

        if (block_Taken || Many_block_taken)
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
            if (block_Taken == true && Many_block_taken == false)
            {
                Block_taken_GO.transform.position = transform.position + transform.forward * dist_to_Block;
            }
            else if (block_Taken == false && Many_block_taken == true)
            {
                foreach (GameObject b in Blocks_taken)
                {
                    b.transform.position = transform.position + transform.forward * dist_to_Block;
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ImpulseBlock();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            EmitSignal();
        }

        if (!block_Taken || !Many_block_taken)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Bas
            {
                signalEvolve -= 0.1f;
                radiusSignal = Mathf.Lerp(signalRadiusMin, radiusSignal, signalEvolve);
                SignalFdbck.rectTransform.localScale = new Vector3(radiusSignal * 0.75f, radiusSignal * 0.75f, 1);
                ManaDistribution();
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // Haut
            {
                signalEvolve += 0.1f;
                radiusSignal = Mathf.Lerp(radiusSignal, signalRadiusMax, signalEvolve);
                SignalFdbck.rectTransform.localScale = new Vector3(radiusSignal * 0.75f, radiusSignal * 0.75f, 1);
                ManaDistribution();
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
			BanqueSons.Catch.start();
        }
    }

    private void TakeSeveralBlock()
    {
        if (SeveralPickable(out Blocks) == true)
        {
            Many_block_taken = true;
            Blocks_taken = Blocks;
            foreach (GameObject a in Blocks_taken)
            {
                a.BroadcastMessage("OnHold");
                //BanqueSons.Catch.start();
            }
        }
    }

    private bool SeveralPickable(out List<GameObject> Blocks)
    {
        Blocks = new List<GameObject>();
        Vector3 maxDistVector = transform.position + cam.forward * maxDist;
        Collider[] cols = Physics.OverlapCapsule(transform.position, maxDistVector, radiusSelected);
        if (cols != null)
        {
            for (int i = 0; i < cols.Length; i ++) 
            {
                if (cols[i].gameObject.tag == "Block")
                {
                    Debug.Log(cols[i]);
                    Blocks_taken.Add(cols[i].gameObject);
                }
            }
            Blocks = Blocks_taken;
            return true;
        }
        Blocks.Clear();
        return false;
    }

    private void RadiusSelectionGrowUp()
    {
        radiusSelected = Mathf.Lerp(radiusSelected, radiusSelection, 0.1f);
        SelectionFdbck.rectTransform.localScale = new Vector3(radiusSelected, radiusSelected, 1);

        ManaDistribution();
    }

    private void LeaveBlock()
    {
        block_Taken = false;
        Block_taken_GO = null;
        Many_block_taken = false;
        Blocks_taken.Clear();
    }

    private void ImpulseBlock()
    {
        if (block_Taken == true)
        {
            Block_taken_GO.BroadcastMessage("OnHold");
            Block_taken_GO.GetComponent<Rigidbody>().AddForce(transform.forward * impulsion, ForceMode.Impulse);
            LeaveBlock();
            BanqueSons.Throw.start();
        }
        else if (Many_block_taken == true)
        {
            foreach (GameObject d in Blocks_taken)
            {
                d.BroadcastMessage("OnHold");
                d.GetComponent<Rigidbody>().AddForce(transform.forward * impulsion, ForceMode.Impulse);
                BanqueSons.Throw.start();
            }
            LeaveBlock();
        }
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
            manaCurrent -= manaWorth;
        }
    }

    private void ManaDistribution()
    {
        if (radiusSelected >= 1.0f || radiusSelected < 1.4f)
        {
            manaWorth = 5;
        }
        if (radiusSelected >= 1.5f && radiusSelected < 2.9f)
        {
            manaWorth = 10;
        }
        if (radiusSelected >= 3.0f && radiusSelected < 4.9f)
        {
            manaWorth = 20;
        }
        if (radiusSelected == 5.0f)
        {
            manaWorth = 30;
        }

        manaM.text = "Ressources Max :" + manaM.ToString();
        manaC.text = "Ressources Actuelles :" + manaCurrent.ToString();
        manaUsed = manaCurrent - manaWorth;
        manaW.text = "Ressources Restantes après action :" + manaUsed.ToString();
    }


    private IEnumerator SignalTimer()
    {
        yield return new WaitForSeconds(signalTimer);
        canEmitSignal = true;
    }

}
