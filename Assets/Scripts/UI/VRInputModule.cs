using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputModule : BaseInputModule {

    public Camera currentCamera;
    [SerializeField] private Canvas[] canvass;
    
    private GameObject currentObject = null;
    private PointerEventData data;

    public Canvas customerCanvas;

    public static VRInputModule instance;
    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Start() {
        data = new PointerEventData(eventSystem);
    }

    public override void UpdateModule()
    {
        base.UpdateModule();
    }
    public override void Process() {
        data.position = new Vector2(currentCamera.pixelWidth / 2, currentCamera.scaledPixelHeight / 2);

        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);

        HandlePointerExitAndEnter(data,currentObject);

        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.dragHandler);
        //Debug.Log("prcoess finish");
        //Debug.LogError("prcoess finish");

    }

    public PointerEventData getData() {
        return data;
    }

    public void ProcessPress() {
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(data.pointerPressRaycast.gameObject);
        data.pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerPressRaycast.gameObject);

        ExecuteEvents.Execute(data.pointerDrag,data, ExecuteEvents.beginDragHandler);
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerDownHandler);
    }

    public void ProcessRelease() {

        GameObject pointerRelease = ExecuteEvents.GetEventHandler<IPointerUpHandler>(data.pointerCurrentRaycast.gameObject);
        if (data.pointerPress == pointerRelease) ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);

        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.endDragHandler);

        data.pointerPress = null;
        data.pointerDrag = null;
        data.pointerCurrentRaycast.Clear();
    }

    public void setCam(Camera cam) {
        currentCamera = cam;
        for (int i = 0; i < canvass.Length; i++) {
            canvass[i].worldCamera = cam;
        }
        if (customerCanvas != null)
        {
            customerCanvas.worldCamera = cam;
        }
    }
}