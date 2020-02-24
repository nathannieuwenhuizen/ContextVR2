using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputModule : BaseInputModule {

    [HideInInspector] public Camera currentCamera;
    [SerializeField] private Canvas canvas;

    private GameObject currentObject = null;
    private PointerEventData data;

    protected override void Awake() {
        base.Awake();

        data = new PointerEventData(eventSystem);
    }

    public override void Process() {
        data.Reset();
        data.position = new Vector2(currentCamera.pixelWidth / 2, currentCamera.scaledPixelHeight / 2);

        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        currentObject = data.pointerCurrentRaycast.gameObject;

        m_RaycastResultCache.Clear();

        HandlePointerExitAndEnter(data,currentObject);
    }

    public PointerEventData getData() {
        return data;
    }

    public void ProcessPress() {
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(currentObject, data, ExecuteEvents.pointerDownHandler);

        if (newPointerPress == null) newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = currentObject;
    }

    public void ProcessRelease() {
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

        GameObject pointerUpHandeler = ExecuteEvents.GetEventHandler<IPointerUpHandler>(currentObject);

        if (data.pointerPress == pointerUpHandeler) ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);

        eventSystem.SetSelectedGameObject(null);

        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;
    }
}