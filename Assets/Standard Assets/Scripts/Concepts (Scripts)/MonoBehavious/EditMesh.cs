using UnityEngine;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class EditMesh : MonoBehaviour, IUpdatable
{
	public Transform trs;
	Mesh originalMesh;
	[HideInInspector]
	public Mesh clonedMesh;
	MeshFilter meshFilter;
	int[] triangles;

	[HideInInspector]
	public Vector3[] vertices;

	[HideInInspector]
	public bool isCloned = false;

	public float handleSize = 0.03f;
	public List<int>[] connectedVertices;
	public List<Vector3[]> allTriangleList;
	public bool moveVertexPoint = true;
	public FloatRange moveDistance;
	public int subdivideCount;
	public int maxPairsToMove;

	void Start ()
	{
		Init ();
	}

	void OnEnable ()
	{
		GameManager.updatables = GameManager.updatables.Add(this);
	}

	void Init ()
	{
		meshFilter = GetComponent<MeshFilter>();
		originalMesh = meshFilter.sharedMesh;
		clonedMesh = new Mesh();
		clonedMesh.name = "clone";
		clonedMesh.vertices = originalMesh.vertices;
		clonedMesh.triangles = originalMesh.triangles;
		clonedMesh.normals = originalMesh.normals;
		clonedMesh.uv = originalMesh.uv;
		meshFilter.mesh = clonedMesh;
		vertices = clonedMesh.vertices;
		triangles = clonedMesh.triangles;
		isCloned = true;
	}

	public void Reset ()
	{
		if (clonedMesh != null && originalMesh != null)
		{
			clonedMesh.vertices = originalMesh.vertices;
			clonedMesh.triangles = originalMesh.triangles;
			clonedMesh.normals = originalMesh.normals;
			clonedMesh.uv = originalMesh.uv;
			meshFilter.mesh = clonedMesh;
			vertices = clonedMesh.vertices;
			triangles = clonedMesh.triangles;
		}
	}

	public virtual void DoUpdate ()
	{
		enabled = false;
		MeshUtilities.MakeTessellatable (moveDistance, subdivideCount, maxPairsToMove);
	}

	void OnDisable ()
	{
		GameManager.updatables = GameManager.updatables.Remove(this);
	}
}