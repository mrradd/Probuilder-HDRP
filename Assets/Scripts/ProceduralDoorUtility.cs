using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ProceduralDoorUtility
{
    //Side names.
    public const string RIGHT = "RIGHT";
    public const string LEFT = "LEFT";
    public const string TOP = "TOP";
    public const string BOTTOM = "BOTTOM";
    public const string CENTER = "CENTER";

    public static void assembleShape(GameObject parent, GameObject center, GameObject top, GameObject left, GameObject bottom, GameObject right)
    {
        ProBuilderMesh centerMesh = center.GetComponent<ProBuilderMesh>();
        ProBuilderMesh topMesh = top.GetComponent<ProBuilderMesh>();
        ProBuilderMesh leftMesh = left.GetComponent<ProBuilderMesh>();
        ProBuilderMesh bottomMesh = bottom.GetComponent<ProBuilderMesh>();
        ProBuilderMesh rightMesh = right.GetComponent<ProBuilderMesh>();
        
        //Bounds for the different meshes.
        Bounds centerBounds = getTotalMeshFilterBounds(center.transform);
        Bounds topBounds = getTotalMeshFilterBounds(top.transform);
        Bounds leftBounds = getTotalMeshFilterBounds(left.transform);
        Bounds bottomBounds = getTotalMeshFilterBounds(bottom.transform);
        Bounds rightBounds = getTotalMeshFilterBounds(right.transform);

        //Position and rotate the meshes properly.
        //right
        right.transform.position = new Vector3(center.transform.position.x,
            center.transform.position.y - topBounds.size.z,
            center.transform.position.z - rightBounds.size.z);

        right.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z
        );

        //top
        top.transform.position = new Vector3(center.transform.position.x,
            center.transform.position.y + centerBounds.size.y + topBounds.size.z,
            center.transform.position.z);

        top.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x + 90f,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z
        );

        //left
        left.transform.position = new Vector3(center.transform.position.x,
            center.transform.position.y - topBounds.size.z,
            center.transform.position.z + centerBounds.size.z);

        left.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //bottom
        bottom.transform.position = new Vector3(center.transform.position.x,
            center.transform.position.y,
            center.transform.position.z);

        bottom.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x + 90f,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //rotateUVs90(rightMesh, true);
        //rotateUVs90(leftMesh, false);

        //bevelEdge(bottomMesh, 2, 1, .062f);
        //bevelEdge(rightMesh, 5, 2, .062f);
        //bevelEdge(leftMesh, 2, 1, .062f);
        //bevelEdge(topMesh, 5, 2, .062f);

        //combineAllMeshes(parent, new List<ProBuilderMesh> { centerMesh, topMesh, leftMesh, bottomMesh, rightMesh });

        //List<ProBuilderMesh> meshes = new List<ProBuilderMesh> { centerMesh, topMesh, leftMesh, bottomMesh, rightMesh };
        //refreshMeshes(meshes);
    }

    public static void bevelEdge(ProBuilderMesh mesh, int faceIndex, int edgeIndex, float amount)
    {
        Edge[] edges = new Edge[] { mesh.faces[faceIndex].edges[edgeIndex] };
        UnityEngine.ProBuilder.MeshOperations.Bevel.BevelEdges(mesh, edges, amount);
    }

    public static List<ProBuilderMesh> combineAllMeshes(GameObject parentObj, List<ProBuilderMesh> meshes)
    {
        //Merge the center and sides into a single mesh.
        List<ProBuilderMesh> combinedMeshes = CombineMeshes.Combine(meshes);
        
        foreach(ProBuilderMesh mesh in combinedMeshes)
        {
            mesh.transform.parent = parentObj.transform;
        }

        return combinedMeshes;
    }

    /// <summary>
    /// Calculates the bounds of the passed in transform.
    /// </summary>
    /// <param name="objectTransform">Transform to calculate bounds for.</param>
    /// <returns></returns>
    public static Bounds getTotalMeshFilterBounds(Transform objectTransform)
    {
        Bounds result = objectTransform.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 scaledMin = result.min;
        scaledMin.Scale(objectTransform.localScale);
        result.min = scaledMin;
        Vector3 scaledMax = result.max;
        scaledMax.Scale(objectTransform.localScale);
        result.max = scaledMax;
        return result;
    }

    public static GameObject makeProbuilderMesh(float extrustionFactor, List<Vector3> shapePoints, Material material, string side)
    {
        GameObject gameObject = new GameObject(side);

        //Create the center mesh.
        ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();
        mesh.CreateShapeFromPolygon(shapePoints.ToArray(), extrustionFactor, false);
        mesh.SetMaterial(mesh.faces, material);
       
        return gameObject;
    }

    public static void refreshMesh(ProBuilderMesh mesh)
    {
        mesh.ToMesh();
        mesh.Refresh();
    }

    public static void refreshMeshes(List<ProBuilderMesh> meshes)
    {
        foreach (ProBuilderMesh mesh in meshes)
        {
            refreshMesh(mesh);
        }
    }

    public static void rotateUVs90(ProBuilderMesh mesh, bool positive)
    {
        foreach (Face face in mesh.faces)
        {
            AutoUnwrapSettings a = face.uv;
            a.rotation = positive ? 90f : -90f;
            face.uv = a;
        }

    }
}
