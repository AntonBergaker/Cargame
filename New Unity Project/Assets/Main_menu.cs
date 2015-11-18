using UnityEngine;
using System.Collections;

public class Main_menu : MonoBehaviour {

    public GameObject[] cars;
    public Maptype[] scenes;
    public GUIStyle Title;
    private Vector2 mousepos;
    
    [System.Serializable]
    public class Maptype
    {
        public string name;
        public string fileName;
        [HideInInspector]public float xoffset = 0;
    }

    void OnGUI()
    {
        mousepos = Event.current.mousePosition;
        for (int i = 0; i < scenes.Length;i++ )
        {
            scenes[i].xoffset = Mathf.Clamp(scenes[i].xoffset - 1F, 0F, 60F);
            float col = scenes[i].xoffset;
            Title.normal.textColor = new Color(col/59,col/59,col/59);
            GUI.Label(new Rect(200+scenes[i].xoffset, 200 + 80 * i, 1, 1), scenes[i].name, Title);
            Title.normal.textColor = Color.black;
        }

        if (mousepos.x > 200 && mousepos.x < 700)
        {
            int mouseval = (int)Mathf.Floor((mousepos.y - 200F) / 80F);
            if (mouseval >= 0 && mouseval < scenes.Length)
            {
                if (Input.GetMouseButtonDown(0))
                { 
                Application.LoadLevel(scenes[mouseval].fileName);
                }
                scenes[mouseval].xoffset = Mathf.Clamp(scenes[mouseval].xoffset+2F,0F,60F);
            }
        }
     }

	
	// Update is called once per frame
	void Update () {
	
	}
}
