using UnityEngine;
using System.Collections;

public class GoForward : MonoBehaviour
{
    public string carName;
    public bool isAI;
    public float checkpointSensetivity;

    public GameObject finishLine;
    public GameObject navGroup;
    private Vector3[] navArray;

    public WheelCollider wheelBR;
    public WheelCollider wheelBL;
    public WheelCollider wheelFR;
    public WheelCollider wheelFL;
    public Transform wheelBRMesh;
    public Transform wheelBLMesh;
    public Transform wheelFLMesh;
    public Transform wheelFRMesh;
    public Rigidbody body;

    public Light backLightL;
    public Light backLightR;

    [HideInInspector] public int gear = 1;
    public float maxTorque;

    public AudioSource engineSound;
    public Texture speedTexture;

    [HideInInspector] public float distanceToNext;
    [HideInInspector] public int raceLoc = 0;
    [HideInInspector] public int navArraySize = 0;
    [HideInInspector] public int lap = 0;

    [HideInInspector] public float lapTime;
    [HideInInspector] public float totalTime;

    private int respawntimer = 0;

    private bool aiRecentCollide;
    private int aiRecentCollideTimer;

    private float brakeBuildup = 0;

    void Start() {
        navArray = new Vector3[navGroup.transform.childCount];
        int rep = navGroup.transform.childCount;
        for (int i = 0; i < rep; i++)
        {
            navArray[i] = navGroup.transform.GetChild(i).position;
            navArray[i] += new Vector3(Random.Range(-0.5F, 0.5F), 0, Random.Range(-0.5F, 0.5F));

            navGroup.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
        finishLine.GetComponent<MeshRenderer>().enabled = false;
        navArraySize = navArray.Length;
        body.centerOfMass = new Vector3(0F,-0.075F,0F);
        lapTime = 0F;
        totalTime = 0F;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //calculate your position
        //check up too 12 checkpoints forward
        Vector3 pointcheck = new Vector3();

        for (int i = 1; i < 15; i++)
        {
            if (i + raceLoc >= navArray.Length)
            { break; }
            pointcheck = navArray[raceLoc + i];
            pointcheck.y = transform.position.y;
            if (Vector3.Distance(transform.position, pointcheck) < checkpointSensetivity)
            {
                raceLoc += i;
                break;
            }
        }

        float speed = body.velocity.magnitude * 15F;
        float stiffness = gear;
        float torque = 1;
        //Change gears!
        if (speed < 10F)
        {
            torque = 2;
            stiffness = 7;
            gear = 1;
        }

        else if (speed < 30F)
        {
            torque = 1.5F;
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

        float vermov = 0;
        float hormov = 0;
        if (!isAI) //If Player Controls
        {
            vermov = Mathf.Clamp(Input.GetAxis("Vertical") - Input.GetAxis("Vertical-"), -1F, 1F);
            hormov = Input.GetAxis("Horizontal");
        }
        else //If AI Controls
        {
            int reverse = 1;
            RaycastHit hit;

            Vector3 origin = body.position;
            origin.y += 0.30F;
            //detect collision
            if (Physics.Raycast(origin, transform.forward, out hit, 0.9F))
            {
                aiRecentCollide = true;
                aiRecentCollideTimer = 15;
            }
            var forward = transform.forward;
            Debug.DrawRay(origin, forward * 0.9F, Color.blue);
            origin += transform.right * 0.15F;

            //detect collision
            if (Physics.Raycast(origin, transform.forward, out hit, 0.9F))
            {
                aiRecentCollide = true;
                aiRecentCollideTimer = 15;
            }
            Debug.DrawRay(origin, forward * 0.9F, Color.blue);
            origin -= transform.right*0.3F;
            
            //detect collision
            if (Physics.Raycast(origin, transform.forward, out hit, 0.9F))
            {
                aiRecentCollide = true;
                aiRecentCollideTimer = 15;
            }
            Debug.DrawRay(origin, forward * 0.9F, Color.blue);
            //if collision correction
            if (aiRecentCollide)
            {
                if (aiRecentCollideTimer <= 0)
                {aiRecentCollide = false;}

                reverse = -1500;
                aiRecentCollideTimer--;
            }

            //normal movement
            Vector3 target;
            if (aiRecentCollide == false)
            { target = navArray[(raceLoc + 1) % navArray.Length]; }
            else { 
                Vector3 oldtarget = navArray[(raceLoc) % navArray.Length];
                Vector3 newtarget = navArray[(raceLoc + 1) % navArray.Length];
                target = ((oldtarget - newtarget) * 0.75F) + newtarget;
            }

            Debug.DrawLine(transform.position, target);

            Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(target.x,transform.position.y,target.z));

            hormov = (Mathf.Clamp(reverse * RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude, -0.9F, 0.9F));
            vermov = Mathf.Clamp(reverse*(1-(Mathf.Abs(hormov)*0.6F)),-1F,1F);
        }

        wheelBR.motorTorque = torque * vermov;
        wheelBL.motorTorque = torque * vermov;
        float stearMod = Mathf.Clamp(40F-(body.velocity.magnitude*7F),10F,40F);
        wheelFL.steerAngle = stearMod * hormov;
        wheelFR.steerAngle = stearMod * hormov;

        body.AddForce(vermov * 100 * body.transform.forward);
        body.AddForce(-body.transform.up * (body.velocity.magnitude * 110F + 200F));

        int touching = 0;

        if (!wheelBR.isGrounded)
        {
            body.AddForce(-body.transform.up * 150F);
            touching++;
        }
        if (!wheelBL.isGrounded)
        {
            body.AddForce(-body.transform.up * 150F);
            touching++;
        }
        if (!wheelFL.isGrounded)
        {
            body.AddForce(-body.transform.up * 100F);
            touching++;
        }
        if (!wheelFR.isGrounded)
        {
            body.AddForce(-body.transform.up * 100F);
            touching++;
        }
        if (touching==4)
        {
            respawntimer++; //if you've beeen upside down a while, go back to safespot
            if (respawntimer>240)
            {
                body.isKinematic = true;
                Vector3 tempV3 = navArray[raceLoc];
                tempV3.z += 1F;

                body.position = tempV3;
                body.transform.LookAt(navArray[raceLoc + 1]);

                Vector3 oldrot = body.rotation.eulerAngles;
                oldrot.z = 0;
                body.rotation = Quaternion.Euler(oldrot);

                body.isKinematic = false;
                enabled = false;
                enabled = true;
                respawntimer = 0;
           }
        }
        else
        { respawntimer = 0;}

    }
    void Update()
    {
        lapTime += Time.deltaTime;
        totalTime += Time.deltaTime;
        //make next targets distance public
       distanceToNext = Vector3.Distance(body.position,navArray[(raceLoc+1) % navArray.Length]);

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

        //sounds
        engineSound.pitch = 1 + body.velocity.magnitude/10F;

        //breaklight
        if (wheelBL.rpm < 0F && Mathf.Clamp(Input.GetAxis("Vertical") - Input.GetAxis("Vertical-"), -1F, 1F) < 0)
        {
            if (brakeBuildup > 1)
            {
                backLightL.enabled = true;
                backLightR.enabled = true;
            }
            else
            { brakeBuildup += Time.deltaTime; }
        }
        else
        {
            backLightL.enabled = false;
            backLightR.enabled = false;
            brakeBuildup = 0;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == finishLine)
        {
            if (raceLoc >= navArray.Length-5)
            {
                lap++;
                raceLoc = 0;
                lapTime = 0F;
            }
        }
    }



    public Texture getTerrainTextureAt(Vector3 position)
    {
        // Set up:
        Texture retval = new Texture();
        Vector3 TS; // terrain size
        Vector2 AS; // control texture size

        TS = Terrain.activeTerrain.terrainData.size;
        AS.x = Terrain.activeTerrain.terrainData.alphamapWidth;
        AS.y = Terrain.activeTerrain.terrainData.alphamapHeight;


        // Lookup texture we are standing on:
        int AX = 250 + (int)((position.x / TS.x) * AS.x + 0.5f);
        int AY = 250 + (int)((position.z / TS.z) * AS.y + 0.5f);
        var TerrCntrl = Terrain.activeTerrain.terrainData.GetAlphamaps(AX, AY, 1, 1);

        TerrainData TD = Terrain.activeTerrain.terrainData;

        for (int i = 0; i < TD.splatPrototypes.Length; i++)
        {
            if (TerrCntrl[0, 0, i] > .5f)
            {
                retval = TD.splatPrototypes[i].texture;
            }

        }

        Debug.Log(retval);
        return retval;
    }
}
