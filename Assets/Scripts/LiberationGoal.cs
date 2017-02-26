using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiberationGoal : Goal
{
    public int maximumBlocks = 25;
    public int minimumBlocks = 0;

    private int actualBlocks;

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("In");
        if (col.tag == "Block" || col.tag == "BlockFrozen" || col.tag == "Untagged")
        {
            actualBlocks++;
            if (achieved)
            {
                Desactivate();
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        Debug.Log("Out");
        if (col.tag == "Block" || col.tag == "BlockFrozen" || col.tag == "Untagged")
        {
            actualBlocks--;
            if (actualBlocks <= minimumBlocks)
            {
                Activate();
            }
        }
    }
}
