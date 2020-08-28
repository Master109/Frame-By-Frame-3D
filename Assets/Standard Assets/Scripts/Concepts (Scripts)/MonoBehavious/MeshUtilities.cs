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

	public static void MakeTessellatable (FloatRange moveDistance, int subdivideCount, int maxPairsToMove)
	{
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		MeshFilter meshFilter = go.GetComponent<MeshFilter>();
		Mesh originalMesh = meshFilter.sharedMesh;
		Mesh clonedMesh = new Mesh();
		clonedMesh.vertices = originalMesh.vertices;
		clonedMesh.triangles = originalMesh.triangles;
		clonedMesh.normals = originalMesh.normals;
		clonedMesh.uv = originalMesh.uv;
		for (int i = 0; i < subdivideCount; i ++)
			Subdivide (clonedMesh);
		List<Vector3> vertices = new List<Vector3>();
		vertices.AddRange(clonedMesh.vertices);
		List<int> vertexIndiciesRemaining = new List<int>();
		for (int i = 0; i < vertices.Count; i ++)
			vertexIndiciesRemaining.Add(i);
		for (int i = 0; i < maxPairsToMove; i ++)
		{
			int vertexIndiciesRemainingIndex = Random.Range(0, vertexIndiciesRemaining.Count);
			int indexOfVertexToMove = vertexIndiciesRemaining[vertexIndiciesRemainingIndex];
			Vector3 offset = Random.insideUnitSphere * moveDistance.Get(Random.value);
			Vector3 vertexToMove = vertices[indexOfVertexToMove];
			Vector3 otherVertex = vertexToMove;
			if (Mathf.Abs(vertexToMove.x) == .5f)
				otherVertex.x *= -1;
			if (Mathf.Abs(vertexToMove.y) == .5f)
				otherVertex.y *= -1;
			else
				otherVertex.z *= -1;
			int indexOfOtherVertex = vertices.IndexOf(otherVertex);
			List<int> relatedVertices = FindRelatedVertices(clonedMesh, vertexToMove, false);
			foreach (int i2 in relatedVertices)
				vertices[i2] += offset;
			relatedVertices = FindRelatedVertices(clonedMesh, otherVertex, false);
			foreach (int i2 in relatedVertices)
				vertices[i2] += offset;
			vertexIndiciesRemaining.RemoveAt(vertexIndiciesRemainingIndex);
			vertexIndiciesRemaining.RemoveAt(indexOfOtherVertex);
			if (vertexIndiciesRemaining.Count == 0)
				return;
		}
		clonedMesh.vertices = vertices.ToArray();
		clonedMesh.RecalculateNormals();
		meshFilter.mesh = clonedMesh;
	}

	public static void MoveSimilarVertices (this Mesh mesh, int index, Vector3 move)
	{
		Vector3 targetVertexPos = mesh.vertices[index];
		List<int> relatedVertices = FindRelatedVertices(mesh, targetVertexPos, false);
		foreach (int i in relatedVertices)
			mesh.vertices[i] += move;
	}

	public static List<int> FindRelatedVertices (this Mesh mesh, Vector3 targetPt, bool findConnected)
	{
		List<int> relatedVertices = new List<int>();
		int idx = 0;
		Vector3 pos;
		for (int t = 0; t < mesh.triangles.Length; t ++)
		{
			idx = mesh.triangles[t];
			pos = mesh.vertices[idx];
			if (pos == targetPt)
			{
				relatedVertices.Add(idx);
				if (findConnected)
				{
					if (t == 0)
					{
						relatedVertices.Add(mesh.triangles[t + 1]);
					}
					if (t == mesh.triangles.Length - 1)
					{
						relatedVertices.Add(mesh.triangles[t - 1]);
					}
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