using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MRTK.Tutorials.MultiUserCapabilities;

public class OnOffRay : MonoBehaviour
{
    public bool onoffstate;

    // Start is called before the first frame update
    void Start()
    {
        onoffstate = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchOnOff()
    {
        /*
         * 버튼 누를 때마다 Ray On/Off
         */
        if (onoffstate == true)
        {
            onoffstate = false;
        }
        else if (onoffstate == false)
        {
            onoffstate = true;
        }
    }
}
