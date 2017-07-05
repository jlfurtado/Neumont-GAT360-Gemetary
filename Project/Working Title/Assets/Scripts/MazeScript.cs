using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeScript : MonoBehaviour {
    public GameObject MazeSectorPrefab;
    public int SectionSize;
    public float SquareSize;
    public float Radius;
 
    private Dictionary<IVec2, GameObject> generated = new Dictionary<IVec2, GameObject>();
    private int lastLowX, lastHighX, lastLowZ, lastHighZ;
    private bool inCoroutine = false;

    // Use this for initialization
    void Start () {
        MazeSectionGenerator.Size = SectionSize;
        MazeSectionGenerator.SquareSize = SquareSize;
        GenerateAround(Vector3.zero);
	}

    public void GenerateAround(Vector3 position)
    {
        if (!inCoroutine)
        {
            StartCoroutine(GenerateAround(position, Radius));
        }
    }

    private IEnumerator GenerateAround(Vector3 position, float radius)
    {
        inCoroutine = true;
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        float mazeSize = SquareSize * SectionSize;

        int lowX = (int)(Mathf.Floor((position.x - radius) / SectionSize));
        int highX = (int)(Mathf.Ceil((position.x + radius) / SectionSize));
        int lowZ = (int)(Mathf.Floor((position.z - radius) / SectionSize));
        int highZ = (int)(Mathf.Ceil((position.z + radius) / SectionSize));

        if (lastLowX != lowX || lastHighX != highX || lastLowZ != lowZ || lastHighZ != highZ)
        {
            foreach (IVec2 v in generated.Keys)
            {
                bool outBounds = v.x < lowX || v.x > highX || v.z < lowZ || v.z > highZ;
                generated[v].SetActive(!outBounds);
            }

            for (int i = lowX; i <= highX; ++i)
            {
                for (int j = lowZ; j <= highZ; ++j)
                {
                    IVec2 currentSection = new IVec2(i, j);
                    if (!generated.ContainsKey(currentSection))
                    {
                        GameObject mazeSection = Instantiate(MazeSectorPrefab);
                        mazeSection.transform.parent = transform;
                        mazeSection.transform.localPosition = new Vector3(i * mazeSize, 0.0f, j * mazeSize);
                        generated.Add(currentSection, mazeSection);
                        yield return wait;
                    }
                }
            }
        }

        lastLowX = lowX;
        lastLowZ = lowZ;
        lastHighX = highX;
        lastHighZ = highZ;
        inCoroutine = false;
    }
}
