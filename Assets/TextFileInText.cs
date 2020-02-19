using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFileInText : MonoBehaviour
{
    public TextAsset txtFile;
    public Text text;

    // Start is called before the first frame update
    void Awake()
    {
        text.text = txtFile.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
