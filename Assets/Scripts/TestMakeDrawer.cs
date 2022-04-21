using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMakeDrawer : MonoBehaviour
{
    [Header("Dimensions get converted from Unity units (m) to feet.")]
    public float width;
    public float height;
    public float railWidth = 2.5f;
    public float railDepth = .04f;
    public float cabinetDepth = .02f;
    public float centerDepth = .02f;
    public int numberOfHandles = 1;
    public int numberOfDrawers = 1;
    public GameObject drawerObject;
    public Material material;
    public bool useRandomDimensions;

    private void Awake()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        //Make all the doors.
        for (int i = 0; i < numberOfDrawers; i++)
        {
            //Determine if we want to use randomly assigned dimensions.
            if (useRandomDimensions)
            {
                width = Random.Range(10f, 101f);
                height = Random.Range(10f, 101f);
            }

            //Make the door.
            GameObject gameObject = new GameObject("ProBuilderDrawer-" + i);
            ProBuilderDrawer drawer = gameObject.AddComponent<ProBuilderDrawer>();
            drawer.Init(width, height, cabinetDepth, railWidth, railDepth, centerDepth, numberOfHandles, material, gameObject);
            gameObject.transform.position = drawerObject.transform.position;
            drawer.MakeShape();

        }

        sw.Stop();
        Debug.Log($"Drew {numberOfDrawers} drawer(s): {sw.ElapsedMilliseconds}ms");
    }
}
