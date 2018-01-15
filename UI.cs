using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {

    private const float maxPScale = 1.0f;
    private const float minPScale = 41.0f;

    public static bool isMenuOn = false;                                // Current menu condition

    public TextMesh score;                                              // Main text field
    public TextMesh beamsCounter;                                       // Where the beams count is shown

    public Animator uiBoxOne;
    public Animator uiBoxMenu;

    public GameObject progressBar;                                      // Level progress displayed here
    public Material uiBarMat;                                           // Progress bar background "gradient" material
    public Material fxMat;                                              // Material used for particle effects

    public Demo demo;                                                   // Main game process

    public float uiMenuScale = 10.0f;


    public void SetColor(Color color)
    {
        SetColor(color, demo.palette[Demo.level + 1]);
    }

    public void SetColor(Color color, Color nextColor)
    {
        color.a = 255;
        score.color = beamsCounter.color = color;

        uiBarMat.SetColor("_Color", color);
        uiBarMat.SetColor("_Color2", nextColor);

        fxMat.SetColor("_EmissionColor", color);

        demo.map.activeMaterial1.SetColor("_EmissionColor", color);
        demo.map.activeMaterial2.SetColor("_EmissionColor", nextColor);

        Material tmp = uiBoxMenu.GetComponent<Renderer>().material;
        tmp.SetColor("_EmissionColor", color);
    }

    public void UpdateScore( string value = "" )                        // Set value can't be empty
    {
        if (string.IsNullOrEmpty(value))
        {
            score.text = Demo.score / 1000 > 0 ? Demo.score.ToString("### ###") : Demo.score.ToString();
        }
        else score.text = value;
    }

    public void UpdateBeamsCounter( int value = 10 )                    // This element designed to display only one numeral, so 10 is error condition
    {
        if (value >= 0 && value <= 9)
        {
            beamsCounter.text = value.ToString();
        }
        else
        {
            beamsCounter.text = Demo.beamsCount.ToString();
        }
    }

    public void ProgressBell()                                          // Let's ring the bell
    {
        uiBoxOne.Play("ui_progress_bar", 0);
    }

    public void SetProgressToStart()
    {
        progressBar.transform.localScale = new Vector3(minPScale, 1.0f, 1.0f);
    }

    public void UpdateProgress( int max = 5000 )
    {
        Debug.Log(" " + max);
        progressBar.transform.localScale = Demo.levelScore < max ? new Vector3(minPScale - minPScale * ((float) Demo.levelScore / max), progressBar.transform.localScale.y, progressBar.transform.localScale.z) : new Vector3(maxPScale, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
    }

    public void ActivateMenu()
    {
        uiBoxMenu.SetBool("active", true);
        uiBoxMenu.speed = 6.0f;

        isMenuOn = true;
        StartCoroutine(WhaitNoAct(uiBoxMenu));

        score.gameObject.SetActive(false);
        beamsCounter.gameObject.SetActive(false);
    }

    public void DeactivateMenu()
    {
        uiBoxMenu.SetBool("active", false);
        uiBoxMenu.speed = 1.0f;

        isMenuOn = false;
        StartCoroutine(WhaitNoAct(uiBoxMenu));

        score.gameObject.SetActive(true);
        beamsCounter.gameObject.SetActive(true);
    }

    public void Exit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void Reaction(Vector2 point)
    {
        float ratio = (float) Screen.height / Screen.width;

        int x = Mathf.CeilToInt((point.x + (Screen.height - Screen.width) * 0.5f)  / (Screen.width / uiMenuScale));
        int y = Mathf.CeilToInt(point.y / (Screen.height / (ratio * uiMenuScale)));

        if (x < y)
        {
            if (x > Mathf.CeilToInt(ratio * uiMenuScale - y))
            {
//              MT
                Application.OpenURL("https://about.me/nanom");
            }
            else
            {
//              Play
                DeactivateMenu();
            }
        }
        else
        {
            if (x < Mathf.CeilToInt(ratio * uiMenuScale - y))
            {
//              Exit
                Exit();
            }
            else
            {
//              Restart
                demo.DropNull();
                demo.map.Setup();
                DeactivateMenu();
            }
        }
    }

    private IEnumerator WhaitNoAct(Animator anim)
    {
        yield return new WaitForEndOfFrame();
        float length = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);
        Demo.noAction = true;
    }

}
