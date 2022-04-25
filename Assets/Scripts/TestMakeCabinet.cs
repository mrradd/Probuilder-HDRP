using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProbuilderUtility;

/// <summary>
/// Creates Procedural Door objects.
/// </summary>
public class TestMakeCabinet : MonoBehaviour
{
    [Header("Dimensions get converted from Unity units (m) to feet.")]
    public float width;
    public float height;
    public float insideBevelDepth;
    public float railWidth = 2.5f;
    public float railDepth = .04f;
    public float cabinetDepth = .02f;
    public float centerDepth = .02f;
    public bool useHandle = true;
    public int numberOfCabinets = 1;
    public GameObject cabinetObject;
    public Material material;
    public bool useRandomDimensions;

    private void Awake()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        //Make all the doors.
        for (int i = 0; i < numberOfCabinets; i++)
        {
            //Determine if we want to use randomly assigned dimensions.
            if (useRandomDimensions)
            {
                width = Random.Range(10f, 101f);
                height = Random.Range(10f, 101f);
            }

            //Make the door.
            GameObject gameObject = new GameObject("ProBuilderCabinet-" + i);
            ProbuilderCabinetWithShakerDoor cabinet = gameObject.AddComponent<ProbuilderCabinetWithShakerDoor>();
            cabinet.Init(width, height, insideBevelDepth, cabinetDepth, railWidth, railDepth, centerDepth, useHandle, HandlePlacement.Bottom, DoorOpenDirection.Left, material, gameObject);
            gameObject.transform.position = cabinetObject.transform.position;
            cabinet.MakeShape();
        }

        sw.Stop();
        Debug.Log($"Drew {numberOfCabinets} door(s): {sw.ElapsedMilliseconds}ms");
    }
}
