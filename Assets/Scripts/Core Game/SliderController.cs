using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SliderController : MonoBehaviour
{
    //cache
    KinematicBubbleManager kinematicBubbleManager=null;
    Slider slider = null;

    //config
    [SerializeField] float sliderIncreseSpeed = 1;


    //variable
    float sliderCurrentValue = 0;

    private void Start()
    {
        kinematicBubbleManager = FindObjectOfType<KinematicBubbleManager>();
        slider = GetComponent<Slider>();
        // initialize slider max value
        slider.maxValue = kinematicBubbleManager.GetRemainingBubbleCount() * 60;

    }

    private void FixedUpdate()
    {
        if(slider.value - sliderCurrentValue <= Mathf.Epsilon && sliderCurrentValue != 0)
        {
            slider.value += Time.deltaTime * sliderIncreseSpeed;
        }
    }


    public void SetAmount(float amount)
    {
        sliderCurrentValue += amount;
    }
}
