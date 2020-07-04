using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshUtilities
{
	static List<Vector3> vertices;
	static List<Vector3> normals;
	// [... all other vertex data arrays you need]
	static List<int> indices;
	static Dictionary<uint,int> newVectices;

	static int GetNewVertex (int i1, int i2)
	{
		// We have to test both directions since the edge
		// could be reversed in another triangle
		uint t1 = ((uint)i1 << 16) | (uint)i2;
		uint t2 = ((uint)i2 << 16) | (uint)i1;
		if (newVectices.ContainsKey(t2))
			return newVectices[t2];
		if (newVectices.ContainsKey(t1))
			return newVectices[t1];
		// generate vertex:
		int newIndex = vertices.Count;
		newVectices.Add(t1,newIndex);
		// calculate new vertex
		vertices.Add((vertices[i1] + vertices[i2]) * 0.5f);
		normals.Add((normals[i1] + normals[i2]).normalized);
		// [... all other vertex data arrays]
		return newIndex;
	}

	public static void Subdivide (this Mesh mesh)
	{
		newVectices = new Dictionary<uint, int>();

		vertices = new List<Vector3>(mesh.vertices);
		normals = new List<Vector3>(mesh.normals);
		// [... all other vertex data arrays]
		indices = new List<int>();

		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int i1 = triangles[i + 0];
			int i2 = triangles[i + 1];
			int i3 = triangles[i + 2];

			int a = GetNewVertex(i1, i2);
			int b = GetNewVertex(i2, i3);
			int c = GetNewVertex(i3, i1);
			indices.Add(i1);   indices.Add(a);   indices.Add(c);
			indices.Add(i2);   indices.Add(b);   indices.Add(a);
			indices.Add(i3);   indices.Add(c);   indices.Add(b);
			indices.Add(a );   indices.Add(b);   indices.Add(c); // center triangle
		}
		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		// [... all other vertex data arrays]
		mesh.triangles = indices.ToArray();

		// since this is a static function and it uses static variables
		// we should erase the arrays to free them:
		newVectices = null;
		vertices = null;
		normals = null;
		// [... all other vertex data arrays]

		indices = null;
	}

	public static Mesh GetTessellatable (FloatRange moveDistance, int subdivideCount = 1, int maxPairsToMove = 1)
	{
		Mesh output = null;
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		MeshFilter meshFilter = go.GetComponent<MeshFilter>();
		Mesh originalMesh = meshFilter.sharedMesh;
		Mesh clonedMesh = new Mesh();
		clonedMesh.vertices = originalMesh.vertices;
		clonedMesh.triangles = originalMesh.triangles;
		clonedMesh.normals = originalMesh.normals;
		clonedMesh.uv = originalMesh.uv;
		meshFilter.mesh = clonedMesh;
		for (int i = 0; i < subdivideCount; i ++)
			Subdivide (clonedMesh);
		for (int i = 0; i < maxPairsToMove; i ++)
		{
			Vector3 offset = Random.insideUnitSphere * moveDistance.Get(Random.value);

		}
		return output;
	}

	// Pulling only one vertex pt, results in broken mesh.
	public static void PullOneVertex (this Mesh mesh, int index, Vector3 newPos)
	{
		mesh.vertices[index] = newPos;
		mesh.RecalculateNormals();
	}

	public static void PullSimilarVertices (this Mesh mesh, int index, Vector3 newPos)
	{
		Vector3 targetVertexPos = vertices[index];
		List<int> relatedVertices = FindRelatedVertices(mesh, targetVertexPos, false);
		foreach (int i in relatedVertices)
			mesh.vertices[i] = newPos;
		mesh.RecalculateNormals();
	}

	// returns List of int that is related to the targetPt.
	public static List<int> FindRelatedVertices (this Mesh mesh, Vector3 targetPt, bool findConnected)
	{
		// list of int
		List<int> relatedVertices = new List<int>();

		int idx = 0;
		Vector3 pos;

		// loop through triangle array of indices
		for (int t = 0; t < mesh.triangles.Length; t++)
		{
			// current idx return from tris
			idx = mesh.triangles[t];
			// current pos of the vertex
			pos = vertices[idx];
			// if current pos is same as targetPt
			if (pos == targetPt)
			{
				// add to list
				relatedVertices.Add(idx);
				// if find connected vertices
				if (findConnected)
				{
					// min
					// - prevent running out of count
					if (t == 0)
					{
						relatedVertices.Add(mesh.triangles[t + 1]);
					}
					// max 
					// - prevent runnign out of count
					if (t == mesh.triangles.Length - 1)
					{
						relatedVertices.Add(mesh.triangles[t - 1]);
					}
					// between 1 ~ max-1 
					// - add idx from triangles before t and after t 
					if (t > 0 && t < mesh.triangles.Length - 1)
					{
						relatedVertices.Add(mesh.triangles[t - 1]);
						relatedVertices.Add(mesh.triangles[t + 1]);
					}
				}
			}
		}
		// return compiled list of int
		return relatedVertices;
	}
}