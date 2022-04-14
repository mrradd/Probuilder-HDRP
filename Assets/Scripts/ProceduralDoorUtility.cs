using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

/// <summary>
/// Provides functions which allow for the creation of procedural doors using ProBuilder.
/// </summary>
public class ProceduralDoorUtility
{
    //Side names.
    public const string RIGHT = "RIGHT";
    public const string LEFT = "LEFT";
    public const string TOP = "TOP";
    public const string BOTTOM = "BOTTOM";
    public const string CENTER = "CENTER";

    /// <summary>
    /// Assembles the shape with the passed in parts. This particular one helps to build a basic door shape.
    /// </summary>
    /// <param name="center">Center object of the door.</param>
    /// <param name="top">Top rail of the door.</param>
    /// <param name="left">Left rail of the door.</param>
    /// <param name="bottom">Bottom rail of the door.</param>
    /// <param name="right">Right rail of the door.</param>
    public static void assembleDoor(GameObject center, GameObject top, GameObject left, GameObject bottom, GameObject right)
    {
        //Bounds for the different meshes.
        Bounds centerBounds = getTotalMeshFilterBounds(center.transform);
        Bounds topBounds = getTotalMeshFilterBounds(top.transform);
        Bounds rightBounds = getTotalMeshFilterBounds(right.transform);

        //Position and rotate the meshes properly.
        //right
        right.transform.position = new Vector3(center.transform.position.x - rightBounds.size.x,
            center.transform.position.y - topBounds.size.y,
            center.transform.position.z);

        right.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //top
        top.transform.position = new Vector3(center.transform.position.x,
            center.transform.position.y + centerBounds.size.y,
            center.transform.position.z);

        top.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //left
        left.transform.position = new Vector3(center.transform.position.x + centerBounds.size.x,
            center.transform.position.y - topBounds.size.y,
            center.transform.position.z);

        left.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //bottom
        bottom.transform.position = new Vector3(center.transform.position.x,
            center.transform.position.y - topBounds.size.y,
            center.transform.position.z);

        bottom.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);
    }

    /// <summary>
    /// Bevels a given edge of a given face by a given amount.
    /// </summary>
    /// <param name="mesh">Mesh to bevel.</param>
    /// <param name="faceIndex">Face index of the Face whose Edge we will bevel.</param>
    /// <param name="edgeIndex">Edge index we will bevel.</param>
    /// <param name="amount">Amount to bevel by.</param>
    public static void bevelEdge(ProBuilderMesh mesh, int faceIndex, int edgeIndex, float amount)
    {
        Edge[] edges = new Edge[] { mesh.faces[faceIndex].edges[edgeIndex] };
        UnityEngine.ProBuilder.MeshOperations.Bevel.BevelEdges(mesh, edges, amount);
    }

    /// <summary>
    /// Combines all passed in meshes into one mesh.
    /// </summary>
    /// <param name="parentObj">Parent GameObject of the meshes.</param>
    /// <param name="meshes">ProBulderMeshes to combine together.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Creates a poly shape with the passed in data. It also extrudes the mesh and applies a material to it.
    /// </summary>
    /// <param name="extrustionFactor">Amount to extrude by.</param>
    /// <param name="shapePoints">Points that make up the shape to create.</param>
    /// <param name="material">Material to apply to the mesh.</param>
    /// <param name="objectName">Name of the mesh being created</param>
    /// <returns>GameObject with the new ProBuilderMesh</returns>
    public static GameObject makeProbuilderMesh(float extrustionFactor, List<Vector3> shapePoints, Material material, string objectName)
    {
        GameObject gameObject = new GameObject(objectName);

        ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();
        mesh.CreateShapeFromPolygon(shapePoints.ToArray(), extrustionFactor, false);
        mesh.SetMaterial(mesh.faces, material);
       
        return gameObject;
    }

    /// <summary>
    /// Refreshes the passed in ProBuilderMesh.
    /// </summary>
    /// <param name="mesh">ProBuilderMesh to refresh.</param>
    public static void refreshMesh(ProBuilderMesh mesh)
    {
        mesh.ToMesh();
        mesh.Refresh();
    }

    /// <summary>
    /// Refresh passed in ProBuilderMeshes.
    /// </summary>
    /// <param name="meshes">ProBuilderMeshes to refresh.</param>
    public static void refreshMeshes(List<ProBuilderMesh> meshes)
    {
        foreach (ProBuilderMesh mesh in meshes)
        {
            refreshMesh(mesh);
        }
    }

    /// <summary>
    /// Rotate the passed in ProBuilderMesh's UVs by 90 degrees
    /// </summary>
    /// <param name="mesh">ProBuilderMesh whose UV we want to rotate.</param>
    /// <param name="positive">TRUE=Rotate by positive 90 degrees. FALSE=Rotate by negative 90 degrees</param>
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
