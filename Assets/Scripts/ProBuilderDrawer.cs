using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using static ProbuilderUtility;

/// <summary>
/// Builds a basic drawer with a body, center, and 4 rails (left, right, top, bottom) using Probuilder.
/// </summary>
public class ProBuilderDrawer : ProBuilderCabinetWithShakerDoor
{
    protected int mNumberOfHandles;
    protected List<Handle> mHandles = new List<Handle>();

    /// <summary>
    /// This instance of the Init function should not be used for the class ProBuilderDrawer. This function throws an exception.
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
    [Obsolete("This instance of ProBuilderDrawer.Init does nothing.", true)]
    public override void Init(float width, float height, float cabinetDepth, float railWidth, float railDepth, float centerDepth, bool useHandleInsteadOfKnob,
        HandlePlacement handlePlacement, DoorOpenDirection doorOpenDirection, Material material, GameObject parent)
    {
        throw new Exception("This instance of ProBuilderDrawer.Init does nothing. Do not use this method.");
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
    /// <param name="numberOfHandles">Denotes where the number of drawer handles.</param>
    /// <param name="useHandleInsteadOfKnob">TRUE=attaches handles to the drawer. FALSE=attaches knobs to the drawer.</param>
    /// <param name="material">Material of the cabinet</param>
    /// <param name="parent">Parent object anchor to.</param>
    public void Init(float width, float height, float cabinetDepth, float railWidth, float railDepth, float centerDepth, int numberOfHandles,
        bool useHandleInsteadOfKnob, Material material, GameObject parent)
    {
        mParent = parent;
        mWidth = width / Constants.FeetInAMeter;
        mHeight = height / Constants.FeetInAMeter;
        mMaterial = material;
        mRailWidth = railWidth / Constants.FeetInAMeter;
        mCenterDepth = centerDepth / Constants.FeetInAMeter;
        mRailDepth = railDepth / Constants.FeetInAMeter;
        mCabinetDepth = cabinetDepth / Constants.FeetInAMeter;
        mNumberOfHandles = numberOfHandles;
        mUseHandleInsteadOfKnob = useHandleInsteadOfKnob;

        mCenterWidth = mWidth - mRailWidth * 2;
        mCenterHeight = mHeight - mRailWidth * 2;
    }

    /// <summary>
    /// Makes a handle.
    /// </summary>
    /// <returns>GameObject for the handle.</returns>
    protected override GameObject MakeHandle()
    {
        return ProbuilderUtility.GenerateCube(new Vector3(.146f, .00952f, .0349f), mMaterial, "Handle");
    }

    /// <summary>
    /// Makes the drawer.
    /// </summary>
    public override void MakeShape()
    {
        InitCenterShapePoints();
        InitLeftAndRightRailPoints();
        InitTopAndBottomRailPoints();
        InitBodyShapePoints();

        GameObject proBuilderParentObj = new GameObject();
        proBuilderParentObj.transform.name = "Drawer";

        //Build each object for each side. 
        mCenterObj = ProbuilderUtility.CreateShapeFromPolygon(mCenterDepth, mCenterShapePoints, mMaterial, ProbuilderUtility.Side.Center.ToString());
        mRightObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Right.ToString());
        mLeftObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mLeftRightRailPoints, mMaterial, ProbuilderUtility.Side.Left.ToString());
        mTopObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Top.ToString());
        mBottomObj = ProbuilderUtility.CreateShapeFromPolygon(mRailDepth, mTopBottomRailPoints, mMaterial, ProbuilderUtility.Side.Bottom.ToString());
        mBodyObj = ProbuilderUtility.CreateShapeFromPolygon(mCabinetDepth - mRailDepth, mBodyShapePoints, mMaterial, ProbuilderUtility.Side.Body.ToString());

        for (int i = 0; i < mNumberOfHandles; i++)
        {
            GameObject handleObj = mUseHandleInsteadOfKnob ? MakeHandle() : MakeKnob();
            handleObj.transform.parent = proBuilderParentObj.transform;
            mHandles.Add(new Handle(handleObj, handleObj.GetComponent<ProBuilderMesh>()));
        }

        mCenterObj.transform.position = proBuilderParentObj.transform.position;

        mCenterObj.transform.parent = proBuilderParentObj.transform;
        mRightObj.transform.parent = proBuilderParentObj.transform;
        mLeftObj.transform.parent = proBuilderParentObj.transform;
        mTopObj.transform.parent = proBuilderParentObj.transform;
        mBottomObj.transform.parent = proBuilderParentObj.transform;
        mBodyObj.transform.parent = proBuilderParentObj.transform;

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

        //Rotate the right and left side UVs so they are oriented properly.
        ProbuilderUtility.RotateUVs90(mRightMesh, true);
        ProbuilderUtility.RotateUVs90(mLeftMesh, false);

        //Center the pivot in the back, bottom, center.
        mBodyMesh.CenterPivot(new int[] { 4, 5 });

        //Combine all the meshes so we only have one object. Something that is interesting is that the anchor point for the combined mesh is determined by the last mesh in this list.
        List<ProBuilderMesh> meshes = new List<ProBuilderMesh>();
        foreach (Handle h in mHandles)
        {
            meshes.Add(h.mesh);
        }

        meshes.AddRange(new List<ProBuilderMesh> { mCenterMesh, mTopMesh, mLeftMesh, mBottomMesh, mRightMesh, mBodyMesh });
        
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

        //foreach (Handle h in mHandles)
        //{
        //    DestroyImmediate(h.gameObject);
        //}

        proBuilderParentObj.transform.parent = mParent.transform;
        proBuilderParentObj.transform.eulerAngles = new Vector3(mParent.transform.eulerAngles.x, mParent.transform.eulerAngles.y, mParent.transform.eulerAngles.z);
        proBuilderParentObj.transform.position = new Vector3(mParent.transform.position.x + OFFSET_X, mParent.transform.position.y, mParent.transform.position.z);
    }

    /// <summary>
    /// Places the handle in the appropriate spot on the drawer.
    /// </summary>
    protected override void PlaceHandle()
    {
        //Place one handle in the middle of the drawer's center.
        if (mNumberOfHandles > 0 && mNumberOfHandles == 1)
        {
            mHandles[0].gameObject.transform.position = new Vector3(mCenterObj.transform.position.x + mCenterWidth / 2, mCenterObj.transform.position.y + mCenterHeight / 2, mCenterObj.transform.position.z);
        }

        //Place two handles on opposite ends in the center area of the drawer. For now we are only using the first 2 handles.
        else if (mNumberOfHandles > 0 && mNumberOfHandles == 2)
        {
            Bounds handleBounds = GetTotalMeshFilterBounds(mHandles[0].gameObject.transform);
            mHandles[0].gameObject.transform.position = new Vector3(mCenterObj.transform.position.x + mCenterWidth - handleBounds.size.x / 2, mCenterObj.transform.position.y + mCenterHeight / 2, mCenterObj.transform.position.z);
            mHandles[1].gameObject.transform.position = new Vector3(mCenterObj.transform.position.x + handleBounds.size.x / 2, mCenterObj.transform.position.y + mCenterHeight / 2, mCenterObj.transform.position.z);
        }
    }

    /// <summary>
    /// Wrapper for a handle's game object and mesh.
    /// </summary>
    protected class Handle
    {
        public GameObject gameObject;
        public ProBuilderMesh mesh;

        public Handle(GameObject gameObject, ProBuilderMesh mesh)
        {
            this.gameObject = gameObject;
            this.mesh = mesh;
        }
    }
}
