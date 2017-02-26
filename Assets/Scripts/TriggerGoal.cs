using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGoal : Goal
{
    private int numberOfBlocksIn = 0;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Block" || col.tag == "BlockFrozen" || col.tag == "Untagged")
        {
            numberOfBlocksIn++;
            if (!achieved)
            {
                Activate();
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Block" || col.tag == "BlockFrozen" || col.tag == "Untagged")
        {
            numberOfBlocksIn--;
            if (numberOfBlocksIn <= 0)
            {
                numberOfBlocksIn = 0;
                Desactivate();
            }
        }
    }
}
