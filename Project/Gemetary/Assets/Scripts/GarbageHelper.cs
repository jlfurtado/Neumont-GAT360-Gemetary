using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GarbageHelper
{
    private static long lastMem;
    private static string beginName;

    public static void Begin(string name)
    {
        lastMem = System.GC.GetTotalMemory(false);
        beginName = name;
    }

    public static void End()
    {
        long mem = System.GC.GetTotalMemory(false);
        long diff = mem - lastMem;
        if (diff != 0)
        {
            Debug.Log(beginName + " used " + diff);
        }
    }
}