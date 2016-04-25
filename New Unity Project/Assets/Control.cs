using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Cars
{ 
    public GameObject obj;
    public Rigidbody rigid;
    public GoForward vars;
    public int score;
    public int sPosition;
    public float xPos = 25F;
}


public class Control : MonoBehaviour
{
    public List<GameObject> carobjects = new List<GameObject>();
    [HideInInspector]public List<Cars> cars = new List<Cars>();
    [HideInInspector]public int playercar = 0;
    public GUIStyle Title;
    public GUIStyle mediumTitle;
    public GUIStyle bigTitle;
    public GUIStyle hugeTitle;
    public GUIStyle giganticTitle;
    public Texture map;
    public Texture mapDot;
    public Texture enemyMapDot;
    public Texture blackSquare;
    public float worldSize;
    public float mapX;
    public float mapY;
    public float mapWidth;
    public float mapHeight;
    public bool mapFromRight;
    public bool mapFromBot;
    public bool debug;
    public GameObject cam;
    public GameObject[] navigationGroups;
    public bool paused;
    float timer = 0;

    private AudioListener listener;
    private Victory_screen vicscr;

    // Use this for initialization
    void Start()
    {
        int selectedcar;
        if (PlayerPrefs.HasKey("selectedcar"))
        {
            selectedcar = PlayerPrefs.GetInt("selectedcar");
            PlayerPrefs.DeleteKey("selectedcar");
        }
        else
        { selectedcar = -1; }

        for (int i = 0; i < carobjects.Count; i++)
        {
            Cars temp = new Cars();
            temp.obj = carobjects[i];
            temp.rigid = carobjects[i].GetComponent<Rigidbody>();
            temp.vars = carobjects[i].GetComponent<GoForward>();

            cars.Add(temp);
            if (selectedcar != -1)
            {
                if (i == selectedcar)
                {
                    cam.GetComponent<CarCam>().carObject = temp.obj;
                    cam.GetComponent<CarCam>().car = carobjects[i].GetComponent<Transform>();
                    cam.GetComponent<CarCam>().carBody = temp.rigid;
                    carobjects[i].GetComponent<AudioListener>().enabled = true;
                    temp.vars.navGroup = navigationGroups[0];
                    temp.vars.isAI = false;
                }
                else
                {
                    temp.vars.isAI = true;
                    carobjects[i].GetComponent<AudioListener>().enabled = false;
                }
            }
            temp.vars.CStart(); //initalize the objects from here
        }
        for (int i = 0; i < navigationGroups.Length;i++ ) //hide the nav arrays
        {
            int rep = navigationGroups[i].transform.childCount;
            for (int ii = 0; ii < rep; ii++)
            {
                navigationGroups[i].transform.GetChild(ii).GetComponent<MeshRenderer>().enabled = false;
            }
        }
        Physics.gravity = new Vector3(0F, -5F, 0F);
        vicscr = GetComponent<Victory_screen>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (cars[playercar].vars.lap == 3)
        { vicscr.StartVictory(); }

        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].xPos = Mathf.Lerp(cars[i].xPos, 25F + 40F * cars[i].sPosition, Time.deltaTime * 5);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            paused = !paused;
            if (paused)
            {
                Time.timeScale = 0;
                listener = cars[playercar].obj.GetComponent<AudioListener>();
                listener.enabled = false;
            }
            else
            {
                Time.timeScale = 1;
                listener.enabled = true;
            }
        }
        if (Input.GetButtonDown("Submit"))
        {
            if (paused)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void OnGUI()
    {
        float width = Screen.width;
        float height = Screen.height;
        if (cars[playercar].vars.lap < 3 && paused == false)
        {
            float mx, my;

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

            Title.fontSize = 25;
            float xx, zz;
            for (int i = 0; i < cars.Count; i++)
            {
                if (debug)
                {
                    Title.fontSize = 12;
                    GUI.Label(new Rect(10 + i * 80, 10, 1, 1), "Name: " + cars[i].vars.carName, Title);
                    GUI.Label(new Rect(10 + i * 80, 30, 1, 1), "KPH: " + Mathf.Round(cars[i].rigid.velocity.magnitude * 12F), Title);
                    GUI.Label(new Rect(10 + i * 80, 50, 1, 1), "Gear: " + cars[i].vars.gear, Title);
                    GUI.Label(new Rect(10 + i * 80, 70, 1, 1), "Chkpnt: " + cars[i].vars.raceLoc + "/" + (cars[i].vars.navArraySize - 1).ToString(), Title);
                    GUI.Label(new Rect(10 + i * 80, 90, 1, 1), "Lap: " + cars[i].vars.lap, Title);
                    GUI.Label(new Rect(10 + i * 80, 110, 1, 1), "Pos: " + cars[i].sPosition, Title);
                }
                else
                {
                    if (i == playercar)
                    { Title.normal.textColor = new Color(0.529F, 0.808F, 0.98F); }
                    else
                    { Title.normal.textColor = new Color(1F, 1F, 1F); }
                    GUI.Label(new Rect(25, cars[i].xPos, 1, 1), (cars[i].sPosition + 1).ToString() + ".", Title);
                    GUI.Label(new Rect(50, cars[i].xPos, 1, 1), cars[i].vars.carName, Title);
                }

                xx = (cars[i].rigid.position.x / worldSize) * mapWidth;
                zz = -(cars[i].rigid.position.z / worldSize) * mapHeight;

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

            GUI.Label(new Rect(10, height-10, 1, 1), getposition(cars[playercar].sPosition+1), bigTitle);

            GUI.Label(new Rect(100, height-20, 1, 1), Mathf.Round(cars[playercar].rigid.velocity.magnitude * 12F) + " KPH", mediumTitle);

            string tottime = Mathf.Round(cars[playercar].vars.lapTime).ToString();
            string laptime = Mathf.Round(cars[playercar].vars.totalTime).ToString();

            string combined = tottime + "s";
            if (tottime != laptime)
            { combined += " (" + laptime + "s)"; }


            Title.fontSize = 15;

            GUI.Label(new Rect(10, height-90,1, 1), combined, Title);
            GUI.Label(new Rect(10, height - 110, 1, 1), (cars[playercar].vars.lap+1).ToString()+"/3", Title);

        }
        else if (paused)
        {
            hugeTitle.fontSize = 150;
            GUI.Label(new Rect(width/2, height/2, 1, 1), "PAUSED", hugeTitle);
            hugeTitle.fontSize = 45;
            GUI.Label(new Rect(width / 2, height / 2 + 100, 1, 1), "Press ENTER to return to menu", hugeTitle);
        }

        if (timer < 1)
        {
            GUI.color = new Color(1F, 1F, 1F, 1F - timer);
            GUI.DrawTexture(new Rect(0, 0, width, height), blackSquare);
            GUI.color = new Color(1F, 1F, 1F, 1F);
        }

        if (timer < 9 && timer > 3)
        {
            if (timer < 8)
            {
                GUI.Label(new Rect(width / 2, height / 2, 1, 1), ((int)(Mathf.Ceil(8 - timer))).ToString(), giganticTitle);
            }
            else
            { GUI.Label(new Rect(width / 2, height / 2, 1, 1), "GO!", giganticTitle); }
        }
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