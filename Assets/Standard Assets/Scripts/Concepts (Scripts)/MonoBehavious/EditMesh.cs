using UnityEngine;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class EditMesh : MonoBehaviour, IUpdatable
{
	Mesh originalMesh;
	[HideInInspector]
	public Mesh clonedMesh;
	MeshFilter meshFilter;
	int[] triangles;

	[HideInInspector]
	public Vector3[] vertices;

	[HideInInspector]
	public bool isCloned = false;

	// For Editor
	public float handleSize = 0.03f;
	public List<int>[] connectedVertices;
	public List<Vector3[]> allTriangleList;
	public bool moveVertexPoint = true;

	public virtual void Start ()
	{
		InitMesh ();
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	public virtual void InitMesh ()
	{
		meshFilter = GetComponent<MeshFilter>();
		originalMesh = meshFilter.sharedMesh; //1
		clonedMesh = new Mesh(); //2
		clonedMesh.name = "clone";
		clonedMesh.vertices = originalMesh.vertices;
		clonedMesh.triangles = originalMesh.triangles;
		clonedMesh.normals = originalMesh.normals;
		clonedMesh.uv = originalMesh.uv;
		meshFilter.mesh = clonedMesh;  //3
		vertices = clonedMesh.vertices; //4
		triangles = clonedMesh.triangles;
		isCloned = true; //5
	}

	public virtual void Reset ()
	{
		if (clonedMesh != null && originalMesh != null) //1
		{
			clonedMesh.vertices = originalMesh.vertices; //2
			clonedMesh.triangles = originalMesh.triangles;
			clonedMesh.normals = originalMesh.normals;
			clonedMesh.uv = originalMesh.uv;
			meshFilter.mesh = clonedMesh; //3
			vertices = clonedMesh.vertices; //4
			triangles = clonedMesh.triangles;
		}
	}

	public virtual void GetConnectedVertices ()
	{
		connectedVertices = new List<int>[vertices.Length];
	}

	public virtual void BuildTriangleList ()
	{
	}

	public virtual void ShowTriangle (int idx)
	{
	}

	public virtual void Subdivide ()
	{
		// vertices[2] = new Vector3(2, 3, 4);
		// vertices[3] = new Vector3(1, 2, 4);
		// clonedMesh.vertices = vertices;
		// clonedMesh.RecalculateNormals();
	}

	public virtual void DoUpdate ()
	{
	}

	public virtual void OnDestroy ()
	{
		GameManager.updatables = GameManager.updatables.Remove(this);
	}
}