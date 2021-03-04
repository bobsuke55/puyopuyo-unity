using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]

public class LineGrid : MonoBehaviour
{
    Vector3[] verts;
    int[] triangles;
    GameObject camera;

    GameObject dummy;
    [SerializeField,Header("使用するMaterial")] Material material;
    [SerializeField,Header("大きさ")] Vector2Int size;
    [SerializeField,Header("線の太さ")] float lineSize;

    void Start()
    {
        size = new Vector2Int(5,11);
        lineSize = 0.1f;
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        dummy = this.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        CreateGlid();

        
    }

void CreateGlidtest()
    {
        Mesh mesh = new Mesh();
        triangles = new int[6];
        verts = new Vector3[6];

        //頂点番号を割り当て
        for (int i=0; i< triangles.Length; i++)
        {
            triangles[i] = i;
        }

        int x=0, y=0;

        for (int i=0; i<(size.x+1)*6; i+=6)
        {
            verts[i] =   new Vector3(x,0,0);
            verts[i+1] = new Vector3(x,size.y,0);
            verts[i+2] = new Vector3(lineSize+x,size.y,0);
            verts[i+3] = new Vector3(lineSize+x,size.y,0);
            verts[i+4] = new Vector3(lineSize+x,0,0);
            verts[i+5] = new Vector3(x,0,0);
            x ++;

        }

        mesh.vertices = verts;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

    }

    void CreateGlid()
    {
        Mesh mesh = new Mesh();
        triangles = new int[(size.x+size.y+2)*6];
        verts = new Vector3[(size.x+size.y+2)*6];

        //頂点番号を割り当て
        for (int i=0; i< triangles.Length; i++)
        {
            triangles[i] = i;
        }

        int x=0, y=0;

        for (int i=0; i<(size.x+1)*6; i+=6)
        {
            verts[i] =   new Vector3(x,0,0);
            verts[i+1] = new Vector3(x,size.y,0);
            verts[i+2] = new Vector3(lineSize+x,size.y,0);
            verts[i+3] = new Vector3(lineSize+x,size.y,0);
            verts[i+4] = new Vector3(lineSize+x,0,0);
            verts[i+5] = new Vector3(x,0,0);
            x ++;

        }

        for (int i=(size.x+1)*6; i<(size.x+size.y+2)*6; i+=6)
        {
            verts[i] =   new Vector3(0,y,0);
            verts[i+1] = new Vector3(size.x+lineSize,y,0);
            verts[i+2] = new Vector3(0,y-lineSize,0);
            verts[i+3] = new Vector3(size.x+lineSize,y,0);
            verts[i+4] = new Vector3(size.x+lineSize,y-lineSize,0);
            verts[i+5] = new Vector3(0,y-lineSize,0);
            y++;
        }
        mesh.vertices = verts;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;

    }

/*
 public void CreateFilledGraphShape (Vector3[] linePoints) {

     Vector3[] filledGraphPoints = new Vector3[linePoints.Length * 2]; // one point below each line point
     for (int i = 0; i < linePoints.Length; ++i) {
         filledGraphPoints[2 * i] = new Vector3(linePoints[i].x, 0, 0);
         filledGraphPoints[2 * i + 1] = linePoints[i];
     }
 
     int numTriangles = (linePoints.Length -1) * 2;
     int[] triangles = new int[numTriangles * 3]; 
 
     int i = 0;
     for (int t = 0; t < numTriangles; t += 2) {
         // lower left triangle
         triangles[i++] = 2 * t;
         triangles[i++] = 2 * t +1
         triangles[i++] = 2 * t +2;
         // upper right triangle - you might need to experiment what are the correct indices
         triangles[i++] = 2 * t + 1;
         triangles[i++] = 2 * t + 2;
         triangles[i++] = 2 * t + 3;
     }
 
     // create mesh
     Mesh filledGraphMesh = new Mesh();
     filledGraphMesh.vertices = filledGraphPoints;
     filledGraphMesh.triangles = triangles;
     // you might need to assign texture coordinates as well
 
     // create game object and add renderer and mesh to it
     GameObject filledGraph = new GameObject("Filled graph");
     MeshRenderer renderer = filledGraph.AddComponent<MeshRenderer>();
     renderer.mesh = mesh;
 }
*/
}
