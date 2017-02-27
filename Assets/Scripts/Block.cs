using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockState
{
    NEUTRAL, HOLD, ACTIVE, FIXED
}

public class Block : MonoBehaviour
{
    [Header("Block's parameters")]
    public float numberOfSignalToUnfix = 3; // OBSOLESSENCE PROGRAMMEEEEEEEEE !!!!!
    public float timeBeforeBackToNeutral = 5.0f;
    public float timeBeforeUnfix = 5.0f;
    public float triggerVelocity = 1.0f;
    public bool canBeHold = true;
    public BlockState state = BlockState.NEUTRAL;

    [Header("Block's colors")]
    public Material neutralMaterial;
    public Material activeMaterial;
    public Material holdMaterial;
    public Material fixedMaterial;
    public Mesh normalMesh;
    public Mesh fixedMesh;
    public bool useNewMeshes = false;

    private Rigidbody rigid;
    private Renderer rend;
    private MeshFilter mesh;
    private cakeslice.Outline outline;
    private float signalsLeft;
    private bool isTimerRunning = false;

    private static bool autoUnfix = false;

    private void Start()
    {   
        rigid = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        signalsLeft = numberOfSignalToUnfix;
        rend.material = neutralMaterial;
        Destroy(GetComponent<cakeslice.Outline>());
        if (canBeHold)
        {
            gameObject.tag = "Block";
        }
        else
        {
            gameObject.tag = "Untagged";
        }
        if (useNewMeshes)
        {
            mesh = GetComponent<MeshFilter>();
            mesh.mesh = normalMesh;
        }        
    }


    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch(state)
        {
            
            case BlockState.NEUTRAL:
                if(IsSleeping() == false)
                    SwitchState(BlockState.ACTIVE);
                break;
                

            case BlockState.ACTIVE:
                if (IsSleeping() && !isTimerRunning)
                {
                    StartCoroutine("FadeToGrey", timeBeforeBackToNeutral);
                }

                if (IsSleeping() == false && isTimerRunning)
                {
                    StopAllCoroutines();
                    isTimerRunning = false;
                    rend.material = activeMaterial;
                }
                break;
        }
    }

    private void EnterState()
    {
        StopAllCoroutines();
        isTimerRunning = false;
        switch (state)
        {
            case BlockState.NEUTRAL:
                if (canBeHold)
                    tag = "Block";
                rend.material = neutralMaterial;
                break;

            case BlockState.ACTIVE:
                if (canBeHold)
                    tag = "Block";
                StartCoroutine("FadeToGrey", timeBeforeBackToNeutral);
                rend.material = activeMaterial;
                break;

            case BlockState.FIXED:
                tag = "BlockFrozen";
                rend.material = fixedMaterial;
                DesactivatePhysics();
                if (autoUnfix)
                    StartCoroutine("FadeToGrey", timeBeforeUnfix);
                if (useNewMeshes)
                {
                    mesh.mesh = fixedMesh;
                }
                break;

            case BlockState.HOLD:
                ActiveOutline();
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).rotation = Quaternion.identity;
                transform.GetChild(0).position = transform.position + new Vector3(0, -10.0f, 0);
                tag = "Untagged";
                rend.material = holdMaterial;
                DesactivatePhysics();
                DesactiveCollision();
                break;
        }
    }

    private void ExitState()
    {
        switch(state)
        {
            case BlockState.HOLD:
                DesactivateOutline();
                ActivatePhysics();
                ActivateCollision();
                transform.GetChild(0).gameObject.SetActive(false);
                break;

            case BlockState.FIXED:
                ActivatePhysics();
                signalsLeft = numberOfSignalToUnfix;
                if (useNewMeshes)
                {
                    mesh.mesh = normalMesh;
                }                
                break;
        }
    }

    private void SwitchState(BlockState newState)
    {
        ExitState();
        state = newState;
        EnterState();
    }

    private IEnumerator FadeToGrey(float timer)
    {
        isTimerRunning = true;
        Color baseColor = rend.material.color;
        Color targetColor = neutralMaterial.color - rend.material.color*0.15f;
        float value = 0.0f;
        while (value < timer)
        {
            rend.material.color = Color.Lerp(baseColor, targetColor, value/timer);
            value += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        SwitchState(BlockState.NEUTRAL);
    }

    private void OnSignal()
    {
        if (state == BlockState.ACTIVE)
        {
            SwitchState(BlockState.FIXED);
        }
        /*
        else if (state == BlockState.FIXED)
        {
            SwitchState(BlockState.ACTIVE);
        }
        */
    }

    private void OnDesactivationSignal()
    {
        if(state == BlockState.FIXED)
        {
            SwitchState(BlockState.NEUTRAL);
        }
    }

    private void OnHold()
    {
        if (state == BlockState.ACTIVE || state == BlockState.NEUTRAL)
        {
            SwitchState(BlockState.HOLD);
        }
        else if (state == BlockState.HOLD)
        {
            SwitchState(BlockState.ACTIVE);
        }
    }

    private void DesactivatePhysics()
    {
        rigid.isKinematic = true;
    }

    private void ActivatePhysics()
    {
        rigid.isKinematic = false;
    }

    private void DesactiveCollision()
    {
        rigid.detectCollisions = false;
    }

    private void ActivateCollision()
    {
        rigid.detectCollisions = true;
    }

    private void ActiveOutline()
    {
        outline = gameObject.AddComponent<cakeslice.Outline>();
    }

    private void DesactivateOutline()
    {
        Destroy(outline);
    }

    private bool IsSleeping()
    {
        return rigid.velocity.magnitude < triggerVelocity;
    }

}
