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
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? editMesh.trs.rotation : Quaternion.identity;
        for (int i = 0; i < editMesh.vertices.Length; i ++)
            ShowPoint (i);
    }

    private void ShowPoint (int index)
    {
        if (editMesh.moveVertexPoint)
        {
            Vector3 point = handleTransform.TransformPoint(editMesh.clonedMesh.vertices[index]);
            Handles.color = Color.blue;
            point = Handles.FreeMoveHandle(point, handleRotation, editMesh.handleSize,
                Vector3.zero, Handles.DotHandleCap);

            if (GUI.changed)
            {
                editMesh.clonedMesh.MoveSimilarVertices (index, handleTransform.InverseTransformPoint(point));
                editMesh.clonedMesh.RecalculateNormals();
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
        if (GUILayout.Button("Reset"))
        {
            editMesh.Reset();
        }
        if (editMesh.isCloned)
        {
            if (GUILayout.Button("Subdivide"))
	        	editMesh.clonedMesh.Subdivide ();
        }
    }


}
