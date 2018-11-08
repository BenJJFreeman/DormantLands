using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;


public class Main : MonoBehaviour {

    //public int level;

    
    public GameMenu gameMenu;
    public GameObject finishLevelButton;

    public GraphicRaycaster[] gRaycaster;
    PointerEventData pointerEventData;
    public EventSystem eventSystem;

    public CameraControl cameraControl;
	public Slider timelineSlider;
    public GameObject actionListContent;
    public GameObject actionListObjectPrefab;
    public List<ActionListObject> actionListObjects = new List<ActionListObject>();

	public Timeline timeline;

	//public Transform[] target;
	public NavMeshAgent[] agent;
    public Animation[] actorAnimation;

    public LineRenderer[] actorLineRenderer;
    public LineRenderer tempLineRenderer;

    public GameObject[] movableObjects;
	//public GameObject[] movableObjectsSimulationBlock;
    //public GameObject[] turretObject;
    public List<GameObject> dartObjects = new List<GameObject>();
    //public GameObject[] pressurePlatesObjects;
    public GameObject[] ziplineObjects;
    public GameObject[] dartwallObjects;
    public GameObject[] unstableFloorObjects;
    public GameObject[] movingPlatformSetObjects;
    public GameObject[] spikeObjects;
    public GameObject[] endAreaObjects;


    public ParticleSystem[] environmentalParticleSystems;
    //public Animation anim;


    public GameObject dartPrefab;

    public bool walking;
    public bool jumping;

    //public GameObject agent;


    public bool endLevelTriggered;

    public int selectedActor;


    public Text simulatingButtonText;
    public bool simualting;


    float jumpDistanceMax;


    void Awake()
    {

        jumpDistanceMax = 3.5f;


    }

    void Start()
    {

        //anim.Play("TestAnim");
        // anim["TestAnim"].speed = 0;
        // anim["TestAnim"].wrapMode = WrapMode.Loop;



        for (int i = 0; i < actorAnimation.Length; i++)
        {
            actorAnimation[i].Play("Walk");
            actorAnimation[i]["Walk"].speed = 0;
            actorAnimation[i]["Walk"].wrapMode = WrapMode.Loop;
            actorAnimation[i]["Walk"].blendMode = AnimationBlendMode.Blend;

            actorAnimation[i].Play("Jump");
            actorAnimation[i]["Jump"].speed = 0;
            actorAnimation[i]["Jump"].wrapMode = WrapMode.Loop;
            actorAnimation[i]["Jump"].blendMode = AnimationBlendMode.Blend;

            actorAnimation[i].Play("Idle");
            actorAnimation[i]["Idle"].speed = 0;
            actorAnimation[i]["Idle"].wrapMode = WrapMode.Loop;
            actorAnimation[i]["Idle"].blendMode = AnimationBlendMode.Blend;

            actorAnimation[i].Play("Falling");
            actorAnimation[i]["Falling"].speed = 0;
            actorAnimation[i]["Falling"].wrapMode = WrapMode.Loop;
            actorAnimation[i]["Falling"].blendMode = AnimationBlendMode.Blend;

            actorAnimation[i].Play("Death");
            actorAnimation[i]["Death"].speed = 0;
            //actorAnimation[i]["Death"].wrapMode = WrapMode.Loop;
            actorAnimation[i]["Death"].blendMode = AnimationBlendMode.Blend;
        }        

        timeline = new Timeline();

        timeline.layerMask = ~LayerMask.GetMask("Platform");       



        //
        Actor[] actor = new Actor[agent.Length];
        for (int i = 0; i < actor.Length; i++)
        {
            actor[i] = new Actor(agent[i].transform.position);
        }
        //
        MovableObject[] movableObject = new MovableObject[movableObjects.Length];
        for (int i = 0; i < movableObject.Length; i++)
        {
            RaycastHit hit;
            Physics.Raycast(movableObjects[i].transform.position, Vector3.down, out hit);
            movableObject[i] = new MovableObject(movableObjects[i].transform.position, hit.point, 10, 0, 0);
        }
        //
        DartWall[] dartWalls = new DartWall[dartwallObjects.Length];
        for (int i = 0; i < dartWalls.Length; i++)
        {
            dartWalls[i] = new DartWall(dartwallObjects[i].transform.position, dartwallObjects[i].transform.localScale, dartwallObjects[i].transform.rotation, new PressurePlate(dartwallObjects[i].transform.GetChild(0).transform.position, dartwallObjects[i].transform.GetChild(0).transform.localScale));
        }
        //
        Zipline[] ziplines = new Zipline[ziplineObjects.Length];
        for (int i = 0; i < ziplines.Length; i++)
        {
            Vector3[] point = new Vector3[2];
            point[0] = ziplineObjects[i].transform.position;
            point[1] = ziplineObjects[i].transform.GetChild(0).transform.position;
            ziplines[i] = new Zipline(point);
        }
        //
        SpikeTrap[] spikeTrap = new SpikeTrap[spikeObjects.Length];
        for (int i = 0; i < spikeTrap.Length; i++)
        {
            if (spikeObjects[i] != null)
            {

                Vector3[] STpoint = new Vector3[2];
                STpoint[0] = spikeObjects[i].transform.GetChild(1).transform.position;
                STpoint[1] = STpoint[0] + new Vector3(0,2.75f,0);
                spikeTrap[i] = new SpikeTrap(STpoint, STpoint[0], spikeObjects[i].transform.localScale, new PressurePlate(spikeObjects[i].transform.GetChild(0).transform.position, spikeObjects[i].transform.GetChild(0).transform.localScale));
            }
        }
        //
        UnstableFloor[] unstableFloors = new UnstableFloor[unstableFloorObjects.Length];
        for (int i = 0; i < unstableFloors.Length; i++)
        {
            Vector3[] UFpoint = new Vector3[2];
            UFpoint[0] = unstableFloorObjects[i].transform.position;
            UFpoint[1] = UFpoint[0] + Vector3.down * 25;
            unstableFloors[i] = new UnstableFloor(UFpoint[0], unstableFloorObjects[i].transform.localScale, UFpoint);
        }
        //
        MovingPlatformSet[] movingPlatformSet = new MovingPlatformSet[movingPlatformSetObjects.Length];
        for (int i = 0; i < movingPlatformSet.Length; i++)
        {
            int platforms = 0;
            int pressurePlateChild = 0;
            int a = 0;
            foreach (Transform child in movingPlatformSetObjects[i].transform)
            {
                if (child.transform.tag == "Platform")
                {
                    platforms++;

                }
                else
                {

                    pressurePlateChild = a;
                }
                a++;
            }

            MovingPlatform[] movingPlatforms = new MovingPlatform[platforms];
            for (int j = 0; j < platforms; j++)
            {
                Vector3[] point = new Vector3[2];
                point[0] = movingPlatformSetObjects[i].transform.GetChild(j).transform.position;
                point[1] = point[0] + movingPlatformSetObjects[i].transform.GetChild(j).transform.forward * 4;
                movingPlatforms[j] = new MovingPlatform(point[0], movingPlatformSetObjects[i].transform.GetChild(j).transform.localScale, point);
            }

            movingPlatformSet[i] = new MovingPlatformSet(movingPlatforms, new PressurePlate(movingPlatformSetObjects[i].transform.GetChild(pressurePlateChild).transform.position, movingPlatformSetObjects[i].transform.GetChild(pressurePlateChild).transform.localScale));

            //movingPlatformSet[i].movingPlatforms = new MovingPlatform[movingPlatformSetObjects[i].transform.childCount];

        }
        //
        EndArea[] endAreas = new EndArea[endAreaObjects.Length];
        for (int i = 0; i < endAreas.Length; i++)
        {

            endAreas[i] = new EndArea(endAreaObjects[i].transform.position, endAreaObjects[i].transform.localScale);

        }

        timeline.Init(timelineSlider.maxValue, actor, movableObject, dartWalls, ziplines, spikeTrap, unstableFloors, movingPlatformSet, endAreas);
        //Timeline.RotateTowardsObject(timeline, timeline.turret[0], timeline.actor[0], 3);
        UpdateTimelineParts();

        SetTimelineCurrentValue(0);
    }
    void Update()
    {   


        
            cameraControl.UpdateCamera(endLevelTriggered);

        if (endLevelTriggered)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (simualting)
            {
                SimulateRestOfTimeline();
            }
            else if (gameMenu.optionsCanvas.activeSelf)
            {
                gameMenu.OpenOptionsMenu(false);
            }
            else if (gameMenu.confimationCanvas.activeSelf)
            {

                gameMenu.RestartLevelInit(false);
                gameMenu.LoadMenuInit(false);
                gameMenu.QuitGameInit(false);
            }
            else
            {

                gameMenu.OpenGameMenu(!gameMenu.gameMenu.activeSelf);
                StopActions();
                //walking = false;
                //jumping = false;

            }
        }


        if (simualting)
        {

            SetTimelineCurrentValue(timelineSlider.value += 1 * Time.deltaTime);

            return;
        }




        /////
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        for (int i = 0; i < gRaycaster.Length; i++)
        {
            gRaycaster[i].Raycast(pointerEventData, results);

        }

        if (results.Count == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);


                NavMeshPath tempPath = new NavMeshPath();
                NavMesh.CalculatePath(Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value), hit.point, NavMesh.AllAreas, tempPath);


                if (hit.transform)
                {                  

                    if (walking)
                    {
                        UpdateTempLineRenderer(tempPath,hit.point);
                    }
                    else if (jumping)
                    {
                        UpdateTempLineRenderer(hit.point);
                    }



                    if (Input.GetMouseButtonDown(0))
                    {
                        if (walking)
                        {
                            ActorMovement(tempPath, hit.point);
                        }
                        else if (jumping)
                        {
                            ActorJump(tempPath, hit.point);
                        }


                    }
                    
                }

            }

        }
    }
    void ActorMovement(NavMeshPath tempPath,Vector3 point)
    {
        if (tempPath.status == NavMeshPathStatus.PathComplete)
        {
            UpdateActorMovement(agent[selectedActor].transform.position, point);  
        }
        else
        {
            gameMenu.audioControl.PlaySoundEffect(1);
            // fail sound fx
        }
    }
    void ActorJump(NavMeshPath tempPath, Vector3 point)
    {
        // if (tempPath.status != NavMeshPathStatus.PathInvalid)
        //{
        if (Vector3.Distance(agent[selectedActor].transform.position, point) < jumpDistanceMax)
        {
            UpdateActorJump(agent[selectedActor].transform.position, point);
        }else if (Vector3.Distance(tempLineRenderer.GetPosition(0), point) < jumpDistanceMax)
        {

            UpdateActorJump(agent[selectedActor].transform.position, point);
        }
        else
        {
            gameMenu.audioControl.PlaySoundEffect(1);
            // fail sound fx
        }
        //}

    }
    void UpdateActorLineRenderer()
    {
        for (int a = 0; a < timeline.actor.Length; a++)
        {
            actorLineRenderer[a].positionCount = 1;

            actorLineRenderer[a].SetPosition(0, Timeline.GetActorLocationAtTime(timeline.actor[a],0.01f));

            int move = 0;
            int jump = 0;
            int fall = 0;

            for (int m = 0; m < timeline.actor[a].actionList.Count; m++)
            {
               

                switch (timeline.actor[a].actionList[m].actionType)
                {



                    case ActionType.walk:

                        

                        for (int i = 0; i < timeline.actor[a].movementList[move].path.corners.Length - 1; i++)
                        {

                            actorLineRenderer[a].positionCount++;

                            actorLineRenderer[a].SetPosition(actorLineRenderer[a].positionCount-1, timeline.actor[a].movementList[move].path.corners[i]);


                        }

                        actorLineRenderer[a].positionCount++;

                        actorLineRenderer[a].SetPosition(actorLineRenderer[a].positionCount - 1, timeline.actor[a].movementList[move].path.corners[timeline.actor[a].movementList[move].path.corners.Length - 1]);

                        move++;

                        break;



                    case ActionType.jump:

                        actorLineRenderer[a].positionCount++;
                        actorLineRenderer[a].SetPosition(actorLineRenderer[a].positionCount-1, timeline.actor[a].jumpList[jump].landPoint);

                        jump++;

                        break;



                    case ActionType.falling:

                        actorLineRenderer[a].positionCount++;
                        actorLineRenderer[a].SetPosition(actorLineRenderer[a].positionCount-1, timeline.actor[a].fallList[fall].point[1]);

                        fall++;

                        break;

                    case ActionType.death:



                        break;


                }



            }




            //for (int m = 0; m < timeline.actor[a].movementList.Count; m++)
            //{

            //    for (int i = 0; i < timeline.actor[a].movementList[m].path.corners.Length - 1; i++)
            //    {


            //        Debug.DrawLine(timeline.actor[a].movementList[m].path.corners[i], timeline.actor[a].movementList[m].path.corners[i + 1], Color.red);

            //    }
            //    for (int i = 0; i < timeline.actor[a].movementList[m].path.corners.Length; i++)
            //    {
            //        Debug.DrawRay(timeline.actor[a].movementList[m].path.corners[i], Vector3.up, Color.blue);

            //    }
            //}
            //for (int m = 0; m < timeline.actor[a].jumpList.Count; m++)
            //{


            //    Debug.DrawLine(timeline.actor[a].jumpList[m].jumpPoint, timeline.actor[a].jumpList[m].landPoint, Color.red);

            //    Debug.DrawRay(timeline.actor[a].jumpList[m].centrePoint, Vector3.up, Color.blue);

            //}
            //for (int m = 0; m < timeline.actor[a].fallList.Count; m++)
            //{

            //    Debug.DrawLine(timeline.actor[a].fallList[m].point[0], timeline.actor[a].fallList[m].point[1], Color.red);

            //}


        }

    }
    void UpdateTempLineRenderer(NavMeshPath tempPath,Vector3 _point)
    {
        tempLineRenderer.positionCount = 1;

        if (Timeline.GetActionAtTime(timeline.actor[selectedActor], timelineSlider.value) == ActionType.jump)
        {

            if (timelineSlider.value == timeline.actor[selectedActor].jumpList[Timeline.GetJumpListSection(timeline.actor[selectedActor], timelineSlider.value)].startTime)
            {
                tempLineRenderer.SetPosition(0, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
                NavMeshPath newPath = new NavMeshPath();
                NavMesh.CalculatePath(Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value), _point, NavMesh.AllAreas, newPath);
                tempPath = newPath;
            }
            else
            {
                tempLineRenderer.SetPosition(0, timeline.actor[selectedActor].jumpList[Timeline.GetJumpListSection(timeline.actor[selectedActor], timelineSlider.value)].landPoint);
                NavMeshPath newPath = new NavMeshPath();
                NavMesh.CalculatePath(timeline.actor[selectedActor].jumpList[Timeline.GetJumpListSection(timeline.actor[selectedActor], timelineSlider.value)].landPoint, _point, NavMesh.AllAreas, newPath);
                tempPath = newPath;
            }

        }
        else if (Timeline.GetActionAtTime(timeline.actor[selectedActor], timelineSlider.value) == ActionType.death)
        {
            tempLineRenderer.SetPosition(0, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
            //tempLineRenderer.SetPosition(1, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
            return;
        }
        else
        {
            tempLineRenderer.SetPosition(0, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
        }


        for (int i = 0; i < tempPath.corners.Length - 1; i++)
        {

            tempLineRenderer.positionCount++;

            tempLineRenderer.SetPosition(tempLineRenderer.positionCount - 1, tempPath.corners[i]);


        }

        tempLineRenderer.positionCount++;

        if (tempPath.corners.Length > 1)
        {
            tempLineRenderer.SetPosition(tempLineRenderer.positionCount - 1, tempPath.corners[tempPath.corners.Length - 1]);
        }
        else
        {
            tempLineRenderer.SetPosition(tempLineRenderer.positionCount - 1, tempLineRenderer.GetPosition(tempLineRenderer.positionCount - 2));
        }




    }
    void UpdateTempLineRenderer(Vector3 point)
    {


        

            tempLineRenderer.positionCount = 2;

        if (Timeline.GetActionAtTime(timeline.actor[selectedActor], timelineSlider.value) == ActionType.jump)
        {
            if (timelineSlider.value == timeline.actor[selectedActor].jumpList[Timeline.GetJumpListSection(timeline.actor[selectedActor], timelineSlider.value)].startTime)
            {

                tempLineRenderer.SetPosition(0, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
            }
            else
            {
                tempLineRenderer.SetPosition(0, timeline.actor[selectedActor].jumpList[Timeline.GetJumpListSection(timeline.actor[selectedActor], timelineSlider.value)].landPoint);
            }

        }
        else if (Timeline.GetActionAtTime(timeline.actor[selectedActor], timelineSlider.value) == ActionType.death)
        {
            tempLineRenderer.SetPosition(0, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
            //tempLineRenderer.SetPosition(1, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
            return;
        }
        else
        {
            tempLineRenderer.SetPosition(0, Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.value));
        }

        if (Vector3.Distance(tempLineRenderer.GetPosition(0), point) < jumpDistanceMax)
        {

            tempLineRenderer.SetPosition(1, point);
        }else
        {

            Vector3 head = point - tempLineRenderer.GetPosition(0);
            float dis = head.magnitude;
            Vector3 dir = head / dis;



            tempLineRenderer.SetPosition(1, tempLineRenderer.GetPosition(0) + (dir * jumpDistanceMax));

        }

        




        // Falling
        //tempLineRenderer.positionCount++;
        //tempLineRenderer.SetPosition(tempLineRenderer.positionCount - 1, timeline.actor[a].fallList[fall].point[1]);

        //fall++;
        //
        

    }


    
    //private void OnDrawGizmos()
    //{
    //    for (int d = 0; d < timeline.dartWalls.Length; d++)
    //    {

    //        Gizmos.DrawCube(timeline.dartWalls[d].pressurePlate.centre, timeline.dartWalls[d].pressurePlate.size);


    //    }
    //    for (int d = 0; d < timeline.movingPlatformSets.Length; d++)
    //    {

    //        Gizmos.DrawCube(timeline.movingPlatformSets[d].pressurePlate.centre, timeline.movingPlatformSets[d].pressurePlate.size);


    //    }
    //}

    void UpdateActorMovement(Vector3 srtPos,Vector3 endPos){

		NavMeshPath newPath = new NavMeshPath ();
		NavMesh.CalculatePath (srtPos, endPos, NavMesh.AllAreas, newPath);



        if(Timeline.UpdateActor (timeline.actor [selectedActor], timelineSlider.value,srtPos,endPos, newPath))
        {


            UpdateTimelineParts();
            gameMenu.audioControl.PlaySoundEffect(0);
            // success sound fx
        }
        else
        {
            gameMenu.audioControl.PlaySoundEffect(1);
            // fail sound fx

        }
        //Timeline.UpdateActor(timeline.actor[0], timelineSlider.value,srtPos,endPos);



        //Timeline.CheckTurretIntersection(timeline, timeline.turret[0]);




    }
    void UpdateActorJump(Vector3 srtPos, Vector3 endPos)
    {
        srtPos += Vector3.down;

        NavMeshPath newPath = new NavMeshPath();
        NavMesh.CalculatePath(srtPos, endPos, NavMesh.AllAreas, newPath);
        //Timeline.UpdateActor (timeline.actor [0], timelineSlider.value,srtPos, newPath);
        if (Timeline.UpdateActor(timeline.actor[selectedActor], timelineSlider.value, srtPos, endPos))
        {
            UpdateTimelineParts();
            gameMenu.audioControl.PlaySoundEffect(0);
            // success sound fx
        }
        else
        {
            gameMenu.audioControl.PlaySoundEffect(1);
            // fail sound fx
        }
    }
    void UpdateTimelineParts()
    {

        

        UpdateActionListObjects();

        for (int a = 0; a < timeline.actor.Length; a++){


            timeline.actor[a].lifeList.Clear();
            timeline.actor[a].lifeList.Add(new Life(0, timeline.length, true,timeline.actor[a].startingPos));
            timeline.actor[a].lifeList.Add(new Life(timeline.length, timeline.length, true, Timeline.GetActorLocationAtTime(timeline.actor[a], timeline.length)));

           

            for (int i = 0; i < timeline.movableObject.Length; i++)
            {
                Timeline.PathCollisionCheck(timeline.movableObject[i], timeline.actor[a], timeline);
            }
        }
        //for (int i = 0; i < timeline.pressurePlates.Length; i++)
        //{
        //    Timeline.UpdatePressurePlate(timeline.pressurePlates[i], timeline);
        //}
        for (int i = 0; i < timeline.spikeTraps.Length; i++)
        {
            Timeline.UpdateSpikeTrap(timeline.spikeTraps[i], timeline);
        }
        for (int i = 0; i < timeline.dartWalls.Length; i++)
        {
            Timeline.UpdateDartWall(timeline.dartWalls[i], timeline);
        }
        
        for (int i = 0; i < timeline.unstableFloors.Length; i++)
        {
            Timeline.UpdateUnstableFloor(timeline.unstableFloors[i], timeline);
        }
        for (int i = 0; i < timeline.actor.Length; i++)
        {
            //
            Timeline.CheckActorIsOnGround(timeline.actor[i], timeline,timelineSlider.value);
            //
        }
        for (int i = 0; i < movingPlatformSetObjects.Length; i++)
        {
            Timeline.UpdateMovingPlatformSet(timeline.movingPlatformSets[i], timeline);
        }
        for (int i = 0; i < timeline.endArea.Length; i++)
        {
            Timeline.UpdateEndArea(timeline.endArea[i], timeline);
           
        }
        for (int a = 0; a < timeline.actor.Length; a++)
        {
            Timeline.CheckActorIsAlive(timeline.actor[a]);


            Timeline.AddIdleSections(timeline.actor[a], timeline.length);

        }

        CheckIfEndViable();


        UpdateActorLineRenderer();
        UpdateActionListObjects();
    }
    void CheckIfEndViable()
    {
        int activeCount = 0;
        for (int i = 0; i < timeline.endArea.Length; i++)
        {

            for (int e = 0; e < timeline.endArea[i].eventTimeline.Count; e++)
            {

                if (timeline.endArea[i].eventTimeline[e].active)
                {

                    if (timelineSlider.maxValue >= timeline.endArea[i].eventTimeline[e].startTime && timelineSlider.maxValue <= timeline.endArea[i].eventTimeline[e].endTime)
                    {


                        activeCount++;
                    }
                    //else
                    //{
                    //    FinishedLevel(false);

                    //}

                }
                //else
                //{
                //    FinishedLevel(false);

                //}

            }
        }

        if(activeCount >= timeline.endArea.Length)
        {

            FinishedLevel(true);

        }
        else
        {
            FinishedLevel(false);

        }

    }
    public void UpdateTimeline()
    {
        for (int a = 0; a < timeline.actor.Length; a++)
        {
            // agent[a].enabled = true;
            Vector3 location = Timeline.GetActorLocationAtTime(timeline.actor[a], timelineSlider.value);
            //print(location);
            timeline.actor[a].position = new Vector3(location.x, location.y + 1, location.z);

            Vector3 nextLocation = Timeline.GetActorLocationAtTime(timeline.actor[a], timelineSlider.value + (1 * Time.deltaTime));

            nextLocation = new Vector3(nextLocation.x, location.y, nextLocation.z);

            Vector3 dir = nextLocation - location;
            Quaternion rotation;
            if (dir != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(dir);
            }
            else
            {
                rotation = agent[a].transform.rotation;
            }

            agent[a].transform.rotation = rotation;


            ActionType at = Timeline.GetActionAtTime(timeline.actor[a], timelineSlider.value);



            switch (at)
            {
                case ActionType.walk:
                    agent[a].enabled = true;
                    agent[a].Warp(new Vector3(location.x, location.y + 1, location.z));


                    agent[a].nextPosition = new Vector3(location.x, location.y + 1, location.z);
                    agent[a].enabled = false;

                    float startTW = timeline.actor[a].actionList[Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value)].startTime;

                    for (int i = Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value); i > 0; i--)
                    {
                        if (timeline.actor[a].actionList[i].actionType == ActionType.walk)
                        {
                            startTW = timeline.actor[a].actionList[i].startTime;
                        }
                        else
                        {
                            break;
                        }
                    }
                    float newTimeW = timelineSlider.value;
                    newTimeW -= startTW;
                    actorAnimation[a].Play("Walk");
                    actorAnimation[a]["Walk"].time = newTimeW;

                    break;

                case ActionType.jump:
                    agent[a].enabled = false;
                    agent[a].transform.position = new Vector3(location.x, location.y + 1, location.z);

                    
                    float startTJ = timeline.actor[a].actionList[Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value)].startTime;

                    for (int i = Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value); i > 0; i--)
                    {
                        if (timeline.actor[a].actionList[i].actionType == ActionType.jump)
                        {
                            startTJ = timeline.actor[a].actionList[i].startTime;
                        }
                        else
                        {
                            break;
                        }
                    }
                    float newTimeJ = timelineSlider.value;
                    newTimeJ -= startTJ;
                    actorAnimation[a].Play("Jump");
                    actorAnimation[a]["Jump"].time = newTimeJ;

                    break;
                case ActionType.falling:
                    agent[a].enabled = false;
                    agent[a].transform.position = new Vector3(location.x, location.y + 1, location.z);
                                        
                    float startTF = timeline.actor[a].actionList[Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value)].startTime;

                    for (int i = Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value); i > 0; i--)
                    {
                        if (timeline.actor[a].actionList[i].actionType == ActionType.falling)
                        {
                            startTF = timeline.actor[a].actionList[i].startTime;
                        }
                        else
                        {
                            break;
                        }
                    }
                    float newTimeF = timelineSlider.value;
                    newTimeF -= startTF;
                    actorAnimation[a].Play("Falling");
                    actorAnimation[a]["Falling"].time = newTimeF;

                    break;
                case ActionType.idle:

                    //agent[a].enabled = true;
                    agent[a].enabled = false;
                    agent[a].transform.position = new Vector3(location.x, location.y + 1, location.z);


                    //agent[a].nextPosition = new Vector3(location.x, location.y + 1, location.z);


                    float startTI = timeline.actor[a].actionList[Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value)].startTime;

                    for (int i = Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value); i > 0; i--)
                    {
                        if (timeline.actor[a].actionList[i].actionType == ActionType.idle)
                        {
                            startTI = timeline.actor[a].actionList[i].startTime;
                        }
                        else
                        {
                            break;
                        }
                    }
                    float newTimeI = timelineSlider.value;
                    newTimeI -= startTI;
                    actorAnimation[a].Play("Idle");
                    actorAnimation[a]["Idle"].time = newTimeI;



                    break;
                case ActionType.death:

                    //agent[a].enabled = true;
                    agent[a].enabled = false;
                    agent[a].transform.position = new Vector3(location.x, location.y + 1, location.z);

                    //agent[a].nextPosition = new Vector3(location.x, location.y + 1, location.z);

                    float startTD = timeline.actor[a].actionList[Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value)].startTime;

                    for (int i = Timeline.GetActionListSection(timeline.actor[a], timelineSlider.value); i > 0; i--)
                    {
                        if (timeline.actor[a].actionList[i].actionType == ActionType.idle)
                        {
                            startTD = timeline.actor[a].actionList[i].startTime;
                        }
                        else
                        {
                            break;
                        }
                    }
                    float newTimeD = timelineSlider.value;
                    newTimeD -= startTD;
                    actorAnimation[a].Play("Death");
                    actorAnimation[a]["Death"].time = newTimeD;



                    break;
            }

        }

        for (int i = 0; i < timeline.movableObject.Length; i++)
        {

            Vector3 location = MovableObject.GetObjectPositionAtTime(timeline.movableObject[i], timelineSlider.value);
            //timeline.movableObject[i].currentPoint = new Vector3(location.x, location.y, location.z);
            //agent [a].Warp (new Vector3 (location.x, location.y + 1, location.z));
            //agent.transform.position = new Vector3 (location.x, location.y + 1, location.z);
            movableObjects[i].transform.position = new Vector3(location.x, location.y, location.z);


            Debug.DrawLine(timeline.movableObject[i].startPoint, timeline.movableObject[i].endPoint, Color.red, 1);


        }

        int DA = -1;
        for (int i = 0; i < timeline.dartWalls.Length; i++)
        {
            //Vector3 tempRotation = Timeline.GetTurretRotationAtTime(timeline.turret[i], timelineSlider.value);

            //turretObject[i].transform = tempRotation;
            for (int b = 0; b < timeline.dartWalls[i].dartList.Count; b++)
            {
                DA++;
                timeline.dartWalls[i].dartList[b].position = Timeline.GetDartPositionAtTime(timeline.dartWalls[i].dartList[b], timelineSlider.value);

                if (dartObjects.Count <= DA)
                {

                    GameObject newDart = Instantiate(dartPrefab, dartwallObjects[i].transform.position, timeline.dartWalls[i].dartList[b].rotation);
                    dartObjects.Add(newDart);
                }
                dartObjects[DA].transform.position = timeline.dartWalls[i].dartList[b].position;

            }
        }

        for (int i = 0; i < timeline.unstableFloors.Length; i++)
        {

            Vector3 location = Timeline.GetUnstableFloorPositionAtTime(timeline.unstableFloors[i], timelineSlider.value);
            //timeline.movableObject[i].currentPoint = new Vector3(location.x, location.y, location.z);
            //agent [a].Warp (new Vector3 (location.x, location.y + 1, location.z));
            //agent.transform.position = new Vector3 (location.x, location.y + 1, location.z);
            unstableFloorObjects[i].transform.position = new Vector3(location.x, location.y, location.z);


            //Debug.DrawLine(timeline.unstableFloors[i].startPoint, timeline.movableObject[i].endPoint, Color.red, 1);


        }

        for (int i = 0; i < timeline.movingPlatformSets.Length; i++)
        {
            int p = 0;
            foreach (Transform child in movingPlatformSetObjects[i].transform)
            {

                if (child.transform.tag == "Platform")
                {

                    child.transform.position = Timeline.GetMovingPlatformPositionAtTime(timeline.movingPlatformSets[i].movingPlatforms[p], timelineSlider.value);
                    p++;
                }

            }
        }

        for (int i = 0; i < timeline.spikeTraps.Length; i++)
        {

            spikeObjects[i].transform.GetChild(1).transform.position = Timeline.GetSpikeTrapAtTime(timeline.spikeTraps[i], timelineSlider.value);

        }



        for (int i = 0; i < environmentalParticleSystems.Length; i++)
        {
            environmentalParticleSystems[i].Simulate(timelineSlider.value);
        }

        //ps.Simulate(timelineSlider.value);
        //anim["TestAnim"].time = timelineSlider.value;

    }
    void UpdateActionListObjects()
    {

        actionListObjects.Clear();
        

        foreach (Transform child in actionListContent.transform)
        {

            Destroy(child.gameObject);

        }


        List<ActionBase> actionList = timeline.actor[selectedActor].actionList;

        for(int i = 0; i < actionList.Count; i++)
        {

            GameObject ob = Instantiate(actionListObjectPrefab, actionListContent.transform);
            float[] time = new float[2];
            time[0] = actionList[i].startTime;
            time[1] = actionList[i].endTime;
            actionListObjects.Add(ob.GetComponent<ActionListObject>());
            actionListObjects[i].SetUpActionList(this,time,actionList[i].actionType.ToString(),i);


        }


    }

    public void SetTimelineCurrentValue(float _time)
    {
        timelineSlider.value = _time;
        UpdateTimeline();


    }
    public void SelectWalking()
    {
        walking = true;
        jumping = false;
    }
    public void SelectJumping()
    {
        walking = false;
        jumping = true;
    }
    public void SelectActor(int _actor)
    {
        selectedActor = _actor;
        cameraControl.SetFocusTarget(agent[_actor].transform);
        UpdateActionListObjects();
    }
    void FinishedLevel(bool _active)
    {

        finishLevelButton.SetActive(_active);
    }
    public void EndLevel()
    {
        endLevelTriggered = true;
        gameMenu.gameplayCanvas.SetActive(false);

        if (gameMenu.level == 12)
        {
            gameMenu.endGameCanvas.SetActive(true);

        }
        else
        {
            StartCoroutine("LoadLevelWait", 2);
        }
    }
    IEnumerator LoadLevelWait(float sec)
    { 
       yield return new WaitForSeconds(sec);
        gameMenu.LoadNextLevel();
    }
    public void SimulateRestOfTimeline()
    {

        simualting = !simualting;


        simulatingButtonText.text = (simualting ? "Stop" : "Simulate");

    }
    public void ClearActorTimeline()
    {
        Vector3 location = Timeline.GetActorLocationAtTime(timeline.actor[selectedActor], timelineSlider.minValue);
        timeline.actor[selectedActor].position = new Vector3(location.x, location.y + 1, location.z);
        agent[selectedActor].enabled = true;
        agent[selectedActor].Warp(new Vector3(location.x, location.y + 1, location.z));


        agent[selectedActor].nextPosition = new Vector3(location.x, location.y + 1, location.z);
        agent[selectedActor].enabled = false;

       
        Timeline.ClearActorLists(timeline.actor[selectedActor]);
        UpdateTimelineParts();
        SetTimelineCurrentValue(timelineSlider.minValue);

        for(int i = 0; i < dartObjects.Count; i++)
        {

            dartObjects[i].transform.position = dartwallObjects[0].transform.position;

        }





        //SimulateRestOfTimeline();
        simualting = false;
        simulatingButtonText.text = "Simulate";
    }
    public void ClearTimeline()
    {
        for (int i = 0; i < timeline.actor.Length; i++)
        {

            Vector3 location = Timeline.GetActorLocationAtTime(timeline.actor[i], timelineSlider.minValue);
            timeline.actor[i].position = new Vector3(location.x, location.y + 1, location.z);
            agent[i].enabled = true;
            agent[i].Warp(new Vector3(location.x, location.y + 1, location.z));


            agent[i].nextPosition = new Vector3(location.x, location.y + 1, location.z);
            agent[i].enabled = false;

            //agent[i].transform.position = Timeline.GetActorLocationAtTime(timeline.actor[i], timelineSlider.minValue);
            Timeline.ClearActorLists(timeline.actor[i]);
            UpdateTimelineParts();

        }

        for (int i = 0; i < dartObjects.Count; i++)
        {
            dartObjects[i].transform.position = dartwallObjects[0].transform.position;
        }

        SetTimelineCurrentValue(timelineSlider.minValue);

        //SimulateRestOfTimeline();
        simualting = false;
        simulatingButtonText.text ="Simulate";

    }
    public void SetTimeFromActionList(int part,int section)
    {



        SetTimelineCurrentValue((part == 0 ? timeline.actor[selectedActor].actionList[section].startTime : timeline.actor[selectedActor].actionList[section].endTime));


    }
    public void StopActions()
    {
        walking = false;
        jumping = false;
        UpdateTempLineRenderer(Timeline.GetActorLocationAtTime(timeline.actor[selectedActor],timelineSlider.value));
    }
}
