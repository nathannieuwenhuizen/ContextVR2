using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physicalSwatch : MonoBehaviour {

    [SerializeField] private HSVColorPanel ColorPanel;
    [SerializeField] private int swatchIndex;
    [SerializeField] private float pressDistance = .1f;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private int pressCooldown = 30;

    //Tweaking
    [SerializeField] private float maxPressDistance = .16f;
    private Vector3 initialPos;

    float startY;
    int cooldown;

    private void Start() {
        startY = transform.position.y;
        renderer.material.color = ColorPanel.swatches[swatchIndex];
        initialPos = transform.position;
    }

    private void FixedUpdate() {

        if (cooldown > 0) cooldown--;

        if (transform.position.y < startY - pressDistance && cooldown <= 0) {
            ColorPanel.PressSwatch(swatchIndex);

            cooldown = pressCooldown;
        }

        if(transform.position.y < startY - maxPressDistance)
        {
            transform.position = new Vector3 (initialPos.x, initialPos.y - maxPressDistance, initialPos.z);
        }

        if(transform.position.y > initialPos.y)
        {
            transform.position = initialPos;
        }
    }
}