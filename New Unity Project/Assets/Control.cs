using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {
    public Rigidbody Car;
    public GameObject car;
    public Rigidbody Car2;
    public GameObject car2;
    public GUIStyle Title;
    public Texture map;
    public Texture mapDot;
    public Texture enemyMapDot;
    private float dt = 0;
    public float mapX;
    public float mapY;
    public float mapWidth;
    public float mapHeight;
    public bool mapFromRight;
    public bool mapFromBot;

    private GoForward car2_vars, car1_vars;

	// Use this for initialization
	void Start () {
        Physics.gravity = new Vector3(0F,-5F,0F);
        car2_vars = car2.GetComponent<GoForward>();
        car1_vars = car.GetComponent<GoForward>();
	}
	
	// Update is called once per frame
	void Update () {
        dt = 1.0F / Time.deltaTime;
	}

    void OnGUI()
    {
        float mx, my;
        float width = Screen.width;
        float height = Screen.height;

        if (mapFromRight == true)
        { mx = width - (mapX + mapWidth); }
        else
        { mx = mapX; }

        if (mapFromBot == true)
        { my = height - (mapY + mapHeight); }
        else
        { my = mapY; }

        GUI.Label(new Rect(10, 10, 1, 1), "KPH: " + Mathf.Round(Car.velocity.magnitude*12F), Title);
        GUI.Label(new Rect(10, 30, 1, 1), "Gear: " + car1_vars.gear, Title);
        GUI.Label(new Rect(10, 50, 1, 1), "Lap Position: " + car1_vars.raceLoc + "/" + car1_vars.navArraySize, Title);
        GUI.Label(new Rect(10, 70, 1, 1), "Lap: " + car1_vars.lap, Title);

        GUI.Label(new Rect(200, 10, 1, 1), "KPH: " + Mathf.Round(Car2.velocity.magnitude * 12F), Title);
        GUI.Label(new Rect(200, 30, 1, 1), "Gear: " + car2_vars.gear, Title);
        GUI.Label(new Rect(200, 50, 1, 1), "Lap Position: " + car2_vars.raceLoc + "/" + car2_vars.navArraySize, Title);
        GUI.Label(new Rect(200, 70, 1, 1), "Lap: " + car2_vars.lap, Title);
        GUI.DrawTexture(new Rect(mx, my, mapWidth, mapHeight), map);

        float xx = (Car2.position.x/500) * mapWidth;
        float zz = -(Car2.position.z/500) * mapHeight;

        GUI.DrawTexture(new Rect(xx+mx+ mapWidth*0.5F -8, zz+my+ mapHeight*0.5F -8, 16,16),enemyMapDot);

        xx = (Car.position.x / 500) * mapWidth;
        zz = -(Car.position.z / 500) * mapHeight;

        GUI.DrawTexture(new Rect(xx + mx + mapWidth * 0.5F - 8, zz + my + mapHeight * 0.5F - 8, 16, 16), mapDot);


        GUI.Label(new Rect(300, 10, 1, 1), "FPS: " + dt.ToString(), Title);
    }
}