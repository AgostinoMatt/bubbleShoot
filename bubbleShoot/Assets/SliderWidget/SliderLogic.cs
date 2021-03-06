﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderLogic : MonoBehaviour
{
    [SerializeField] private Slider slider_obj;
    [SerializeField] private GameObject ShooterObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeAngleEvent()
    {
        // use this to handle changing the shooter angle if needed
        Debug.Log("CHANGING ANGLE : " + slider_obj.value.ToString());
    }

 
    public void MouseUpEvent()
    {
        // use this to handle mouse up, firing the bubble
        Debug.Log("FIRE THE BUBBLE!");

        if (ShooterObj)
        {
            Mkey.BubblesShooter bs = ShooterObj.GetComponent<Mkey.BubblesShooter>();
            if (bs)
            {
                GameObject bubble = bs.GetShootBubble();
                if (bubble)
                {
                    Mkey.ShootBubble shoot_component = bubble.GetComponent<Mkey.ShootBubble>();
                    if (shoot_component)
                    {
                        shoot_component.PointerUp();
                    }
                }
            }
        }
    }

    public void MouseDownEvent()
    {
        // use this to handle mouse down, if needed, maybe to reset the shooter or something?
        Debug.Log("RESET SOMETHING IF WE NEED TO");
    }
}
