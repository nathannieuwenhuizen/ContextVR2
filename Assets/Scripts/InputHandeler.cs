﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputHandeler : MonoBehaviour
{

    // a reference to the action
    public SteamVR_Action_Boolean triggerAction;
    // a reference to the hand
    public SteamVR_Input_Sources handType;
    //reference to the sphere
    public VRGrabber grabber;

    void Start()
    {
        triggerAction.AddOnStateDownListener(TriggerDown, handType);
        triggerAction.AddOnStateUpListener(TriggerUp, handType);
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1 pressed");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Fire2 pressed");
        }
    }
    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is up");
        grabber.Release();
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is down");
        grabber.Grab();
    }
}
