using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandInput : MonoBehaviour {
    
    public SteamVR_Action_Boolean triggerAction;
    public SteamVR_Action_Boolean holdAction;
    public SteamVR_Action_Vector2 scaleAction;
    
    public SteamVR_Input_Sources handType;

    public UIBeam UIBeam;
    public Grabber grabber;

    void Start() {
        triggerAction.AddOnStateDownListener(TriggerDown, handType);
        triggerAction.AddOnStateUpListener(TriggerUp, handType);

        holdAction.AddOnStateDownListener(HoldDown, handType);
        holdAction.AddOnStateUpListener(HoldUp, handType);

        scaleAction.AddOnUpdateListener(scale, handType);
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        UIBeam.Press(); 
    }
    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        UIBeam.Release();
    }

    public void HoldDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        grabber.Grab();
    }
    public void HoldUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)  {
        grabber.Release();
    }

    //still doesent work
    public void scale(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) {
        Debug.Log("axis control: " + axis.y);
        grabber.ScaleObject(axis.y);
        grabber.ScaleObject(Input.GetAxis("horizontal"));
    }
}