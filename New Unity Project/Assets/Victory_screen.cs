using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Car
{
    public int position;
    public int totalTime;
    public GoForward carVars;
    public bool finished = false;
    public float xpos = 300F;
}

public class Victory_screen : MonoBehaviour {

    bool triggered = false;
    bool won = false;
    float timer = 0;
    public Control control;
    Car[] cars;
    public GUIStyle bigText;
    public GUIStyle normalText;
    public GUIStyle smallText;
    
    public Texture blackBox;

    int playercar = 0;

    public void StartVictory()
    {
        if (triggered == false)
        {
            control = GetComponent<Control>();
            cars = new Car[control.cars.Count];
            for (int i = 0; i < cars.Length; i++)
            {
                cars[i] = new Car();
                cars[i].carVars = control.cars[i].vars;
            }
            playercar = control.playercar;

            triggered = true;
            if (control.cars[playercar].sPosition == 0)
            { won = true; }
        }
    }
	
	// Update is called once per frame
    void Update()
    {
        if (triggered == true)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < cars.Length; i++)
            {
                if (cars[i].finished == false)
                {
                    cars[i].totalTime = (int)cars[i].carVars.totalTime;
                    cars[i].position = control.cars[i].sPosition;
                    if (cars[i].carVars.lap >= 3)
                    {
                        cars[i].finished = true;
                    }
                }
                cars[i].xpos = Mathf.Lerp(cars[i].xpos, 340F + 100F * cars[i].position, Time.deltaTime * 5);
            }

            if (Input.GetButton("Submit"))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
    void OnGUI () {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1920.0f, Screen.height / 1080.0f, 1));
        if (triggered)
        {
            GUI.color = new Color(0F,0F,0F,Mathf.Clamp(timer*2,0F,0.4F));
            GUI.DrawTexture(new Rect(0,0,1920,1080), blackBox);
            GUI.color = new Color(1F, 1F, 1F, 1F);

            string wincondition;
            if (won)
            { wincondition = "YOU WIN!!"; }
            else
            { wincondition = "YOU LOST"; }

            int preSize = bigText.fontSize;
            bigText.fontSize = (int)Mathf.Clamp((float)preSize*timer*5,0F,(float)preSize);
            GUI.Label(new Rect(1920F / 2F, 150F, 1, 1), wincondition, bigText);
            bigText.fontSize = preSize;
            for (int i=0;i<cars.Length;i++)
            {
                if (i == playercar)
                { normalText.normal.textColor = new Color(0.529F, 0.808F, 0.98F, Mathf.Clamp(timer * 2 - 0.7F, 0, 1)); }
                else
                { normalText.normal.textColor = new Color(1F, 1F, 1F, Mathf.Clamp(timer * 2 - 0.7F, 0, 1)); }
                GUI.Label(new Rect(350F, cars[i].xpos, 1, 1), (cars[i].position + 1).ToString() + ".", normalText);
                GUI.Label(new Rect(450F, cars[i].xpos, 1, 1), cars[i].carVars.carName, normalText);
                GUI.Label(new Rect(800F, cars[i].xpos, 1, 1), FormatTime(cars[i].totalTime), normalText);
                for (int ii = 0; ii < 3;ii++ )
                {
                    if (ii < cars[i].carVars.lapTimes.Count)
                    { GUI.Label(new Rect(1080F + 180F*ii, cars[i].xpos, 1, 1), (int)cars[i].carVars.lapTimes[ii]+"s", normalText); }
                    else
                    { GUI.Label(new Rect(1080F + 180F*ii, cars[i].xpos, 1, 1), "--", normalText); }
                }                
            }

            if (timer > 5)
            {
                GUI.Label(new Rect(1920/2, 540F + cars.Length * 100, 1, 1), "Press ENTER to return to menu", smallText); 
            }

        }
    }

    string FormatTime (int num)
    {
        string str = "";
        if (num >= 60)
        { str += (num / 60).ToString() + "m "; }
        if (num > 0)
        { str += num % 60 + "s"; }

        return str;
    }
}
