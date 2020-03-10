using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputModule : BaseInputModule {

    public Camera currentCamera;
    [SerializeField] private Canvas[] canvass;

    private GameObject currentObject = null;
    private PointerEventData data;

    protected override void Start() {
        base.Awake();

        data = new PointerEventData(eventSystem);
        data.position = new Vector2(currentCamera.pixelWidth / 2, currentCamera.scaledPixelHeight / 2);
    }

    public override void Process() {
        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);

        HandlePointerExitAndEnter(data,currentObject);

        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.dragHandler);
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
    }
}