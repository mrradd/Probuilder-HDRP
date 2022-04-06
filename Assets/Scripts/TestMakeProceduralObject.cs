using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;


public class TestMakeProceduralObject : MonoBehaviour
{
    public GameObject thing1;
    public GameObject thing2;
    public GameObject thing3;
    public Material material;
    public float centerExtrusionFactorThing1;
    public float centerExtrusionFactorThing2;
    public float centerExtrusionFactorThing3;
    public bool draw1;
    public bool draw2;
    public bool draw3;
    public int numberOfCopies;

    protected GameObject childObject;

    // Start is called before the first frame update
    void Start()
    {
        //Draw the first shape.
        if (draw1)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Draw multiple times.
            for (int i = 0; i < numberOfCopies; i++)
            {
                PolyShapeThing shape = makePolyShapeThing(i);
                shape.parentObject.transform.position = new Vector3(thing1.transform.position.x, thing1.transform.position.y, thing1.transform.position.z + i * 10);
            }

            sw.Stop();
            Debug.Log("draw1: " + sw.ElapsedMilliseconds + " ms");
        }

        //Draw the second shape.
        if (draw2)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Draw multiple times.
            for (int i = 0; i < numberOfCopies; i++)
            {
                PolyShapeThing shape = makePolyShapeThing2(i);
                shape.parentObject.transform.position = new Vector3(thing2.transform.position.x, thing2.transform.position.y + 3, thing2.transform.position.z + i * centerExtrusionFactorThing2 + i * 10);
            }
            
            sw.Stop();
            Debug.Log("draw2: " + sw.ElapsedMilliseconds + " ms");
        }

        //Draw the third shape.
        if (draw3)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Draw multiple times.
            for (int i = 0; i < numberOfCopies; i++)
            {
                PolyShapeThing shape = makePolyShapeThing3(i);
                shape.parentObject.transform.position = new Vector3(thing3.transform.position.x, thing3.transform.position.y + 6, thing3.transform.position.z + i * centerExtrusionFactorThing3 + i * 10);
            }
            
            sw.Stop();
            Debug.Log("draw3: " + sw.ElapsedMilliseconds + " ms");
        }

    }
    
    /// <summary>
    /// Makes the first shape
    /// </summary>
    /// <param name="i">Number appended to the name of the object.</param>
    /// <returns></returns>
    PolyShapeThing makePolyShapeThing(int i)
    {
        GameObject parent = new GameObject("Thing1-" + i);

        ScriptableObject polySO = ScriptableObject.CreateInstance("PolyShapeThing");
        PolyShapeThing polyShape = (PolyShapeThing)polySO;
        polyShape.init(parent, material, centerExtrusionFactorThing1);
        polyShape.makePredefinedShape();

        return polyShape;
    }

    /// <summary>
    /// Makes the second shape
    /// </summary>
    /// <param name="i">Number appended to the name of the object.</param>
    /// <returns></returns>
    PolyShapeThing makePolyShapeThing2(int i)
    {
        GameObject parent = new GameObject("Thing2-" + i);

        ScriptableObject polySO = ScriptableObject.CreateInstance("PolyShapeThing");
        PolyShapeThing polyShape = (PolyShapeThing)polySO;
        polyShape.init(parent, material, centerExtrusionFactorThing2);
        polyShape.makePredefinedShape2();

        return polyShape;
    }

    /// <summary>
    /// Makes the third shape
    /// </summary>
    /// <param name="i">Number appended to the name of the object.</param>
    /// <returns></returns>
    PolyShapeThing makePolyShapeThing3(int i)
    {
        GameObject parent = new GameObject("Thing3-" + i);

        ScriptableObject polySO = ScriptableObject.CreateInstance("PolyShapeThing");
        PolyShapeThing polyShape = (PolyShapeThing)polySO;
        polyShape.init(parent, material, centerExtrusionFactorThing3);
        polyShape.makePredefinedShape3();

        return polyShape;
    }

}

