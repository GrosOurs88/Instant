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
    public float numberOfSignalToUnfix = 3;
    public float timeBeforeBackToNeutral = 5.0f;

    [Header("Block's colors")]
    public Material neutralMaterial;
    public Material activeMaterial;
    public Material holdMaterial;
    public Material fixedMaterial;

    [SerializeField]
    private BlockState state = BlockState.NEUTRAL;

    private Rigidbody rigid;
    private Renderer rend;
    private cakeslice.Outline outline;
    private float signalsLeft;
    private bool isTimerRunning = false;

    private void Start()
    {   
        rigid = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        outline = GetComponent<cakeslice.Outline>();
        signalsLeft = numberOfSignalToUnfix;
        rend.material = neutralMaterial;
        outline.eraseRenderer = true;
    }

    private void Update()
    {
        UpdateState();
        GizmosService.Text(state.ToString() + '\n' + rigid.velocity.magnitude.ToString(), transform.position + transform.up * 1.5f);
    }

    private void UpdateState()
    {
        switch(state)
        {
            
            case BlockState.NEUTRAL:
                if (IsSleeping() == false)
                {
                    SwitchState(BlockState.ACTIVE);
                }
                break;
                

            case BlockState.ACTIVE:
                if (IsSleeping() && !isTimerRunning)
                {
                    StartCoroutine("FadeToGrey");
                }

                if (IsSleeping() == false && isTimerRunning)
                {
                    StopAllCoroutines();
                    isTimerRunning = false;
                    rend.material = activeMaterial;
                }
                break;

            case BlockState.FIXED:
                if (signalsLeft <= 0)
                {
                    SwitchState(BlockState.NEUTRAL);
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
                tag = "Block";
                rend.material = neutralMaterial;
                rigid.isKinematic = false;
                break;

            case BlockState.ACTIVE:
                tag = "Block";
                rend.material = activeMaterial;
                rigid.isKinematic = false;
                break;

            case BlockState.FIXED:
                tag = "Untagged";
                rend.material = fixedMaterial;
                rigid.isKinematic = true;
                break;

            case BlockState.HOLD:
                outline.eraseRenderer = false;
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).rotation = Quaternion.identity;
                transform.GetChild(0).position = transform.position + new Vector3(0, -10.0f, 0);
                tag = "Untagged";
                rend.material = holdMaterial;
                rigid.isKinematic = true;
                break;
        }
    }

    private void ExitState()
    {
        switch(state)
        {
            case BlockState.HOLD:
                outline.eraseRenderer = true;
                transform.GetChild(0).gameObject.SetActive(false);
                break;

            case BlockState.FIXED:
                signalsLeft = numberOfSignalToUnfix;
                break;
        }
    }

    private void SwitchState(BlockState newState)
    {
        ExitState();
        state = newState;
        EnterState();
    }

    private IEnumerator FadeToGrey()
    {
        isTimerRunning = true;
        Color baseColor = rend.material.color;
        Color targetColor = neutralMaterial.color - rend.material.color*0.15f;
        float value = 0.0f;
        while (value < timeBeforeBackToNeutral)
        {
            rend.material.color = Color.Lerp(baseColor, targetColor, value/timeBeforeBackToNeutral);
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
        else if (state == BlockState.FIXED)
        {
            signalsLeft--;
            rend.material.color = Color.Lerp(fixedMaterial.color , neutralMaterial.color - fixedMaterial.color * 0.15f, (-1.0f/numberOfSignalToUnfix) * signalsLeft + 1.0f);
        }
    }

    private void OnHold()
    {
        Debug.Log("OnHold");
        if (state == BlockState.ACTIVE || state == BlockState.NEUTRAL)
        {
            SwitchState(BlockState.HOLD);
        }
        else if (state == BlockState.HOLD)
        {
            SwitchState(BlockState.ACTIVE);
        }
    }

    private bool IsSleeping()
    {
        return rigid.velocity.magnitude < 1.0f;
    }

}
