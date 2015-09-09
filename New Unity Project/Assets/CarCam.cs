using UnityEngine;
using System.Collections;

public class CarCam : MonoBehaviour {

    public Transform car;
    public float distance = 6.4F;
    public float height = 1.4F;
    public float rotationDampening = 3.0F;
    public float heightDampening = 2.0F;
    public float zoomRacio = 0.5F;
    public float DefaultFOV = 80;
    private Vector3 rotationVector;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame


    void LateUpdate () {
        float wantedAngel = rotationVector.y;
        float wantedHeight = car.position.y + height;
        float myAngel = transform.eulerAngles.y;
        float myHeight = transform.position.y;
        myAngel = Mathf.LerpAngle(myAngel,wantedAngel,rotationDampening * Time.deltaTime);
        myHeight = Mathf.Lerp(myHeight,wantedHeight,heightDampening * Time.deltaTime);
        var currentRotation = Quaternion.Euler(0,myAngel,0);
        transform.position = car.position;
        transform.position -= currentRotation*Vector3.forward*distance;
        Vector3 temp = transform.position; // copy to an auxiliary variable...
        temp.y = myHeight; // modify the component you want in the variable...
        transform.position = temp; // and save the modified value 
        transform.LookAt(car);
    }
    void FixedUpdate (){
        Vector3 localVilocity = car.InverseTransformDirection(car.GetComponent<Rigidbody>().velocity);
        if (localVilocity.z < -0.5F){
        rotationVector.y = car.eulerAngles.y + 180;
        }
        else {
        rotationVector.y = car.eulerAngles.y;
        }
        float acc = car.GetComponent<Rigidbody>().velocity.magnitude;
        GetComponent<Camera>().fieldOfView = DefaultFOV + acc*zoomRacio;

 

	}
}
