using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {
    public Rigidbody Car;
    public Object car;
    public GUIStyle Title;
    private float dt = 0;

	// Use this for initialization
	void Start () {
        Physics.gravity = new Vector3(0F,-5F,0F);
	}
	
	// Update is called once per frame
	void Update () {
        dt = 1.0F / Time.deltaTime;
	}

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 1, 1), "KPH: " + Mathf.Round(Car.velocity.magnitude*15F), Title);
        GUI.Label(new Rect(10, 30, 1, 1), "Gear: " + GoForward.gear, Title);
        GUI.Label(new Rect(10, 50, 1, 1), "FPS: " + dt.ToString(), Title);
    }
}


