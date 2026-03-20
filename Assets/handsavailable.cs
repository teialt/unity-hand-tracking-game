using UnityEngine;

public class LeapInputDisabler : MonoBehaviour
{
    public MonoBehaviour leapMotionScript;

    public void DisableLeapInput()
    {
        if (leapMotionScript != null)
        {
            leapMotionScript.enabled = false;
            Debug.Log("Leap Motion control is forbidden£¡");
        }
    }

    public void EnableLeapInput()
    {
        if (leapMotionScript != null)
        {
            leapMotionScript.enabled = true;
            Debug.Log("Leap Motion control is activated£¡");
        }
    }
}