using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Click : MonoBehaviour
{
    [SerializeField]
    int x, y;
   
    public void ButtonClick()
    {
        //Debug.Log(gameObject.name);
        GameManager.getGM.GameButtonClick(x, y);
    }
}
