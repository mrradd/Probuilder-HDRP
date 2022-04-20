using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

/// <summary>
/// A wrapper for Probuilder functionality.
/// </summary>
public class ProbuilderUtility
{
    public enum HandlePlacement
    {
        Top,
        Middle,
        Bottom,
    }

    public enum DoorOpenDirection
    {
        Left,
        Right
    }

    public enum Side
    {
        Right,
        Left,
        Top,
        Bottom,
        Center,
        Back,
        Body
    }
    
    /// <summary>
    /// Bevels a given edge of a given face by a given amount.
    /// </summary>
    /// <param name="mesh">Mesh to bevel.</param>
    /// <param name="faceIndex">Face index of the Face whose Edge we will bevel.</param>
    /// <param name="edgeIndex">Edge index we will bevel.</param>
    /// <param name="amount">Amount to bevel by.</param>
    public static void BevelEdge(ProBuilderMesh mesh, int faceIndex, int edgeIndex, float amount)
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
    public static List<ProBuilderMesh> CombineAllMeshes(GameObject parentObj, List<ProBuilderMesh> meshes)
    {
        //Merge the all meshes into one.
        List<ProBuilderMesh> combinedMeshes = CombineMeshes.Combine(meshes);

        foreach (ProBuilderMesh mesh in combinedMeshes)
        {
            mesh.transform.parent = parentObj.transform;
            mesh.transform.position = parentObj.transform.position;
        }

        return combinedMeshes;
    }

    /// <summary>
    /// Calculates the bounds of the passed in transform.
    /// </summary>
    /// <param name="objectTransform">Transform to calculate bounds for.</param>
    /// <returns></returns>
    public static Bounds GetTotalMeshFilterBounds(Transform objectTransform)
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
    /// Creates a cube with the passed in parameters.
    /// </summary>
    /// <param name="dimensions">The size dimensions to make the cube.</param>
    /// <param name="material">Material to apply to the mesh.</param>
    /// <param name="objectName">Name of the game object the mesh is attached to.</param>
    /// <returns>GameObject with the new ProBuilderMesh</returns>
    public static GameObject GenerateCube(Vector3 dimensions, Material material, string objectName)
    {
        ProBuilderMesh cube = ShapeGenerator.GenerateCube(PivotLocation.Center, new Vector3(dimensions.x, dimensions.y, dimensions.z));
        cube.SetMaterial(cube.faces, material);        
        cube.transform.name = objectName;

        return cube.gameObject;
    }

    /// <summary>
    /// Creates a poly shape with the passed in data. It also extrudes the mesh and applies a material to it.
    /// </summary>
    /// <param name="extrustionFactor">Amount to extrude by.</param>
    /// <param name="shapePoints">Points that make up the shape to create.</param>
    /// <param name="material">Material to apply to the mesh.</param>
    /// <param name="objectName">Name of the mesh being created</param>
    /// <returns>GameObject with the new ProBuilderMesh</returns>
    public static GameObject CreateShapeFromPolygon(float extrustionFactor, List<Vector3> shapePoints, Material material, string objectName)
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
    public static void RefreshMesh(ProBuilderMesh mesh)
    {
        mesh.ToMesh();
        mesh.Refresh();
    }

    /// <summary>
    /// Refresh passed in ProBuilderMeshes.
    /// </summary>
    /// <param name="meshes">ProBuilderMeshes to refresh.</param>
    public static void RefreshMeshes(List<ProBuilderMesh> meshes)
    {
        foreach (ProBuilderMesh mesh in meshes)
        {
            RefreshMesh(mesh);
        }
    }

    /// <summary>
    /// Rotate the passed in ProBuilderMesh's UVs by 90 degrees
    /// </summary>
    /// <param name="mesh">ProBuilderMesh whose UV we want to rotate.</param>
    /// <param name="positive">TRUE=Rotate by positive 90 degrees. FALSE=Rotate by negative 90 degrees</param>
    public static void RotateUVs90(ProBuilderMesh mesh, bool positive)
    {
        foreach (Face face in mesh.faces)
        {
            AutoUnwrapSettings a = face.uv;
            a.rotation = positive ? 90f : -90f;
            face.uv = a;
        }
    }
}
