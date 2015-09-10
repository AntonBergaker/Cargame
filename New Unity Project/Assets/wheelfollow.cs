using UnityEngine;
using System.Collections;

// ADD THIS SCRIPT TO EACH OF THE WHEEL MESHES / WHEEL MESH CONTAINER OBJECTS
public class wheelfollow: MonoBehaviour
{

    public WheelCollider wheelC;

    private Vector3 wheelCCenter;
    private RaycastHit hit;

    // Initialization
    void Start()
    {

    }

    // Display
    void Update()
    {
        wheelCCenter = wheelC.transform.TransformPoint(wheelC.center);

        if (Physics.Raycast(wheelCCenter, -wheelC.transform.up, out hit, wheelC.suspensionDistance + wheelC.radius))
        {
            transform.position = hit.point + (wheelC.transform.up * wheelC.radius);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, wheelCCenter - (wheelC.transform.up * wheelC.suspensionDistance), 6F*Time.deltaTime);
        }
    }

    // Physics
    void FixedUpdate()
    {

    }
}