using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisInputHelper : MonoBehaviour {
    private bool[] lastAxisStates = null;
    private bool[] currentAxisStates = null;

    void Awake()
    {
        lastAxisStates = new bool[Strings.AXIS_KEYS.Length];
        currentAxisStates = new bool[lastAxisStates.Length];
    }

    public void Update()
    {
        for (int i = 0; i < currentAxisStates.Length; ++i)
        {
            lastAxisStates[i] = currentAxisStates[i];
            currentAxisStates[i] = (Mathf.RoundToInt(Input.GetAxis(Strings.AXIS_KEYS[i])) != 0);
            //Debug.Log(Strings.AXIS_KEYS[i] + ": " + (lastAxisStates[i] ? "1" : "0") + ":" + (currentAxisStates[i] ? "1" : "0"));
        }
    }

    public bool AxisPressed(int index)
    {
        return currentAxisStates[index] && !lastAxisStates[index];
    }
}
