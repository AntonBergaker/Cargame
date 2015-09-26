using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cars
{ 
    public GameObject obj;
    public Rigidbody rigid;
    public GoForward vars;
    public int score;
    public int sPosition;
}


public class Control : MonoBehaviour
{
    public List<GameObject> carobjects = new List<GameObject>();
    public static List<Cars> cars = new List<Cars>();

    public GUIStyle Title;
    public GUIStyle bigTitle;
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

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < carobjects.Count; i++)
        {
            Cars temp = new Cars();
            temp.obj = carobjects[i];
            temp.rigid = carobjects[i].GetComponent<Rigidbody>();
            temp.vars = carobjects[i].GetComponent<GoForward>();

            cars.Add(temp);
        }
        Physics.gravity = new Vector3(0F, -5F, 0F);
    }

    // Update is called once per frame
    void Update()
    {
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

        GUI.DrawTexture(new Rect(mx, my, mapWidth, mapHeight), map);

        //Set every cars points
        for (int i = 0; i < cars.Count; i++) { 
            cars[i].score = cars[i].vars.lap * 300000;
            cars[i].score += cars[i].vars.raceLoc * 1000;
            cars[i].score -= (int)(cars[i].vars.distanceToNext*10F);
        }

        for (int i = 0; i < cars.Count; i++) {
            int compare = cars[i].score;
            int pos = 0;
            for (int ii = 0; ii < cars.Count; ii++) { 
                if (compare < cars[ii].score)
                { pos++; }
            }
            cars[i].sPosition = pos;
        }


        float xx, zz;
        int playercar = 0;
        for (int i = 0; i < cars.Count; i++)
        {
            GUI.Label(new Rect(10 + i * 80, 10, 1, 1), "Name: " + cars[i].vars.carName, Title);
            GUI.Label(new Rect(10 + i * 80, 30, 1, 1), "KPH: " + Mathf.Round(cars[i].rigid.velocity.magnitude * 12F), Title);
            GUI.Label(new Rect(10 + i * 80, 50, 1, 1), "Gear: " + cars[i].vars.gear, Title);
            GUI.Label(new Rect(10 + i * 80, 70, 1, 1), "Chkpnt: " + cars[i].vars.raceLoc + "/" + (cars[i].vars.navArraySize-1).ToString(), Title);
            GUI.Label(new Rect(10 + i * 80, 90, 1, 1), "Lap: " + cars[i].vars.lap, Title);
            GUI.Label(new Rect(10 + i * 80, 110, 1, 1), "Pos: " + cars[i].sPosition, Title);


            xx = (cars[i].rigid.position.x / 500) * mapWidth;
            zz = -(cars[i].rigid.position.z / 500) * mapHeight;

            Texture dot;
            if (cars[i].vars.isAI)
            { dot = enemyMapDot; }
            else
            {
                dot = mapDot;
                playercar = i;
            }

            GUI.DrawTexture(new Rect(xx + mx + mapWidth * 0.5F - 8, zz + my + mapHeight * 0.5F - 8, 16, 16), dot);
        }

        GUI.Label(new Rect(10, height - 10, 1, 1), getposition(cars[playercar].sPosition+1), bigTitle);
        GUI.Label(new Rect(10, height-10, 1, 1), "FPS: " + dt.ToString(), Title);
    }

    string getposition(int pos)
    {
        string spos = pos.ToString();
        string suffix;
        if (spos[0] == '1' && spos.Length == 2)
        { suffix = "th"; }
        else
        {
            switch (spos[spos.Length - 1])
            {
                case '1':
                    suffix = "st";
                    break;
                case '2':
                    suffix = "nd";
                    break;
                case '3':
                    suffix = "rd";
                    break;
                default:
                    suffix = "th";
                    break;

            }
        }
        return spos + suffix;
    }
}