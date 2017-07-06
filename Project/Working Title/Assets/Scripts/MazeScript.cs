using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Bounds
{
    public Bounds(int x, int z, int w, int h) { this.x = x; this.z = z; this.h = h; this.w = w; }
    public int x, w, z, h;

    public bool Equals(Bounds other)
    {
        return this.x == other.x && this.h == other.h && this.z == other.z && this.w == other.w;
    }

    public static Bounds ContainBoth(Bounds a, Bounds b)
    {
        return new Bounds(Mathf.Min(a.x, b.x),
                          Mathf.Min(a.z, b.z),
                          Mathf.Max(a.w, b.w),
                          Mathf.Max(a.h, b.h));
    }

    public bool InBounds(IVec2 pos)
    {
        return pos.x >= x && pos.x <= w && pos.z >= z && pos.z <= h;
    }
}

public class MazeScript : MonoBehaviour {
    public GameObject MazeSectorPrefab;
    public GameObject WallPrefab;
    public GameObject GemPrefab;
    public GameObject FloorPrefab;
    public GameObject PowerupPrefab;
    public int SectionSize;
    public float SquareSize;
    public float Radius;

    private int genTiles;
    private int numSections;
    private int totalTiles;
    private float mazeSize;
    private Dictionary<IVec2, MazeSectionGenerator> generatedMazes = new Dictionary<IVec2, MazeSectionGenerator>(); // TODO: PREVENT DUPLICATE DICTIONARIES
    //private Dictionary<IVec2, int> sectionSeeds = new Dictionary<IVec2, int>();
    private GameObject[] wallPool;
    private EatForPoints[] gemPool;
    private GameObject[] floorPool;
    private GameObject[] powerupPool;
    private GameObject wallHolder;
    private GameObject gemHolder;
    private GameObject floorHolder;
    private GameObject powerupHolder;
    private Bounds lastBounds;
    private System.Random rand = new System.Random();

    private GameObject Parent(GameObject obj, GameObject parent)
    {
        obj.transform.parent = parent.transform;
        return obj;
    }

    // Use this for initialization
    void Start () {
        MazeSectionGenerator.Size = SectionSize;
        MazeSectionGenerator.SquareSize = SquareSize;

        int sideLength = 2 + (2 * (int)Mathf.Ceil(Radius / (SquareSize * SectionSize)));
        numSections = sideLength * sideLength;
        genTiles = SectionSize * SectionSize;
        totalTiles = genTiles * numSections;
        mazeSize = SectionSize * SquareSize;

        Parent(floorHolder = new GameObject(), this.gameObject).name = "FloorHolder";
        Parent(powerupHolder = new GameObject(), this.gameObject).name = "PowerupHolder";
        Parent(wallHolder = new GameObject(), this.gameObject).name = "WallHolder";
        Parent(gemHolder = new GameObject(), this.gameObject).name = "GemHolder";

        floorPool = new GameObject[numSections];
        powerupPool = new GameObject[numSections];
        for (int i = 0; i < numSections; ++i)
        {
            Parent(floorPool[i] = Instantiate(FloorPrefab), floorHolder);
            Parent(powerupPool[i] = Instantiate(PowerupPrefab), powerupHolder);
            floorPool[i].SetActive(false);
            powerupPool[i].SetActive(false);
        }

        wallPool = new GameObject[totalTiles];
        gemPool = new EatForPoints[totalTiles];
        for (int i = 0; i < totalTiles; ++i)
        {
            Parent(wallPool[i] = Instantiate(WallPrefab), wallHolder).name = "Wall";
            Parent((gemPool[i] = Instantiate(GemPrefab).GetComponent<EatForPoints>()).gameObject, gemHolder).name = "Gem";

            wallPool[i].SetActive(false);
            gemPool[i].gameObject.SetActive(false);
        }

        Bounds currentBounds = new Bounds((int)(Mathf.Floor((-Radius) / SectionSize)),
                                          (int)(Mathf.Floor((-Radius) / SectionSize)),
                                          (int)(Mathf.Ceil((+Radius) / SectionSize)),
                                          (int)(Mathf.Ceil((+Radius) / SectionSize)));

        for (int i = currentBounds.x; i <= currentBounds.w; ++i)
        {
            for (int j = currentBounds.z; j <= currentBounds.h; ++j)
            {
                Enable(new IVec2(i, j), (i - currentBounds.x) * (currentBounds.h - currentBounds.z + 1) + (j - currentBounds.z));
            }
        }

        lastBounds = currentBounds;

        GenerateAround(Vector3.zero);
	}

    public void EatGem(IVec2 mazeLoc, IVec2 sectionLoc)
    {
        generatedMazes[mazeLoc].EatGem(sectionLoc.x, sectionLoc.z);
    }

    public void GenerateAround(Vector3 position)
    {
        GenerateAround(position, Radius);
    }

    private void GenerateAround(Vector3 position, float radius)
    {
        Bounds currentBounds = new Bounds((int)(Mathf.Floor((position.x - radius) / SectionSize)),
                                        (int)(Mathf.Floor((position.z - radius) / SectionSize)),
                                        (int)(Mathf.Ceil((position.x + radius) / SectionSize)),
                                        (int)(Mathf.Ceil((position.z + radius) / SectionSize)));

        //if (!lastBounds.Equals(currentBounds))
        //{
        //    if (currentBounds.w > lastBounds.w)
        //    {
        //        int height = (currentBounds.h - currentBounds.z + 1);
        //        for (int i = 0; i < height; ++i)
        //        {
        //            Disable(i);
        //            Enable((currentBounds.w - currentBounds.x) * height + i, new IVec2(currentBounds.x, i + currentBounds.z));
        //        }
        //    }
        //    if (currentBounds.x < lastBounds.x)
        //    {
        //        int height = (currentBounds.h - currentBounds.z);
        //        for (int i = 0; i < height; ++i)
        //        {
        //            Disable((lastBounds.w - lastBounds.x) * height + i);
        //            Enable(i, new IVec2(currentBounds.x, i + currentBounds.z));
        //        }
        //    }

        //    if (currentBounds.h > lastBounds.h)
        //    {
        //        int width = (currentBounds.w - currentBounds.x);
        //        for (int i = 0; i < width; ++i)
        //        {
        //            Disable(i * width + (currentBounds.h - currentBounds.z));
        //            Enable(i* width, new IVec2(i + currentBounds.x, currentBounds.h));
        //        }
        //    }

        //    if (currentBounds.z < lastBounds.z)
        //    {
        //        int width = (currentBounds.w - currentBounds.x);
        //        for (int i = 0; i < width; ++i)
        //        {
        //            Disable(i * width);
        //            Enable(i * width + (currentBounds.h - currentBounds.z), new IVec2(i + currentBounds.x, currentBounds.z));
        //        }
        //    }
        //}

        if (!lastBounds.Equals(currentBounds))
        {
            for (int i = lastBounds.x; i <= lastBounds.w; ++i)
            {
                for (int j = lastBounds.z; j <= lastBounds.h; ++j)
                {
                    Disable(new IVec2(i, j), (i - lastBounds.x) * (lastBounds.h - lastBounds.z + 1) + (j - lastBounds.z));
                }
            }

            for (int i = currentBounds.x; i <= currentBounds.w; ++i)
            {
                for (int j = currentBounds.z; j <= currentBounds.h; ++j)
                {
                    Enable(new IVec2(i, j), (i - currentBounds.x) * (currentBounds.h - currentBounds.z + 1) + (j - currentBounds.z));
                }
            }
        }

        lastBounds = currentBounds;
    }

    private void Disable(IVec2 loc, int idx)
    {
        generatedMazes[loc].gameObject.SetActive(false);

        for (int k = 0; k < genTiles; ++k)
        {
            int w = idx * genTiles + k;
            Parent(wallPool[w], wallHolder).SetActive(false);
            Parent(gemPool[w].gameObject, gemHolder).SetActive(false);
        }
    }

    private void Enable(IVec2 loc, int idx)
    {
        bool newMaze = !generatedMazes.ContainsKey(loc);
        if (newMaze) { generatedMazes.Add(loc, GenerateMazeSection(loc.x, loc.z)); }
        
        MazeSectionGenerator gen = generatedMazes[loc];
        gen.gameObject.SetActive(true);
        gen.gameObject.transform.localPosition = new Vector3(loc.x * mazeSize, 0.0f, loc.z * mazeSize);
        SetMazeParams(gen, idx);

        if (newMaze) { gen.GenerateMaze(rand.Next()); }
        gen.RedoMazeGeometry(loc);
    }

    private void SetMazeParams(MazeSectionGenerator gen, int idx)
    {
        gen.FloorPool[0] = floorPool[idx];
        gen.PowerupPool[0] = powerupPool[idx];
        System.Array.Copy(wallPool, idx * genTiles, gen.WallPool, 0, genTiles);
        System.Array.Copy(gemPool, idx * genTiles, gen.GemPool, 0, genTiles);
    }

    private MazeSectionGenerator GenerateMazeSection(int x, int z)
    {
        GameObject obj = Parent(Instantiate(MazeSectorPrefab), this.gameObject);
        obj.name = ("MazeSection[" + x + "," + z + "]");
        MazeSectionGenerator gen = obj.GetComponent<MazeSectionGenerator>();
        gen.FloorPool = new GameObject[1];
        gen.PowerupPool = new GameObject[1];
        gen.WallPool = new GameObject[genTiles];
        gen.GemPool = new EatForPoints[genTiles];
        return gen;
    }
}
