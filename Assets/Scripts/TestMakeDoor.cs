using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates Procedural Door objects.
/// </summary>
public class TestMakeDoor : MonoBehaviour
{
    public float width;
    public float height;
    public float sideWidth = 2.5f;
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
            ProceduralBasicDoor door = gameObject.AddComponent<ProceduralBasicDoor>();
            door.init(width, height, material, sideWidth);
            door.makeDoor();
            door.transform.position = new Vector3(0, height / 4 + 5, i * width + i * 10);
        }

        sw.Stop();
        Debug.Log($"Drew {numberOfObjects} door(s): {sw.ElapsedMilliseconds}ms");
    }
}
