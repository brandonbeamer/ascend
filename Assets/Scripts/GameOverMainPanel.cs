using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMainPanel : MonoBehaviour
{
    public Animator curtainAnim;
    
    public Text resultsText;
    public GameObject pbText;
    private Animator rtAnim;
    private Animator mpAnim;
    private GameObject particleSystemGameObject;

    private void Awake()
    {
        rtAnim = resultsText.GetComponent<Animator>();
        mpAnim = GetComponent<Animator>();
        particleSystemGameObject = FindObjectOfType<ParticleSystem>().gameObject;
    }

    private void Start()
    {
        GameManager.Results results = GameManager.Instance.PlayerResults;
        int level = results != null ? results.Level : 1;
        string lvlStr = level.ToString();
        resultsText.text = "Ascended to Level " + lvlStr;

        pbText.SetActive(false);
        particleSystemGameObject.SetActive(false);

        // check for non-arcade pb
        if (GameManager.Instance.PersonalBest < level && GameManager.Instance.Mode != GameManager.GameMode.Arcade)
        {
            GameManager.Instance.PersonalBest = level;
            pbText.SetActive(true);
            particleSystemGameObject.SetActive(true);
            GameManager.Instance.SavePlayerData();
        }

        // completing arcade always gets shinies, no pb though
        if (GameManager.Instance.Mode == GameManager.GameMode.Arcade) 
        {
            pbText.SetActive(true);
            particleSystemGameObject.SetActive(true);
        }

        // for drab layout, must have been non-arcade, non-pb

        StartCoroutine(StartCR());
    }

    public void Exit()
    {
        StartCoroutine(ExitCR());
    }

    private IEnumerator StartCR()
    {
        curtainAnim.SetTrigger("FadeIn");
        while (!curtainAnim.GetCurrentAnimatorStateInfo(0).IsName("Up"))
            yield return null;

        rtAnim.SetTrigger("FadeIn");
    }

    private IEnumerator ExitCR()
    {
        curtainAnim.SetTrigger("FadeOut");
        mpAnim.SetTrigger("Exit");
        while (!curtainAnim.GetCurrentAnimatorStateInfo(0).IsName("Down"))
            yield return null;

        SceneManager.LoadScene(0);
    }

}
