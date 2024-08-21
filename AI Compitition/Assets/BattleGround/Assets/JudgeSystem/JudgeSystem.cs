using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class JudgeSystem : MonoBehaviour
{
    public enum EventTypes
    {
        None = 0,
        ITEM_GOAL,
        ITEM_SPEED_UP,
        ITEM_SPEED_DOWN,
        ITEM_SCORE_UP,
        ITEM_SCORE_DOWN,
        ITEM_BOMB,
        AGENT_GOAL,
        AGENT_REMOVED,
    };

    public enum AgentStatus
    {
        None = 0,
        STAT_LIVE,
        STAT_WIN,
        STAT_DEAD,
        STAT_VIOLATED,
    };

    [SerializeField]
    private GameObject[] AgentList;

    public delegate void MapEvent(EventTypes type, Vector3 pos);
    public static event MapEvent OnMapEvent;

    [SerializeField]
    private GameObject GoalMaster;
    [SerializeField]
    private Vector3 GoalPosition = Vector3.zero;

    [SerializeField]
    private float itemRandomMin;
    [SerializeField]
    private float itemRandomMax;

    [SerializeField]
    private float itemSpeedValue;
    [SerializeField]
    private int itemScoreValue;

    [SerializeField]
    private GameObject itemSpeedUp;
    [SerializeField]
    private int SpeedUpMax;
    [SerializeField]
    private int SpeedUpInitial;

    [SerializeField]
    private GameObject itemSpeedDown;
    [SerializeField]
    private int SpeedDownMax;
    [SerializeField]
    private int SpeedDownInitial;

    [SerializeField]
    private GameObject itemScoreUp;
    [SerializeField]
    private int ScoreUpMax;
    [SerializeField]
    private int ScoreUpInitial;

    [SerializeField]
    private GameObject itemScoreDown;
    [SerializeField]
    private int ScoreDownMax;
    [SerializeField]
    private int ScoreDownInitial;

    [SerializeField]
    private GameObject itemBomb;
    [SerializeField]
    private int BombMax;
    [SerializeField]
    private int BombInitial;


    private GameObject AgentInstancesNode = null;

    private List<RankItem> gRankingList;

    private List<String> consoleStrings = new List<string>();
    [SerializeField]
    private int ConsoleLineMax = 10;
    [SerializeField]
    private TMPro.TextMeshProUGUI consolePanel;
    [SerializeField]
    private TMPro.TextMeshProUGUI countdownClock;
    [SerializeField]
    private TMPro.TextMeshProUGUI pausedText;
    [SerializeField]
    private TMPro.TextMeshProUGUI rankingTable;


    private Material oNavMat;
    private RenderParams oNavMatParam;
    private Mesh oNavMesh;

    private GameObject[] SpawnPoints;

    private GameObject Fences;
    private GameObject Entities;

    [SerializeField]
    private int TimeLimitSeconds = 600;

    private static int iCountDownSeconds = 0;
    private int iAccumulateSeconds = 0;
    [SerializeField]
    private int GoalScore = 1000;
    [SerializeField]
    private int TimeDecrementScore = 100;
    [SerializeField]
    private int DecrementScoreRate = 2;

    [SerializeField]
    private GameObject templateInfoText;

    [SerializeField]
    private GameObject ParticleBomb;
    [SerializeField]
    private GameObject ParticleWin;
    [SerializeField]
    private GameObject ParticleViolate;

    public static int GetRemainedTime()
    {
        return iCountDownSeconds;
    }

    void GenerateMapEvent(EventTypes type, Vector3 pos)
    {
        if (OnMapEvent != null)
            OnMapEvent(type, pos);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setup MAP
        oNavMesh = GetMeshFromNavmesh();
        Fences = GameObject.FindGameObjectWithTag("Fences");
        Entities = GameObject.FindGameObjectWithTag("Entities");

        if (itemBomb != null)
        {
            for (int i = 0; i < BombInitial; i++)
            {
                BombMax--;
                //RandomSpawnEntity(EventTypes.ITEM_BOMB, Entities.transform);
                RangeSpawnEntity(EventTypes.ITEM_BOMB, -6.0f, 6.0f, Entities.transform);
            }
        }
        if (itemSpeedUp != null)
        {
            for (int i = 0; i < SpeedUpInitial; i++)
            {
                SpeedUpMax--;
                RandomSpawnEntity(EventTypes.ITEM_SPEED_UP, Entities.transform);
            }
        }
        if (itemSpeedDown != null)
        {
            for (int i = 0; i < SpeedDownInitial; i++)
            {
                SpeedDownMax--;
                //RandomSpawnEntity(EventTypes.ITEM_SPEED_DOWN, Entities.transform);
                RangeSpawnEntity(EventTypes.ITEM_SPEED_DOWN, -8.0f, 8.0f, Entities.transform);
            }
        }
        if (itemScoreUp != null)
        {
            for (int i = 0; i < ScoreUpInitial; i++)
            {
                ScoreUpMax--;
                RandomSpawnEntity(EventTypes.ITEM_SCORE_UP, Entities.transform);
            }
        }
        if (itemScoreDown != null)
        {
            for (int i = 0; i < ScoreDownInitial; i++)
            {
                ScoreDownMax--;
                //RandomSpawnEntity(EventTypes.ITEM_SCORE_DOWN, Entities.transform);
                RangeSpawnEntity(EventTypes.ITEM_SCORE_DOWN, -7.0f, 7.0f, Entities.transform);
            }
        }

        // Prepare Spawn points
        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        Debug.Log("Spawn points : " + SpawnPoints.Length);
        int[] shuffleTable = new int[SpawnPoints.Length];
        for (int i = 0; i < SpawnPoints.Length; i++)
            shuffleTable[i] = i;
        //Shuffle
        for (int i = 0; i < shuffleTable.Length; i++)
        {
            int k = Random.Range(0, shuffleTable.Length);
            int value = shuffleTable[k];
            shuffleTable[k] = shuffleTable[i];
            shuffleTable[i] = value;
        }

        // Spawn ALL agent
        int index = -1;
        AgentInstancesNode = GameObject.FindGameObjectWithTag("Instances");
        foreach (var agent in AgentList)
        {
            ++index;
            Vector3 spawnPos = SpawnPoints[shuffleTable[index]].transform.position;
            //print(spawnPos);

            NavMeshAgent thisAgent = agent.transform.GetComponent<NavMeshAgent>();
            thisAgent.speed = 0.5f;
            thisAgent.Warp(spawnPos);
            //thisAgent.SetDestination(spawnPos);

            agent.tag = "Agent";
            agent.transform.LookAt(AgentInstancesNode.transform.position);
            GameObject a = Instantiate(agent, AgentInstancesNode.transform);

            //Add info text
            GameObject info = Instantiate(templateInfoText, a.transform);
            TMP_Text infoText = info.transform.GetComponentInChildren<TMP_Text>();
            BaseAgent agentInfo = a.GetComponent<BaseAgent>();
            if (agentInfo != null)
            {
                infoText.text = agentInfo.AgentNickname;
            }
            else
            {
                infoText.text = a.transform.name.Replace("(Clone)", "");
            }
            //Add tracker
            AgentTracker tracker = a.AddComponent<AgentTracker>();
            tracker.enabled = true;
            tracker.fSpeed = thisAgent.speed;
            tracker.iScore = 0;
            tracker.iIndex = index;
            tracker.eStatus = AgentStatus.STAT_LIVE;
        }

        UpdateConsoleStrings();
        countdownClock.text = GetTimeString(iCountDownSeconds) + "\n" + "<size=30%><color=\"black\">Goal:<color=\"blue\">" + GoalScore + "</size>";
        pausedText.gameObject.SetActive(false);

        gRankingList = BuildRankingList();
        UpdateRankingTable();
    }

    private class RankItem : IComparable<RankItem>
    {
        public GameObject gObject;
        public BaseAgent aBase;
        public AgentTracker aTracker;

        public int CompareTo(RankItem next)
        {
            //Decent ordering
            if (aTracker.iScore > next.aTracker.iScore)
                return -1;
            else if (aTracker.iScore < next.aTracker.iScore)
                return 1;
            else
            {
                //return 0;
                if (aTracker.iTimeStamp > next.aTracker.iTimeStamp)
                    return 1;
                else if (aTracker.iTimeStamp < next.aTracker.iTimeStamp)
                    return -1;
                else
                {
                    /*if (aTracker.eStatus == AgentStatus.STAT_WIN)
                        return 1;
                    else if (aTracker.eStatus == AgentStatus.STAT_DEAD)
                        return -1;
                    else if (aTracker.eStatus == AgentStatus.STAT_VIOLATED)
                        return -1;
                    else*/
                        return 0;
                }
            }
        }
    };

    List<RankItem> BuildRankingList()
    {
        //People.Sort((p1, p2) => p1.LastName.CompareTo(p2.LastName));
        GameObject[] aList = GameObject.FindGameObjectsWithTag("Agent");
        List<RankItem> RankingTable = new List<RankItem>();
        foreach (GameObject a in aList)
        {
            RankItem item = new RankItem();
            item.gObject = a;
            item.aBase = a.GetComponent<BaseAgent>();
            item.aTracker = a.GetComponent<AgentTracker>();
            //if((item.aTracker.eStatus == AgentStatus.STAT_LIVE) || (item.aTracker.eStatus == AgentStatus.STAT_WIN))
            {
                RankingTable.Add(item);
            }
        }
        Debug.Log("BuildRankingList:" + RankingTable.Count);
        return RankingTable;
    }

    void UpdateRankingTable()
    {

        int iRank = 0;
        String rankText = "[ RANKING ]\n";
        gRankingList.Sort();
        foreach (RankItem i in gRankingList)
        {
            String t = i.aTracker.iTimeStamp < int.MaxValue ? i.aTracker.iTimeStamp.ToString() : "-";
            rankText += ("<color=\"black\">" + i.aBase.AgentNickname + " (" + t + ") : <color=\"blue\">" + i.aTracker.iScore + " </color><color=\"red\">[" + (++iRank).ToString("00") + "]</color></color>\n");
        }

        rankingTable.text = rankText;
    }

    String GetTimeString(int iSeconds)
    {
        int m = iSeconds / 60;
        int s = iSeconds % 60;
        return (m.ToString("00") + ":" + s.ToString("00"));
    }

    void TickSeconds()
    {
        if (iCountDownSeconds > 0)
        {
            iCountDownSeconds--;
            iAccumulateSeconds = TimeLimitSeconds - iCountDownSeconds;

            // End game
            if (iCountDownSeconds <= 0)
            {
                CancelInvoke();

                String name = DateTime.Now.ToString("ddMMyy_hhmmss");
                Rank2File("rank_" + name + ".csv");
                Logs2File("logs_" + name + ".txt");
            }

            // When score decrement
            if ((iAccumulateSeconds > TimeDecrementScore) && (GoalScore > 0))
            {
                GoalScore -= DecrementScoreRate;
                if (GoalScore < 0)
                    GoalScore = 0;
            }

            // Update time clock
            countdownClock.text = GetTimeString(iCountDownSeconds) + "\n" + "<size=30%><color=\"black\">Goal:<color=\"blue\">" + GoalScore + "</size>";
        }
        /*
            private int GoalScore = 1000;
            private int TimeDecrementScore = 100;
            private int DecrementScoreRate = 2;*/
    }

    void IgniteParticle(GameObject gParticlePrefab,Vector3 vPos)
    {
        GameObject particle = Instantiate(gParticlePrefab, vPos, Quaternion.identity);
        ParticleSystem parts = particle.GetComponent<ParticleSystem>();
        float totalDuration = parts.main.duration + parts.main.startLifetime.constant;
        Destroy(particle, totalDuration);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Time.timeScale != 0.0f)
            {
                Time.timeScale = 0.0f;
                pausedText.gameObject.SetActive(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                pausedText.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Fences.activeSelf == true)
            {
                // Game Start
                Fences.SetActive(false);

                // Start countdown
                iCountDownSeconds = TimeLimitSeconds;
                iAccumulateSeconds = 0;
                InvokeRepeating("TickSeconds", 1.0f, 1.0f);
                //RangeSpawnEntity(EventTypes.ITEM_GOAL,-1.0f,1.0f, Entities.transform);
                Vector3 newPos = Vector3.zero;
                newPos.y += 1.0f;
                SpawnEntity(EventTypes.ITEM_GOAL, newPos, Entities.transform);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //RandomSpawnEntity(EventTypes.ITEM_SCORE_DOWN,Entities.transform);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //RandomSpawnEntity(EventTypes.ITEM_SCORE_UP, Entities.transform);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //RandomSpawnEntity(EventTypes.ITEM_SPEED_DOWN, Entities.transform);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //RandomSpawnEntity(EventTypes.ITEM_SPEED_UP, Entities.transform);
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            String name = DateTime.Now.ToString("ddMMyy_hhmmss");
            Rank2File("rank_" + name + ".csv");
            Logs2File("logs_" + name + ".txt");
        }


        //Graphics.RenderMesh(oNavMatParam, oNavMesh, 0, Matrix4x4.identity);
    }

    private void RandomSpawnEntity(EventTypes spawnEvent, Transform parent = null)
    {
        Vector3 newPos = RendomPointOnNavMeshWitnLimit(itemRandomMin, itemRandomMax, 10.0f);
        newPos.y += 0.5f;

        SpawnEntity(spawnEvent, newPos, parent);
    }

    private void RangeSpawnEntity(EventTypes spawnEvent, float min, float max, Transform parent = null)
    {
        Vector3 newPos = RendomPointOnNavMeshWitnLimit(min, max, 10.0f);
        newPos.y += 0.5f;

        SpawnEntity(spawnEvent, newPos, parent);
    }

    private void AttachSpawnEntity(EventTypes spawnEvent, GameObject attach, float min, float max, Transform parent = null)
    {
        Vector3 newPos = RendomPointOnNavMeshWithCenter(attach.transform.position.x, attach.transform.position.z, min, max, 10.0f);
        newPos.y += 0.5f;

        SpawnEntity(spawnEvent, newPos, parent);
    }

    private void SpawnEntity(EventTypes spawnEvent, Vector3 newPos, Transform parent = null)
    {
        GameObject e;

        switch (spawnEvent)
        {
            case EventTypes.ITEM_GOAL:
                {
                    e = Instantiate(GoalMaster, parent);
                    e.transform.position = newPos;
                    GenerateMapEvent(spawnEvent, newPos);
                }
                break;
            case EventTypes.ITEM_BOMB:
                {
                    e = Instantiate(itemBomb, parent);
                    e.transform.position = newPos;
                    GenerateMapEvent(spawnEvent, newPos);
                }
                break;
            case EventTypes.ITEM_SPEED_DOWN:
                {
                    e = Instantiate(itemSpeedDown, parent);
                    e.transform.position = newPos;
                    GenerateMapEvent(spawnEvent, newPos);
                }
                break;
            case EventTypes.ITEM_SPEED_UP:
                {
                    e = Instantiate(itemSpeedUp, parent);
                    e.transform.position = newPos;
                    GenerateMapEvent(spawnEvent, newPos);
                }
                break;
            case EventTypes.ITEM_SCORE_DOWN:
                {
                    e = Instantiate(itemScoreDown, parent);
                    e.transform.position = newPos;
                    GenerateMapEvent(spawnEvent, newPos);
                }
                break;
            case EventTypes.ITEM_SCORE_UP:
                {
                    e = Instantiate(itemScoreUp, parent);
                    e.transform.position = newPos;
                    GenerateMapEvent(spawnEvent, newPos);
                }
                break;
            default:
                break;
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    float[] GetTriSizes(int[] tris, Vector3[] verts)
    {
        int triCount = tris.Length / 3;
        float[] sizes = new float[triCount];
        for (int i = 0; i < triCount; i++)
        {
            sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
        }
        return sizes;
    }

    bool CheckSpace(Vector3 pos, float limitDistance = 2.0f)
    {
        if (gRankingList != null)
        {
            foreach (RankItem i in gRankingList)
            {
                if (i.gObject.activeSelf)
                {
                    float dis = Vector3.Distance(i.gObject.transform.position, pos);
                    if (dis < limitDistance)
                        return false;
                }
            }
        }
        if (Entities != null)
        {
            foreach (Transform child in Entities.transform)
            {
                float dis = Vector3.Distance(child.position, pos);
                if (dis < limitDistance)
                    return false;
            }
        }
        return true;
    }

    Mesh GetMeshFromNavmesh()
    {
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        Mesh mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;

        return mesh;
    }

    Vector3 PointOnNavMesh(Vector3 Point, float maxDistance = 1.0f)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(Point, out hit, maxDistance, NavMesh.AllAreas);
        return hit.position;
    }

    Vector3 RendomPointOnNavMeshWitnLimit(float min, float max, float maxDistance = 1.0f)
    {
        bool bQuit = false;
        int count = 0;
        Vector3 point = Vector3.zero;
        do
        {
            point.x = (float)Math.Floor(Random.Range(min, max));
            point.z = (float)Math.Floor(Random.Range(min, max));
            point.y = 1.0f;
            bQuit = CheckSpace(point);
            if (!bQuit)
                count++;
        } while (!bQuit);
        if (count > 0)
            Debug.LogError("CheckSpace FALSE : " + count);
        return PointOnNavMesh(point, maxDistance);
    }

    Vector3 RendomPointOnNavMeshWithCenter(float x, float z, float min, float max, float maxDistance = 1.0f)
    {
        bool bQuit = false;
        int count = 0;
        Vector3 point = Vector3.zero;
        do
        {
            point.x = (float)Math.Floor(Random.Range(min, max)) + x;
            point.z = (float)Math.Floor(Random.Range(min, max)) + z;
            point.y = 1.0f;
            bQuit = CheckSpace(point);
            if (!bQuit)
                count++;
            if (count > 20)
                bQuit = true;
        } while (!bQuit);
        if (count > 20)
        {
            Debug.LogError("CheckSpace FALSE : Alternative!");
            return RendomPointOnNavMeshWitnLimit(itemRandomMin, itemRandomMax, 10.0f);
        }
        else
        {
            return PointOnNavMesh(point, maxDistance);
        }
    }

    Vector3 RendomPointOnNavMesh(float maxDistance = 1.0f)
    {
        //Triangulated NavMesh is not the same as current NavMesh, so we do sampling.
        return PointOnNavMesh(RandomPointOnMesh(oNavMesh), maxDistance);
    }

    Vector3 RandomPointOnMesh(Mesh mesh)
    {
        //if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
        float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
        float[] cumulativeSizes = new float[sizes.Length];
        float total = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            total += sizes[i];
            cumulativeSizes[i] = total;
        }

        //so everything above this point wants to be factored out

        float randomsample = Random.value * total;

        int triIndex = -1;

        for (int i = 0; i < sizes.Length; i++)
        {
            if (randomsample <= cumulativeSizes[i])
            {
                triIndex = i;
                break;
            }
        }

        if (triIndex == -1) Debug.LogError("triIndex should never be -1");

        Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
        Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
        Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

        //generate random barycentric coordinates

        float r = Random.value;
        float s = Random.value;

        if (r + s >= 1)
        {
            r = 1 - r;
            s = 1 - s;
        }
        //and then turn them back to a Vector3
        Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
        return pointOnMesh;

    }

    public void IssueViolation(GameObject complainant)
    {
        if (complainant != null)
        {
            String AgentName = GetAgentNickname(complainant);
            AgentTracker tracker = complainant.GetComponent<AgentTracker>();

            //Debug.LogError("Speed violation : " + AgentName);
            AddConsoleString("<color=\"purple\">Speed violation : <color=\"red\">" + AgentName + "<color=\"black\">.");

            tracker.eStatus = AgentStatus.STAT_VIOLATED;
            tracker.iTimeStamp = iAccumulateSeconds;
            IgniteParticle(ParticleViolate, complainant.transform.position);
            DoRemoveAgent(complainant.gameObject);
            UpdateRankingTable();
        }
    }

    public void IssueCollider(Collider other, GameObject complainant)
    {
        GameObject currentAgent = other.gameObject;
        String AgentName = GetAgentNickname(currentAgent);
        NavMeshAgent agent = currentAgent.GetComponent<NavMeshAgent>();
        AgentTracker tracker = currentAgent.GetComponent<AgentTracker>();

        if (complainant.CompareTag("Goal") == true)
        {
            Debug.LogWarning("Goal Reached : " + AgentName);
            AddConsoleString("<color=\"yellow\">Goal Reached <color=\"black\">: " + AgentName);

            tracker.iScore += GoalScore;
            tracker.eStatus = AgentStatus.STAT_WIN;
            tracker.iTimeStamp = iAccumulateSeconds;
            DoRemoveAgent(currentAgent);

            IgniteParticle(ParticleWin, complainant.transform.position);
            // Remove complainant
            Destroy(complainant);

            //RandomSpawnEntity(EventTypes.ITEM_GOAL, Entities.transform);
            RangeSpawnEntity(EventTypes.ITEM_GOAL, -18, 18, Entities.transform);
        }
        else if (complainant.CompareTag("ItemSpeedUp") == true)
        {
            Debug.LogWarning("SpeedUp to : " + AgentName);
            AddConsoleString("<color=\"black\">Speed <color=\"green\">UP <color=\"black\">: " + AgentName);

            agent.speed += itemSpeedValue;
            tracker.fSpeed = agent.speed;

            // Remove complainant
            Destroy(complainant);

            if (SpeedUpMax > 0)
            {
                SpeedUpMax--;
                RandomSpawnEntity(EventTypes.ITEM_SPEED_UP, Entities.transform);
            }
        }
        else if (complainant.CompareTag("ItemSpeedDown") == true)
        {
            Debug.LogWarning("SpeedDown : " + AgentName);
            AddConsoleString("<color=\"black\">Speed <color=\"red\">DOWN <color=\"black\">: " + AgentName);

            if (agent.speed > itemSpeedValue)
                agent.speed -= itemSpeedValue;
            else
                agent.speed = 0.0f;
            tracker.fSpeed = agent.speed;

            // Remove complainant
            Destroy(complainant);

            if (SpeedDownMax > 0)
            {
                SpeedDownMax--;
                RandomSpawnEntity(EventTypes.ITEM_SPEED_DOWN, Entities.transform);
            }
        }
        else if (complainant.CompareTag("ItemScoreUp") == true)
        {
            Debug.LogWarning("ScoreUp to : " + AgentName);
            AddConsoleString("<color=\"black\">Score <color=\"green\">UP <color=\"black\">: " + AgentName);

            tracker.iScore += itemScoreValue;

            // Remove complainant
            Destroy(complainant);

            if (ScoreUpMax > 0)
            {
                ScoreUpMax--;
                RandomSpawnEntity(EventTypes.ITEM_SCORE_UP, Entities.transform);
            }
        }
        else if (complainant.CompareTag("ItemScoreDown") == true)
        {
            Debug.LogWarning("ScoreDown to : " + AgentName);
            AddConsoleString("<color=\"black\">Score <color=\"red\">DOWN <color=\"black\">: " + AgentName);

            tracker.iScore -= itemScoreValue;

            // Remove complainant
            Destroy(complainant);

            if (ScoreDownMax > 0)
            {
                ScoreDownMax--;
                RandomSpawnEntity(EventTypes.ITEM_SCORE_DOWN, Entities.transform);
            }
        }
        else if (complainant.CompareTag("ItemBomb") == true)
        {
            Debug.LogWarning("Bomb to : " + AgentName);
            AddConsoleString("<color=\"red\">BOOOOOOOOOOOOOOOM! : " + AgentName + "<color=\"black\">.");

            tracker.eStatus = AgentStatus.STAT_DEAD;
            tracker.iTimeStamp = iAccumulateSeconds;
            DoRemoveAgent(currentAgent);

            IgniteParticle(ParticleBomb, complainant.transform.position);
            // Remove complainant
            Destroy(complainant);

            

            if (BombMax > 0)
            {
                BombMax--;
                GameObject g = GameObject.FindGameObjectWithTag("Goal");
                //AttachSpawnEntity(EventTypes.ITEM_BOMB, g, -6.0f, 6.0f, Entities.transform);
                //RandomSpawnEntity(EventTypes.ITEM_BOMB, Entities.transform);
                RangeSpawnEntity(EventTypes.ITEM_BOMB, -10.0f, 10.0f, Entities.transform);
            }
        }

        UpdateRankingTable();
    }

    void DoRemoveAgent(GameObject targetAgent)
    {
        targetAgent.SetActive(false);

        GenerateMapEvent(EventTypes.AGENT_REMOVED, targetAgent.transform.position);
    }

    void UpdateConsoleStrings()
    {
        String temp = "BattleGround v.2024.04.25.01\n";
        int line = 0;
        foreach (String str in consoleStrings)
        {
            temp += (str + "\n");
            line++;
            if (line >= ConsoleLineMax)
                break;
        }
        consolePanel.text = temp;
    }

    void AddConsoleString(String str)
    {
        String time = "<color=\"black\">[" + DateTime.Now + "] ";
        consoleStrings.Insert(0, time + str);
        UpdateConsoleStrings();
    }

    string GetAgentNickname(GameObject a)
    {
        if (a != null)
        {
            BaseAgent agentInfo = a.GetComponent<BaseAgent>();
            if (agentInfo != null)
            {
                return agentInfo.AgentNickname;
            }
            else
            {
                return a.transform.name.Replace("(Clone)", "");
            }
        }
        else
        { return null; }
    }

    string GetAgentID(GameObject a)
    {
        if (a != null)
        {
            BaseAgent agentInfo = a.GetComponent<BaseAgent>();
            if (agentInfo != null)
            {
                return agentInfo.StudentId;
            }
            else
            {
                return null;
            }
        }
        else
        { return null; }
    }

    private string getPath(string fileName)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Data/" + fileName;
        //"Participant " + "   " + DateTime.Now.ToString("dd-MM-yy   hh-mm-ss") + ".csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+fileName;
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+fileName;
#else
        return Application.dataPath +"/"+fileName;
#endif
    }

    void Rank2File(string fileName)
    {
        string filePath = getPath(fileName);
        StreamWriter writer = new StreamWriter(filePath);

        writer.WriteLine("rank,score,name,timestamp,status,id");

        int iRank = 0;
        gRankingList.Sort();
        foreach (RankItem i in gRankingList)
        {
            writer.WriteLine((++iRank) + "," + i.aTracker.iScore + "," + i.aBase.AgentNickname + "," + i.aTracker.iTimeStamp + "," + i.aTracker.eStatus + "," + i.aBase.StudentId);
        }

        writer.Flush();
        writer.Close();
    }

    void Logs2File(string fileName)
    {
        string filePath = getPath(fileName);
        StreamWriter writer = new StreamWriter(filePath);

        writer.WriteLine("BattleGround Logs : " + DateTime.Now);

        foreach (String str in consoleStrings)
        {
            writer.WriteLine(str);
        }

        writer.Flush();
        writer.Close();
    }

}
