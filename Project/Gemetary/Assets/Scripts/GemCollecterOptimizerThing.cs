using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCollecterOptimizerThing : MonoBehaviour {
    private List<EatForPoints> flyingGems;
    private RectTransform powerupBarRef;
    private Camera mainCamera;
    private Vector3 screenPowPos;


    void Awake()
    {
        flyingGems = new List<EatForPoints>(10000);

        mainCamera = GameObject.FindGameObjectWithTag(Strings.MAIN_CAMERA_TAG).GetComponent<Camera>();
        powerupBarRef = GameObject.FindGameObjectWithTag(Strings.POWERUP_BAR_TAG).GetComponent<RectTransform>();
        screenPowPos = new Vector3(powerupBarRef.position.x + (powerupBarRef.rect.width) - 25.0f, powerupBarRef.position.y, mainCamera.nearClipPlane);
    }

    void LateUpdate()
    {
        for (int i = 0; i < flyingGems.Count; ++i)
        {
            if (flyingGems[i].Collected) { flyingGems[i].FlyTo(mainCamera, screenPowPos); }
            else { flyingGems.RemoveAt(i--); }
        }
    }

    public void BeginFlyGem(EatForPoints gem)
    {
        gem.transform.parent = mainCamera.transform;
        flyingGems.Add(gem);
    }
}
