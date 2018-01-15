using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

    const int levelCap = 3;
    const int startBeams = 5;
    const int startBonuses = 3;
    const float startSpeed = 0.5f;

    public Color[] palette;                                             // Default color sсheme

    public static int score = 0;                                        // Current game session score
    public static int levelScore = 0;                                   // Current level score
    public static int beamsCount;                                       // Beams left for this session
    public static int maxBonuses;                                       // Maximum boxes with bonus beams
    public static float generalSpeed;                                   // General game speed

    public static bool noAction = true;                                 // beamOn coroutine condition
    public static bool noMove = false;
    public static byte level = 0;                                       // Current level

    public Map map;                                                     // Actual playground
    public UI mainUI;                                                   // Main user interface
    public Beam beam;                                                   // The Beam

    private int[] levelGoal = { 5000, 5000, 5000, 6000 };
    private float levelSpeed = 0.5f;

    public void DropNull()
    {
        level = 0;
        score = levelScore = 0;
        generalSpeed = levelSpeed = startSpeed;
        beamsCount = startBeams;
        maxBonuses = startBonuses;

        mainUI.SetProgressToStart();
        mainUI.SetColor(palette[0]);
    }

    public void LevelUp()
    {
        if (level == levelCap)
        {
//          Game Over
            mainUI.UpdateScore("-U WIN-");
            return;
        }

        levelScore = 0;

        level++;
        maxBonuses++;
        generalSpeed = levelSpeed += 0.25f;
        beamsCount = startBeams - level;

        if(level == levelCap)
            mainUI.SetColor(palette[levelCap], palette[0]);
        else
            mainUI.SetColor(palette[level]);

        mainUI.SetProgressToStart();
        map.Setup();
    }

    void Start ()
    {
//      Locking undesirable functions
        Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.Portrait;

        maxBonuses = startBonuses;
        UI.isMenuOn = false;

        DropNull();
        map.Setup();
    }

    void Update ()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && noAction)
            {
                noAction = false;

                if (UI.isMenuOn)
                {
                    mainUI.Reaction(Input.GetTouch(0).position);
                }
                else
                {
                    UserActionStart(Input.GetTouch(0).position);
                }
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            {
                if (UI.isMenuOn)
                {

                }
                else
                {
                    if (noAction)
                        UserActionStop();
                    else
                        StartCoroutine(BeamOff());
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && noAction)
        {
            if (UI.isMenuOn)
            {
                mainUI.Reaction(Input.mousePosition);
            }
            else
                UserActionStart(Input.mousePosition);
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (noAction && !UI.isMenuOn)
                UserActionStop();
            else
                StartCoroutine(BeamOff());
        }

    }

//  Undefined user interaction, mouse click or touch, starts here

    public void UserActionStart(Vector2 point)
    {
        noAction = false;

        int x = Mathf.CeilToInt(point.x / (Screen.width / Map.maxColumns)) - 1;
        int y = Mathf.CeilToInt(point.y / (Screen.height / (Map.maxRows + 1))) - 1;

//      Processing user interaction

        if (y <= 1)
        {
            if (x <= 2)
            {
                mainUI.ActivateMenu();
            }

/*          else if (x > 6)
            {
                LevelUp();
                FPSDisplay.active = !FPSDisplay.active;
                noAction = true;
            }
*/
            else noAction = true;
        }

        else if (beamsCount > 0 && map.HitLocation(point, out x, out y))
        {
            beamsCount--;
            mainUI.ProgressBell();

//          Adding points

            map.AddPoints(x, y, 1);

            for (int c = 1; x + c < Map.maxColumns; c++)
            {
                map.AddPoints(x + c, y, c + 1);
                if (map.mTable[x + c, y].bonus) break;
            }
            for (int c = 1; x - c >= 0 && x - c < Map.maxColumns; c++)
            {
                map.AddPoints(x - c, y, c + 1);
                if (map.mTable[x - c, y].bonus) break;
            }
            for (int c = 1; y + c < Map.maxRows; c++)
            {
                map.AddPoints(x, y + c, c + 1);
                if (map.mTable[x, y + c].bonus) break;
            }
            for (int c = 1; y - c >= 0 && y - c < Map.maxRows; c++)
            {
                map.AddPoints(x, y - c, c + 1);
                if (map.mTable[x, y - c].bonus) break;
            }

            if (x < Map.maxColumns && x >= 0 && y >= 0 && y < Map.maxRows)
            {
                StartCoroutine(BeamOn(new Vector2Int(x, y)));
            }
        }
        else
        {
            noAction = true;
        }
    }

    //  Stop user interaction

    public void UserActionStop()
    {
        noAction = false;

        StopAllCoroutines();
        generalSpeed = levelSpeed;

        for (int c = 0; c < Map.maxColumns; c++)
        {
            for (int r = 0; r < Map.maxRows; r++)
            {
                if (!map.mTable[c, r].status && !map.mTable[c, r].bonus)
                {
                    map.mTable[c, r].iBox.transform.GetChild(0).gameObject.SetActive(false);
                    map.mTable[c, r].iBox.transform.GetChild(1).gameObject.SetActive(false);

                    switch (map.mTable[c, r].counter)
                    {
                        case 1:
                            map.mTable[c, r].iBox.GetComponent<Renderer>().material = map.activeMaterial2;
                            map.mTable[c, r].iBox.transform.GetChild(1).gameObject.SetActive(true);
                            break;
                        case 2:
                            map.mTable[c, r].iBox.GetComponent<Renderer>().material = map.activeMaterial1;
                            break;
                        default:
                            map.mTable[c, r].iBox.GetComponent<Renderer>().material = map.deaMaterial;
                            break;
                    }

                    if (map.mTable[c, r].counter > 0)
                    {
                        map.mTable[c, r].status = true;
                        map.mTable[c, r].counter--;
                    }
                }
            }
        }

        mainUI.UpdateScore();
        mainUI.UpdateBeamsCounter();

        beam.TurnOff();
        noAction = true;

        if (levelScore >= levelGoal[level] && (beamsCount == 0 || noMove) )
        {
            LevelUp();
        }
    }

    private IEnumerator BeamOn(Vector2Int point)
    {
        noAction = false;

        int hitScore = 0;
        Quaternion q = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

        for (int i = 0; i < Map.maxRows + Map.maxColumns; i++)
        {
            for (int c = 0; c < Map.maxColumns; c++)
                for (int r = 0; r < Map.maxRows; r++)
                    if (map.mTable[c, r].points > 0)
                    {
                        map.mTable[c, r].points -= 1;

                        if (map.mTable[c, r].points == 0)
                        {
                            int boxScore = Mathf.RoundToInt(map.mTable[c, r].iBox.transform.position.y / map.mTable[c, r].dYmax * 100.0f);
                            boxScore = boxScore > 100 ? 100 : boxScore;

                            score += boxScore;
                            levelScore += boxScore;
                            hitScore += boxScore;

                            if (map.mTable[c, r].bonus)
                            {
                                if (c < point.x) q.w = 0.0f;
                                if (c > point.x) q.z = 0.0f;
                                if (r > point.y) q.x = 0.0f;
                                if (r < point.y) q.y = 0.0f;

                                map.mTable[c, r].iBox.GetComponent<blowBox>().Activate();
                            }
                            else map.mTable[c, r].iBox.GetComponent<blowBox>().Bang(map.deaMaterial);

                            if (boxScore > 0)
                            {
                                map.mTable[c, r].iBox.transform.GetChild(0).gameObject.SetActive(true);

                                GameObject tmp = map.mTable[c, r].iBox.transform.GetChild(1).gameObject;
                                tmp.GetComponent<TextMesh>().text = "+" + boxScore;
                                tmp.transform.position = new Vector3(tmp.transform.position.x + Random.value * 0.6f, tmp.transform.position.y, tmp.transform.position.z + Random.value * 0.6f);
                                tmp.SetActive(true);
                            }
                        }
                    }

            beam.TurnOn(point, q);

            mainUI.UpdateScore("+ " + hitScore);
            mainUI.UpdateProgress(levelGoal[level]);

            generalSpeed = Mathf.Lerp(generalSpeed, 0.0f, 0.1f * (i - 5));
            yield return new WaitForSeconds(0.04f * generalSpeed);
        }

        noAction = true;

    }

    private  IEnumerator BeamOff()
    {
        Debug.Log("Attention!!");

        while (!noAction)
            yield return new WaitForSeconds(0.1f * generalSpeed);
        UserActionStop();
    }

}
