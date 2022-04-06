using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

/// <summary>
/// Trying to make a basic door with a center and 4 sides.
/// </summary>
public class ProceduralBasicDoor : MonoBehaviour
{
    public float width;
    public float height;
    public float sideWidth = 2.5f;
    public Material material;
    
    protected float centerWidth = 0f;
    
    protected List<Vector3> mCenterShapePoints = new List<Vector3>();
    protected List<Vector3> mSideShapePoints = new List<Vector3>();

    /// <summary>
    /// Initialize the shape. 
    /// </summary>
    /// <param name="width">Total width of the door.</param>
    /// <param name="height">Total height of the door.</param>
    /// <param name="material">Material to put on the door.</param>
    /// <param name="sideWidth">Width of the rails around the door.</param>
    public void init(float width, float height, Material material, float sideWidth)
    {
        this.width = width;
        this.height = height;
        this.material = material;
        this.sideWidth = sideWidth;
    }

    /// <summary>
    /// Initialize the points for the center of the door. All points must be plotted counter clockwise in the X,Z plane.
    /// </summary>
    protected void initCenterShapePoints()
    {
        centerWidth = width - sideWidth * 2;

        mCenterShapePoints.Add(new Vector3(0, 0, 0));
        mCenterShapePoints.Add(new Vector3(0, 0, centerWidth));
        mCenterShapePoints.Add(new Vector3(.5f, 0, centerWidth));
        mCenterShapePoints.Add(new Vector3(.5f, 0, 0));

        //mCenterShapePoints.Add(new Vector3(0, 0, 0));
        //mCenterShapePoints.Add(new Vector3(3, 0, 0));
        //mCenterShapePoints.Add(new Vector3(3, 0, 2));
        //mCenterShapePoints.Add(new Vector3(2, 0, 3));
        //mCenterShapePoints.Add(new Vector3(0, 0, 3));
    }

    /// <summary>
    /// Initialize the points for the sides of the door. All points must be plotted counter clockwise in the X,Z plane.
    /// </summary>
    protected void initSideShapePoints()
    {
        mSideShapePoints.Add(new Vector3(0, 0, 0));
        mSideShapePoints.Add(new Vector3(0, 0, sideWidth));
        mSideShapePoints.Add(new Vector3(.75f, 0, sideWidth));
        mSideShapePoints.Add(new Vector3(.75f, 0, 0));
    }

    /// <summary>
    /// Makes the door.
    /// </summary>
    public void makeDoor()
    {
        initCenterShapePoints();
        initSideShapePoints();

        float centerHeight = height - sideWidth * 2;

        //Build each object for each side. 
        GameObject centerObj = ProceduralDoorUtility.makeProbuilderMesh(centerHeight, mCenterShapePoints, material, ProceduralDoorUtility.CENTER);
        GameObject rightObj = ProceduralDoorUtility.makeProbuilderMesh(centerHeight + sideWidth * 2, mSideShapePoints, material, ProceduralDoorUtility.RIGHT);
        GameObject leftObj = ProceduralDoorUtility.makeProbuilderMesh(centerHeight + sideWidth * 2, mSideShapePoints, material, ProceduralDoorUtility.LEFT);
        GameObject topObj = ProceduralDoorUtility.makeProbuilderMesh(centerWidth, mSideShapePoints, material, ProceduralDoorUtility.TOP);
        GameObject bottomObj = ProceduralDoorUtility.makeProbuilderMesh(centerWidth, mSideShapePoints, material, ProceduralDoorUtility.BOTTOM);

        //Combine all meshes into one object.
        ProceduralDoorUtility.assembleShape(centerObj, topObj, leftObj, bottomObj, rightObj);

        //Get all the meshes so we can bevel them and refresh them.
        ProBuilderMesh rightMesh = rightObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh bottomMesh = bottomObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh leftMesh = leftObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh topMesh = topObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh centerMesh = centerObj.GetComponent<ProBuilderMesh>();

        //Bevel the edges.
        ProceduralDoorUtility.bevelEdge(bottomMesh, 2, 1, .062f);
        ProceduralDoorUtility.bevelEdge(rightMesh, 5, 2, .062f);
        ProceduralDoorUtility.bevelEdge(leftMesh, 2, 1, .062f);
        ProceduralDoorUtility.bevelEdge(topMesh, 5, 2, .062f);

        //Rotate the right and left side UVs so they are oriented properly.
        ProceduralDoorUtility.rotateUVs90(rightMesh, true);
        ProceduralDoorUtility.rotateUVs90(leftMesh, false);

        //Combine all the meshes so we only have one object.
        List<ProBuilderMesh> meshes = new List<ProBuilderMesh> { centerMesh, topMesh, leftMesh, bottomMesh, rightMesh };
        ProceduralDoorUtility.combineAllMeshes(this.gameObject, meshes);

        //Refresh the meshes so everything is synced up.
        ProceduralDoorUtility.refreshMeshes(meshes);

        //Clean up the unneeded objects.
        Destroy(centerObj);
        Destroy(topObj);
        Destroy(leftObj);
        Destroy(bottomObj);
        Destroy(rightObj);
    }
}

