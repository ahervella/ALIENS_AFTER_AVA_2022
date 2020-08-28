using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//requires mesh
[RequireComponent(typeof(Mesh))]
public class SickMesh : MonoBehaviour
{
    

    Mesh mesh;
    Vector3[] vertices;

    int[] trinagleRenPnts;

    public bool drawShapes = true;

    public int height = 10;
    public int width = 5;

    public Terrainnnn testTerrain;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //StartCoroutine(CreateVerticiesShapes());
        CreateVerticies();
        CreateShapes();


    }

    private void Update()
    {
        UpdateMesh();
    }


    void CreateVerticies()
    {
        vertices = new Vector3[(height + 1) * (width + 1)];

        for (int i = 0, vertIndex = 0; i <= width; i++)
        {
            for (int k = 0; k <= height; k++)
            {
                //vertices[i * width + k] = new Vector3(i, 0, k);
                vertices[vertIndex] = new Vector3(i, 0, k);
                vertIndex++;

            }


        }

    }

    void CreateShapes()
    {

        trinagleRenPnts = new int[height * width * 6];
        int vert = 0;
        int triIndex = 0;

        for (int i = 0; i < width; i++)
        {
            for (int k = 0; k < height; k++)
            {
                //verticies are drawn from bottom up, and moves collumn to the right

                //draws from bottom left to top right, clockwise
                trinagleRenPnts[triIndex] = vert;
                trinagleRenPnts[triIndex + 1] = vert + 1;
                //need to add 2 because moved over a column (one extra vertice per column)
                trinagleRenPnts[triIndex + 2] = vert + height + 2; 

                //draws from top right to bottom left, clockwise
                trinagleRenPnts[triIndex + 3] = vert + height + 2;
                trinagleRenPnts[triIndex + 4] = vert + height + 1;
                trinagleRenPnts[triIndex + 5] = vert;
                vert++;
                triIndex += 6;

            }
            vert++;
        }
        


    }

    private void OnDrawGizmos()
    {
        if (vertices == null) { return; }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }


    void UpdateMesh()
    {
        mesh.vertices = vertices;
        if (drawShapes) { mesh.triangles = trinagleRenPnts; }
        //for recalculating normals with lighting
        mesh.RecalculateNormals();

        if (testTerrain == null) { return; }

        if (testTerrain.terrainData.Count == 0) { return; }



        for (int i = 0; i < testTerrain.height; i++)
        {
            for (int k = 0; k < testTerrain.width; k++)
            {
                int vertIndex = i * width + k;
                int terrainIndex = i * testTerrain.width + k;
                vertices[vertIndex] = testTerrain.terrainData[terrainIndex];
            }
        }

        CreateShapes();
        mesh.RecalculateNormals();
    }
}


