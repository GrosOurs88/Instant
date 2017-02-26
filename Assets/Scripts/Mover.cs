using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public GameObject path;
    public iTween.EaseType easeType = iTween.EaseType.linear;
    public iTween.LoopType loopType = iTween.LoopType.none;
    public float moveSpeed = 1.0f;
    public Goal trigger;
    public bool stopOnCollision = false;

    private bool isABlock;
    private bool useTrigger;
    private bool collision = false;
    private int numberOfCollision = 0;
    private Transform[] transforms;
    private bool active = false;
    private Block block;
    private Hashtable parameters = new Hashtable();

    void Start()
    {   
        transforms = path.GetComponentsInChildren<Transform>();
        parameters.Add("name", gameObject.name);
        parameters.Add("path", transforms);
        parameters.Add("speed", moveSpeed);
        parameters.Add("looptype", loopType);
        parameters.Add("easetype", easeType);
        if (trigger == null)
        {
            Activate();
            useTrigger = false;
        }
        else
        {
            useTrigger = true;
        }        
        block = GetComponent<Block>();
        if (block != null)
            isABlock = true;

        if (stopOnCollision)
        {
            GetComponent<Rigidbody>().mass = float.Epsilon;
        }
    }

    void Update()
    {
        if (isABlock && !useTrigger)
        {
            if (block.state == BlockState.FIXED)
            {
                Desactivate();
            }
            else
            {
                Activate();
            }
        }
        else if (isABlock && useTrigger)
        {
            if (block.state == BlockState.FIXED || trigger.achieved == false)
            {
                Desactivate();
            }
            else if (trigger.achieved)
            {
                Activate();
            }
        }
        else if (useTrigger)
        {
            if (trigger.achieved)
            {
                Activate();
            }
            else
            {
                Desactivate();
            }
        }
    }

    private void Activate()
    {
        if (!active && !collision)
        {
            active = true;
            if (GetComponent<iTween>() != null)
            {
                GetComponent<iTween>().isRunning = true;
            }
            
            iTween.MoveTo(gameObject, parameters);
        }   
    }

    private void Desactivate()
    {
        if (active)
        {
            active = false;
            GetComponent<iTween>().isRunning = false;
        }
    }

    void OnDrawGizmos()
    {
        iTween.DrawPath(transforms);
    }

    void OnCollisionEnter()
    {
        collision = true;
        //numberOfCollision++;
        Desactivate();
    }

    void OnCollisionExit()
    {
        //numberOfCollision--;
        
            //numberOfCollision = 0;
            collision = false;
            Activate();
        
    }
}
