using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeScript : MonoBehaviour {
    public GameObject MazeSectorPrefab;
    public int SectionSize;
    public float SquareSize;
    public int NumSections;

    // Use this for initialization
    void Start () {
        GenerateMaze();
	}

    private void GenerateMaze()
    {
        for (int i = 0; i < NumSections; ++i)
        {
            for (int j = 0; j < NumSections; ++j)
            {
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
