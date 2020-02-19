using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintCostText : MonoBehaviour
{
    public HintAbilityManager ham;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = ham.cost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
