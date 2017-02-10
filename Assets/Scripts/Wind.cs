using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public bool active = false;
    public Vector3 appliedForce = Vector3.forward;
    public float multiplicator = 3.0f;

    private GameObject[] blocks;

    void Start()
    {
        blocks = GameObject.FindGameObjectsWithTag("Block");
    }

    void Update()
    {
        if (active)
        {
            foreach(GameObject go in blocks)
            {
                go.GetComponent<Rigidbody>().AddForce(appliedForce*multiplicator);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            active = !active;
        }
    }
}
