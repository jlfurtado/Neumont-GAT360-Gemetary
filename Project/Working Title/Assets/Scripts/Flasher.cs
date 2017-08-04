using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour {
    public float FlashUpTime;
    public float FlashDownTime;
    public float EndIntensity;
    private float transitionTime;
    private bool isFlashing;
    private bool flashingUp;
    private float prevIntensity;

    void Awake () {
        isFlashing = false;
	}
	
	void Update () {
		if (isFlashing)
        {
            transitionTime -= Time.deltaTime;
            if (flashingUp)
            {
                RenderSettings.ambientIntensity = Mathf.Lerp(prevIntensity, EndIntensity, 1.0f - (transitionTime / FlashUpTime));

                if (transitionTime <= 0.0f)
                {
                    Transition();
                }
            }
            else
            {
                RenderSettings.ambientIntensity = Mathf.Lerp(EndIntensity, prevIntensity, 1.0f - (transitionTime / FlashDownTime));

                if (transitionTime <= 0.0f)
                {
                    StopFlashing();
                }
            }
        }
	}

    private void Transition()
    {
        flashingUp = false;
        transitionTime = FlashDownTime;
        RenderSettings.ambientIntensity = EndIntensity;
    }

    private void StopFlashing()
    {
        isFlashing = false;
        flashingUp = false;
        RenderSettings.ambientIntensity = prevIntensity;
        transitionTime = 0.0f;
    }

    public void Flash()
    {
        if (!isFlashing)
        {
            isFlashing = true;
            flashingUp = true;
            prevIntensity = RenderSettings.ambientIntensity;
            transitionTime = FlashUpTime;
        }
    }
}
