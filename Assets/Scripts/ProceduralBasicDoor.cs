﻿
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
    public float sideWidth;
    public Material material;
    public GameObject parent;
    public float sideRailThickness;
    public float centerThickness;
    const float FeetInAMeter = 3.28084f;
    protected float centerWidth = 0f;

    protected List<Vector3> mCenterShapePoints = new List<Vector3>();
    protected List<Vector3> mSideShapePoints = new List<Vector3>();

    /// <summary>
    /// Initialize the shape. Converts dimensions from meters to feet.
    /// </summary>
    /// <param name="width">Total width of the door.</param>
    /// <param name="height">Total height of the door.</param>
    /// <param name="material">Material to put on the door.</param>
    /// <param name="sideWidth">Width of the rails around the door.</param>
    /// <param name="sideRailThickness">The thickness of the side rail in the x direction.</param>
    /// <param name="centerThickness">The thickness of the center of the door in the x direction. If this is larger than the side rail thickness, the door could look odd.</param>
    /// <param name="parent">Gamne Object to attach the door to.</param>
    public void init(float width, float height, Material material, float sideWidth, float sideRailThickness, float centerThickness, GameObject parent)
    {
        this.parent = parent;
        this.width = width / FeetInAMeter;
        this.height = height / FeetInAMeter;
        this.material = material;
        this.sideWidth = sideWidth / FeetInAMeter;
        this.centerThickness = centerThickness / FeetInAMeter;
        this.sideRailThickness = sideRailThickness / FeetInAMeter;
        centerWidth = this.width - this.sideWidth * 2;
    }

    /// <summary>
    /// Initialize the points for the center of the door. All points must be plotted counter clockwise in the X,Z plane.
    /// </summary>
    protected void initCenterShapePoints()
    {
        mCenterShapePoints.Add(new Vector3(0, 0, 0));
        mCenterShapePoints.Add(new Vector3(0, 0, centerWidth));
        mCenterShapePoints.Add(new Vector3(centerThickness, 0, centerWidth));
        mCenterShapePoints.Add(new Vector3(centerThickness, 0, 0));
    }

    /// <summary>d
    /// Initialize the points for the sides of the door. All points must be plotted counter clockwise in the X,Z plane.
    /// </summary>
    protected void initSideShapePoints()
    {
        mSideShapePoints.Add(new Vector3(0, 0, 0));
        mSideShapePoints.Add(new Vector3(0, 0, sideWidth));
        mSideShapePoints.Add(new Vector3(sideRailThickness, 0, sideWidth));
        mSideShapePoints.Add(new Vector3(sideRailThickness, 0, 0));
    }

    /// <summary>
    /// Makes the door.
    /// </summary>
    public void makeDoor()
    {
        initCenterShapePoints();
        initSideShapePoints();

        float centerHeight = height - sideWidth * 2;

        GameObject pbObj = new GameObject();
        pbObj.transform.parent = parent.transform;
        pbObj.transform.position = parent.transform.position;
        pbObj.transform.name = "TEST";//TODO CH  TESTING

        //Build each object for each side. 
        GameObject centerObj = ProceduralDoorUtility.makeProbuilderMesh(centerHeight, mCenterShapePoints, material, ProceduralDoorUtility.CENTER);
        GameObject rightObj = ProceduralDoorUtility.makeProbuilderMesh(centerHeight + sideWidth * 2, mSideShapePoints, material, ProceduralDoorUtility.RIGHT);
        GameObject leftObj = ProceduralDoorUtility.makeProbuilderMesh(centerHeight + sideWidth * 2, mSideShapePoints, material, ProceduralDoorUtility.LEFT);
        GameObject topObj = ProceduralDoorUtility.makeProbuilderMesh(centerWidth, mSideShapePoints, material, ProceduralDoorUtility.TOP);
        GameObject bottomObj = ProceduralDoorUtility.makeProbuilderMesh(centerWidth, mSideShapePoints, material, ProceduralDoorUtility.BOTTOM);

        centerObj.transform.position = pbObj.transform.position;

        //Combine all meshes into one object.
        ProceduralDoorUtility.assembleDoor(centerObj, topObj, leftObj, bottomObj, rightObj);

        //Get all the meshes so we can bevel them and refresh them.
        ProBuilderMesh rightMesh = rightObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh bottomMesh = bottomObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh leftMesh = leftObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh topMesh = topObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh centerMesh = centerObj.GetComponent<ProBuilderMesh>();

        //Bevel the edges.
        //ProceduralDoorUtility.bevelEdge(bottomMesh, 2, 1, .062f);
        //ProceduralDoorUtility.bevelEdge(rightMesh, 5, 2, .062f);
        //ProceduralDoorUtility.bevelEdge(leftMesh, 2, 1, .062f);
        //ProceduralDoorUtility.bevelEdge(topMesh, 5, 2, .062f);

        //Rotate the right and left side UVs so they are oriented properly.
        ProceduralDoorUtility.rotateUVs90(rightMesh, true);
        ProceduralDoorUtility.rotateUVs90(leftMesh, false);

        //Combine all the meshes so we only have one object.
        List<ProBuilderMesh> meshes = new List<ProBuilderMesh> { centerMesh, topMesh, leftMesh, bottomMesh, rightMesh };
        //ProceduralDoorUtility.combineAllMeshes(this.gameObject, meshes);

        //Refresh the meshes so everything is synced up.
        ProceduralDoorUtility.refreshMeshes(meshes);

        //Clean up the unneeded objects. //TODO CH  CAN'T DESTROY OBJECTS WHEN IN EDITOR. CAN ONLY BE DONE DURING GAME.
        //Destroy(centerObj);
        //Destroy(topObj);
        //Destroy(leftObj);
        //Destroy(bottomObj);
        //Destroy(rightObj);

        centerObj.transform.parent = pbObj.transform;
        rightObj.transform.parent = pbObj.transform;
        leftObj.transform.parent = pbObj.transform;
        bottomObj.transform.parent = pbObj.transform;
        topObj.transform.parent = pbObj.transform;

        //TODO CH  WHY IS THE Y = -90 NOT GETTING HONERED AT IMPORT? INSTEAD IT IS AT Y = 60.
        //pbObj.transform.eulerAngles = new Vector3(parent.transform.eulerAngles.x, parent.transform.eulerAngles.y, parent.transform.eulerAngles.z);
    }
}

