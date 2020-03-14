using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {

        GameManager myScript = (GameManager)target;
        if (GUILayout.Button("Save Hair"))
        {
            if (myScript.currentCustomer != null && Application.isPlaying)
            {
                myScript.currentCustomer.SaveHair(myScript.directory, myScript.fileName);
            }
        }
        if (GUILayout.Button("Load Hair"))
        {
            if (myScript.currentCustomer != null && Application.isPlaying)
            {
                myScript.currentCustomer.LoadHair(myScript.directory, myScript.fileName);
            }
        }

        DrawDefaultInspector();

    }
}
