using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterManager : MonoBehaviour
{
    /// <summary>
    /// Displays and manipulates a linear grouping of letters, for player entry and puzzle letters
    /// </summary>

    public LetterSet letterSet;
    public float gapWidth = 0.1f;
    public Color clearColor;
    public Color failureColor;
    public GameObject sparkle;
    public float sparkleLaunchForce;
    public float sparkleFizzleForce;
    //public Vector3 sparkleLaunchLocation;
    public CameraTools cTools;

    public string LetterString { get
        {
            string letterString = "";
            foreach(LetterInfo i in letterList) {
                letterString += i.letterChar;
            }
            return letterString;
        } }

    private bool needsUpdate = false;
    private class LetterInfo
    {
        public char letterChar;
        public GameObject gameObject;
        public bool available;
        public LetterInfo(char letter, GameObject go, bool avail)
        {
            letterChar = letter;
            gameObject = go;
            available = avail;
        }
    }
    private List<LetterInfo> letterList = new List<LetterInfo>();
    private float totalWidth = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!needsUpdate) return;
        needsUpdate = false;
        PositionLetters();

    }

    public void SetLetters(string letters)
    {
        ClearLetters();

        foreach (char l in letters.ToCharArray())
        {
            GameObject go = Instantiate(letterSet.GetLetterObject(l), transform);
            
            letterList.Add(new LetterInfo(l, go, true));
            totalWidth += GetSpriteWidth(go);
        }
        totalWidth += gapWidth * (letterList.Count - 1);
        needsUpdate = true;
    }

    public void AddLetter(char letter)
    {
        GameObject go = Instantiate(letterSet.GetLetterObject(letter), transform);
        letterList.Add(new LetterInfo(letter, go, true));
        if (letterList.Count > 0)
            totalWidth += gapWidth;
        totalWidth += GetSpriteWidth(go);
        needsUpdate = true;
    }

    public char RemoveLastLetter()
    {
        if (letterList.Count == 0)
            return ('\0');

        LetterInfo info = letterList[letterList.Count - 1];
        letterList.RemoveAt(letterList.Count - 1);

        if (letterList.Count == 0)
            totalWidth = 0;
        else
            totalWidth -= GetSpriteWidth(info.gameObject) + gapWidth;

        Destroy(info.gameObject);
        needsUpdate = true;
        return info.letterChar;
    }

    public void ClearLetters()
    {
        for(int i = 0; i < letterList.Count; i++)
        {
            Destroy(letterList[i].gameObject);
        }
        letterList.Clear();
        totalWidth = 0;
    }

    public void DramaticallyClear(bool failure = false)
    {
        foreach (LetterInfo info in letterList)
        {
            GameObject go = info.gameObject;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            go.GetComponent<SpriteRenderer>().color = failure ? failureColor : clearColor;
            rb.gravityScale = 4;
            rb.AddForceAtPosition(Random.insideUnitCircle.normalized * 100,
                new Vector2(Random.Range(sr.bounds.min.x, sr.bounds.max.x), Random.Range(sr.bounds.min.y, sr.bounds.max.y)));
            StartCoroutine(DestroyAfterDelay(go, 2f));
        }

        letterList.Clear();
        totalWidth = 0;
    }

    public void DramaticallySubmit(int numSparkles, bool fizzle = false)
    {
        //for(int i = 0; i< letterList.Count; i++)
        //{
        //    GameObject letterObj = letterList[i].gameObject;
        //    Vector3 launchPosition = letterObj.transform.position + new Vector3(GetSpriteWidth(letterObj) / 2, -GetSpriteHeight(letterObj) / 2, transform.position.z + 1);

        //    for (int j = 0; j< (int)(numSparkles / letterList.Count); j++)
        //    {
        //        LaunchSparkle(launchPosition, fizzle);

        //    }
        //    if (i < numSparkles % letterList.Count)
        //    {
        //        LaunchSparkle(launchPosition, fizzle);
        //    }
        //}

        LaunchSparkles(numSparkles);

        ClearLetters();
    }

    public void Shuffle()
    {
        for(int i = 0; i < letterList.Count; i++)
        { // for each position in list
            int j = Random.Range(i, letterList.Count);
            if(i != j)
            {
                LetterInfo temp = letterList[i];
                letterList[i] = letterList[j];
                letterList[j] = temp;
            }
        }
        needsUpdate = true;
    }   

    private IEnumerator DestroyAfterDelay(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(go);
    }

    public bool UseLetter(char letter)  // marks first matching available letter as unavailable
                                        // return value indicates success
    {
        for(int i = 0; i < letterList.Count; i++)
        {
            LetterInfo e = letterList[i];
            if (e.letterChar == letter && e.available)
            {
                e.available = false;
                needsUpdate = true;
                return true;
            }

        }
        return false;
    }

    public bool FreeLetter(char letter) // marks first matching unavilable letter as available
                                        // return value indicates success
    {
        for (int i = 0; i < letterList.Count; i++)
        {
            LetterInfo e = letterList[i];
            if (e.letterChar == letter && e.available == false)
            {
                e.available = true;
                needsUpdate = true;
                return true;
            }

        }
        return false;
    }

    public void FreeAllLetters()
    {
        foreach(LetterInfo e in letterList)
        {
            e.available = true;
        }
        needsUpdate = true;
    }

    public string GetString()
    {
        string str = "";
        foreach(LetterInfo e in letterList)
        {
            str += e.letterChar;
        }
        return str;
    }

    public bool IsEmpty()
    {
        return letterList.Count == 0;
    }

    // ***** PRIVATE METHODS

    void PositionLetters()
    {
        float x = transform.position.x - totalWidth / 2;
        foreach (LetterInfo l in letterList)
        {
            GameObject go = l.gameObject;
            go.transform.localPosition = new Vector2(x, 0);
            x += GetSpriteWidth(go) + gapWidth;
            Color c = go.GetComponent<SpriteRenderer>().color;
            go.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, l.available ? 1f : 0.5f);
        }
    }

    float GetSpriteWidth(GameObject go)
    {
        return go.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    float GetSpriteHeight(GameObject go)
    {
        return go.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    // vvvvvvvvv NOT LONGER USED vvvvvvvvvvvvv
    void LaunchSparkle(Vector3 pos, bool fizzle = false)
    {
        GameObject go = Instantiate(sparkle, pos, Quaternion.Euler(0,0,Random.Range(0, 360)));
        //go.transform.position = pos;
        go.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle.normalized * (fizzle ? sparkleFizzleForce : sparkleLaunchForce));
        
        if (fizzle)
            StartCoroutine(DestroyAfterDelay(go, 0.5f));
    }

    void LaunchSparkles(int n)
    {

        Bounds cBounds = cTools.GetOrthographicBounds();
        Vector3 launchPos = new Vector3(cBounds.center.x, cBounds.min.y, transform.position.z + 1);

        for(int i = 0; i < n; i++)
        {
            //Quaternion rotation = Quaternion.AngleAxis(360f - 360f * ((float)i / n) - 270, Vector3.forward);
            Quaternion rotation = Quaternion.AngleAxis(180f * ((float)(i+1) / (n+1)), Vector3.forward);
            GameObject go = Instantiate(sparkle, launchPos, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));
            go.GetComponent<Rigidbody2D>().AddForce( rotation * Vector3.right * sparkleLaunchForce);
        }
    }
}
