using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainPanel : MonoBehaviour
{
    private Animator anim;
    public Button continueButton;
    public AudioMixer mixer;
    public Text currentRecordText;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Start()
    {
        if (GameManager.Instance.PlayerLevel == GameManager.DEFAULT_PLAYERLEVEL)
            continueButton.interactable = false;
        else
            continueButton.interactable = true;

        if(GameManager.Instance.PersonalBest > 0)
        {
            currentRecordText.text = "Record: " + GameManager.Instance.PersonalBest.ToString();
            currentRecordText.gameObject.SetActive(true);
        }
        else
        {
            currentRecordText.gameObject.SetActive(false);
        }
        Debug.Log(Application.persistentDataPath);
    }

    public void StartButton()
    {
        anim.SetTrigger("Exit");

        GameManager.Instance.Mode = GameManager.GameMode.Normal;
        GameManager.Instance.ResetPlayerLevel();

        StartCoroutine(StartButtonCR());
    }

    public void ContinueButton() // start arcade mode
    {
        anim.SetTrigger("Exit");
        StartCoroutine(StartButtonCR());
    }

    private IEnumerator StartButtonCR()
    {
        float masterlevel;
        if(!mixer.GetFloat("MasterLevel", out masterlevel))
        {
            Debug.LogError("Couldn't get master sound level.");
        }

        AnimationCurve volCurve = new AnimationCurve();
        volCurve.AddKey(new Keyframe(0, masterlevel));
        volCurve.AddKey(new Keyframe(1, -80f));
        float volCurveStart = Time.time;

        while(Time.time - volCurveStart < 1f)
        {
            mixer.SetFloat("MasterLevel", volCurve.Evaluate(Time.time - volCurveStart));
            yield return null;
        }

        while (true)
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("MainPanelIdle"))
                break;
            else
            {
                yield return null;
            }


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return null;
    }
}
