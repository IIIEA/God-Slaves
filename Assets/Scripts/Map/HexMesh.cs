using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    public bool useCollider, useCellData, useUVCoordinates, useUV2Coordinates;

    [NonSerialized] private List<Vector3> _vertices, _cellIndices;
    [NonSerialized] private List<Color> _cellWeights;
    [NonSerialized] private List<Vector2> _uvs, _uv2s;
    [NonSerialized] private List<int> _triangles;

    private Mesh _hexMesh;
    private MeshCollider _meshCollider;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();

        if (useCollider)
        {
            _meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        _hexMesh.name = "Hex Mesh";
    }

    public void Clear()
    {
        _hexMesh.Clear();
        _vertices = ListPool<Vector3>.Get();

        if (useCellData)
        {
            _cellWeights = ListPool<Color>.Get();
            _cellIndices = ListPool<Vector3>.Get();
        }
        if (useUVCoordinates)
        {
            _uvs = ListPool<Vector2>.Get();
        }
        if (useUV2Coordinates)
        {
            _uv2s = ListPool<Vector2>.Get();
        }

        _triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        _hexMesh.SetVertices(_vertices);
        ListPool<Vector3>.Add(_vertices);

        if (useCellData)
        {
            _hexMesh.SetColors(_cellWeights);
            ListPool<Color>.Add(_cellWeights);
            _hexMesh.SetUVs(2, _cellIndices);
            ListPool<Vector3>.Add(_cellIndices);
        }
        if (useUVCoordinates)
        {
            _hexMesh.SetUVs(0, _uvs);
            ListPool<Vector2>.Add(_uvs);
        }
        if (useUV2Coordinates)
        {
            _hexMesh.SetUVs(1, _uv2s);
            ListPool<Vector2>.Add(_uv2s);
        }

        _hexMesh.SetTriangles(_triangles, 0);
        ListPool<int>.Add(_triangles);
        _hexMesh.RecalculateNormals();

        if (useCollider)
        {
            _meshCollider.sharedMesh = _hexMesh;
        }
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(HexMetrics.Perturb(v1));
        _vertices.Add(HexMetrics.Perturb(v2));
        _vertices.Add(HexMetrics.Perturb(v3));
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
    {
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);
    }

    public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3)
    {
        _uv2s.Add(uv1);
        _uv2s.Add(uv2);
        _uv2s.Add(uv3);
    }

    public void AddTriangleCellData(Vector3 indices, Color weights1, Color weights2, Color weights3)
    {
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellWeights.Add(weights1);
        _cellWeights.Add(weights2);
        _cellWeights.Add(weights3);
    }

    public void AddTriangleCellData(Vector3 indices, Color weights)
    {
        AddTriangleCellData(indices, weights, weights, weights);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(HexMetrics.Perturb(v1));
        _vertices.Add(HexMetrics.Perturb(v2));
        _vertices.Add(HexMetrics.Perturb(v3));
        _vertices.Add(HexMetrics.Perturb(v4));
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 3);
    }

    public void AddQuadUnperturbed( Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _vertices.Add(v4);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 3);
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
    {
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);
        _uvs.Add(uv4);
    }

    public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
    {
        _uv2s.Add(uv1);
        _uv2s.Add(uv2);
        _uv2s.Add(uv3);
        _uv2s.Add(uv4);
    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        _uvs.Add(new Vector2(uMin, vMin));
        _uvs.Add(new Vector2(uMax, vMin));
        _uvs.Add(new Vector2(uMin, vMax));
        _uvs.Add(new Vector2(uMax, vMax));
    }

    public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
    {
        _uv2s.Add(new Vector2(uMin, vMin));
        _uv2s.Add(new Vector2(uMax, vMin));
        _uv2s.Add(new Vector2(uMin, vMax));
        _uv2s.Add(new Vector2(uMax, vMax));
    }

    public void AddQuadCellData(Vector3 indices, Color weights1, Color weights2, Color weights3, Color weights4)
    {
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellIndices.Add(indices);
        _cellWeights.Add(weights1);
        _cellWeights.Add(weights2);
        _cellWeights.Add(weights3);
        _cellWeights.Add(weights4);
    }

    public void AddQuadCellData(Vector3 indices, Color weights1, Color weights2)
    {
        AddQuadCellData(indices, weights1, weights1, weights2, weights2);
    }

    public void AddQuadCellData(Vector3 indices, Color weights)
    {
        AddQuadCellData(indices, weights, weights, weights, weights);
    }
}