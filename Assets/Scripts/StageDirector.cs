using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class StageDirector : MonoBehaviour
{
    public static StageDirector Instance;
    public Vector3 SimulatedCameraVelocity { get; set; } = new Vector3(0, 50, 0);

    public LevelManager lm;
    public LevelNumberTextManager lntm;
    public QuoteTextManager qtm;
    public GamePanelManager gpm;
    public Animator curtainAnimator;
    public GameObject escapePanel;
    public AudioSource bgn;
    public AudioMixer mixer;
    public GameObject tutorialPanel;
    public GameObject tutorialBox;

    private bool sceneIntroFlag = false;
    //private bool tutorialOverFlag = false;
    private bool waitForAnyKeyFlag = false;
    private bool butNotEsc = false; // combine with waitForAnyKeyFlag to omit escape from consideration
    private bool escapeToLeave = false;
    private bool playerLeaving = false;

    public void Awake()
    {
        //lm = FindObjectOfType<LevelManager>();
        //lntm = FindObjectOfType<LevelNumberTextManager>();
        //qtm = FindObjectOfType<QuoteTextManager>();
        //gpm = FindObjectOfType<GamePanelManager>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Tried in instantiate two stage directors!");
            Destroy(gameObject);
        }

    }

    public void Update()
    {
        if (waitForAnyKeyFlag && Input.anyKeyDown) {
            if(!butNotEsc || !Input.GetKeyDown(KeyCode.Escape))
            {
                waitForAnyKeyFlag = false;
                butNotEsc = false;
            }
        }

        if(escapeToLeave && Input.GetKeyDown(KeyCode.Escape))
        {
            playerLeaving = true;
        }
    }

    public void DoSceneIntro() // Before each level starts
    {
        sceneIntroFlag = true;
        StartCoroutine(DoLevelTransitionCR()); // maybe this'll be different in the future
    }

    // private  IEnumerator DoSceneIntroCR() {
    //    TODO?
    // }

    public void DoLevelTransition() // Before each level starts
    {
        StartCoroutine(DoLevelTransitionCR());
    }

    private IEnumerator DoLevelTransitionCR()
    {
        Animator levelNumAnim = lntm.GetComponent<Animator>();
        Animator quoteAnim = qtm.GetComponent<Animator>();
        Animator gpmAnim = gpm.GetComponent<Animator>();

        bool wait = false; // will be used for nexted coroutines

        // pick up the pace
        SimulatedCameraVelocity = new Vector3(0, 50, 0);
        bgn.volume = 1f;

        // ask any sparkles to leave
        foreach(SparkleMotion s in FindObjectsOfType<SparkleMotion>())
        {
            s.Leave();
        }

        if(GameManager.Instance.PlayerLevel != 1)
            escapePanel.SetActive(true);

        // either fade in or chill for a moment
        if (sceneIntroFlag)
        {
           //sceneIntroFlag = false;
            curtainAnimator.SetTrigger("FadeIn");
            float masterlevel;
            if (!mixer.GetFloat("MasterLevel", out masterlevel))
            {
                Debug.LogError("Couldn't get master sound level.");
            }

            AnimationCurve volCurve = new AnimationCurve();
            volCurve.AddKey(new Keyframe(0, -80f));
            volCurve.AddKey(new Keyframe(1, 0f));
            float volCurveStart = Time.time;

            while (Time.time - volCurveStart < 1f)
            {
                mixer.SetFloat("MasterLevel", volCurve.Evaluate(Time.time - volCurveStart));
                yield return null;
            }
        }
        else
        {

            yield return new WaitForSeconds(2f);
        }

        // once the curtains are up (fade in complete)
        while (!curtainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Up"))
            yield return null;

        // show the level number text
        levelNumAnim.SetTrigger("FadeIn");     // Text fades in
        //quoteAnim.SetTrigger("FadeIn");

        yield return null;                      // wait a frame so the keydown bullshit doesn't happen

        // wait or keypress
        waitForAnyKeyFlag = true;                               // Player must press a key to start gameplay
        //butNotEsc = true;                                       // Don't treat Esc as an "any key"
        escapeToLeave = true;                                   // allow player to leave by pressing escape
        while (waitForAnyKeyFlag)
        {
            yield return null;
        }

        waitForAnyKeyFlag = false;
        escapeToLeave = false;

        // turn off the escape to leave notification
        escapePanel.SetActive(false);

        // fade out
        levelNumAnim.SetTrigger("FadeOut");     // Text fades out
        //quoteAnim.SetTrigger("FadeOut");


        while (!levelNumAnim.GetCurrentAnimatorStateInfo(0).IsName("Ready")) // Wait for fade out to complete
        {
            yield return null;
        }


        // after fade, branch depending on whether player decided to leave
        if(!playerLeaving)
        {
            // if not leaving, start a new level
            lm.StartNewLevel(); // the game panel watches for play states on its own

            while (!gpmAnim.GetCurrentAnimatorStateInfo(0).IsName("Active")) // wait for game panel to be fully on screen
            {
                yield return null;
            }

            // slow down camera when gameplay starts
            SimulatedCameraVelocity = new Vector3(0, 1, 0);                 // slow down the camera for the next level
            bgn.volume = 0.5f;
        }
        else
        {
            // if player is leaving
            // fade out
            curtainAnimator.SetTrigger("FadeOut");
            yield return null;                                              // wait for animator to start
            while (!curtainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Down")) // wait for fadeout to complete
                yield return null;

            // after fade, go to menu
            SceneManager.LoadScene(0); // main menu
            yield break;
        }


        // Do tutorial
        if(sceneIntroFlag && GameManager.Instance.ShowTutorial)
        {      

            wait = true;
            StartCoroutine(DoTutorial(x => wait = !x));
            while (wait) yield return null;
            
            
        }

        sceneIntroFlag = false;
    }

    public  void DoLoseGame() // When the player loses
    {
        StartCoroutine(DoLoseGameCR());
    }

    public IEnumerator DoLoseGameCR()
    {
        curtainAnimator.SetTrigger("FadeOut");
        while (!curtainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Down")) // wait for fadeout to complete
            yield return null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public  void DoLeaveGame() // When the player stops volunarily
    {
        SceneManager.LoadScene(0);
    }

    void RemoveSparkles()
    {
        GameObject[] sparkles = GameObject.FindGameObjectsWithTag("Sparkle");
        foreach (GameObject s in sparkles)
        {
            Destroy(s);
        }
    }

    #region TUTORIAL 

    void tutorialAdvanceButtonClickedHandler()
    {
        
    }

    IEnumerator DoTutorial(System.Action<bool> finished)
    {
        bool wait = false;
        Time.timeScale = 0f; // stop time
        GameManager.Instance.TutorialActiveFlag = true;
        tutorialPanel.SetActive(true);
        GameObject go;

        do { 
            wait = true;
            go = ShowTutorialBox(0f, 0f, 850f, 100f, 
                "Welcome",
@"Welcome to Ascend, the anagram game for those who are 
a <b>little</b> more serious about anagrams.", delegate () { wait = false;  });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, 0f, 850f, 100f,
                "Welcome",
@"Your goal is simple: ascend to the highest level you can, 
<b>then higher the next time</b>.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, 0f, 850f, 140f,
                "Puzzle Letters",
@"In the center here are the puzzle letters. Shuffle them 
around with <b>SPACEBAR</b> (if needed) and type any hidden 
words you find.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(400f, 200f, 850f, 140f,
                "Quest Words",
@"Each level has one or more quest words, which are 
displayed up here. <b>Quest words are hidden words that you 
must find</b> in order to clear the level.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, 200f, 850f, 100f,
                "Timer",
@"At the top of the screen is the timer. If it expires before
all quest words are found, your ascent is over.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, 200f, 850f, 200f,
                "Timer",
@"After all quest words are found, the level is cleared and 
the timer turns yellow to indicate this. It's OK if the 
timer expires after the level is cleared; you'll 
automatically ascend to the next. If you don't want to wait, 
<b>UPARROW</b> will ascend immediately.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(-400f, -200f, 850f, 200f,
                "Inspiration",
@"Down here is your inspiration counter. You earn inspiration
for making words with the puzzle letters. <b>Earning as
much inspiration as you can is key</b>, because hint abilities
cost inspiration and the puzzles are going to get really 
challenging really fast.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, -200f, 850f, 140f,
                "Hint Abilities",
@"Over here are your hint abilities and their costs below. 
You can use any ability that you can afford (with 
inspiration) by pressing the number key associated with it.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, -200f, 850f, 200f,
                "Hint Abilities",
@"You have three hint abilities at your disposal:

1.  Reveal a random letter of every quest word.
2.  Reveal the first and last letter of every quest word.
3.  Flip a fair coin to reveal each letter of every quest word.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(-400f, -200f, 850f, 100f,
                "Anagram Chains",
@"The secret to earning lots of inspiration is to build 
<b>anagram chains</b>.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(-400f, -200f, 850f, 180f,
                "Anagram Chains",
@"When you enter two (or more) words in succession which 
are anagrams of each other, you build an <b>inspiration 
multiplier</b>. A 2-chain is worth twice as much as normal. 
A 3-chain is worth 3 times as much, and so on.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(-400f, -200f, 850f, 240f,
                "Anagram Chains",
@"Wherever possible, <b>exhaust the anagrams of the word 
you just entered</b> before using a different set of letters.
For example: given the letters S O T P, entering TOP, POT, 
and OPT, before moving on to TOPS, OPTS, and POTS is 
worth much more than, for instance, TOP, TOPS, POT, POTS, 
OPT, OPTS, and so on.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, 0f, 850f, 140f,
                "Leaving",
@"Leave at any time by pressing <b>ESCAPE</b>. If you leave 
between levels, your progress will be saved. If you leave 
during a level, your progress will be reset.", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);

            if (!GameManager.Instance.ShowTutorial) break;

            wait = true;
            go = ShowTutorialBox(0f, 0f, 850f, 60f,
                "Good Luck!",
@"That's it! How high will <b>you</b> ascend?", delegate () { wait = false; });
            while (wait) yield return null;
            Destroy(go);
        } while (false);

        tutorialPanel.SetActive(false);
        GameManager.Instance.TutorialActiveFlag = false;
        Time.timeScale = 1f;
        finished(true);
    }

    private GameObject ShowTutorialBox(float x, float y, float width, float height, string title, string content, TutorialBox.ButtonClickDelegate onClose)
    {
        GameObject go = Instantiate(tutorialBox, tutorialPanel.transform, false);
        TutorialBox tBox = go.GetComponent<TutorialBox>();
        tBox.Title = title;
        tBox.Content = content;
        tBox.advanceButtonClickedHandler = onClose;
        tBox.SetSize(width, height);
        tBox.transform.localPosition = new Vector3(x, y, 0);

        return go;
    }
    #endregion
}
