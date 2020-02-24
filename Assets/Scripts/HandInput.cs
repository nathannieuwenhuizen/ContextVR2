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

    private void Update() {


        if (scaleAction.axis == Vector2.zero) return;
        grabber.ScaleObject(scaleAction.axis.x);

    }
}