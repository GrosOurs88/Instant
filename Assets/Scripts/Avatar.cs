using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour {

    private Transform cam;

    [Header("Block Manipulation")]
    private List<GameObject> ManyBlocksTaken = new List<GameObject>();
    private List<GameObject> Blocks = new List<GameObject>();
    private bool manyBlocksHold = false;
    private GameObject OneBlockTaken = null;
    private bool oneBlockHold = false;
    private bool oneFrozenBlockTargeted = false;
    private GameObject oneFrozenBlockHold = null;

    [Header("Selection Parameters")]
    public Image SelectionFdbck;
    private float selectionClicked = 0.0f;
    private float currentRadiusSelectionOfBlocks = 1.0f;
    private float maxRadiusSelection = 7.0f;
    private float radiusSelectionEvolve = 1.0f;
    public float maxDistToGrab;
    public float distToBlock = 5.0f;
    public float impulsion = 5.0f;
    private float clicked = 0.0f;

    [Header("Signal Parameters")] 
    private bool canEmitSignal = true;
    public float signalTimer;
    public float radiusSignal = 1.0f;
    public Image SignalFeedbackVisuel;
    public GameObject[] AllBlocks;
    public float timerE = 0.5f;
    public float signalRange = 5.0f;

    [Header("Mana Gestion")]
    private int manaCurrent;
    public int manaMax = 100;
    private int manaWillBeUsed = 0;
    public bool useThisFuckingMana = false;
    
    /* 
        - On peut vérifier qu'un block est saissisable, 
        saisir un block, 
        le relâcher,
        faire varier la distance entre nous et le block lorsque saisi (distance max et min modifiable),
        lui donner une impulsion

        - On peut vérifier que plusieurs blocks sont saissisables,
        saisir plusieurs blocks,
        les relâcher,
        faire varier la distance entre nous et les blocks lorsque saisis,
        leur donner une impulsion

        - On peut envoyer un signal (taille réglable dans l'inspecteur)

        - On peut défiger tous les blocks
         */


    private void Awake()
    {
        cam = gameObject.GetComponent<Camera>().transform;
        manaCurrent = manaMax;
        AllBlocks = GameObject.FindGameObjectsWithTag("Block");
    }

    private void Update()
    {
        SignalFeedbackVisuel.rectTransform.localScale = new Vector3(radiusSignal * 0.75f, radiusSignal * 0.75f, 1);
        SelectionFdbck.rectTransform.localScale = new Vector3(radiusSelectionEvolve, radiusSelectionEvolve, 1.0f);

        //// Gestion du mana ////
        if (useThisFuckingMana)
        {
            ManaUpdate();
            if (manaWillBeUsed > manaCurrent)
            {
                canEmitSignal = false;
            }
            else
            {
                canEmitSignal = true;
            }
        }

        //// Pour défiger tous les blocks ////
        if (Input.GetKey(KeyCode.E))
        {
            timerE -= Time.deltaTime;
            if (timerE < 0.0f)
            {
                UnfrozeOneBlock();
                oneFrozenBlockHold.BroadcastMessage("OnDesactivationSignal");
                timerE = 0.5f;
                manaCurrent = manaMax;
            }
            oneFrozenBlockHold = null;
        }

        //// Pour saisir un block ou plusieurs blocks ////
        if (Input.GetKey(KeyCode.Mouse1))
        {
            currentRadiusSelectionOfBlocks *= 1.01f;
            selectionClicked += 2 * Time.deltaTime;
            MultiSelection();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            if (radiusSelectionEvolve <= 2.0f)
            {
                if (oneBlockHold == false)
                {
                    TakeOneBlock();
                    currentRadiusSelectionOfBlocks = 1.0f;
                    radiusSelectionEvolve = currentRadiusSelectionOfBlocks;
                }
                else
                {
                    OneBlockTaken.BroadcastMessage("OnHold");
                    LeaveBlock();
                }
            }
            if (radiusSelectionEvolve > 2.0f)
            {
                if (manyBlocksHold == false)
                {
                    TakeSeveralBlocks();
                    currentRadiusSelectionOfBlocks = 1.0f;
                    radiusSelectionEvolve = currentRadiusSelectionOfBlocks;
                }
                else
                {
                    foreach (GameObject a in ManyBlocksTaken)
                    {
                        a.BroadcastMessage("OnHold");
                        LeaveBlock();
                    }
                }
            }
        }
        

        //// Lorsqu'un block est saisi ////
        if (oneBlockHold == true && manyBlocksHold == false)
        {
//            Debug.Log("et ici ?");
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Scroll vers le bas
            {
                distToBlock -= 0.5f; // ajouter variable dans l'inspecteur si besoin de tester ça aussi
                if (distToBlock < 3.0f)
                {
                    distToBlock = 3.0f;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // Scroll vers le haut
            {
                distToBlock += 0.5f; // Idem puisque même variable
                if (distToBlock > 10.0f)
                {
                    distToBlock = 10.0f;
                }
            }

            OneBlockTaken.transform.position = transform.position + transform.forward * distToBlock;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                clicked += 2 * Time.deltaTime;
                if (clicked > 1.0f)
                {
                    clicked = 1.0f;
                }
                impulsion = Mathf.Lerp(5f, 100f, clicked);
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                ImpulseBlock();
            }
        }

        //// Lorsque plusieurs blocks sont saisis ////
        if (manyBlocksHold == true && oneBlockHold == false)
        {
            Debug.Log("ça passe ici ?");
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Scroll vers le bas
            {
                distToBlock -= 0.5f; // ajouter variable dans l'inspecteur si besoin de tester ça aussi
                if (distToBlock < 3.0f)
                {
                    distToBlock = 3.0f;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // Scroll vers le haut
            {
                distToBlock += 0.5f; // Idem puisque même variable
                if (distToBlock > 7.0f)
                {
                    distToBlock = 7.0f;
                }
            }

            int numberOfBlokcsHold = ManyBlocksTaken.Count;
            //Vector3 pos = transform.position + transform.forward * (distToBlock + 360 / numberOfBlokcsHold);
            for (int i = 0; i < ManyBlocksTaken.Count; i++)
            {
                Debug.Log("aaaarttttta");
                float thisAngle = AngleOnACircle(i, numberOfBlokcsHold);
                Vector2 pos = PointOnACircle(distToBlock, i, numberOfBlokcsHold);
                ManyBlocksTaken[i].transform.position = pos;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                ImpulseSeveralBlocks();
            }
        }

        //// Pour émettre un signal ////
        if (Input.GetKey(KeyCode.Mouse2))
        {
            EmitSignal();
        }


        //// Lorsqu'aucun block n'est saisi ////
        /*if (OneBlockTaken == false)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Scroll vers le bas 
            {
                signalEvolve -= 0.1f;
                SignalShrink();
                // Ajouter Image Feedback evolution
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                signalEvolve += 0.1f;
                SignalGrow();
                // Ajouter Image Feedback evolution
            }
        }*/

    }

    private bool Pickable(out GameObject Block)
    {
        Ray r = new Ray(transform.position + cam.forward * 0.5f, cam.forward);
        RaycastHit hit;
        int layer_mask = Physics.DefaultRaycastLayers;
        if (Physics.Raycast(r, out hit, maxDistToGrab, layer_mask))
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

    private bool SeveralPickable (out List<GameObject> Blocks)
    {
        Blocks = new List<GameObject>();
        Vector3 maxDistVector = transform.position + cam.forward * maxDistToGrab;
        Collider[] cols = Physics.OverlapCapsule(transform.position, maxDistVector, 1.0f);
        if ( cols != null)
        {
            for (int i = 0; i < cols.Length; i ++)
            {
                if (cols[i].gameObject.tag == "Block")
                {
                    ManyBlocksTaken.Add(cols[i].gameObject);
                }
            }
            Blocks = ManyBlocksTaken;
            return true;
        }
        Blocks.Clear();
        return false;
    }

    private bool BlockFrozenPickable(out GameObject BlockFrozen)
    {
        Ray r = new Ray(transform.position + cam.forward * 0.5f, cam.forward);
        RaycastHit hit;
        int layer_mask = Physics.DefaultRaycastLayers;
        if (Physics.Raycast(r, out hit, maxDistToGrab, layer_mask))
        {
            if (hit.collider.gameObject.tag == "BlockFrozen")
            {
                BlockFrozen = hit.collider.gameObject;
                return true;
            }
        }
        BlockFrozen = null;
        return false;
    }

    private void TakeOneBlock()
    {
        GameObject target = null;
        if (Pickable(out target) == true)
        {
            oneBlockHold = true;
            manyBlocksHold = false;
            OneBlockTaken = target;
            OneBlockTaken.BroadcastMessage("OnHold");
            //BanqueSons.Catch.start();
        }
    }

    private void TakeSeveralBlocks()
    {
        if (SeveralPickable(out Blocks) == true)
        {
            manyBlocksHold = true;
            oneBlockHold = true;
            ManyBlocksTaken = Blocks;
            foreach (GameObject a in ManyBlocksTaken)
            {
                a.BroadcastMessage("OnHold");
                //BanqueSons.Catch.start();
            }
        }
    }

    public static float AngleOnACircle(int indexPoint, int nbTotalPoint, float startAngle = 0f, float angle = 360f)
    {
        float angleOnePoint = (angle / nbTotalPoint);
        float decalToInverse = (nbTotalPoint % 2 == 1) ? (3 * (angle / nbTotalPoint) / 4) : 0;
        return angleOnePoint * indexPoint + decalToInverse + startAngle;
    }

    public static Vector2 PointOnACircle(float radius, int indexPoint, int nbTotalPoint, float startAngle = 0f, float angle = 360f)
    {
        float currentAngle = AngleOnACircle(indexPoint, nbTotalPoint, startAngle, angle) * Mathf.Deg2Rad;
        return new Vector2(radius * Mathf.Cos(currentAngle), radius * Mathf.Sin(currentAngle));
    }

    private void UnfrozeOneBlock()
    {
        GameObject thisFrozenBlock = null;
        if (BlockFrozenPickable(out thisFrozenBlock) == true)
        {
            oneFrozenBlockTargeted = true;
            oneFrozenBlockHold = thisFrozenBlock;
        }
    }


    private void LeaveBlock()
    {
        oneBlockHold = false;
        OneBlockTaken = null;
        manyBlocksHold = false;
        ManyBlocksTaken.Clear();
    } 

    private void ImpulseBlock()
    {
        if (oneBlockHold == true)
        {
            OneBlockTaken.BroadcastMessage("OnHold");
			OneBlockTaken.GetComponent<Rigidbody> ().velocity = transform.forward * impulsion;
            Debug.Log(impulsion);
            LeaveBlock();
            //BanqueSons.Throw.start();
            impulsion = 5.0f;
            clicked = 0.0f;
        }
    }

    private void ImpulseSeveralBlocks()
    {
        foreach (GameObject a in ManyBlocksTaken)
        {
            a.BroadcastMessage("OnHold");
            a.GetComponent<Rigidbody>().AddForce(transform.forward * impulsion, ForceMode.Impulse);
       //     BanqueSons.Throw.start();
        }
        LeaveBlock();
    }

    private void MultiSelection()
    {
        radiusSelectionEvolve = Mathf.Lerp(radiusSelectionEvolve, currentRadiusSelectionOfBlocks, selectionClicked);
    } 

    private void EmitSignal()
    {
        if (canEmitSignal == true)
        {
            canEmitSignal = false;
            StartCoroutine("SignalTimer");
            Vector3 signalRangeCoord = transform.position + cam.forward * signalRange;
            Collider[] cols = Physics.OverlapCapsule(transform.position, signalRangeCoord, radiusSignal);
            foreach (Collider col in cols)
            {
                col.BroadcastMessage("OnSignal");
            }
            //BanqueSons.Signal.start();
            //manaCurrent -= manaWillBeUsed;
        }
    } 

    private IEnumerator SignalTimer()
    {
        yield return new WaitForSeconds(signalTimer);
        canEmitSignal = true;
    }

    /*private void SignalShrink()
    {
        float radiusSignalMin = 1.0f;
        radiusSignal = Mathf.Lerp(radiusSignalMin, radiusSignal, signalEvolve);
        //ManaUpdate();
    }*/

    /*private void SignalGrow()
    {
        float radiusSignalMax = 5.0f;
        radiusSignal = Mathf.Lerp(radiusSignal, radiusSignalMax, signalEvolve);
        //ManaUpdate();
    }*/

    private void ManaUpdate()
    {
        manaWillBeUsed = (int)Mathf.Lerp(5.0f, 30.0f, (radiusSignal - 1.0f) / 4.0f);
        //MaxMana.text = "Ressources Max :" + manaMax.ToString();
        //CurrentMana.text = "Ressources Actuelles :" + manaCurrent.ToString();
        int manaUsed = 0;
        manaUsed = manaCurrent - manaWillBeUsed;
        //UsedMana.text = "Ressources restantes après action :" + manaUsed.ToString();
    }
}
