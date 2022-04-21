
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

    protected List<Vector3> mCenterShapePoints = new List<Vector3>();
    protected List<Vector3> mLeftRightRailPoints = new List<Vector3>();
    protected List<Vector3> mTopBottomRailPoints = new List<Vector3>();
    protected List<Vector3> mBodyShapePoints = new List<Vector3>();

    protected float mWidth;
    protected float mHeight;
    protected float mRailWidth;
    protected Material mMaterial;
    protected GameObject mParent;
    protected float mRailDepth;
    protected float mCenterDepth;
    protected float mCabinetDepth;

    protected GameObject mCenterObj;
    protected GameObject mRightObj;
    protected GameObject mLeftObj;
    protected GameObject mTopObj;
    protected GameObject mBottomObj;
    protected GameObject mBodyObj;
    protected GameObject mHandleObj;

    protected ProBuilderMesh mRightMesh;
    protected ProBuilderMesh mBottomMesh;
    protected ProBuilderMesh mLeftMesh;
    protected ProBuilderMesh mTopMesh;
    protected ProBuilderMesh mCenterMesh;
    protected ProBuilderMesh mBodyMesh;
    protected ProBuilderMesh mHandleMesh;

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
    public virtual void Init(float width, float height, float cabinetDepth, float railWidth, float railDepth, float centerDepth, HandlePlacement handlePlacement, DoorOpenDirection doorOpenDirection, Material material, GameObject parent)
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
    protected void InitBodyShapePoints()
    {
        mBodyShapePoints.Add(new Vector3(mWidth, 0, 0));
        mBodyShapePoints.Add(new Vector3(mWidth, mHeight, 0));
        mBodyShapePoints.Add(new Vector3(0, mHeight, 0));
        mBodyShapePoints.Add(new Vector3(0, 0, 0));
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
    /// Makes a handle.
    /// </summary>
    /// <returns>GameObject for the handle.</returns>
    protected virtual GameObject MakeHandle()
    {
        return ProbuilderUtility.GenerateCube(new Vector3(.02f, .1f, .05f), mMaterial, "Handle");
    }

    /// <summary>
    /// Makes the cabinet.
    /// </summary>
    public virtual void MakeShape()
    {
        InitCenterShapePoints();
        InitLeftAndRightRailPoints();
        InitTopAndBottomRailPoints();
        InitBodyShapePoints();

        GameObject proBuilderParentObj = new GameObject();
        proBuilderParentObj.transform.name = "TEST";//TODO CH  TESTING

        //Build each object for each side. 
        mCenterObj = ProbuilderUtility.CreateShapeFromPolygon(mCenterDepth, mCenterShapePoints, mMaterial, ProbuilderUtility.Side.Center.ToString());
        mRightObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Right.ToString());
        mLeftObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Left.ToString());
        mTopObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Top.ToString());
        mBottomObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Bottom.ToString());
        mBodyObj = ProbuilderUtility.CreateShapeFromPolygon(mCabinetDepth - mRailDepth, mBodyShapePoints, mMaterial, ProbuilderUtility.Side.Body.ToString());
        mHandleObj = MakeHandle();

        mCenterObj.transform.position = proBuilderParentObj.transform.position;

        //Put all the pieces together into a cabinet shape.
        PlaceParts();
        PlaceHandle();

        //Get all the meshes so we can bevel them and refresh them.
        mRightMesh = mRightObj.GetComponent<ProBuilderMesh>();
        mBottomMesh = mBottomObj.GetComponent<ProBuilderMesh>();
        mLeftMesh = mLeftObj.GetComponent<ProBuilderMesh>();
        mTopMesh = mTopObj.GetComponent<ProBuilderMesh>();
        mCenterMesh = mCenterObj.GetComponent<ProBuilderMesh>();
        mBodyMesh = mBodyObj.GetComponent<ProBuilderMesh>();
        mHandleMesh = mHandleObj.GetComponent<ProBuilderMesh>();

        //Bevel the edges.
        //ProceduralDoorUtility.bevelEdge(bottomMesh, 2, 1, .062f);
        //ProceduralDoorUtility.bevelEdge(rightMesh, 5, 2, .062f);
        //ProceduralDoorUtility.bevelEdge(leftMesh, 2, 1, .062f);
        //ProceduralDoorUtility.bevelEdge(topMesh, 5, 2, .062f);

        //Rotate the right and left side UVs so they are oriented properly.
        ProbuilderUtility.RotateUVs90(mRightMesh, true);
        ProbuilderUtility.RotateUVs90(mLeftMesh, false);

        //Center the pivot in the back, bottom, center.
        mBodyMesh.CenterPivot(new int[] { 4, 5 });

        //Combine all the meshes so we only have one object. Something that is interesting is that the anchor point for the combined mesh is determined by the last mesh in this list.
        List<ProBuilderMesh> meshes = new List<ProBuilderMesh> { mCenterMesh, mTopMesh, mLeftMesh, mBottomMesh, mRightMesh, mHandleMesh, mBodyMesh };
        ProbuilderUtility.CombineAllMeshes(proBuilderParentObj, meshes);

        //Refresh the meshes so everything is synced up.
        ProbuilderUtility.RefreshMeshes(meshes);

        //Clean up the unneeded objects.
        DestroyImmediate(mCenterObj);
        DestroyImmediate(mTopObj);
        DestroyImmediate(mLeftObj);
        DestroyImmediate(mBottomObj);
        DestroyImmediate(mRightObj);
        DestroyImmediate(mBodyObj);
        DestroyImmediate(mHandleObj);

        proBuilderParentObj.transform.parent = mParent.transform;
        proBuilderParentObj.transform.eulerAngles = new Vector3(mParent.transform.eulerAngles.x, mParent.transform.eulerAngles.y, mParent.transform.eulerAngles.z);
        proBuilderParentObj.transform.position = new Vector3(mParent.transform.position.x + OFFSET_X, mParent.transform.position.y, mParent.transform.position.z);
    }

    /// <summary>
    /// Places the handle in the appropriate spot on the door; either the left or right side, and either in the top, middle, or bottom of the cabinet door.
    /// </summary>
    protected virtual void PlaceHandle()
    {
        Bounds railBounds;
        GameObject rail;

        if (mDoorOpenDirection == ProbuilderUtility.DoorOpenDirection.Right)
        {
            rail = mLeftObj;
        }
        else
        {
            rail = mRightObj;
        }

        mHandleObj.transform.parent = rail.transform;
        railBounds = ProbuilderUtility.GetTotalMeshFilterBounds(rail.transform);

        float xWithOffset = rail.transform.position.x + railBounds.size.x / 2;
        Bounds handleBounds = ProbuilderUtility.GetTotalMeshFilterBounds(rail.transform);
        
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Top)
        {
            mHandleObj.transform.position = new Vector3(xWithOffset, railBounds.size.y - railBounds.size.y / 3, rail.transform.position.z);
        }
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Middle)
        {
            mHandleObj.transform.position = new Vector3(xWithOffset, railBounds.size.y - railBounds.size.y / 4 - handleBounds.size.y / 3, rail.transform.position.z);
        }
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Bottom)
        {
            mHandleObj.transform.position = new Vector3(xWithOffset, rail.transform.position.y + railBounds.size.y / 4, railBounds.size.z);
        }
    }

    /// <summary>
    /// Places the cabinet sides and center parts.
    /// </summary>
    protected virtual void PlaceParts()
    {
        //Bounds for the different meshes.
        Bounds centerBounds = ProbuilderUtility.GetTotalMeshFilterBounds(mCenterObj.transform);
        Bounds topBounds = ProbuilderUtility.GetTotalMeshFilterBounds(mTopObj.transform);
        Bounds rightBounds = ProbuilderUtility.GetTotalMeshFilterBounds(mRightObj.transform);
        Bounds bodyBounds = ProbuilderUtility.GetTotalMeshFilterBounds(mBodyObj.transform);

        //Amount the door should be separated from the cabinet body.
        float doorFloatFactor = .0018f * Constants.FeetInAMeter;

        //Position and rotate the meshes properly.
        //right
        mRightObj.transform.position = new Vector3(
            mCenterObj.transform.position.x - rightBounds.size.x,
            mCenterObj.transform.position.y - topBounds.size.y,
            mCenterObj.transform.position.z + doorFloatFactor);

        mRightObj.transform.eulerAngles = new Vector3(
            mCenterObj.transform.eulerAngles.x,
            mCenterObj.transform.eulerAngles.y,
            mCenterObj.transform.eulerAngles.z);

        //top
        mTopObj.transform.position = new Vector3(
            mCenterObj.transform.position.x,
            mCenterObj.transform.position.y + centerBounds.size.y,
            mCenterObj.transform.position.z + doorFloatFactor);

        mTopObj.transform.eulerAngles = new Vector3(
            mCenterObj.transform.eulerAngles.x,
            mCenterObj.transform.eulerAngles.y,
            mCenterObj.transform.eulerAngles.z);

        //left
        mLeftObj.transform.position = new Vector3(
            mCenterObj.transform.position.x + centerBounds.size.x,
            mCenterObj.transform.position.y - topBounds.size.y,
            mCenterObj.transform.position.z + doorFloatFactor);

        mLeftObj.transform.eulerAngles = new Vector3(
            mCenterObj.transform.eulerAngles.x,
            mCenterObj.transform.eulerAngles.y,
            mCenterObj.transform.eulerAngles.z);

        //bottom
        mBottomObj.transform.position = new Vector3(
            mCenterObj.transform.position.x,
            mCenterObj.transform.position.y - topBounds.size.y,
            mCenterObj.transform.position.z + doorFloatFactor);

        mBottomObj.transform.eulerAngles = new Vector3(
            mCenterObj.transform.eulerAngles.x,
            mCenterObj.transform.eulerAngles.y,
            mCenterObj.transform.eulerAngles.z);

        //back
        mBodyObj.transform.position = new Vector3(
            mCenterObj.transform.position.x - rightBounds.size.x,
            mCenterObj.transform.position.y - rightBounds.size.x,
            mCenterObj.transform.position.z - bodyBounds.size.z);

        mBodyObj.transform.eulerAngles = new Vector3(
            mCenterObj.transform.eulerAngles.x,
            mCenterObj.transform.eulerAngles.y,
            mCenterObj.transform.eulerAngles.z);

        //center
        mCenterObj.transform.position = new Vector3(
            mCenterObj.transform.position.x,
            mCenterObj.transform.position.y,
            mCenterObj.transform.position.z + doorFloatFactor);
    }
}

