using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates Procedural Door objects.
/// </summary>
public class TestMakeDoor : MonoBehaviour
{
    [Header("Dimensions get converted from Unity units (m) to feet.")]
    public float width;
    public float height;
    public float railWidth = 2.5f;
    public float railDepth = .04f;
    public float centerDepth = .02f;
    public int numberOfObjects;
    public GameObject doorObject;
    public Material material;
    public bool useRandomDimensions;

    private void Awake()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        //Make all the doors.
        for (int i = 0; i < numberOfObjects; i++)
        {
            //Determine if we want to use randomly assigned dimensions.
            if (useRandomDimensions)
            {
                width = Random.Range(10f, 101f);
                height = Random.Range(10f, 101f);
            }

            //Make the door.
            GameObject gameObject = new GameObject("ProceduralDoor-" + i);
            ProbuilderShakerDoor door = gameObject.AddComponent<ProbuilderShakerDoor>();
            door.init(width, height, railWidth, railDepth, centerDepth, material, gameObject);
            door.transform.position = doorObject.transform.position;
            door.makeDoor();
            
        }

        sw.Stop();
        Debug.Log($"Drew {numberOfObjects} door(s): {sw.ElapsedMilliseconds}ms");
    }
}
