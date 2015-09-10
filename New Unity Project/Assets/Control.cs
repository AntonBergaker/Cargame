using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {
    public Rigidbody Car;
    public GUIStyle Title;

	// Use this for initialization
	void Start () {
        Physics.gravity = new Vector3(0F,-5F,0F);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 1, 1), "KPH: " + Mathf.Round(Car.velocity.magnitude*10F), Title);
    
    }
}


