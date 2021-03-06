using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using static ProbuilderUtility;

/// <summary>
/// Builds a basic cabinet with a body, and a shaker door with a center and 4 rails (left, right, top, bottom) using Probuilder.
/// </summary>
public class ProBuilderCabinetWithShakerDoor : MonoBehaviour
{
    /**
     * TODO CH  For some reason the X value of the pivot points of the imported models is slightly off by this much. This will
     * most likely need to be removed when the pivot point issue is fixed at the source.
     */
    protected const float OFFSET_X = .021f;

    protected float mCenterWidth = 0f;
    protected float mCenterHeight = 0f;
    protected bool mUseHandleInsteadOfKnob = true;
    protected float mWidth;
    protected float mHeight;
    protected float mRailWidth;
    protected float mRailDepth;
    protected float mCenterDepth;
    protected float mCabinetDepth;

    protected HandlePlacement mHandlePlacement;
    protected DoorOpenDirection mDoorOpenDirection;
    protected Material mMaterial;
    protected GameObject mParent;

    protected List<Vector3> mCenterShapePoints = new List<Vector3>();
    protected List<Vector3> mLeftRightRailPoints = new List<Vector3>();
    protected List<Vector3> mTopBottomRailPoints = new List<Vector3>();
    protected List<Vector3> mBodyShapePoints = new List<Vector3>();

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
    /// <param name="useHandleInsteadOfKnob">TRUE=attaches handles to the cabinet. FALSE=attaches knobs to the cabinet.</param>
    /// <param name="handlePlacement">Denotes where the placement of the handle should be.</param>
    /// <param name="doorOpenDirection">The direction the door would open.</param>
    /// <param name="material">Material of the cabinet</param>
    /// <param name="parent">Parent object anchor to.</param>
    public virtual void Init(float width, float height, float cabinetDepth, float railWidth, float railDepth, float centerDepth, bool useHandleInsteadOfKnob,
        HandlePlacement handlePlacement, DoorOpenDirection doorOpenDirection, Material material, GameObject parent)
    {
        mParent = parent;
        mWidth = width / Constants.FeetInAMeter;
        mHeight = height / Constants.FeetInAMeter;
        mMaterial = material;
        mRailWidth = railWidth / Constants.FeetInAMeter;
        mCenterDepth = centerDepth / Constants.FeetInAMeter;
        mRailDepth = railDepth / Constants.FeetInAMeter;
        mCabinetDepth = cabinetDepth / Constants.FeetInAMeter;
        mHandlePlacement = handlePlacement;
        mDoorOpenDirection = doorOpenDirection;
        mUseHandleInsteadOfKnob = useHandleInsteadOfKnob;
        mCenterWidth = mWidth - mRailWidth * 2;
        mCenterHeight = mHeight - mRailWidth * 2;
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

    protected virtual GameObject MakeKnob()
    {
        return ProbuilderUtility.GenerateCube(new Vector3(.05f, .05f, .05f), mMaterial, "Knob");
    }

    /// <summary>
    /// Makes a handle.
    /// </summary>
    /// <returns>GameObject for the handle.</returns>
    protected virtual GameObject MakeHandle()
    {
        return ProbuilderUtility.GenerateCube(new Vector3(.00952f, .14605f, .0349f), mMaterial, "Handle");
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
        proBuilderParentObj.transform.name = "Cabinet";

        //Build each object for each side. 
        mCenterObj = ProbuilderUtility.CreateShapeFromPolygon(mCenterDepth, mCenterShapePoints, mMaterial, ProbuilderUtility.Side.Center.ToString());
        mRightObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Right.ToString());
        mLeftObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Left.ToString());
        mTopObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Top.ToString());
        mBottomObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Bottom.ToString());
        mBodyObj = ProbuilderUtility.CreateShapeFromPolygon(mCabinetDepth - mRailDepth, mBodyShapePoints, mMaterial, ProbuilderUtility.Side.Body.ToString());
        mHandleObj = mUseHandleInsteadOfKnob ? MakeHandle() : MakeKnob();
        
        mCenterObj.transform.position = proBuilderParentObj.transform.position;

        mCenterObj.transform.parent = proBuilderParentObj.transform;
        mRightObj.transform.parent = proBuilderParentObj.transform;
        mLeftObj.transform.parent = proBuilderParentObj.transform;
        mTopObj.transform.parent = proBuilderParentObj.transform;
        mBottomObj.transform.parent = proBuilderParentObj.transform;
        mBodyObj.transform.parent = proBuilderParentObj.transform;
        mHandleObj.transform.parent = proBuilderParentObj.transform;

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

        //Rotate the right and left side UVs so they are oriented properly.
        ProbuilderUtility.RotateUVs90(mRightMesh, true);
        ProbuilderUtility.RotateUVs90(mLeftMesh, false);

        //Center the pivot in the back, bottom, center.
        mBodyMesh.CenterPivot(new int[] { 4, 5 });

        //Combine all the meshes so we only have one object. Something that is interesting is that the anchor point for the combined mesh is determined by the last mesh in this list.
        List<ProBuilderMesh> meshes = new List<ProBuilderMesh> { mCenterMesh, mTopMesh, mLeftMesh, mBottomMesh, mRightMesh, mHandleMesh, mBodyMesh };
        
        //Uncomment this to combine all meshes.
        //ProbuilderUtility.CombineAllMeshes(proBuilderParentObj, meshes);

        //Refresh the meshes so everything is synced up.
        ProbuilderUtility.RefreshMeshes(meshes);

        //Clean up the unneeded objects.
        //DestroyImmediate(mCenterObj);
        //DestroyImmediate(mTopObj);
        //DestroyImmediate(mLeftObj);
        //DestroyImmediate(mBottomObj);
        //DestroyImmediate(mRightObj);
        //DestroyImmediate(mBodyObj);
        //DestroyImmediate(mHandleObj);

        proBuilderParentObj.transform.parent = mParent.transform;
        proBuilderParentObj.transform.eulerAngles = new Vector3(mParent.transform.eulerAngles.x, mParent.transform.eulerAngles.y, mParent.transform.eulerAngles.z);
        proBuilderParentObj.transform.position = new Vector3(mParent.transform.position.x + OFFSET_X, mParent.transform.position.y, mParent.transform.position.z);
    }

    /// <summary>
    /// Places the handle in the appropriate spot on the door; either the left or right side, and either in the top, middle, or bottom of the cabinet door.
    /// Handle pivot points are dead center in the object.
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
        Bounds handleBounds = ProbuilderUtility.GetTotalMeshFilterBounds(mHandleObj.transform);

        //Handle is resting just below the top rail.
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Top)
        {
            //Handle
            if (mUseHandleInsteadOfKnob)
            {
                mHandleObj.transform.position = new Vector3(xWithOffset, mCenterHeight - handleBounds.size.y / 2, rail.transform.position.z);
            }

            //Knob
            else
            {
                mHandleObj.transform.position = new Vector3(xWithOffset, mCenterHeight + handleBounds.size.y / 2, rail.transform.position.z);
            }

        }

        //Handle is right in the middle of the door.
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Middle)
        {
            //Works for handles or knobs.
            mHandleObj.transform.position = new Vector3(xWithOffset, railBounds.size.y - railBounds.size.y / 2 - handleBounds.size.y / 2, rail.transform.position.z);
        }

        //Handle is resting just above the bottom rail.
        if (mHandlePlacement == ProbuilderUtility.HandlePlacement.Bottom)
        {
            //Handle
            if (mUseHandleInsteadOfKnob)
            {
                //Sometimes the handle goes slightly past the bottom rail. This is because the handle is too large for the rail, so we adjust by the difference so the handle is 
                //resting just above the bottom rail.
                float diffY = handleBounds.size.y - railBounds.size.y;
                float newY = diffY > 0f ? diffY : 0;
                mHandleObj.transform.position = new Vector3(xWithOffset, handleBounds.size.y / 2 + newY, railBounds.size.z);
            }

            //Knob
            else
            {
                mHandleObj.transform.position = new Vector3(xWithOffset, 0 - handleBounds.size.y / 2, railBounds.size.z);
            }
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

