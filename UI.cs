using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {

    private const float maxPScale = 1.0f;
    private const float minPScale = 41.0f;

    public TextMesh score;                                              // Main text field
    public TextMesh beamsCounter;                                       // 

    public Animator uiBoxOne;
    public Animator uiBoxMenu;

    public GameObject progressBar;

    public void UpdateScore()
    {
        score.text = Demo.Score / 1000 > 0 ? Demo.Score.ToString("### ###") : Demo.Score.ToString();
    }

    public void UpdateScore( string value )
    {
        score.text = value;
    }

    public void UpdateBeamsCounter()
    {
        beamsCounter.text = Demo.beamsCount.ToString();
    }

    public void UpdateBeamsCounter( int value )
    {
        if ( value >= 0 && value <= 9)
        {
            beamsCounter.text = value.ToString();
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

    public void UpdateProgress()
    {
        progressBar.transform.localScale = Demo.Score < 5000 ? new Vector3(minPScale - minPScale * (Demo.Score / 5000.0f), progressBar.transform.localScale.y, progressBar.transform.localScale.z) : new Vector3(maxPScale, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
    }

    public void UpdateProgress( int max )
    {
        progressBar.transform.localScale = Demo.Score < max ? new Vector3(minPScale - minPScale * (Demo.Score / max), progressBar.transform.localScale.y, progressBar.transform.localScale.z) : new Vector3(maxPScale, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
    }

    public void ActivateMenu()
    {
        uiBoxMenu.SetBool("active", true);
        uiBoxMenu.speed = 6.0f;

        score.gameObject.SetActive(false);
        beamsCounter.gameObject.SetActive(false);
    }

    public void DeactivateMenu()
    {
        uiBoxMenu.SetBool("active", false);
        uiBoxMenu.speed = 1.0f;

        score.gameObject.SetActive(true);
        beamsCounter.gameObject.SetActive(true);
    }

}
