using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EditMesh))]
public class EditMeshEditor : Editor
{
    private EditMesh editMesh;
    private Transform handleTransform;
    private Quaternion handleRotation;
    string triangleIdx;

    void OnSceneGUI ()
    {
        editMesh = target as EditMesh;
        EditMesh ();
    }

    void EditMesh ()
    {
        handleTransform = editMesh.transform; //1
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity; //2
        for (int i = 0; i < editMesh.vertices.Length; i++) //3
            ShowPoint (i);
    }

    private void ShowPoint (int index)
    {
        if (editMesh.moveVertexPoint)
        {
            Vector3 point = handleTransform.TransformPoint(editMesh.vertices[index]); //1
            Handles.color = Color.blue;
            point = Handles.FreeMoveHandle(point, handleRotation, editMesh.handleSize,
                Vector3.zero, Handles.DotHandleCap); //2

            if (GUI.changed) //3
            {
                editMesh.clonedMesh.PullSimilarVertices (index, handleTransform.InverseTransformPoint(point)); //4
            }
        }
        else
        {
            //click
        }
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        editMesh = target as EditMesh;
        if (GUILayout.Button("Reset")) //1
        {
            editMesh.Reset(); //2
        }
        // For testing Reset function
        if (editMesh.isCloned)
        {
            if (GUILayout.Button("Subdivide"))
	        	editMesh.clonedMesh.Subdivide ();
        }
    }


}
