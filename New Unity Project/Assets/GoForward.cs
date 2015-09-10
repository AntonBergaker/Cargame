using UnityEngine;
using System.Collections;

public class GoForward : MonoBehaviour
{
    public WheelCollider wheelBR;
    public WheelCollider wheelBL;
    public WheelCollider wheelFR;
    public WheelCollider wheelFL;
    public Transform wheelFLMesh;
    public Transform wheelFRMesh;
    public Rigidbody body;
    public float maxTorque;

    // Update is called once per frame
    void FixedUpdate()
    {
        wheelBR.motorTorque = maxTorque * Input.GetAxis("Vertical");
        wheelBL.motorTorque = maxTorque * Input.GetAxis("Vertical");
        wheelFL.steerAngle = 40 * Input.GetAxis("Horizontal");
        wheelFR.steerAngle = 40 * Input.GetAxis("Horizontal");
        body.AddForce(Input.GetAxis("Vertical")*100*body.transform.forward);
    }
    void Update()
    {
        //rotate the wheels
        Vector3 temp = wheelFLMesh.localEulerAngles;
        temp.y = wheelFL.steerAngle - wheelFLMesh.localEulerAngles.z +90;

        wheelFLMesh.localEulerAngles = temp;

        temp = wheelFRMesh.localEulerAngles;
        temp.y = wheelFR.steerAngle - wheelFRMesh.localEulerAngles.z +90;

        wheelFRMesh.localEulerAngles = temp;
    }
}
