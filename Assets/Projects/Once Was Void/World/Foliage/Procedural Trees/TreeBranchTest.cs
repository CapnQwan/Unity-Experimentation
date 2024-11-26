using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBranchTest : MonoBehaviour
{
    public Vector3[] offsets;

    Vector3[] vertices;
    Vector3[] verticesF;
    Vector3[] verticesB;
    Vector3[] verticesU;
    Vector3[] verticesD;

    public float yaw;

    void OnValidate()
    {
        int length = offsets.Length;
        vertices = new Vector3[length];
        verticesF = new Vector3[length];
        verticesB = new Vector3[length];
        verticesU = new Vector3[length];
        verticesD = new Vector3[length];

        changeVerts();
    }

    void Update()
    {
        changeVerts();
    }

    void changeVerts()
    {
        int length = offsets.Length;

        for (int i = 0; i < length; i++)
        {
            Vector3 point = new Vector3(i, 0.5f, 0.0f) + offsets[i];
            vertices[i] = point;
            verticesF[i] = point + Vector3.forward;
            verticesB[i] = point + Vector3.back;
            verticesU[i] = point + Vector3.up * 0.5f;
            verticesD[i] = point + Vector3.down;
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(vertices[i], 0.05f);
            Gizmos.DrawSphere(verticesF[i], 0.05f);
            Gizmos.DrawSphere(verticesB[i], 0.05f);
            Gizmos.DrawSphere(verticesU[i], 0.05f);
            Gizmos.DrawSphere(verticesD[i], 0.05f);
        }

        for (int i = 0; i < offsets.Length - 1; i++)
        {
            Gizmos.DrawLine(vertices[i], vertices[i + 1]);
            Gizmos.DrawLine(verticesF[i], verticesF[i + 1]);
            Gizmos.DrawLine(verticesB[i], verticesB[i + 1]);
            Gizmos.DrawLine(verticesU[i], verticesU[i + 1]);
            Gizmos.DrawLine(verticesD[i], verticesD[i + 1]);
        }
    }
}
