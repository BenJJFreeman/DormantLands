using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Timeline {

	public float length;
	public Actor[] actor;
	public MovableObject[] movableObject;
    public DartWall[] dartWalls;
    //public PressurePlate[] pressurePlates;
    public Zipline[] ziplines;
    //public MovableObject[] movableObjects;
    public SpikeTrap[] spikeTraps;    
    public UnstableFloor[] unstableFloors;
    public MovingPlatformSet[] movingPlatformSets;

    public EndArea[] endArea;
    public LayerMask layerMask;

    public void Init(float _length,Actor[] _actor,MovableObject[] _movableObjects,DartWall[] _dartWalls,Zipline[] _zipline,SpikeTrap[] _spikeTraps,UnstableFloor[] _unstableFloors,MovingPlatformSet[] _movingPlatformSets,EndArea[] _endAreas)
    {
        length = _length;
        actor = _actor;
        movableObject = _movableObjects;
        dartWalls = _dartWalls;
        ziplines = _zipline;
        spikeTraps = _spikeTraps;
        unstableFloors = _unstableFloors;
        movingPlatformSets = _movingPlatformSets;
        endArea = _endAreas;
    }
	public void SetUpActor (int _actor){

		actor [_actor] = new Actor ();

	}
    public static void UpdateActor(Actor _actor, float time,Vector3 endPos)
    {

        int section = GetActionListSection(_actor, time);
        ActionType currentAction = ActionType.walk;
        if (section >= 0)
        {
            currentAction = _actor.actionList[section].actionType;
        }

        switch (currentAction)
        {
            case ActionType.walk:
                int move = GetMovementListSection(_actor, time);

                if (move >= 0)
                {
                    NavMeshPath newPath = new NavMeshPath();
                    NavMesh.CalculatePath(_actor.movementList[move].path.corners[0], endPos, NavMesh.AllAreas, newPath);
                    _actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, time, ActionType.walk);
                    _actor.movementList[move] = new Movement(_actor.actionList[section].startTime, time, newPath);
                }

                break;

            case ActionType.jump:
                //int jump = GetJumpListSection(_actor, time);

                //if (jump >= 0)
                //{
                //    time = _actor.jumpList[jump].endTime;
                //    endPos = _actor.jumpList[jump].landPoint;

                //    NavMeshPath tempPath = new NavMeshPath();
                //    NavMesh.CalculatePath(endPos, endPos, NavMesh.AllAreas, tempPath);
                //    path = tempPath;
                //}

                break;
            case ActionType.idle:
                int idle = GetIdleListSection(_actor, time);

                if (idle >= 0)
                {
                    //NavMeshPath newPath = new NavMeshPath();
                    //NavMesh.CalculatePath(_actor.idleList[idle].point, pos, NavMesh.AllAreas, newPath);
                    _actor.actionList.RemoveAt(section);
                    //_actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, time, ActionType.walk);
                    _actor.idleList.RemoveAt(idle);
                    //_actor.idleList[idle] = new Idle(_actor.actionList[section].startTime, time, pos);
                }


                break;
        }





       // //Action newAction = new Action(time, time + GetPathTime(path), ActionType.walk);
        ////_actor.actionList.Add(newAction);
        _actor.SortActionList();
        _actor.actionList.RemoveRange(GetActionListSection(_actor,time) + 1, _actor.actionList.Count - GetActionListSection(_actor, time) - 1);

        ////Movement newMovement = new Movement(time, time + GetPathTime(path), path);
        // //_actor.movementList.Add(newMovement);
        // _actor.SortMovementList();
        // _actor.movementList.RemoveRange(GetMovementListSection(_actor,time)  + 1, _actor.movementList.Count - GetMovementListSection(_actor, time) - 1);

        _actor.lastPos = endPos;
        // int jumpSection = GetJumpListSection(_actor, time);
        // _actor.SortJumpList();
        //_actor.jumpList.RemoveRange(jumpSection + 1, _actor.jumpList.Count - jumpSection - 1);


        UpdateActorLists(_actor, time);


    }
    public static bool UpdateActor(Actor _actor, float time, Vector3 pos, Vector3 endPos, NavMeshPath path) {


        int section = GetActionListSection(_actor, time);
        ActionType currentAction = ActionType.walk;
        if (section >= 0)
        {
            currentAction = _actor.actionList[section].actionType;
        }

        switch (currentAction)
        {
            case ActionType.walk:
                int move = GetMovementListSection(_actor, time);

                if (move >= 0)
                {
                    NavMeshPath newPath = new NavMeshPath();
                    NavMesh.CalculatePath(_actor.movementList[move].path.corners[0], pos, NavMesh.AllAreas, newPath);
                    _actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, time, ActionType.walk);
                    _actor.movementList[move] = new Movement(_actor.actionList[section].startTime, time, newPath);
                }

                break;

            case ActionType.jump:
                int jump = GetJumpListSection(_actor, time);

                if (jump >= 0)
                {
                    if (time > _actor.jumpList[jump].startTime)
                    {
                        if (time > _actor.jumpList[jump].startTime)
                        {
                            time = _actor.jumpList[jump].endTime;
                            pos = _actor.jumpList[jump].landPoint;

                            NavMeshPath tempPath = new NavMeshPath();
                            NavMesh.CalculatePath(pos, endPos, NavMesh.AllAreas, tempPath);
                            path = tempPath;
                        }
                        else
                        {
                            _actor.jumpList.RemoveAt(jump);

                        }
                    }
                }

                break;

            case ActionType.falling:
                int fall = GetFallListSection(_actor, time);

                if (fall >= 0)
                {
                    time = _actor.fallList[fall].endTime;
                    pos = _actor.fallList[fall].point[1];

                    NavMeshPath tempPath = new NavMeshPath();
                    NavMesh.CalculatePath(pos, endPos, NavMesh.AllAreas, tempPath);
                    path = tempPath;
                }

                break;

            case ActionType.idle:
                int idle = GetIdleListSection(_actor, time);

                if (idle >= 0)
                {
                    //NavMeshPath newPath = new NavMeshPath();
                    //NavMesh.CalculatePath(_actor.idleList[idle].point, pos, NavMesh.AllAreas, newPath);
                    _actor.actionList.RemoveAt(section);
                    //_actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, time, ActionType.walk);
                    _actor.idleList.RemoveAt(idle);
                    //_actor.idleList[idle] = new Idle(_actor.actionList[section].startTime, time, pos);
                }


                break;

            case ActionType.death:
                return false;

        }

        



        ActionBase newAction = new ActionBase(time, time + GetPathTime(path), ActionType.walk);
        _actor.actionList.Add(newAction);
        _actor.SortActionList();
        _actor.actionList.RemoveRange(_actor.actionList.IndexOf(newAction) + 1, _actor.actionList.Count - _actor.actionList.IndexOf(newAction) - 1);

        Movement newMovement = new Movement(time, time + GetPathTime(path), path);
        _actor.movementList.Add(newMovement);
        //_actor.SortMovementList();
        //_actor.movementList.RemoveRange(_actor.movementList.IndexOf(newMovement) + 1, _actor.movementList.Count - _actor.movementList.IndexOf(newMovement) - 1);

        _actor.lastPos = endPos;
       // int jumpSection = GetJumpListSection(_actor, time);
        //_actor.SortJumpList();
       // _actor.jumpList.RemoveRange(jumpSection + 1, _actor.jumpList.Count - jumpSection - 1);




        UpdateActorLists(_actor, time + GetPathTime(path));


        return true;
    }
    public static bool UpdateActor(Actor _actor, float time, Vector3 _jmpPoint,Vector3 _landPoint)
    {

        int section = GetActionListSection(_actor, time);
        ActionType currentAction = ActionType.walk;
        if (section >= 0)
        {
            currentAction = _actor.actionList[section].actionType;
        }

        switch (currentAction)
        {
            case ActionType.walk:
                int move = GetMovementListSection(_actor, time);

                if (move >= 0)
                {
                    NavMeshPath newPath = new NavMeshPath();
                    NavMesh.CalculatePath(_actor.movementList[move].path.corners[0], _jmpPoint, NavMesh.AllAreas, newPath);
                    _actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, time, ActionType.walk);
                    _actor.movementList[move] = new Movement(_actor.actionList[section].startTime, time, newPath);
                }

                break;

            case ActionType.jump:
                int jump = GetJumpListSection(_actor, time);

                if (jump >= 0)
                {
                    if (time > _actor.jumpList[jump].startTime)
                    {
                        time = _actor.jumpList[jump].endTime;
                        _jmpPoint = _actor.jumpList[jump].landPoint;

                    }else
                    {
                        _actor.jumpList.RemoveAt(jump);
                        //time = _actor.jumpList[jump].endTime;
                        //_jmpPoint = _actor.jumpList[jump].landPoint;

                    }
                }

                break;
            case ActionType.falling:
                int fall = GetFallListSection(_actor, time);

                if (fall >= 0)
                {
                    if (time > _actor.fallList[fall].startTime)
                    {
                        time = _actor.fallList[fall].endTime;
                        _jmpPoint = _actor.fallList[fall].point[1];

                    }
                    else
                    {
                        //_actor.fallList.RemoveAt(fall);
                        //time = _actor.jumpList[jump].endTime;
                        //_jmpPoint = _actor.jumpList[jump].landPoint;

                    }
                }
                break;
            case ActionType.idle:
                int idle = GetIdleListSection(_actor, time);

                if (idle >= 0)
                {
                    //NavMeshPath newPath = new NavMeshPath();
                    //NavMesh.CalculatePath(_actor.idleList[idle].point, pos, NavMesh.AllAreas, newPath);
                    _actor.actionList.RemoveAt(section);
                    //_actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, time, ActionType.walk);
                    _actor.idleList.RemoveAt(idle);
                    //_actor.idleList[idle] = new Idle(_actor.actionList[section].startTime, time, pos);
                }


                break;
            case ActionType.death:
                return false;
        }


       


        float jumpTime = Vector3.Distance(_jmpPoint, _landPoint) / 6;


        ActionBase newAction = new ActionBase(time, time + jumpTime, ActionType.jump);
        _actor.actionList.Add(newAction);
        _actor.SortActionList();
        _actor.actionList.RemoveRange(_actor.actionList.IndexOf(newAction) + 1, _actor.actionList.Count - _actor.actionList.IndexOf(newAction) - 1);

        Vector3 center = (_jmpPoint + _landPoint) * 0.5F;
        center -= new Vector3(0, 1, 0);


        Jump newJump = new Jump(time, time + jumpTime,_jmpPoint, center, _landPoint);
        _actor.jumpList.Add(newJump);
        //_actor.SortJumpList();
        //_actor.jumpList.RemoveRange(_actor.jumpList.IndexOf(newJump) + 1, _actor.jumpList.Count - _actor.jumpList.IndexOf(newJump) - 1);

        //int moveSection = GetMovementListSection(_actor, time);
        //_actor.SortMovementList();
        // _actor.movementList.RemoveRange(moveSection + 1, _actor.movementList.Count - moveSection - 1);
        _actor.lastPos = _landPoint;


        UpdateActorLists(_actor, time + jumpTime);

        return true;

    }
    public static void UpdateActor(Actor _actor,Timeline _timeline, Vector3 _point,float _time)
    {
        float t = _time;




        int section = GetActionListSection(_actor, t);
        ActionType currentAction = ActionType.walk;
        if (section >= 0)
        {
            currentAction = _actor.actionList[section].actionType;
        }

        switch (currentAction)
        {
            case ActionType.walk:
                int move = GetMovementListSection(_actor, t);

                if (move >= 0)
                {
                    NavMeshPath newPath = new NavMeshPath();
                    NavMesh.CalculatePath(_actor.movementList[move].path.corners[0], GetActorLocationAtTime(_actor,t), NavMesh.AllAreas, newPath);
                    _actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, t, ActionType.walk);
                    _actor.movementList[move] = new Movement(_actor.actionList[section].startTime, t, newPath);
                }

                break;
            case ActionType.idle:
                int idle = GetIdleListSection(_actor, t);

                if (idle >= 0)
                {
                    //NavMeshPath newPath = new NavMeshPath();
                    //NavMesh.CalculatePath(_actor.idleList[idle].point, pos, NavMesh.AllAreas, newPath);
                    _actor.actionList.RemoveAt(section);
                    //_actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, time, ActionType.walk);
                    _actor.idleList.RemoveAt(idle);
                    //_actor.idleList[idle] = new Idle(_actor.actionList[section].startTime, time, pos);
                }


                break;
            case ActionType.death:
                return;
        }





        float startTime = t;
        float endTime = startTime;
        Vector3[] point = new Vector3[2];
        point[0] = GetActorLocationAtTime(_actor, t);
       // while (t < _timeline.length)
        //{
            //Vector3 loc = GetActorLocationAtTime(_actor, t);
            //Ray collisionRay = new Ray(loc, Vector3.down);
            //RaycastHit hit;
            //Physics.Raycast(collisionRay, out hit, 100);

        point[1] = _point;

        float dis = Vector3.Distance(point[0], point[1]);


        //if (Vector3.Distance(hit.point, loc) > 0.4f)
        //{

        endTime += dis / 8;


            //}

          //  t += 1 * Time.deltaTime;
        //}






        ActionBase newAction = new ActionBase(startTime, endTime, ActionType.falling);
        _actor.actionList.Add(newAction);
        _actor.SortActionList();
        _actor.actionList.RemoveRange(_actor.actionList.IndexOf(newAction) + 1, _actor.actionList.Count - _actor.actionList.IndexOf(newAction) - 1);

        

        Fall newFall = new Fall(startTime, endTime,point);
        _actor.fallList.Add(newFall);

        _actor.lastPos = _point;

        UpdateActorLists(_actor, endTime);


       

        // _actor.SortFallList();
        // _actor.fallList.RemoveRange(_actor.fallList.IndexOf(newFall) + 1, _actor.fallList.Count - _actor.fallList.IndexOf(newFall) - 1);

        /////int fallSection = GetFallListSection(_actor, startTime);
        //// //_actor.SortFallList();
        /////_actor.fallList.RemoveRange(fallSection + 1, _actor.fallList.Count - fallSection - 1);


        //int moveSection = GetMovementListSection(_actor, endTime);
        //_actor.SortMovementList();
        //_actor.movementList.RemoveRange(moveSection + 1, _actor.movementList.Count - moveSection - 1);


    }
    public static void UpdateActorLists(Actor _actor, float _time)
    {

        //_actor.SortActionList();

        //for (int i = 0; i < _actor.actionList.Count; i++)
        //{

        //    if (_actor.actionList[i].startTime >= _time)
        //    {

        //        _actor.actionList.RemoveRange(i, _actor.actionList.Count - i - 1);
        //        break;
        //    }


        //}

        int numberOfMovements = -1;
        int numberOfJumps = -1;
        int numberOfFalls = -1;
        int numberOfIdles = -1;
        for (int i = 0; i < _actor.actionList.Count; i++)
        {
            switch (_actor.actionList[i].actionType)
            {
                case ActionType.walk:
                    numberOfMovements++;
                    break;

                case ActionType.jump:
                    numberOfJumps++;
                    break;


                case ActionType.falling:
                    numberOfFalls++;
                    break;

                case ActionType.idle:
                    numberOfIdles++;
                    break;
                default:

                    break;
            }
        }


        _actor.SortMovementList();
        _actor.movementList.RemoveRange(numberOfMovements +1, _actor.movementList.Count - numberOfMovements -1);



        _actor.SortJumpList();
        _actor.jumpList.RemoveRange(numberOfJumps+1, _actor.jumpList.Count - numberOfJumps-1);



        _actor.SortFallList();
        _actor.fallList.RemoveRange(numberOfFalls+1, _actor.fallList.Count - numberOfFalls-1);


        _actor.SortIdleList();
       _actor.idleList.RemoveRange(numberOfIdles + 1, _actor.idleList.Count - numberOfIdles - 1);

        // AddIdleSections(_actor, _timeline.length);


    }
    public static void AddIdleSections(Actor _actor,float _timelineLength)
    {
        //_actor.idleList.Clear();

        int count = _actor.actionList.Count;
        //count -= 1;

        List<ActionBase> tempList = new List<ActionBase>();

        //Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            if (i == 0)
            {

                if (_actor.actionList[i].startTime > 0)
                {
                    tempList.Add(new ActionBase(0, _actor.actionList[i].startTime, ActionType.idle));
                    _actor.idleList.Add(new Idle(0, _actor.actionList[i].startTime, _actor.startingPos));
                    //Debug.Log("add");
                }

            }
            else
            {
                if(_actor.actionList[i].startTime > _actor.actionList[i-1].endTime)
                {
                    tempList.Add(new ActionBase(_actor.actionList[i-1].endTime, _actor.actionList[i].startTime, ActionType.idle));
                    _actor.idleList.Add(new Idle(_actor.actionList[i-1].endTime, _actor.actionList[i].startTime, GetActorLocationAtTime(_actor, _actor.actionList[i].startTime)));
                   // Debug.Log("add");
                }
            }

        }
        count -= 1;
        if (count >= 0)
        {
            _actor.actionList.Add(new ActionBase(_actor.actionList[count].endTime, _timelineLength, ActionType.idle));
            _actor.idleList.Add(new Idle(_actor.actionList[count].endTime, _timelineLength, _actor.lastPos));
        }
        else
        {
            _actor.actionList.Add(new ActionBase(0, _timelineLength, ActionType.idle));
            _actor.idleList.Add(new Idle(0, _timelineLength, _actor.startingPos));
        }


        for (int i = 0; i < tempList.Count; i++)
        {
            _actor.actionList.Add(tempList[i]);

        }

            _actor.SortActionList();
        _actor.SortIdleList();

    }
    //public static void ActorOnGround (Actor _actor, Timeline _timeline)
    //{
    //    float t = 0;


    //    while(t < _timeline.length)
    //    {

    //        Vector3 loc = GetActorLocationAtTime(_actor, t);
    //        Ray collisionRay = new Ray(loc, Vector3.down);
    //        RaycastHit hit;
    //        Physics.Raycast(collisionRay, out hit, 100);

    //        Vector3 firstHit = hit.point;

    //        if (Vector3.Distance(firstHit, loc) > 1.5f)
    //        {

    //            if (hit.transform.tag == "Platform")
    //            {
    //                float tempT = t;
    //                while (tempT < _timeline.length)
    //                {
    //                    tempT = t;
    //                    for (int s = 0; s < _timeline.movingPlatformSets.Length; s++)
    //                    {
    //                        for (int p = 0; p < _timeline.movingPlatformSets[s].movingPlatforms.Length; p++)
    //                        {
    //                            if (PointWithinBounds(new Bounds(GetMovingPlatformAtTime(_timeline.movingPlatformSets[s].movingPlatforms[p], tempT), _timeline.movingPlatformSets[s].movingPlatforms[p].size), loc + Vector3.down))
    //                            {
    //                                // // platform hit
    //                                Debug.Log("hit box");
    //                            }
    //                            else
    //                            {

    //                                // no platform
    //                                Physics.Raycast(collisionRay, out hit, 100, _timeline.layerMask);

    //                                if (Vector3.Distance(firstHit, loc) <= Vector3.Distance(hit.point, loc))
    //                                {
    //                                    UpdateActor(_actor, _timeline, hit.point, t);
    //                                    Debug.Log("Falling");
    //                                    break;


    //                                }

    //                            }
    //                        }
    //                        tempT += 1 * Time.deltaTime;
    //                    }
    //                }

    //            }
    //            else
    //            {
    //                //if (GetActionAtTime(_actor, t) != ActionType.jump && GetActionAtTime(_actor, t) != ActionType.falling)
    //                //{
    //                    //point[1] = hit.point;
    //                    UpdateActor(_actor, _timeline,firstHit, t);
    //                    Debug.Log("Falling");
    //                    break;
    //                //}
    //            }
    //        }

    //        t += 1 * Time.deltaTime;
    //    }  
    //}
    public static bool CheckActorIsAlive(Actor _actor)
    {

        for (int i = 0; i < _actor.lifeList.Count; i++)
        {
            if(_actor.lifeList[1].alive == false)
            {
                float _time = _actor.lifeList[1].startTime;
                int section = GetActionListSection(_actor, _time);

                ActionType currentAction = ActionType.walk;
                if (section >= 0)
                {
                    currentAction = _actor.actionList[section].actionType;
                }


                switch (currentAction)
                {
                    case ActionType.walk:                       

                        int move = GetMovementListSection(_actor, _time);

                        if (move >= 0)
                        {
                            NavMeshPath newPath = new NavMeshPath();
                            NavMesh.CalculatePath(_actor.movementList[move].path.corners[0], _actor.lifeList[1].point, NavMesh.AllAreas, newPath);
                            _actor.actionList[section] = new ActionBase(_actor.actionList[section].startTime, _time, ActionType.walk);
                            _actor.movementList[move] = new Movement(_actor.actionList[section].startTime, _time, newPath);
                        }

                        break;                                      
                }


                return false;

            }
        }
        return true;
    }
    public static void CheckActorIsOnGround(Actor _actor, Timeline _timeline,float _time)
    {
        //
        float t = _time;
        //
        while (t < _timeline.length)
        {

            Vector3 location = GetActorLocationAtTime(_actor, t);
            location += Vector3.up;

            Ray collisionRay = new Ray(location, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(collisionRay, out hit, 100);

            Vector3 firstHit = hit.point;
            //Transform tr = hit.transform;

            ActionType actionType = GetActionAtTime(_actor, t);
            int section = GetActionListSection(_actor, t);

            if (Vector3.Distance(firstHit, location) > 2.5f)
            {
                if (actionType == ActionType.jump)
                {
                    if (t >= _actor.actionList[section].endTime)
                    {
                        Debug.Log("Jump end");
                        UpdateActor(_actor, _timeline, firstHit, t);
                        break;
                    }
                }
                else if (actionType != ActionType.falling)
                {
                    Debug.Log("walking ");
                    UpdateActor(_actor, _timeline, firstHit, t);
                    break;
                }
                else
                {
                    if (t >= _actor.actionList[section].endTime)
                    {
                        Debug.Log("falling end");
                        UpdateActor(_actor, _timeline, firstHit, t);
                        break;
                    }

                }


            }
            else
            {

               // float tempT = t;

               // while (tempT < _timeline.length)
//{
                    //for (int s = 0; s < _timeline.movingPlatformSets.Length; s++)
                    //{
                    //    for (int p = 0; p < _timeline.movingPlatformSets[s].movingPlatforms.Length; p++)
                    //    {
                    //        if (PointWithinBounds(new Bounds(GetMovingPlatformAtTime(_timeline.movingPlatformSets[s].movingPlatforms[p], t), _timeline.movingPlatformSets[s].movingPlatforms[p].size), location + Vector3.down))
                    //        {
                    //            // // platform hit
                    //            Debug.Log("hit box");

                            
                    //        }
                    //        else
                    //        {
                    //            break;
                    //        }

                    //    }
                    //}
                   // tempT += 1 * Time.deltaTime;
                //}







                Physics.Raycast(collisionRay, out hit, 100, _timeline.layerMask);

                if (Vector3.Distance(firstHit, location) < Vector3.Distance(hit.point, location))
                {

                    Debug.Log("ignore raycast");

                    if (actionType == ActionType.jump)
                    {
                        if (t >= _actor.actionList[section].endTime)
                        {
                            Debug.Log("Jump end");
                            UpdateActor(_actor, _timeline, hit.point, t);
                            break;
                        }
                    }
                    else if (actionType != ActionType.falling)
                    {
                        Debug.Log("walking ");
                        UpdateActor(_actor, _timeline, hit.point, t);
                        break;
                    }
                    else
                    {
                        if (t >= _actor.actionList[section].endTime)
                        {
                            Debug.Log("falling end");
                            UpdateActor(_actor, _timeline, hit.point, t);
                            break;
                        }

                    }

                }
            }



            




            t += 1 * Time.deltaTime;
        }
    }
    public static Vector3 GetActorLocationAtTime(Actor _actor,float _time){

		int section = GetActionListSection (_actor, _time);

        ActionType currentAction = ActionType.walk;
        if (section >= 0)
        {
            currentAction = _actor.actionList[section].actionType;
        }else
        {

            return new Vector3(_actor.startingPos.x, _actor.startingPos.y - 1, _actor.startingPos.z);
        }


        switch (currentAction)
        {
            case ActionType.walk:
                 return GetActorLocationOnMovementList(_actor, _time, GetMovementListSection(_actor, _time));
               

            case ActionType.jump:   
                return GetActorLocationOnJumpList(_actor, _time, GetJumpListSection(_actor, _time));

            case ActionType.falling:
                return GetActorOnFallList(_actor, _time, GetFallListSection(_actor, _time));

            case ActionType.idle:
                return GetActorOnIdleList(_actor, _time,GetIdleListSection(_actor, _time));
            case ActionType.death:
                return GetActorOnLifeList(_actor, _time, GetLifeListSection(_actor, _time));
        }



        return new Vector3(_actor.position.x, _actor.position.y - 1, _actor.position.z);


    }
    public static Vector3 GetActorLocationOnMovementList (Actor _actor,float _time,int movementSection)
    {
        if (movementSection < 0)
        {
            return new Vector3(_actor.startingPos.x, _actor.startingPos.y - 1, _actor.startingPos.z);
        }

        Vector3[] corners = _actor.movementList[movementSection].path.corners;

        float startTime = _actor.movementList[movementSection].startTime;
        float endTime = _actor.movementList[movementSection].endTime;

        if (_time > endTime)
        {
            return corners[corners.Length - 1];
        }

        int section = GetPathSection(_actor.movementList[movementSection].path, startTime, _time);

        float sectionTime = 0;
        float sectionDistance = 0;
        if (corners.Length > 1)
        {
            sectionDistance = Vector3.Distance(corners[0], corners[1]);
        }
        //int sectionTemp = section;
        for (int i = 0; i < section; i++)
        {

            sectionTime += Vector3.Distance(corners[i], corners[i + 1]) / 8;

        }
        if (section < corners.Length - 1)
        {
            sectionDistance = Vector3.Distance(corners[section], corners[section + 1]);
        }
        else
        {
            sectionDistance = Vector3.Distance(corners[section], corners[section]);
        }

        float distCovered = (_time - (startTime + sectionTime)) * 8;
        float fracJourney = distCovered / sectionDistance;



        if (section < corners.Length - 1)
        {
            return Vector3.Lerp(corners[section], corners[section + 1], fracJourney);
        }
        else
        {
            return Vector3.Lerp(corners[section], corners[section], fracJourney);
        }
    }
    public static Vector3 GetActorLocationOnJumpList(Actor _actor, float _time, int jumpSection)
    {
        if (jumpSection < 0)
        {
            return new Vector3(_actor.position.x, _actor.position.y - 1, _actor.position.z);
        }

        float startTime = _actor.jumpList[jumpSection].startTime;
        float sectionTime = Vector3.Distance(_actor.jumpList[jumpSection].jumpPoint, _actor.jumpList[jumpSection].landPoint) / 8;

        Vector3 center = (_actor.jumpList[jumpSection].jumpPoint + _actor.jumpList[jumpSection].landPoint) * 0.5F;
        center -= new Vector3(0, 1, 0);

        Vector3 riseRelCenter = _actor.jumpList[jumpSection].jumpPoint - center;
        Vector3 setRelCenter = _actor.jumpList[jumpSection].landPoint - center;

        float fracComplete = (_time - startTime) / sectionTime;

        Vector3 pos = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        pos += center;

        //transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        //transform.position += center;


        return pos;
    }
    public static Vector3 GetActorOnFallList(Actor _actor,float _time,int fallSection)
    {

        float startTime = _actor.fallList[fallSection].startTime;

        float distCovered = (_time - startTime) * 10;
        float fracJourney = distCovered / Vector3.Distance(_actor.fallList[fallSection].point[0], _actor.fallList[fallSection].point[1]);

        return Vector3.Lerp(_actor.fallList[fallSection].point[0], _actor.fallList[fallSection].point[1], fracJourney);

    }
    public static Vector3 GetActorOnIdleList(Actor _actor,float _time,int idleSection)
    {     
        return _actor.idleList[idleSection].point;
    }
    public static Vector3 GetActorOnLifeList(Actor _actor, float _time, int lifeSection)
    {
        if (lifeSection >= 0)
        {
            return _actor.lifeList[lifeSection].point;
        }
        else
        {
            return _actor.startingPos;

        }
    }
    public static int GetActionListSection(Actor _actor, float time){

		if (_actor.actionList.Count == 0) {

			return -1;
		}

		int section = 0;


		for (int i = 0; i < _actor.actionList.Count; i++) {

			if (time >= _actor.actionList [i].startTime && time <= _actor.actionList [i].endTime) {
				return i;
			}
			if (time < _actor.actionList [i].startTime) {
				return i;

			}
			if (time > _actor.actionList [i].endTime) {
				section = i;

			}

		}


		return section;
	}

	public static int GetPathSection (NavMeshPath path ,float startTime,float time){

		float newTime = startTime;


		for (int i = 0; i < path.corners.Length - 1; i++) {
			newTime += Vector3.Distance (path.corners [i], path.corners [i + 1]) / 8;
			if (time <= newTime) {
				return i;
			}
		}

		return path.corners.Length -1;
	}

	public static float GetPathTime(NavMeshPath path){
		float time = 0;
		float length = GetPathLength (path);
		float speed = 8;

		time = length / speed;


		return time;
	}
	public static float GetPathLength( NavMeshPath path ){
		float lng = 0.0f;

		if ((path.status != NavMeshPathStatus.PathInvalid)) {
			for (int i = 0; i < path.corners.Length - 1; i++) {
				lng += Vector3.Distance (path.corners [i], path.corners [i + 1]);
			}
		}
		return lng;
	}
	public static bool CheckTimeline(Timeline _timeLine,MovableObject _mo,GameObject _block){

		float t = 0;

		while (t < _timeLine.length) {

			for (int i = 0; i < _timeLine.actor.Length; i++) {

				for (int j = 0; j < _timeLine.actor[i].actionList.Count; j++) {


					_block.transform.position = MovableObject.GetObjectPositionAtTime (_mo, t);

					NavMeshPath tempPath = new NavMeshPath ();

					Vector3[] originalCorners = _timeLine.actor [i].movementList[j].path.corners;

					NavMesh.CalculatePath (originalCorners [0], originalCorners[originalCorners.Length-1], NavMesh.AllAreas, tempPath);

					Vector3[] tempCorners = tempPath.corners;

					if (tempCorners != originalCorners) {
						_timeLine.actor [i].movementList [j].path = tempPath; 
						return false;
					}


					tempPath.ClearCorners ();
				}



			}

			t += 1 * Time.deltaTime;
		}

		return true;

	}
    public static ActionType GetActionAtTime(Actor _actor,float _time)
    {
        ActionType at = ActionType.walk;


        if (_actor.actionList.Count == 0)
        {

            return ActionType.walk;
        }

        for (int i = 0; i < _actor.actionList.Count; i++)
        {

            if (_time >= _actor.actionList[i].startTime && _time <= _actor.actionList[i].endTime)
            {
                return _actor.actionList[i].actionType;
            }
            if (_time < _actor.actionList[i].startTime)
            {
                return _actor.actionList[i].actionType;

            }
            if (_time > _actor.actionList[i].endTime)
            {
                at = _actor.actionList[i].actionType;
            }

        }
        at = ActionType.idle;
        return at;
    }
    public static int GetMovementListSection (Actor _actor, float _time)
    {
        if (_actor.movementList.Count == 0)
        {

            return -1;
        }

        int section = 0;


        for (int i = 0; i < _actor.movementList.Count; i++)
        {

            if (_time >= _actor.movementList[i].startTime && _time <= _actor.movementList[i].endTime)
            {
                return i;
            }
            if (_time < _actor.movementList[i].startTime)
            {
                return i;

            }
            if (_time > _actor.movementList[i].endTime)
            {
                section = i;

            }

        }


        return section;
    }
    public static int GetJumpListSection(Actor _actor, float _time)
    {
        if (_actor.jumpList.Count == 0)
        {

            return -1;
        }

        int section = 0;


        for (int i = 0; i < _actor.jumpList.Count; i++)
        {

            if (_time >= _actor.jumpList[i].startTime && _time <= _actor.jumpList[i].endTime)
            {
                return i;
            }
            if (_time < _actor.jumpList[i].startTime)
            {
                return i;

            }
            if (_time > _actor.jumpList[i].endTime)
            {
                section = i;

            }

        }


        return section;
    }
    public static int GetFallListSection(Actor _actor, float _time)
    {
        if (_actor.fallList.Count == 0)
        {

            return -1;
        }

        int section = 0;


        for (int i = 0; i < _actor.fallList.Count; i++)
        {

            if (_time >= _actor.fallList[i].startTime && _time <= _actor.fallList[i].endTime)
            {
                return i;
            }
            if (_time < _actor.fallList[i].startTime)
            {
                return i;

            }
            if (_time > _actor.fallList[i].endTime)
            {
                section = i;

            }

        }


        return section;
    }
    public static int GetIdleListSection(Actor _actor, float _time)
    {
        if (_actor.idleList.Count == 0)
        {

            return -1;
        }

        int section = 0;


        for (int i = 0; i < _actor.idleList.Count; i++)
        {

            if (_time >= _actor.idleList[i].startTime && _time <= _actor.idleList[i].endTime)
            {
                return i;
            }
            if (_time < _actor.idleList[i].startTime)
            {
                return i;

            }
            if (_time > _actor.idleList[i].endTime)
            {
                section = i;

            }

        }


        return section;
    }
    public static int GetLifeListSection(Actor _actor, float _time)
    {
        if (_actor.lifeList.Count == 0)
        {

            return -1;
        }

        int section = 0;


        for (int i = 0; i < _actor.lifeList.Count; i++)
        {

            if (_time >= _actor.lifeList[i].startTime && _time <= _actor.lifeList[i].endTime)
            {
                return i;
            }
            if (_time < _actor.lifeList[i].startTime)
            {
                return i;

            }
            if (_time > _actor.lifeList[i].endTime)
            {
                section = i;

            }

        }


        return section;
    }
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }
  
    public static Dart CreateDartJourney(Timeline _timeline, Vector3[] point, DartWall _dartwall, float _time)
    {

        //Vector3[] newPoint = new Vector3[2];
        //newPoint[0] = _turret.position;
        //newPoint[1] = _turret.position + (Quaternion.Euler(GetTurretRotationAtTime(_turret, _time)) * new Vector3(0, 0, _distance));





        for (int i = 0; i < _timeline.actor.Length; i++)
        {
            float t = 0;

            Ray ray = new Ray(point[0], (_dartwall.rotation * new Vector3(0, 0, 1)));

            while (t < _timeline.length)
            {

                Vector3 actorLoc = GetActorLocationAtTime(_timeline.actor[i], t) + Vector3.up;

                float distance = Vector3.Distance(point[0], actorLoc);

                Vector3 interPoint = ray.GetPoint(distance);

                if (Vector3.Distance(interPoint, actorLoc) < 1.5f)
                {

                    Vector3[] tempPoint = new Vector3[2];
                    tempPoint[0] = point[0];
                    tempPoint[1] = interPoint;                    

                    float tempDis = Vector3.Distance(point[0], point[1]);

                    Dart tempDart = new Dart(point[0], _dartwall.rotation, _time, _time + tempDis / 16, point);

                    if(Vector3.Distance(actorLoc,GetDartPositionAtTime(tempDart, t)) < 0.75f){

                        point[1] = interPoint;
                        if (t < _timeline.actor[i].lifeList[1].startTime)
                        {

                            ActionBase newAction = new ActionBase(t, _timeline.length, ActionType.death);
                            _timeline.actor[i].actionList.Add(newAction);
                            _timeline.actor[i].SortActionList();
                            _timeline.actor[i].actionList.RemoveRange(_timeline.actor[i].actionList.IndexOf(newAction) + 1, _timeline.actor[i].actionList.Count - _timeline.actor[i].actionList.IndexOf(newAction) - 1);
                            
                            _timeline.actor[i].lifeList.Clear();
                            _timeline.actor[i].lifeList.Add(new Life(0, t, true, actorLoc - Vector3.up));
                            _timeline.actor[i].lifeList.Add(new Life(t, _timeline.length, false, actorLoc - Vector3.up));

                            UpdateActorLists(_timeline.actor[i], t);
                        }
                        break;
                    }


                   
                }                

                t += 1 * Time.deltaTime;
            }
        }


        // needs to add moving objects collision aswell

        Ray collisionRay = new Ray(point[0], (_dartwall.rotation * new Vector3(0, 0, 1)));
        RaycastHit hit;
        Physics.Raycast(collisionRay, out hit,100);

        if (Vector3.Distance(hit.point, point[0]) < Vector3.Distance(point[1], point[0]))
        {
            point[1] = hit.point;
        }

        /////



        //Quaternion rot;

        //rot = Quaternion.Euler(point[1] - point[0]);

        float dis = Vector3.Distance(point[0], point[1]);

        Dart newDart = new Dart(point[0], _dartwall.rotation, _time, _time + dis / 16, point);




        return newDart;
    }
    public static Vector3 GetDartPositionAtTime(Dart _dart,float _time)
    {


        float distCovered = (_time - _dart.startTime) * 16;
        float fracJourney = distCovered / Vector3.Distance(_dart.points[0], _dart.points[1]);

        
        return Vector3.Lerp(_dart.points[0], _dart.points[1], fracJourney);
        
    }
    public static bool PointWithinBounds (Bounds bounds,Vector3 point)
    {
        return bounds.Contains(point);
    }
    public static Vector3 GetActorLocationOnZipline(Actor _actor,Zipline _zipline,float _time)
    {           

        float startTime = _actor.actionList[GetActionListSection(_actor, _time)].startTime;


        float distCovered = (_time - startTime) * 10;
        float fracJourney = distCovered / Vector3.Distance(_zipline.point[0], _zipline.point[1]);
        

        return Vector3.Lerp(_zipline.point[0], _zipline.point[1], fracJourney);

    }
    public static bool PathCollisionCheck(MovableObject _movableObject, Actor _actor,Timeline _timeline)
    {

        float t = 0;
        while (t < _timeline.length)
        {

            Vector3 point = GetActorLocationAtTime(_actor, t);


            if(PointWithinBounds(new Bounds(MovableObject.GetObjectPositionAtTime(_movableObject,t),_movableObject.size), point))
            {

                point = GetActorLocationAtTime(_actor, t - 1 * Time.deltaTime);

                if (PointWithinBounds(new Bounds(MovableObject.GetObjectPositionAtTime(_movableObject, t), _movableObject.size), point))
                {

                    Debug.Log("Crush");

                    if (t < _actor.lifeList[1].startTime)
                    {

                        ActionBase newAction = new ActionBase(t, _timeline.length, ActionType.death);
                        _actor.actionList.Add(newAction);
                        _actor.SortActionList();
                        _actor.actionList.RemoveRange(_actor.actionList.IndexOf(newAction) + 1, _actor.actionList.Count - _actor.actionList.IndexOf(newAction) - 1);



                        _actor.lifeList.Clear();
                        _actor.lifeList.Add(new Life(0, t, true,point));
                        _actor.lifeList.Add(new Life(t, _timeline.length, false, point));
                    }
                    return true;
                }else
                {

                    UpdateActor(_actor, t - 5 * Time.deltaTime, point);
                    Debug.Log("Path Obstruction");
                    return true;
                }
            }


            t += 1 * Time.deltaTime;
        }


        


        return false;
    }
    
    public static Vector3 GetMovingPlatformAtTime(MovingPlatform _movingPlatform,float _time)
    {
        float startTime = 0;

        float distCovered = (_time - startTime) * 10;
        float fracJourney = distCovered / Vector3.Distance(_movingPlatform.point[0], _movingPlatform.point[1]);

        return Vector3.Lerp(_movingPlatform.point[0], _movingPlatform.point[1], fracJourney);
    }
    public static Vector3 GetSpikeTrapAtTime(SpikeTrap _SpikeTrap, float _time)
    {


        float startTime = 0;
        int section = -1;
        float distCovered = 0;
        float fracJourney = 0;
        for (int i = 0; i < _SpikeTrap.eventTimeline.Count; i++)
        {

            if (_SpikeTrap.eventTimeline[i].startTime <= _time && _SpikeTrap.eventTimeline[i].endTime >= _time)
            {
                startTime = _SpikeTrap.eventTimeline[i].startTime;
                section = i;
                break;
            }

        }

        if(section == -1)
        {
            return _SpikeTrap.centre;

        }

        if (_SpikeTrap.eventTimeline[section].active)
        {
            distCovered = (_time - startTime) * 20;
            fracJourney = distCovered / Vector3.Distance(_SpikeTrap.eventTimeline[section].point[0], _SpikeTrap.eventTimeline[section].point[1]);
        }
        else
        {
            distCovered = (_time - startTime) * 20;
            fracJourney = distCovered / Vector3.Distance(_SpikeTrap.eventTimeline[section].point[0], _SpikeTrap.eventTimeline[section].point[1]);

            //fracJourney = InverseLerp(_movingPlatform.eventTimeline[section].point[1], _movingPlatform.eventTimeline[section].point[0], Vector3.Lerp(_movingPlatform.eventTimeline[section].point[0], _movingPlatform.eventTimeline[section].point[1], fracJourney));


        }

        return Vector3.Lerp(_SpikeTrap.eventTimeline[section].point[0], _SpikeTrap.eventTimeline[section].point[1], fracJourney);


    }
    public static void UpdateSpikeTrap(SpikeTrap _spikeTrap, Timeline _timeline)
    {

        UpdatePressurePlate(_spikeTrap.pressurePlate, _timeline);



        float addedT = 0.25f;
        _spikeTrap.eventTimeline = new List<MovingEvent>();

        for (int e = 0; e < _spikeTrap.pressurePlate.eventTimeline.Count; e++)
        {
            ActivatableEvent ae = new ActivatableEvent(_spikeTrap.pressurePlate.eventTimeline[e].startTime, _spikeTrap.pressurePlate.eventTimeline[e].endTime, _spikeTrap.pressurePlate.eventTimeline[e].active);


            Vector3[] newPoint = new Vector3[2];
            newPoint[0] = _spikeTrap.centre;
            newPoint[1] = newPoint[0];

            if (e > 0)
            {
                newPoint[0] = _spikeTrap.eventTimeline[e - 1].point[1];
                ae.startTime += addedT;
            }
            else
            {
                //newPoint[0] = _movingPlatformSet.movingPlatforms[p].centre;
                if (addedT > 0)
                {
                    //_movingPlatformSet.movingPlatforms[p].eventTimeline.Add(new MovingEvent(ae.startTime, ae.startTime + addedT, ae.active, newPoint));
                    //ae.startTime += addedT;
                }
            }


            ae.endTime += addedT;

            Vector3 destinationPoint;

            float distCovered = (ae.endTime - ae.startTime) * 10;
            float fracJourney;

            if (ae.active)
            {
                fracJourney = distCovered / Vector3.Distance(newPoint[0], _spikeTrap.point[1]);
                destinationPoint = Vector3.Lerp(newPoint[0], _spikeTrap.point[1], fracJourney);
                if (e > 0)
                {
                    //  _movingPlatformSet.movingPlatforms[p].eventTimeline[e - 1].endTime += addedT;
                }
            }
            else
            {
                fracJourney = distCovered / Vector3.Distance(newPoint[0], _spikeTrap.point[0]);
                destinationPoint = Vector3.Lerp(newPoint[0], _spikeTrap.point[0], fracJourney);
            }

            

            if (destinationPoint == newPoint[0])
            {
                newPoint[1] = newPoint[0] + Vector3.up;

            }
            else
            {

                newPoint[1] = destinationPoint;
            }

            _spikeTrap.eventTimeline.Add(new MovingEvent(ae.startTime, ae.endTime, ae.active, newPoint));

        }






        _spikeTrap.hitEventTimeline.Clear();
        for (int i = 0; i < _timeline.actor.Length; i++)
        {
            float t = 0;
            float st = t;
            float et = st;
            bool a = false;
            while (t < _timeline.length)
            {
                if (a != Timeline.PointWithinBounds(new Bounds(GetSpikeTrapAtTime(_spikeTrap, t), _spikeTrap.size), Timeline.GetActorLocationAtTime(_timeline.actor[i], t)))
                {                    
                    et = t;
                    _spikeTrap.hitEventTimeline.Add(new ActivatableEvent(st, et, a));


                    Debug.Log("player Hit SpikeTrap");
                    if (t < _timeline.actor[i].lifeList[1].startTime)
                    {

                        ActionBase newAction = new ActionBase(t, _timeline.length, ActionType.death);
                        _timeline.actor[i].actionList.Add(newAction);
                        _timeline.actor[i].SortActionList();
                        _timeline.actor[i].actionList.RemoveRange(_timeline.actor[i].actionList.IndexOf(newAction) + 1, _timeline.actor[i].actionList.Count - _timeline.actor[i].actionList.IndexOf(newAction) - 1);


                        _timeline.actor[i].lifeList.Clear();
                        _timeline.actor[i].lifeList.Add(new Life(0, t, true, Timeline.GetActorLocationAtTime(_timeline.actor[i], t)));
                        _timeline.actor[i].lifeList.Add(new Life(t, _timeline.length, false, Timeline.GetActorLocationAtTime(_timeline.actor[i], t)));
                    }
                    st = t;
                    a = !a;
                }
                else
                {


                }
                t += 1 * Time.deltaTime;

            }

            if (t >= _timeline.length)
            {

                et = t;
                _spikeTrap.hitEventTimeline.Add(new ActivatableEvent(st, et, a));


                st = t;
                a = !a;

            }
        }


    }
    public static void UpdatePressurePlate(PressurePlate _pressurePlate, Timeline _timeline)
    {
        _pressurePlate.eventTimeline.Clear();
        
        float t = 0;
        float st = t;
        float et = st;
        bool a = false;
        while (t < _timeline.length)
        {
            bool tempA = false;
            for (int i = 0; i < _timeline.actor.Length; i++)
            {

                if (tempA == false)
                {
                    tempA = Timeline.PointWithinBounds(new Bounds(_pressurePlate.centre, _pressurePlate.size), Timeline.GetActorLocationAtTime(_timeline.actor[i], t));
                }

            }

            if (a != tempA)
            {
                et = t;
                _pressurePlate.eventTimeline.Add(new ActivatableEvent(st, et, a));


                st = t;
                a = !a;
            }
            else
            {


            }
            t += 1 * Time.deltaTime;
        }
        if (t >= _timeline.length)
        {
            t = _timeline.length;
            et = t;
            _pressurePlate.eventTimeline.Add(new ActivatableEvent(st, et, a));


            st = t;
            a = !a;

        }


    }
    public static void UpdateDartWall (DartWall _dartWall,Timeline _timeline)
    {
        UpdatePressurePlate(_dartWall.pressurePlate, _timeline);


        _dartWall.dartList.Clear();
        for ( int i = 0; i < _dartWall.pressurePlate.eventTimeline.Count; i++)
        {


            if (_dartWall.pressurePlate.eventTimeline[i].active)
            {

                float t = _dartWall.pressurePlate.eventTimeline[i].startTime;


                while (t < _dartWall.pressurePlate.eventTimeline[i].endTime)
                {

                    Vector3[] point = new Vector3[2];
                    point[0] = _dartWall.centre;
                    point[1] = point[0] + _dartWall.rotation * Vector3.forward * 500;

                    _dartWall.dartList.Add(CreateDartJourney(_timeline, point,_dartWall, t));

                    t += 2;
                }


            }


        }
    }
    public static void UpdateEndArea(EndArea _endArea, Timeline _timeline)
    {
        _endArea.eventTimeline.Clear();

        float t = 0;
        float st = t;
        float et = st;
        bool a = false;
        while (t < _timeline.length)
        {
            bool tempA = false;
            for (int i = 0; i < _timeline.actor.Length; i++)
            {
                if (tempA == false)
                {
                    tempA = Timeline.PointWithinBounds(new Bounds(_endArea.centre, _endArea.size), Timeline.GetActorLocationAtTime(_timeline.actor[i], t));
                }
            }


            if (a != tempA)
            {
                et = t;
                _endArea.eventTimeline.Add(new ActivatableEvent(st, et, a));
                Debug.Log("player at end Area");

                st = t;
                a = !a;
            }

            t += 1 * Time.deltaTime;

        }
        if (t >= _timeline.length)
        {

            et = t;
            _endArea.eventTimeline.Add(new ActivatableEvent(st, et, a));

            st = t;
            a = !a;

        }       

    }
    public static void UpdateUnstableFloor(UnstableFloor _unstableFloor, Timeline _timeline)
    {
        _unstableFloor.eventTimeline.Clear();

        float t = 0;
        float st = t;
        float et = st;
        bool a = false;
        while (t < _timeline.length)
        {
            bool tempA = false;
            for (int i = 0; i < _timeline.actor.Length; i++)
            {
                if(tempA == false)
                {

                    tempA = Timeline.PointWithinBounds(new Bounds(_unstableFloor.centre, _unstableFloor.size + new Vector3(0, 0.25f, 0)), Timeline.GetActorLocationAtTime(_timeline.actor[i], t));
                }                
            }

            if (a != tempA)
            {
                et = t;
                _unstableFloor.eventTimeline.Add(new ActivatableEvent(st, et, a));
                Debug.Log("player on unstable floor");

                st = t;
                a = !a;
            }

            t += 1 * Time.deltaTime;

        }
        if (t >= _timeline.length)
        {
            et = t;
            _unstableFloor.eventTimeline.Add(new ActivatableEvent(st, et, a));

            st = t;
            a = !a;

        }
    }
    public static Vector3 GetUnstableFloorPositionAtTime(UnstableFloor _unstableFloor, float _time)
    {

        float startTime = 0;

        float distCovered = 0;
        float fracJourney = 0;

        for (int i = 0; i < _unstableFloor.eventTimeline.Count; i++)
        {

            if (_unstableFloor.eventTimeline[i].active)
            {
                startTime = _unstableFloor.eventTimeline[i].startTime;

                distCovered = (_time - startTime) * 10;
                fracJourney = distCovered / Vector3.Distance(_unstableFloor.point[0], _unstableFloor.point[1]);

                break;

            }
        }
       

        

        return Vector3.Lerp(_unstableFloor.point[0], _unstableFloor.point[1], fracJourney);
    }
    public static void UpdateMovingPlatform(MovingPlatform _movingPlatform)
    {



    }
    public static Vector3 GetMovingPlatformPositionAtTime(MovingPlatform _movingPlatform,float _time)
    {

        float startTime = 0;
        int section = 0;
        float distCovered = 0;
        float fracJourney = 0;
        for (int i = 0; i < _movingPlatform.eventTimeline.Count; i++)
        {

            if(_movingPlatform.eventTimeline[i].startTime <= _time && _movingPlatform.eventTimeline[i].endTime >= _time)
            {
                startTime = _movingPlatform.eventTimeline[i].startTime;
                section = i;
                break;
            }           

        }

        if (_movingPlatform.eventTimeline[section].active)
        {
            distCovered = (_time - startTime) * 10;
            fracJourney = distCovered / Vector3.Distance(_movingPlatform.eventTimeline[section].point[0], _movingPlatform.eventTimeline[section].point[1]);
        }
        else
        {
            distCovered = (_time - startTime) * 10;
            fracJourney = distCovered / Vector3.Distance(_movingPlatform.eventTimeline[section].point[0], _movingPlatform.eventTimeline[section].point[1]);

            //fracJourney = InverseLerp(_movingPlatform.eventTimeline[section].point[1], _movingPlatform.eventTimeline[section].point[0], Vector3.Lerp(_movingPlatform.eventTimeline[section].point[0], _movingPlatform.eventTimeline[section].point[1], fracJourney));


        }



        return Vector3.Lerp(_movingPlatform.eventTimeline[section].point[0], _movingPlatform.eventTimeline[section].point[1], fracJourney);



    }
    public static void UpdateMovingPlatformSet(MovingPlatformSet _movingPlatformSet,Timeline _timeline)
    {

        UpdatePressurePlate(_movingPlatformSet.pressurePlate, _timeline);


        //_dartWall.dartList.Clear();
        for (int i = 0; i < _movingPlatformSet.pressurePlate.eventTimeline.Count; i++)
        {


            // if (_movingPlatformSet.pressurePlate.eventTimeline[i].active)
            //{

            //float t = _movingPlatformSet.pressurePlate.eventTimeline[i].startTime;


            // while (t < _movingPlatformSet.pressurePlate.eventTimeline[i].endTime)
            // {
            float addedT = 0;
            for (int p = 0; p < _movingPlatformSet.movingPlatforms.Length; p++)
            {
                _movingPlatformSet.movingPlatforms[p].eventTimeline = new List<MovingEvent>();
                
                for (int e = 0; e < _movingPlatformSet.pressurePlate.eventTimeline.Count; e++)
                {
                    ActivatableEvent ae = new ActivatableEvent(_movingPlatformSet.pressurePlate.eventTimeline[e].startTime, _movingPlatformSet.pressurePlate.eventTimeline[e].endTime, _movingPlatformSet.pressurePlate.eventTimeline[e].active);
                    
                    
                    Vector3[] newPoint = new Vector3[2];
                    newPoint[0] = _movingPlatformSet.movingPlatforms[p].centre;
                    newPoint[1] = newPoint[0];

                    if (e > 0)
                    {
                        newPoint[0] = _movingPlatformSet.movingPlatforms[p].eventTimeline[e-1].point[1];
                        ae.startTime += addedT;
                    }
                    else
                    {
                        //newPoint[0] = _movingPlatformSet.movingPlatforms[p].centre;
                        if (addedT > 0)
                        {
                            //_movingPlatformSet.movingPlatforms[p].eventTimeline.Add(new MovingEvent(ae.startTime, ae.startTime + addedT, ae.active, newPoint));
                            //ae.startTime += addedT;
                        }
                    }

                    
                    ae.endTime += addedT;

                    Vector3 destinationPoint;                   

                    float distCovered = (ae.endTime - ae.startTime) * 10;
                    float fracJourney;

                    if (ae.active)
                    {
                        fracJourney = distCovered / Vector3.Distance(newPoint[0], _movingPlatformSet.movingPlatforms[p].point[1]);
                        destinationPoint = Vector3.Lerp(newPoint[0], _movingPlatformSet.movingPlatforms[p].point[1], fracJourney);
                        if (e > 0)
                        {
                          //  _movingPlatformSet.movingPlatforms[p].eventTimeline[e - 1].endTime += addedT;
                        }
                    }
                    else
                    {
                        fracJourney = distCovered / Vector3.Distance(newPoint[0], _movingPlatformSet.movingPlatforms[p].point[0]);
                        destinationPoint = Vector3.Lerp(newPoint[0], _movingPlatformSet.movingPlatforms[p].point[0], fracJourney);
                    }

                    newPoint[1] = destinationPoint;


                    _movingPlatformSet.movingPlatforms[p].eventTimeline.Add(new MovingEvent(ae.startTime, ae.endTime, ae.active, newPoint));

                   
                }
                addedT += 0.25f;
            }


                //    t += 0.5f;
                //}


            //}


        }


    }
    public static void ClearActorLists(Actor _actor)
    {
        _actor.actionList.Clear();
        
        _actor.movementList.Clear();
        _actor.jumpList.Clear();

        _actor.fallList.Clear();
    }
    //public static Vector3 GetMovingPlatformSetPlatformPosition (MovingPlatformSet _movingPlatformSet,int _platform,float _time)
    //{

    //    float startTime = 0;

    //    float distCovered = (_time - startTime) * 10;
    //    float fracJourney = distCovered / Vector3.Distance(_movingPlatformSet.movingPlatforms[_platform].point[0], _movingPlatformSet.movingPlatforms[_platform].point[1]);

    //    return Vector3.Lerp(_movingPlatformSet.movingPlatforms[_platform].point[0], _movingPlatformSet.movingPlatforms[_platform].point[1], fracJourney);

    //}
}
[System.Serializable]
public class Actor {

	public List<ActionBase> actionList = new List<ActionBase> ();

    public List<Idle> idleList = new List<Idle>();
    public List<Movement> movementList = new List<Movement>();
    public List<Jump> jumpList = new List<Jump>();
    public List<Fall> fallList = new List<Fall>();

    public List<Life> lifeList = new List<Life>();

    public Vector3 startingPos;
    public Vector3 lastPos;
    public Vector3 position;

	public Actor (){

		position = Vector3.zero;
        startingPos = position;
        lastPos = position;
		actionList = new List<ActionBase> ();
        movementList = new List<Movement>();
        jumpList = new List<Jump>();
        fallList = new List<Fall>();
        idleList = new List<Idle>();
        lifeList = new List<Life>();
        
    }
	public Actor (Vector3 pos){
        startingPos = pos;
        position = pos;
        lastPos = position;
        actionList = new List<ActionBase> ();
        movementList = new List<Movement>();
        jumpList = new List<Jump>();
        fallList = new List<Fall>();
        idleList = new List<Idle>();
        lifeList = new List<Life>();
    }
	public Actor (Vector3 _pos,List<ActionBase> ac){
        startingPos = _pos;
        position = _pos;
        lastPos = position;
        actionList = ac;
        movementList = new List<Movement>();
        jumpList = new List<Jump>();
        fallList = new List<Fall>();
        idleList = new List<Idle>();
        lifeList = new List<Life>();
    }
	public void AddToActionList (ActionBase action){

		actionList.Add (action);

	}
	public void SortActionList (){


		if (actionList.Count > 0) {
			actionList.Sort (delegate(ActionBase a, ActionBase b) {
				return (a.startTime).CompareTo (b.startTime);
			});

		}
	}
    public void SortMovementList()
    {


        if (movementList.Count > 0)
        {
            movementList.Sort(delegate (Movement a, Movement b) {
                return (a.startTime).CompareTo(b.startTime);
            });

        }
    }
    public void SortJumpList()
    {


        if (jumpList.Count > 0)
        {
            jumpList.Sort(delegate (Jump a, Jump b) {
                return (a.startTime).CompareTo(b.startTime);
            });

        }
    }
    public void SortFallList()
    {
        if (fallList.Count > 0)
        {
            fallList.Sort(delegate (Fall a, Fall b) {
                return (a.startTime).CompareTo(b.startTime);
            });

        }
    }
    public void SortIdleList()
    {

        if (idleList.Count > 0)
        {
            idleList.Sort(delegate (Idle a, Idle b) {
                return (a.startTime).CompareTo(b.startTime);
            });

        }
    }
}
[System.Serializable]
public class Life
{
    public float startTime, endTime;
    public Vector3 point;
    public bool alive;

    public Life(float _startTime,float _endTime,bool _alive,Vector3 _point)
    {

        startTime = _startTime;
        endTime = _endTime;
        alive = _alive;
        point = _point;
    }
}
[System.Serializable]
public class Movement
{
    public float startTime;
    public float endTime;

    public NavMeshPath path;

    public Movement(float st, float et, NavMeshPath p)
    {
        startTime = st;
        endTime = et;
        path = p;
    }    
}
[System.Serializable]
public class ActionBase
{

    public float startTime;
    public float endTime;

    public ActionType actionType;
    public ActionBase(float st, float et,ActionType _actionType)
    {
        startTime = st;
        endTime = et;
        actionType = _actionType;
    }

}
[System.Serializable]
public class Jump
{
    public float startTime;
    public float endTime;
    public Vector3 jumpPoint;
    public Vector3 centrePoint;
    public Vector3 landPoint;
    public Vector3 currentPoint;
    

    public Jump(float st, float et,Vector3 _jumpPoint, Vector3 _centrePoint, Vector3 _landPoint)
    {
        startTime = st;
        endTime = et;
        jumpPoint = _jumpPoint;
        centrePoint = _centrePoint;
        landPoint = _landPoint;
        //startTime = st;
        //endTime = et;
        //path = p;
    }
}
[System.Serializable]
public class Fall
{
    public float startTime;
    public float endTime;
    public Vector3[] point;

    public Fall(float st, float et, Vector3[] _point)
    {
        startTime = st;
        endTime = et;
        point = _point;
    }
}
[System.Serializable]
public class Idle
{
    public float startTime;
    public float endTime;
    public Vector3 point;

    public Idle (float _st,float _et, Vector3 _point)
    {
        startTime = _st;
        endTime = _et;
        point = _point;
    }

}
[System.Serializable]
public class RotationMovement
{
    public float startTime;
    public float endTime;
    public float[] angle;

    public RotationMovement(float st, float et, float[] _angle)
    {
        startTime = st;
        endTime = et;
        angle = _angle;


    }
}
//[System.Serializable]
//public class RotationMovement
//{
//    public float startTime;
//    public float endTime;
//    public Quaternion pointA;
//    public Quaternion pointB;

//    public RotationMovement(float st, float et, Quaternion _pointA, Quaternion _pointB)
//    {
//        startTime = st;
//        endTime = et;
//        pointA = _pointA;
//        pointB = _pointB;

//    }
//}
[System.Serializable]
public class MovableObject {
	public Vector3 startPoint;
	//public Vector3 currentPoint;
	public Vector3 endPoint;
	public float speed;
	public float startTime;
	public float endTime;
    public Vector3 size;

	public MovableObject (Vector3 _startPoint,Vector3 _endPoint,float _speed,float _startTime,float _endTime){
		startPoint = _startPoint;
		endPoint = _endPoint;
		speed = _speed;
		startTime = _startTime;
		endTime = _endTime;
        size = new Vector3(1, 1, 1);
	}
	public static Vector3 GetObjectPositionAtTime (MovableObject _mo,float _time){

		float startTime = _mo.startTime;
		float sectionDistance = Vector3.Distance (_mo.startPoint, _mo.endPoint);

		float distCovered = (_time - (startTime )) * 10;
		float fracJourney = distCovered / sectionDistance;


		return Vector3.Lerp (_mo.startPoint, _mo.endPoint, fracJourney);


	}
}
//[System.Serializable]
//public class Turret
//{

//    public List<RotationMovement> rotationList = new List<RotationMovement>();

//    public Vector3 position;
//    public Quaternion rotation;

//    public List<Bullet> bulletList = new List<Bullet>();

//    public Turret (Vector3 _position, Quaternion _rotation, List<RotationMovement> _rotationList)
//    {
//        position = _position;
//        rotation = _rotation;
//        rotationList = _rotationList;
//    }
//    public void SortRotationList()
//    {


//        if (rotationList.Count > 0)
//        {
//            rotationList.Sort(delegate (RotationMovement a, RotationMovement b) {
//                return (a.startTime).CompareTo(b.startTime);
//            });

//        }
//    }

//}
[System.Serializable]
public class Dart
{

    public Vector3[] points;
    public Vector3 position;
    public Quaternion rotation;
    public float startTime;
    public float endTime;

    public Dart(Vector3 _position, Quaternion _rotation,float _startTime,float _endTine,Vector3[] _points)
    {
        position = _position;
        rotation = _rotation;
        startTime = _startTime;
        endTime = _endTine;
        points = _points;
    }

}
[System.Serializable]
public class Activatable
{

    public List<ActivatableEvent> eventTimeline = new List<ActivatableEvent>();

    public void SortEventTimeline()
    {
        if (eventTimeline.Count > 0)
        {
            eventTimeline.Sort(delegate (ActivatableEvent a, ActivatableEvent b) {
                return (a.startTime).CompareTo(b.startTime);
            });

        }
    }
}
[System.Serializable]
public class PressurePlate : Activatable{

    public Vector3 centre;
    public Vector3 size;
  

    public PressurePlate(Vector3 _centre, Vector3 _size)
    {

        centre = _centre;
        size = _size;

    }

}
[System.Serializable]
public class ActivatableEvent
{

    public float startTime;
    public float endTime;
    public bool active;

    public ActivatableEvent(float _startTime, float _endTime, bool _active)
    {
        startTime = _startTime;
        endTime = _endTime;
        active = _active;
    }

}
[System.Serializable]
public class MovingEvent : ActivatableEvent
{
    public Vector3[] point;

    public MovingEvent(float _startTime, float _endTime, bool _active,Vector3[] _point) : base(_startTime,_endTime,_active)
    {
        point = _point;
        startTime = _startTime;
        endTime = _endTime;
        active = _active;
    }

}
[System.Serializable]
public class Zipline : MovingPoint
{
    public Zipline(Vector3[] _point) : base(_point)
    {

        point = _point;

    }

}
[System.Serializable]
public class MovingPlatform : MovingPoint
{
    public Vector3 centre;
    public Vector3 size;
    public List<MovingEvent> eventTimeline = new List<MovingEvent>();

    public MovingPlatform(Vector3 _centre,Vector3 _size,Vector3[] _point) : base(_point)
    {
        centre = _centre;
        size = _size;
        point = _point;

    }

}
[System.Serializable]
public class SpikeTrap : MovingPoint
{

    public Vector3 centre;
    public Vector3 size;

    public PressurePlate pressurePlate;

    public List<MovingEvent> eventTimeline = new List<MovingEvent>();
    public List<ActivatableEvent> hitEventTimeline = new List<ActivatableEvent>();

    public SpikeTrap(Vector3[] _point, Vector3 _centre, Vector3 _size, PressurePlate _pressurePlate) : base(_point)
    {

        point = _point;
        centre = _centre;
        size = _size;
        pressurePlate = _pressurePlate;

        //Vector3[] startingPoint = new Vector3[2];
        //startingPoint[0] = centre;
        //startingPoint[1] = centre - Vector3.up;

        //eventTimeline.Add(new MovingEvent(0, 0, false, startingPoint));
    }

}
[System.Serializable]
public class MovingPoint
{
    public Vector3[] point;

    public MovingPoint(Vector3[] _point)
    {

        point = _point;

    }

}
[System.Serializable]
public class DartWall
{
    public Vector3 centre;
    public Vector3 size;
    public Quaternion rotation;

    public List<Dart> dartList = new List<Dart>();
    public PressurePlate pressurePlate;

    public DartWall(Vector3 _centre, Vector3 _size,Quaternion _rotation,PressurePlate _pressurePlate)
    {
        centre = _centre;
        size = _size;
        rotation = _rotation;
        pressurePlate = _pressurePlate;
    }
}
[System.Serializable]
public class EndArea
{
    public Vector3 centre;
    public Vector3 size;
    public List<ActivatableEvent> eventTimeline = new List<ActivatableEvent>();

    public EndArea(Vector3 _centre, Vector3 _size)
    {
        centre = _centre;
        size = _size;
    }

}
[System.Serializable]
public class UnstableFloor : MovingPoint
{

    public Vector3 centre;
    public Vector3 size;

    public List<ActivatableEvent> eventTimeline = new List<ActivatableEvent>();

    public UnstableFloor(Vector3 _centre,Vector3 _size,Vector3[] _point) : base(_point)
    {
        centre = _centre;
        size = _size;
        point = _point;
    }



}
[System.Serializable]
public class MovingPlatformSet
{

    public MovingPlatform[] movingPlatforms;

    public PressurePlate pressurePlate;

    public List<ActivatableEvent> eventTimeline = new List<ActivatableEvent>();

    public MovingPlatformSet (MovingPlatform[] _movingPlatforms,PressurePlate _pressurePlate)
    {

        movingPlatforms = _movingPlatforms;
        pressurePlate = _pressurePlate;
    }


}
[System.Serializable]
public class ActorObject
{
    

}
[System.Serializable]
public class PushableObject : ActorObject
{   


}
[System.Serializable]
public class LiftableObject : ActorObject
{


}

public enum ActionType { walk,run, jump,zipline,falling,movingPlatform,idle,death }