using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customerMaterials : MonoBehaviour {
    [System.Serializable]
    public struct MaterialPreset {
        public Material face;
        public Material arms;
        public Material torso;
        public Material legs;
    }

    [SerializeField] private MaterialPreset[] CustomerPresets;

    [SerializeField] private MeshRenderer face;
    [SerializeField] private SkinnedMeshRenderer[] arms;
    [SerializeField] private SkinnedMeshRenderer torso;
    [SerializeField] private SkinnedMeshRenderer legs;

    //public int i;
    //private void Update() {
    //    SetMaterial(i);
    //}

    public void SetMaterial(int i) {
        if (i < CustomerPresets.Length && i >= 0) {
            face.material = CustomerPresets[i].face;
            arms[0].material = CustomerPresets[i].arms;
            arms[1].material = CustomerPresets[i].arms;
            torso.material = CustomerPresets[i].torso;
            legs.material = CustomerPresets[i].legs;
        }
        else Debug.LogWarning("customer preset out of rainge");
    }
}