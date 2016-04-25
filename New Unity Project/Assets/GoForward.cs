using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public Gear[] gearBox;
    public float maxTorque;
    public float turnPower;

    public AudioSource engineSound;
    public bool useSpeedTexture;
    public Texture speedTexture;
    public float speedTextureLoss;
    public bool useSpeedCollider;
    public GameObject speedCollider;
    public float speedColliderGain;

    [HideInInspector] public int gear = 0;
    [HideInInspector] public float distanceToNext;
    [HideInInspector] public int raceLoc = 0;
    [HideInInspector] public int navArraySize = 0;
    [HideInInspector] public int lap = 0;

    [HideInInspector] public float lapTime;
    [HideInInspector] public float totalTime;

    [HideInInspector]public List<float> lapTimes = new List<float>();

    private float timer = 0;
    private int respawntimer = 0;

    private bool aiRecentCollide;
    private int aiRecentCollideTimer;

    private float brakeBuildup = 0;
    private float gearTimer = 0;

    [System.Serializable]
    public class Gear
    {
        public float startSpeed;
        public float endSpeed;
        public float gearWheelPower;
        public float gearTorquePower;
    }


    public void CStart() {
        //create an array of the the navpoints and add a slight randomification
        navArray = new Vector3[navGroup.transform.childCount];
        int rep = navGroup.transform.childCount;
        for (int i = 0; i < rep; i++)
        {
            navArray[i] = navGroup.transform.GetChild(i).position;
            navArray[i] += new Vector3(Random.Range(-0.25F, 0.25F), 0, Random.Range(-0.25F, 0.25F));

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
        if (timer > 8)
        {
        //calculate your position
        //check up too 15 checkpoints forward
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

        float speed = body.velocity.magnitude * 12F;
        //Change gears!
        
        //change downwards
        if (gear > 0)
        {
            if (speed < gearBox[gear-1].endSpeed)
            { 
                gear--;
            }
        }
        if (gear < gearBox.Length-1)
        {
            if (speed > gearBox[gear+1].startSpeed)
            { 
                gear++;
                if (gear != 1) {gearTimer = .5F;}
            }
        }

        float stiffness = gearBox[gear].gearWheelPower;
        float torque = gearBox[gear].gearTorquePower;
        if (useSpeedTexture)
        {
            if (speedTexture != getTerrainTextureAt(body.transform.position))
            { torque *= speedTextureLoss; }
        }
        if (useSpeedCollider)
        {
            RaycastHit hit;
            if (Physics.Raycast(body.position,-transform.up,out hit,0.5F))
            {
                if (hit.transform.gameObject == speedCollider)
                {
                    torque *=speedColliderGain;
                }
            }
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
            origin -= transform.right * 0.15F;

            for (int i = 0; i < 3;i++ )
            {
                if (Physics.Raycast(origin, transform.forward, out hit, 0.9F))
                {
                    aiRecentCollide = true;
                    aiRecentCollideTimer = 15;
                }
                var forward = transform.forward;
                Debug.DrawRay(origin, forward * 0.9F, Color.blue);
                origin += transform.right * 0.15F;
            }

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
        if ((wheelBL.rpm < 0F && speed > 30) || gearTimer > 0.3F)
        { vermov = 0; }

        wheelBR.motorTorque = torque * vermov;
        wheelBL.motorTorque = torque * vermov;
        float stearMod = Mathf.Clamp(40F-(body.velocity.magnitude*7F),10F,40F)*turnPower;
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

    }
    void Update()
    {
        timer += Time.deltaTime;
        gearTimer -= Time.deltaTime;
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
        temp.y = wheelFL.steerAngle*2 - wheelFLMesh.localEulerAngles.z;

        wheelFLMesh.localEulerAngles = temp;

        temp = wheelFRMesh.localEulerAngles;
        temp.y = wheelFR.steerAngle*1.8F - wheelFRMesh.localEulerAngles.z;

        wheelFRMesh.localEulerAngles = temp;

        //sounds
        engineSound.pitch = 1 + body.velocity.magnitude/10F;
        if (gearTimer > 0)
        { engineSound.volume = Mathf.Clamp(engineSound.volume - Time.deltaTime, 0.4F, 1F); }
        else
        { engineSound.volume = Mathf.Clamp(engineSound.volume + 2F * Time.deltaTime, 0F, 1F); }

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
                lapTimes.Add(lapTime);
                lapTime = 0F;

                if (lap > 3)
                { }
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
        int AX = (int)(((250 + position.x) / TS.x) * AS.x + 0.5f);
        int AY = (int)(((250 + position.z) / TS.z) * AS.y + 0.5f);
        var TerrCntrl = Terrain.activeTerrain.terrainData.GetAlphamaps(AX, AY, 1, 1);

        TerrainData TD = Terrain.activeTerrain.terrainData;

        for (int i = 0; i < TD.splatPrototypes.Length; i++)
        {
            if (TerrCntrl[0, 0, i] > .5f)
            {
                retval = TD.splatPrototypes[i].texture;
            }

        }
        return retval;
    }
}
