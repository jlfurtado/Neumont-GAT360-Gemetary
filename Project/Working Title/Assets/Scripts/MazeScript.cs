using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeScript : MonoBehaviour {
    public GameObject MazeSectorPrefab;
    public int SectionSize;
    public float SquareSize;
    public float Radius;

    private Dictionary<IVec2, bool> generated = new Dictionary<IVec2, bool>();

    // Use this for initialization
    void Start () {
        GenerateAround(Vector3.zero, Radius);
	}


    public void GenerateAround(Vector3 position, float radius)
    {
        int lowX = (int)(Mathf.Floor((position.x - radius) / SectionSize));
        int highX = (int)(Mathf.Ceil((position.x + radius) / SectionSize));
        int lowZ = (int)(Mathf.Floor((position.z - radius) / SectionSize));
        int highZ = (int)(Mathf.Ceil((position.z + radius) / SectionSize));

        for (int i = lowX; i <= highX; ++i)
        {
            for (int j = lowZ; j <= highZ; ++j)
            {
                if (!generated.ContainsKey(new IVec2(i, j)))
                {
                    generated.Add(new IVec2(i, j), true);
                    GameObject mazeSection = Instantiate(MazeSectorPrefab);
                    MazeSectionGenerator compRef = mazeSection.GetComponent<MazeSectionGenerator>();
                    compRef.Size = SectionSize;
                    compRef.SquareSize = SquareSize;
                    mazeSection.transform.parent = transform;
                    mazeSection.transform.localPosition = new Vector3(i * SectionSize * SquareSize, 0.0f, j * SquareSize * SectionSize);
                }
            }
        }

    }
}
