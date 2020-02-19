using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterSet : MonoBehaviour
{
    /// <summary>
    /// Simple interface to an array of letter prefabs
    /// </summary>
    public GameObject[] letters;

    public GameObject GetLetterObject(string letter)
    {
        return letters[char.ToUpper(letter[0]) - 'A'];
    }

    public GameObject GetLetterObject(char letter)
    {
        return letters[char.ToUpper(letter) - 'A'];
    }
}
