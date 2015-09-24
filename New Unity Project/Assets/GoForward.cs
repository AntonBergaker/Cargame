using UnityEngine;
using System.Collections;

public class GoForward : MonoBehaviour
{
    public WheelCollider wheelBR;
    public WheelCollider wheelBL;
    public WheelCollider wheelFR;
    public WheelCollider wheelFL;
    public Transform wheelBRMesh;
    public Transform wheelBLMesh;
    public Transform wheelFLMesh;
    public Transform wheelFRMesh;
    public Rigidbody body;
    public static int gear = 1;
    public float maxTorque;

    // Update is called once per frame
    void FixedUpdate()
    {
        float speed = body.velocity.magnitude * 15F;
        float stiffness = gear;
        float torque = 1;
        //Change gears!
        if (speed < 15F)
        {
            torque = 5;
            stiffness = 10;
            gear = 1;
        }

        else if (speed < 30F)
        {
            torque = 2;
            stiffness = 5;
            gear = 1;
        }
        else if (speed < 60)
        {
            torque = 1;
            stiffness = 4;
            gear = 2;
        }
        else if (speed < 90)
        {
            torque = 1;
            stiffness = 3;
            gear = 3;
        }
        else if (speed < 120)
        {
            torque = 1;
            stiffness = 2;
            gear = 4;
        }
        else
        {
            torque = 1;
            stiffness = 1;
            gear = 5;
        }

        torque = torque * maxTorque;

        WheelFrictionCurve temp = wheelBL.forwardFriction;
        temp.stiffness = stiffness;

        wheelBL.forwardFriction = temp;
        wheelBR.forwardFriction = temp;
        wheelFL.forwardFriction = temp;
        wheelFR.forwardFriction = temp;


        wheelBR.motorTorque = torque * Input.GetAxis("Vertical");
        wheelBL.motorTorque = torque * Input.GetAxis("Vertical");
        float stearMod = Mathf.Clamp(40F-(body.velocity.magnitude*1F),20F,40F);
        wheelFL.steerAngle = stearMod * Input.GetAxis("Horizontal");
        wheelFR.steerAngle = stearMod * Input.GetAxis("Horizontal");

        body.AddForce(Input.GetAxis("Vertical") * 100 * body.transform.forward);
        body.AddForce(-body.transform.up * (body.velocity.magnitude * 100F + 750F));

        if (wheelBR.isGrounded)
        { body.AddForce(-body.transform.up * 350F); }
        if (wheelBL.isGrounded)
        { body.AddForce(-body.transform.up * 350F); }
        if (wheelFL.isGrounded)
        { body.AddForce(-body.transform.up * 250F); }
        if (wheelFR.isGrounded)
        { body.AddForce(-body.transform.up * 250F); }

    }
    void Update()
    {
        //rotate the wheels
        wheelFLMesh.Rotate(wheelFL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        wheelFRMesh.Rotate(wheelFR.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        wheelBLMesh.Rotate(wheelBL.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        wheelBRMesh.Rotate(wheelBR.rpm / 60 * 360 * Time.deltaTime, 0, 0);


        Vector3 temp = wheelFLMesh.localEulerAngles;
        temp.y = wheelFL.steerAngle - wheelFLMesh.localEulerAngles.z;

        wheelFLMesh.localEulerAngles = temp;

        temp = wheelFRMesh.localEulerAngles;
        temp.y = wheelFR.steerAngle - wheelFRMesh.localEulerAngles.z;

        wheelFRMesh.localEulerAngles = temp;

        //extra forces
    }
}
