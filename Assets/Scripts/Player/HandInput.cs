using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandInput : MonoBehaviour {
    
    [SerializeField] GameObject colorUI;
    [SerializeField] private float colorUIDistance;

    public SteamVR_Action_Boolean colorAction;
    public SteamVR_Action_Boolean triggerAction;
    public SteamVR_Action_Boolean holdAction;
    //public SteamVR_Action_Vector2 scaleAction;
    
    public SteamVR_Input_Sources handType;

    public UIBeam UIBeam;
    public Grabber grabber;

    public bool ToggleToggle;
    bool ui;

    public void ColorDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (!ToggleToggle) {
            colorUI.SetActive(true);
            UIBeam.drawline = true;
            UIBeam.eventSystem.currentCamera = UIBeam.cam;
            colorUI.transform.position = transform.forward * colorUIDistance + transform.position;
            colorUI.transform.LookAt(2* colorUI.transform.position - Camera.main.transform.position);
        } else {
            if(ui){
                colorUI.SetActive(false);
                UIBeam.drawline = false;
                ui = false;
            } else {
                colorUI.SetActive(true);
                UIBeam.drawline = true;
                UIBeam.eventSystem.currentCamera = UIBeam.cam;
                colorUI.transform.position = transform.forward * colorUIDistance + transform.position;
                colorUI.transform.LookAt(2* colorUI.transform.position - Camera.main.transform.position);
                ui = true;
            }
        }
    }
    public void ColorUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (!ToggleToggle) {
            colorUI.SetActive(false);
            UIBeam.drawline = false;
        }
    }

    public void OnEnable()
    {
        colorAction.AddOnStateDownListener(ColorDown, handType);
        colorAction.AddOnStateUpListener(ColorUp, handType);

        triggerAction.AddOnStateDownListener(TriggerDown, handType);
        triggerAction.AddOnStateUpListener(TriggerUp, handType);

        holdAction.AddOnStateDownListener(HoldDown, handType);
        holdAction.AddOnStateUpListener(HoldUp, handType);
    }

    public void OnDisable()
    {
        colorAction.RemoveOnStateDownListener(ColorDown, handType);
        colorAction.RemoveOnStateUpListener(ColorUp, handType);

        triggerAction.RemoveOnStateDownListener(TriggerDown, handType);
        triggerAction.RemoveOnStateUpListener(TriggerUp, handType);

        holdAction.RemoveOnStateDownListener(HoldDown, handType);
        holdAction.RemoveOnStateUpListener(HoldUp, handType);

    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        UIBeam.Press(); 
    }
    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        UIBeam.Release();
    }

    public void HoldDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        Debug.Log(" Grab!");
        grabber.Grab();
    }
    public void HoldUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)  {
        grabber.Release();
    }

    //private void Update() {
    //    if (scaleAction.axis == Vector2.zero) return;
    //    grabber.ScaleObject(scaleAction.axis.x);
    //}
}