using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

/// <summary>
/// Use this to test making predefined shapes with ProBuilder.
/// </summary>
public class PolyShapeThing : ScriptableObject
{
    //Side names.
    const string RIGHT = "RIGHT";
    const string LEFT = "LEFT";
    const string TOP = "TOP";
    const string BOTTOM = "BOTTOM";
    const string CENTER = "CENTER";

    //Object the Thing is attached to.
    public GameObject parentObject;

    //Objects to attach the center and side meshes to.
    public GameObject centerObj;
    public GameObject leftRailObj;
    public GameObject topRailObj;
    public GameObject rightRailObj;
    public GameObject bottomRailObj;

    //Meshes for the center and sides.
    public ProBuilderMesh centerMesh;
    public ProBuilderMesh leftMesh;
    public ProBuilderMesh topMesh;
    public ProBuilderMesh bottomMesh;
    public ProBuilderMesh rightMesh;

    //Bounds for the different meshes.
    public Bounds centerBounds = new Bounds();
    public Bounds rightBounds = new Bounds();
    public Bounds topBounds = new Bounds();
    public Bounds leftBounds = new Bounds();
    public Bounds bottomBounds = new Bounds();

    //How far to extrude the Center Mesh.
    public float extrusionFactorCenter = 1;

    //Points that define the Poly Shape to be drawn.
    public List<Vector3> polyShapePoints = new List<Vector3>();

    //Material to apply to the Meshes.
    public Material material;

    /// <summary>
    /// Initializes the properties.
    /// </summary>
    /// <param name="parent">GameObject that will be the parent of the Thing.</param>
    /// <param name="material">Material to apply to all meshes.</param>
    /// <param name="extrusionFactorCenter">Amount to extrude the center mesh.</param>
    public void init(GameObject parent, Material material, float extrusionFactorCenter)
    {
        parentObject = parent;
        this.material = material;
        this.extrusionFactorCenter = extrusionFactorCenter;

        // Init GameObjects
        centerObj = new GameObject(CENTER);
        rightRailObj = new GameObject(RIGHT);
        topRailObj = new GameObject(TOP);
        leftRailObj = new GameObject(LEFT);
        bottomRailObj = new GameObject(BOTTOM);

        //Add the objects to the parent object.
        centerObj.transform.parent = parentObject.transform;
        rightRailObj.transform.parent = parentObject.transform;
        topRailObj.transform.parent = parentObject.transform;
        leftRailObj.transform.parent = parentObject.transform;
        bottomRailObj.transform.parent = parentObject.transform;
    }

    /// <summary>
    /// Makes a shape.
    /// </summary>
    public void makePredefinedShape()
    {
        polyShapePoints.Clear();

        //Make center using the predefined shape.
        addPredefinedPoints();
        addPartsOfShape();
    }

    /// <summary>
    /// Makes a different shape.
    /// </summary>
    public void makePredefinedShape2()
    {
        polyShapePoints.Clear();

        //Make center using the predefined shape.
        addPredefinedPoints2();
        addPartsOfShape();
    }

    /// <summary>
    /// Makes an even more different shape.
    /// </summary>
    public void makePredefinedShape3()
    {
        polyShapePoints.Clear();

        //Make center using the predefined shape.
        addPredefinedPoints3();
        addPartsOfShape();
    }

    public async Task writeToFile(string text, bool overwrite)
    {
        
        StreamWriter file = new StreamWriter("c://workspace//test.csv", overwrite);
        await file.WriteLineAsync(text);
        file.Close();
    }

    /// <summary>
    /// Creates the center mesh and all the side meshes. Also merges all the objects together and deletes the unneeded center and side rail GameObjects.
    /// </summary>
    public async void addPartsOfShape()
    {
        //Create the center mesh.
        centerMesh = new GameObject(CENTER).AddComponent<ProBuilderMesh>();
        centerMesh.transform.parent = centerObj.transform;
        centerMesh.CreateShapeFromPolygon(polyShapePoints.ToArray(), extrusionFactorCenter, false);
        centerMesh.SetMaterial(centerMesh.faces, material);
        centerBounds = GetTotalMeshFilterBounds(centerMesh.transform);
        centerMesh.ToMesh();
        centerMesh.Refresh();

        //Make each of the sides.
        ProBuilderMesh rightMesh = makeSide(ref rightBounds, rightRailObj, RIGHT);
        ProBuilderMesh leftMesh = makeSide(ref leftBounds, leftRailObj, LEFT);
        ProBuilderMesh topMesh = makeSide(ref topBounds, topRailObj, TOP);
        ProBuilderMesh bottomMesh = makeSide(ref bottomBounds, bottomRailObj, BOTTOM);

        //Merge the center and sides into a single mesh.
        List<ProBuilderMesh> meshes = CombineMeshes.Combine(new List<ProBuilderMesh> {centerMesh, rightMesh, leftMesh, topMesh, bottomMesh });

        foreach (ProBuilderMesh mesh in meshes)
        {
            mesh.transform.parent = parentObject.transform;
        }

        //Delete the sides so we just have the single targetMesh and the center.
        Destroy(rightRailObj);
        Destroy(leftRailObj);
        Destroy(topRailObj);
        Destroy(bottomRailObj);
        Destroy(centerObj);
    }

    /// <summary>
    /// Makes a single side with the passed in parameters.
    /// </summary>
    /// <param name="bounds">Bounds of the shape to create. This will be set here.</param>
    /// <param name="gameObject">GameObject to attach the mesh to.</param>
    /// <param name="side">Name of the side e.g. BOTTOM or TOP</param>
    /// <returns>A ProBuilderMesh</returns>
    private ProBuilderMesh makeSide(ref Bounds bounds, GameObject gameObject, string side)
    {
        ProBuilderMesh mesh;
        
        //Make a left or right side mesh.
        if (side == RIGHT || side == LEFT)
        {
            mesh = new GameObject(side).AddComponent<ProBuilderMesh>();
            mesh.transform.parent = gameObject.transform;

            //Here we extrude by the height of the center mesh.
            mesh.CreateShapeFromPolygon(polyShapePoints.ToArray(), centerBounds.size.y, false);
            
            mesh.SetMaterial(mesh.faces, material);
            bounds = GetTotalMeshFilterBounds(mesh.transform);
            mesh.name = side;
        }

        //Make a bottom or top mesh.
        else
        {
            mesh = new GameObject(side).AddComponent<ProBuilderMesh>();
            mesh.transform.parent = gameObject.transform;

            //Here we extrude by the sum of the center width plus the side widths.
            mesh.CreateShapeFromPolygon(polyShapePoints.ToArray(), centerBounds.size.z + leftBounds.size.z + rightBounds.size.z, false);
            
            mesh.SetMaterial(mesh.faces, material);
            bounds = GetTotalMeshFilterBounds(mesh.transform);
            mesh.name = side;
        }

        //Position the mesh properly.
        switch (side)
        {
            case "RIGHT":
                {
                    gameObject.transform.position = new Vector3(parentObject.transform.position.x,
                        parentObject.transform.position.y,
                        parentObject.transform.position.z);

                    gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x - 90,
                        gameObject.transform.eulerAngles.y,
                        gameObject.transform.eulerAngles.z
                    );

                    break;
                }
            case "TOP":
                {
                    gameObject.transform.position = new Vector3(parentObject.transform.position.x,
                        parentObject.transform.position.y + centerBounds.size.y,
                        parentObject.transform.position.z - rightBounds.size.z);

                    break;
                }
            case "LEFT":
                {
                    gameObject.transform.position = new Vector3(parentObject.transform.position.x,
                        parentObject.transform.position.y + centerBounds.size.y,
                        parentObject.transform.position.z + centerBounds.size.z);

                    gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x + 90,
                        gameObject.transform.eulerAngles.y,
                        gameObject.transform.eulerAngles.z);
                    break;
                }
            case "BOTTOM":
                {
                    gameObject.transform.position = new Vector3(parentObject.transform.position.x,
                        parentObject.transform.position.y,
                        parentObject.transform.position.z + centerBounds.size.z + leftBounds.size.y);

                    gameObject.transform.eulerAngles = new Vector3(
                        gameObject.transform.eulerAngles.x + 180,
                        gameObject.transform.eulerAngles.y,
                        gameObject.transform.eulerAngles.z);
                    break;
                }
        }

        return mesh;
    }

    /// <summary>
    /// Calculates the bounds of the passed in transform.
    /// </summary>
    /// <param name="objectTransform">Transform to calculate bounds for.</param>
    /// <returns></returns>
    private static Bounds GetTotalMeshFilterBounds(Transform objectTransform)
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
    /// Adds a point to the polyShapePoint list.
    /// </summary>
    /// <param name="point"></param>
    protected void addPoint(Vector3 point)
    {
        polyShapePoints.Add(point);
    }

    /// <summary>
    /// Points for a shape.
    /// </summary>
    public void addPredefinedPoints()
    {
        addPoint(new Vector3(0.4308f, 0.4996f, 0f));
        addPoint(new Vector3(0.25f, 0.5f, 0f));
        addPoint(new Vector3(0.1622f, 0.4841f, 0f));
        addPoint(new Vector3(0.0775f, 0.431f, 0f));
        addPoint(new Vector3(0.0193f, 0.3463f, 0f));
        addPoint(new Vector3(0f, 0.25f, 0f));
        addPoint(new Vector3(0f, 0f, 0f));
        addPoint(new Vector3(2.5f, 0f, 0f));
        addPoint(new Vector3(2.5f, 0.3569f, 0f));
        addPoint(new Vector3(2.4793f, 0.4566f, 0f));
        addPoint(new Vector3(2.4203f, 0.5399f, 0f));
        addPoint(new Vector3(2.3343f, 0.5922f, 0f));
        addPoint(new Vector3(2.2351f, 0.6064f, 0f));
        addPoint(new Vector3(2.2084f, 0.6034f, 0f));
        addPoint(new Vector3(2.0351f, 0.5769f, 0f));
        addPoint(new Vector3(1.8605f, 0.5553f, 0f));
        addPoint(new Vector3(1.6847f, 0.538f, 0f));
        addPoint(new Vector3(1.5077f, 0.5247f, 0f));
        addPoint(new Vector3(1.3298f, 0.5148f, 0f));
        addPoint(new Vector3(1.1511f, 0.5079f, 0f));
        addPoint(new Vector3(0.9717f, 0.5034f, 0f));
        addPoint(new Vector3(0.7917f, 0.5008f, 0f));
        addPoint(new Vector3(0.6114f, 0.4997f, 0f));
    }

    /// <summary>
    /// Points for another shape.
    /// </summary>
    public void addPredefinedPoints2()
    {
        addPoint(new Vector3(0.5f, 3.8208f, 0f));
        addPoint(new Vector3(0.5f, 4.25f, 0f));
        addPoint(new Vector3(0.4842f, 4.3374f, 0f));
        addPoint(new Vector3(0.4319f, 4.4215f, 0f));
        addPoint(new Vector3(0.3489f, 4.4796f, 0f));
        addPoint(new Vector3(0.25f, 4.5f, 0f));
        addPoint(new Vector3(0f, 4.5f, 0f));
        addPoint(new Vector3(0f, 0f, 0f));
        addPoint(new Vector3(1.5f, 0f, 0f));
        addPoint(new Vector3(1.4949f, 0.0873f, 0f));
        addPoint(new Vector3(1.4813f, 0.1665f, 0f));
        addPoint(new Vector3(1.4555f, 0.2545f, 0f));
        addPoint(new Vector3(1.4157f, 0.3454f, 0f));
        addPoint(new Vector3(1.3627f, 0.4325f, 0f));
        addPoint(new Vector3(1.298f, 0.5121f, 0f));
        addPoint(new Vector3(1.2225f, 0.5825f, 0f));
        addPoint(new Vector3(1.1387f, 0.6414f, 0f));
        addPoint(new Vector3(1.0504f, 0.6872f, 0f));
        addPoint(new Vector3(0.9554f, 0.7213f, 0f));
        addPoint(new Vector3(0.8556f, 0.7425f, 0f));
        addPoint(new Vector3(0.75f, 0.75f, 0f));
        addPoint(new Vector3(0.75f, 1.671f, 0f));
    }

    /// <summary>
    /// Points for a different shape.
    /// </summary>
    public void addPredefinedPoints3()
    {
        addPoint(new Vector3(4.5f, 0, 0f));
        addPoint(new Vector3(4.5f, 0.5f, 0f));
        addPoint(new Vector3(0.5f, 4.5f, 0f));
        addPoint(new Vector3(0, 4.5f, 0f));
        addPoint(new Vector3(0, 0, 0f));
    }
}
