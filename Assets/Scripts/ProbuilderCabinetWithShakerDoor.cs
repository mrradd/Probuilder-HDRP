
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using static ProbuilderUtility;

/// <summary>
/// Builds a basic cabinet with a shaker door with a center and 4 rails (left, right, top, bottom) using Probuilder.
/// </summary>
public class ProbuilderCabinetWithShakerDoor : MonoBehaviour
{
    /**
     * TODO CH  For some reason the X value of the pivot points of the imported models is slightly off by this much. This will
     * most likely need to be removed when the pivot point issue is fixed at the source.
     */
    protected const float OFFSET_X = .021f;

    protected float mCenterWidth = 0f;
    protected float mCenterHeight = 0f;
    protected HandlePlacement mHandlePlacement;
    protected DoorOpenDirection mDoorOpenDirection;
    protected ProBuilderMesh mHandleMesh;

    protected List<Vector3> mCenterShapePoints = new List<Vector3>();
    protected List<Vector3> mLeftRightRailPoints = new List<Vector3>();
    protected List<Vector3> mTopBottomRailPoints = new List<Vector3>();
    protected List<Vector3> mCabinetBodyPoints = new List<Vector3>();

    protected float mWidth;
    protected float mHeight;
    protected float mRailWidth;
    protected Material mMaterial;
    protected GameObject mParent;
    protected float mRailDepth;
    protected float mCenterDepth;
    protected float mCabinetDepth;

    /// <summary>
    /// Assembles the cabinet with the passed in parts.
    /// </summary>
    /// <param name="center">Center object of the door.</param>
    /// <param name="top">Top rail of the door.</param>
    /// <param name="left">Left rail of the door.</param>
    /// <param name="bottom">Bottom rail of the door.</param>
    /// <param name="right">Right rail of the door.</param>
    /// <param name="body">Back of the cabinet.</param>
    /// <param name="handle">Handle of the cabinet.</param>
    protected void AssembleCabinet(GameObject center, GameObject top, GameObject left, GameObject bottom, GameObject right, GameObject body, GameObject handle)
    {
        PlaceParts(center, top, left, bottom, right, body);
        PlaceHandle(left, right, handle);
    }

    /// <summary>
    /// Initialize the shape. Converts dimensions from meters to feet.
    /// </summary>
    /// <param name="width">Total width of the door.</param>
    /// <param name="height">Total height of the door.</param>
    /// <param name="cabinetDepth">Depth of the cabinet we are attaching the door to.</param>
    /// <param name="railWidth">Width each rail should be.</param>
    /// <param name="railDepth">Depth (z-axis) each rail should be.</param>
    /// <param name="centerDepth">Depth (z-axis) the center should be.</param>
    /// <param name="handlePlacement">Denotes where the placement of the handle should be.</param>
    /// <param name="doorOpenDirection">The direction the door would open.</param>
    /// <param name="material">Material of the cabinet</param>
    /// <param name="parent">Parent object anchor to.</param>
    public void Init(float width, float height, float cabinetDepth, float railWidth, float railDepth, float centerDepth, HandlePlacement handlePlacement, DoorOpenDirection doorOpenDirection, Material material, GameObject parent)
    {
        this.mParent = parent;
        this.mWidth = width / Constants.FeetInAMeter;
        this.mHeight = height / Constants.FeetInAMeter;
        this.mMaterial = material;
        this.mRailWidth = railWidth / Constants.FeetInAMeter;
        this.mCenterDepth = centerDepth / Constants.FeetInAMeter;
        this.mRailDepth = railDepth / Constants.FeetInAMeter;
        this.mCabinetDepth = cabinetDepth / Constants.FeetInAMeter;
        this.mHandlePlacement = handlePlacement;
        this.mDoorOpenDirection = doorOpenDirection;
        mCenterWidth = this.mWidth - this.mRailWidth * 2;
        mCenterHeight = this.mHeight - this.mRailWidth * 2;
    }

    /// <summary>
    /// Initializes the body of the cabinet.
    /// </summary>
    protected void InitCabinetBodyPoints()
    {
        mCabinetBodyPoints.Add(new Vector3(mWidth, 0, 0));
        mCabinetBodyPoints.Add(new Vector3(mWidth, mHeight, 0));
        mCabinetBodyPoints.Add(new Vector3(0, mHeight, 0));
        mCabinetBodyPoints.Add(new Vector3(0, 0, 0));
    }

    /// <summary>
    /// Initialize the points for the center of the door. All points must be plotted counter clockwise in the X,Z plane.
    /// </summary>
    protected void InitCenterShapePoints()
    {
        mCenterShapePoints.Add(new Vector3(mCenterWidth, 0, 0));
        mCenterShapePoints.Add(new Vector3(mCenterWidth, mCenterHeight, 0));
        mCenterShapePoints.Add(new Vector3(0, mCenterHeight, 0));
        mCenterShapePoints.Add(new Vector3(0, 0, 0));
    }

    /// <summary>d
    /// Initialize the points for the left and right rails of the door. All points must be plotted counter clockwise in the X,Y plane.
    /// </summary>
    protected void InitLeftAndRightRailPoints()
    {
        mLeftRightRailPoints.Add(new Vector3(mRailWidth, 0, 0));
        mLeftRightRailPoints.Add(new Vector3(mRailWidth, mHeight, 0));
        mLeftRightRailPoints.Add(new Vector3(0, mHeight, 0));
        mLeftRightRailPoints.Add(new Vector3(0, 0, 0));
    }

    /// <summary>d
    /// Initialize the points for the top and bottom rails of the door. All points must be plotted counter clockwise in the X,Y plane.
    /// </summary>
    protected void InitTopAndBottomRailPoints()
    {
        mTopBottomRailPoints.Add(new Vector3(mCenterWidth, 0, 0));
        mTopBottomRailPoints.Add(new Vector3(mCenterWidth, mRailWidth, 0));
        mTopBottomRailPoints.Add(new Vector3(0, mRailWidth, 0));
        mTopBottomRailPoints.Add(new Vector3(0, 0, 0));
    }

    /// <summary>
    /// Makes the door.
    /// </summary>
    public void MakeDoor()
    {
        InitCenterShapePoints();
        InitLeftAndRightRailPoints();
        InitTopAndBottomRailPoints();
        InitCabinetBodyPoints();

        GameObject proBuilderParentObj = new GameObject();
        proBuilderParentObj.transform.name = "TEST";//TODO CH  TESTING

        //Build each object for each side. 
        GameObject centerObj = ProbuilderUtility.CreateShapeFromPolygon(mCenterDepth, mCenterShapePoints, mMaterial, ProbuilderUtility.Side.Center.ToString());
        GameObject rightObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Right.ToString());
        GameObject leftObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Left.ToString());
        GameObject topObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Top.ToString());
        GameObject bottomObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Bottom.ToString());
        GameObject bodyObj = ProbuilderUtility.CreateShapeFromPolygon(mCabinetDepth - mRailDepth, mCabinetBodyPoints, mMaterial, ProbuilderUtility.Side.Body.ToString());
        GameObject handleObj = ProbuilderUtility.GenerateCube(new Vector3(.02f, .1f, .05f), mMaterial, "Handle");

        centerObj.transform.position = proBuilderParentObj.transform.position;

        //Put all the pieces together into a cabinet shape.
        AssembleCabinet(centerObj, topObj, leftObj, bottomObj, rightObj, bodyObj, handleObj);

        //Get all the meshes so we can bevel them and refresh them.
        ProBuilderMesh rightMesh = rightObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh bottomMesh = bottomObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh leftMesh = leftObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh topMesh = topObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh centerMesh = centerObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh bodyMesh = bodyObj.GetComponent<ProBuilderMesh>();
        ProBuilderMesh handleMesh = handleObj.GetComponent<ProBuilderMesh>();

        //Bevel the edges.
        //ProceduralDoorUtility.bevelEdge(bottomMesh, 2, 1, .062f);
        //ProceduralDoorUtility.bevelEdge(rightMesh, 5, 2, .062f);
        //ProceduralDoorUtility.bevelEdge(leftMesh, 2, 1, .062f);
        //ProceduralDoorUtility.bevelEdge(topMesh, 5, 2, .062f);

        //Rotate the right and left side UVs so they are oriented properly.
        ProbuilderUtility.RotateUVs90(rightMesh, true);
        ProbuilderUtility.RotateUVs90(leftMesh, false);

        //Center the pivot in the back, bottom, center.
        bodyMesh.CenterPivot(new int[] { 4, 5 });

        //Combine all the meshes so we only have one object. Something that is interesting is that the anchor point for the combined mesh is determined by the last mesh in this list.
        List<ProBuilderMesh> meshes = new List<ProBuilderMesh> { centerMesh, topMesh, leftMesh, bottomMesh, rightMesh, handleMesh, bodyMesh };
        ProbuilderUtility.CombineAllMeshes(proBuilderParentObj, meshes);

        //Refresh the meshes so everything is synced up.
        ProbuilderUtility.RefreshMeshes(meshes);

        //Clean up the unneeded objects.
        DestroyImmediate(centerObj);
        DestroyImmediate(topObj);
        DestroyImmediate(leftObj);
        DestroyImmediate(bottomObj);
        DestroyImmediate(rightObj);
        DestroyImmediate(bodyObj);
        DestroyImmediate(handleObj);

        proBuilderParentObj.transform.parent = mParent.transform;
        proBuilderParentObj.transform.eulerAngles = new Vector3(mParent.transform.eulerAngles.x, mParent.transform.eulerAngles.y, mParent.transform.eulerAngles.z);
        proBuilderParentObj.transform.position = new Vector3(mParent.transform.position.x + OFFSET_X, mParent.transform.position.y, mParent.transform.position.z);
    }

    /// <summary>
    /// Places the handle in the appropriate spot on the door; either the left or right side, and either in the top, middle, or bottom of the cabinet door.
    /// </summary>
    /// <param name="left">Left Rail of the door.</param>
    /// <param name="right">Right Rail of the door.</param>
    /// <param name="handle">Handle of the door.</param>
    public void PlaceHandle(GameObject left, GameObject right, GameObject handle)
    {
        Bounds railBounds;
        GameObject rail;

        if (mDoorOpenDirection == ProbuilderUtility.DoorOpenDirection.Right)
        {
            rail = left;
        }
        else
        {
            rail = right;
        }

        handle.transform.parent = rail.transform;
        railBounds = ProbuilderUtility.GetTotalMeshFilterBounds(rail.transform);

        float xWithOffset = rail.transform.position.x + railBounds.size.x / 2;
        Bounds handleBounds = ProbuilderUtility.GetTotalMeshFilterBounds(rail.transform);
        
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Top)
        {
            handle.transform.position = new Vector3(xWithOffset, railBounds.size.y - railBounds.size.y / 3, rail.transform.position.z);
        }
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Middle)
        {
            handle.transform.position = new Vector3(xWithOffset, railBounds.size.y - railBounds.size.y / 4 - handleBounds.size.y / 3, rail.transform.position.z);
        }
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Bottom)
        {
            handle.transform.position = new Vector3(xWithOffset, rail.transform.position.y + railBounds.size.y / 4, railBounds.size.z);
        }
    }

    /// <summary>
    /// Places the cabinet parts.
    /// </summary>
    /// <param name="center">Center object of the door.</param>
    /// <param name="top">Top rail of the door.</param>
    /// <param name="left">Left rail of the door.</param>
    /// <param name="bottom">Bottom rail of the door.</param>
    /// <param name="right">Right rail of the door.</param>
    /// <param name="body">Back of the cabinet.</param>
    protected void PlaceParts(GameObject center, GameObject top, GameObject left, GameObject bottom, GameObject right, GameObject body)
    {
        //Bounds for the different meshes.
        Bounds centerBounds = ProbuilderUtility.GetTotalMeshFilterBounds(center.transform);
        Bounds topBounds = ProbuilderUtility.GetTotalMeshFilterBounds(top.transform);
        Bounds rightBounds = ProbuilderUtility.GetTotalMeshFilterBounds(right.transform);
        Bounds bodyBounds = ProbuilderUtility.GetTotalMeshFilterBounds(body.transform);

        //Amount the door should be separated from the cabinet body.
        float doorFloatFactor = .0018f * Constants.FeetInAMeter;

        //Position and rotate the meshes properly.
        //right
        right.transform.position = new Vector3(
            center.transform.position.x - rightBounds.size.x,
            center.transform.position.y - topBounds.size.y,
            center.transform.position.z + doorFloatFactor);

        right.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //top
        top.transform.position = new Vector3(
            center.transform.position.x,
            center.transform.position.y + centerBounds.size.y,
            center.transform.position.z + doorFloatFactor);

        top.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //left
        left.transform.position = new Vector3(
            center.transform.position.x + centerBounds.size.x,
            center.transform.position.y - topBounds.size.y,
            center.transform.position.z + doorFloatFactor);

        left.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //bottom
        bottom.transform.position = new Vector3(
            center.transform.position.x,
            center.transform.position.y - topBounds.size.y,
            center.transform.position.z + doorFloatFactor);

        bottom.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //back
        body.transform.position = new Vector3(
            center.transform.position.x - rightBounds.size.x,
            center.transform.position.y - rightBounds.size.x,
            center.transform.position.z - bodyBounds.size.z);

        body.transform.eulerAngles = new Vector3(
            center.transform.eulerAngles.x,
            center.transform.eulerAngles.y,
            center.transform.eulerAngles.z);

        //center
        center.transform.position = new Vector3(
            center.transform.position.x,
            center.transform.position.y,
            center.transform.position.z + doorFloatFactor);
    }
}

