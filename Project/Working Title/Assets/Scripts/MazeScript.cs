using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeScript : MonoBehaviour {
    public Text GemsInSectionText;
    public GameObject ChaseEnemyPrefab;
    public GameObject MazeSectorPrefab;
    public GameObject WallPrefab;
    public GameObject GemPrefab;
    public GameObject FloorPrefab;
    public GameObject PowerupPrefab;
    public GameObject FollowEnemyPrefab;
    public GameObject DepthEnemyPrefab;
    public GameObject RestorerPrefab;
    public Material[] GemColors;
    public Material[] FloorColors;

    public int SectionSize;
    public float SquareSize;
    public int RenderDistance;

    private int genTiles;
    private int numSections;
    private int totalTiles;
    private float mazeSize;
    private Dictionary<string, MazeSectionGenerator> generatedMazes = new Dictionary<string, MazeSectionGenerator>(10000);
    private GameObject[] wallPool;
    private GameObject[] restorerPool;
    private EatForPoints[] gemPool;
    private FollowMazeSolution[] followSolutionPool;
    private DepthFirstExplore[] depthEnemyPool;
    private ChaseEnemy[] chaseEnemyPool;
    private Renderer[] floorPool;
    private Powerup[] powerupPool;
    private GameObject wallHolder;
    private GameObject gemHolder;
    private GameObject floorHolder;
    private GameObject powerupHolder;
    private GameObject followHolder;
    private GameObject chaseHolder;
    private GameObject depthHolder;
    private GameObject restorerHolder;
    private IVec2 lastSection;
    private System.Random rand = new System.Random();
    private int sideLength;

    private string ToKey(int x, int z)
    {
        return "X:" + x + "-Z:" + z;
    }

    private string ToKey(IVec2 pos)
    {
        return ToKey(pos.x, pos.z);
    }

    private GameObject Parent(GameObject obj, GameObject parent)
    {
        obj.transform.parent = parent.transform;
        return obj;
    }

    // Use this for initialization
    void Start () {
        MazeSectionGenerator.Size = SectionSize;
        MazeSectionGenerator.SquareSize = SquareSize;

        sideLength = 1 + 2 * RenderDistance;
        numSections = sideLength * sideLength;
        genTiles = SectionSize * SectionSize;
        totalTiles = genTiles * numSections;
        mazeSize = SectionSize * SquareSize;

        Parent(floorHolder = new GameObject(), this.gameObject).name = "FloorHolder";
        Parent(powerupHolder = new GameObject(), this.gameObject).name = "PowerupHolder";
        Parent(wallHolder = new GameObject(), this.gameObject).name = "WallHolder";
        Parent(gemHolder = new GameObject(), this.gameObject).name = "GemHolder";
        Parent(followHolder = new GameObject(), this.gameObject).name = "FollowEnemyHolder";
        Parent(depthHolder = new GameObject(), this.gameObject).name = "DepthEnemyHolder";
        Parent(chaseHolder = new GameObject(), this.gameObject).name = "ChaseHolder";
        Parent(restorerHolder = new GameObject(), this.gameObject).name = "RestorerHolder";

        restorerPool = new GameObject[numSections];
        followSolutionPool = new FollowMazeSolution[numSections];
        depthEnemyPool = new DepthFirstExplore[numSections];
        chaseEnemyPool = new ChaseEnemy[numSections];
        floorPool = new Renderer[numSections];
        powerupPool = new Powerup[numSections];
        for (int i = 0; i < numSections; ++i)
        {
            Parent((followSolutionPool[i] = Instantiate(FollowEnemyPrefab).GetComponent<FollowMazeSolution>()).gameObject, followHolder);
            Parent((depthEnemyPool[i] = Instantiate(DepthEnemyPrefab).GetComponent<DepthFirstExplore>()).gameObject, depthHolder);
            Parent((chaseEnemyPool[i] = Instantiate(ChaseEnemyPrefab).GetComponent<ChaseEnemy>()).gameObject, chaseHolder);
            Parent((powerupPool[i] = Instantiate(PowerupPrefab).GetComponent<Powerup>()).gameObject, powerupHolder);
            Parent((floorPool[i] = Instantiate(FloorPrefab).GetComponent<Renderer>()).gameObject, floorHolder);
            Parent(restorerPool[i] = Instantiate(RestorerPrefab), restorerHolder);

            followSolutionPool[i].gameObject.SetActive(false);
            depthEnemyPool[i].gameObject.SetActive(false);
            chaseEnemyPool[i].gameObject.SetActive(false);
            floorPool[i].gameObject.SetActive(false);
            powerupPool[i].gameObject.SetActive(false);
            restorerPool[i].SetActive(false);
        }

        wallPool = new GameObject[totalTiles];
        gemPool = new EatForPoints[totalTiles];
        wallHolder.transform.position = Vector3.down * 10.0f;
        gemHolder.transform.position = Vector3.down * 10.0f;
        for (int i = 0; i < totalTiles; ++i)
        {
            Parent(wallPool[i] = Instantiate(WallPrefab), wallHolder).name = "Wall";
            Parent((gemPool[i] = Instantiate(GemPrefab).GetComponent<EatForPoints>()).gameObject, gemHolder).name = "Gem";

            wallPool[i].SetActive(false);
            gemPool[i].gameObject.SetActive(false);
        }

        int halfSide = (sideLength-1) / 2;
        for (int i = -halfSide; i <= halfSide; ++i)
        {
            for (int j = -halfSide; j <= halfSide; ++j)
            {
                Enable(i, j, true);
            }
        }

        SetGemsText(generatedMazes[ToKey(0, 0)].NumGems());

        lastSection = new IVec2(0, 0);

        GenerateAround(Vector3.zero);
	}

    public void EatAt(IVec2 mazeLoc, IVec2 sectionLoc)
    {
        generatedMazes[ToKey(mazeLoc)].EatAt(sectionLoc.x, sectionLoc.z);
        SetGemsText(generatedMazes[ToKey(mazeLoc)].NumGems());

    }

    public void EatAt(Vector3 pos)
    {
        int halfSize = SectionSize / 2;
        float mhs = mazeSize / 2;
        IVec2 mazeLoc = new IVec2((int)Mathf.Floor((pos.x + mhs) / mazeSize), (int)Mathf.Floor((pos.z + mhs) / mazeSize));
        IVec2 sectionLoc = new IVec2((int)Mathf.Floor((pos.x - (mazeLoc.x * mazeSize)) / SquareSize) + (halfSize), (int)Mathf.Floor((pos.z - (mazeLoc.z * mazeSize)) / SquareSize) + (halfSize));
        EatAt(mazeLoc, sectionLoc);
    }

    public MazeSectionGenerator SectionAt(Vector3 pos)
    {
        float mhs = mazeSize / 2;
        IVec2 mazeLoc = new IVec2((int)Mathf.Floor((pos.x + mhs) / mazeSize), (int)Mathf.Floor((pos.z + mhs) / mazeSize));
        return generatedMazes[ToKey(mazeLoc)];
    }

    public IVec2 SectionLocFor(Vector3 pos)
    {
        int halfSize = SectionSize / 2;
        float mhs = mazeSize / 2;
        IVec2 mazeLoc = new IVec2((int)Mathf.Floor((pos.x + mhs) / mazeSize), (int)Mathf.Floor((pos.z + mhs) / mazeSize));
        return new IVec2((int)Mathf.Floor((pos.x - (mazeLoc.x * mazeSize)) / SquareSize) + (halfSize), (int)Mathf.Floor((pos.z - (mazeLoc.z * mazeSize)) / SquareSize) + (halfSize));
    }

    private int Mod(int n, int m)
    {
        return ((n % m) + m) % m;
    }

    private int MagicMath(int x, int z)
    {
        return Mod(x, sideLength) * sideLength + Mod(z, sideLength);
    }

    public void GenerateAround(Vector3 position)
    {
        GenerateNear(position);
    }

    private void GenerateNear(Vector3 position)
    {
        IVec2 currentSection = new IVec2((int)(Mathf.Floor((position.x + (mazeSize / 2)) / mazeSize)),
                                        (int)(Mathf.Floor((position.z + (mazeSize / 2)) / mazeSize)));

        if (!lastSection.Equals(currentSection))
        {
            if (currentSection.x > lastSection.x)
            {
                // move all the old left ones to the new right
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x - RenderDistance, i + lastSection.z - RenderDistance);
                    Enable(currentSection.x + RenderDistance, i + currentSection.z - RenderDistance, false);
                }
            }
            else if (currentSection.x < lastSection.x)
            {
                // move all the old right ones to the new left
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x + RenderDistance, i + lastSection.z - RenderDistance);
                    Enable(currentSection.x - RenderDistance, i + currentSection.z - RenderDistance, false);
                }
            }

            if (currentSection.z > lastSection.z)
            {
                // move all the top old ones to the new bottom
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x - RenderDistance + i, lastSection.z - RenderDistance);
                    Enable(currentSection.x - RenderDistance + i, currentSection.z + RenderDistance, false);
                }
            }
            else if (currentSection.z < lastSection.z)
            {
                // move all the bottom old ones to the new top
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x - RenderDistance + i, lastSection.z + RenderDistance);
                    Enable(currentSection.x - RenderDistance + i, currentSection.z - RenderDistance, false);
                }
            }

            SetGemsText(generatedMazes[ToKey(currentSection.x, currentSection.z)].NumGems());
        }

        lastSection = currentSection;
    }

    private void SetGemsText(int numGems)
    {
        GemsInSectionText.text = "Gems In Section: " + numGems;
    }

    private void Disable(int x, int z)
    {
        string mazeKey = ToKey(x, z);
        int idx = MagicMath(x, z);
        if (!generatedMazes.ContainsKey(mazeKey)) { return; }

        generatedMazes[mazeKey].gameObject.SetActive(false);

        for (int k = 0; k < genTiles; ++k)
        {
            int w = idx * genTiles + k;
            Parent(wallPool[w], wallHolder);
            Parent(gemPool[w].gameObject, gemHolder);
        }
    }

    private void Enable(int x, int z, bool sync)
    {
        string mazeKey = ToKey(x, z);
        int idx = MagicMath(x, z);
        bool newMaze = !generatedMazes.ContainsKey(mazeKey);
        if (newMaze) { generatedMazes.Add(mazeKey, GenerateMazeSection(x, z)); }
        
        MazeSectionGenerator gen = generatedMazes[mazeKey];
        gen.gameObject.SetActive(true);
        gen.gameObject.transform.localPosition = new Vector3(x * mazeSize, 0.0f, z * mazeSize);
        SetMazeParams(gen, idx);

        if (newMaze) { gen.GenerateMaze(rand.Next()); }
        gen.RedoGeometry(x, z, sync);
    }

    private void SetMazeParams(MazeSectionGenerator gen, int idx)
    {
        gen.FloorPool.start = idx;
        gen.FloorPool.count = 1;

        gen.RestorerPool.start = idx;
        gen.RestorerPool.count = 1;

        gen.PowerupPool.start = idx;
        gen.PowerupPool.count = 1;

        gen.WallPool.start = idx * genTiles;
        gen.WallPool.count = genTiles;

        gen.GemPool.start = idx * genTiles;
        gen.GemPool.count = genTiles;

        gen.FollowSolutionPool.start = idx;
        gen.FollowSolutionPool.count = 1;

        gen.DepthEnemyPool.start = idx;
        gen.DepthEnemyPool.count = 1;

        gen.ChaseEnemyPool.start = idx;
        gen.ChaseEnemyPool.count = 1;
    }

    private MazeSectionGenerator GenerateMazeSection(int x, int z)
    {    
        GameObject obj = Parent(Instantiate(MazeSectorPrefab), this.gameObject);
        obj.name = ("MazeSection[" + x + "," + z + "]");
        MazeSectionGenerator gen = obj.GetComponent<MazeSectionGenerator>();
        gen.RestorerPool = new RefArray<GameObject>(restorerPool, 0, 0);
        gen.FloorPool = new RefArray<Renderer>(floorPool, 0, 0);
        gen.PowerupPool = new RefArray<Powerup>(powerupPool, 0, 0);
        gen.WallPool = new RefArray<GameObject>(wallPool, 0, 0);
        gen.GemPool = new RefArray<EatForPoints>(gemPool, 0, 0);
        gen.FollowSolutionPool = new RefArray<FollowMazeSolution>(followSolutionPool, 0, 0);
        gen.DepthEnemyPool = new RefArray<DepthFirstExplore>(depthEnemyPool, 0, 0);
        gen.ChaseEnemyPool = new RefArray<ChaseEnemy>(chaseEnemyPool, 0, 0);
        gen.GemMat = GemColors[Mod(x - z, GemColors.Length)];
        gen.FloorMat = FloorColors[Mod(x - z, FloorColors.Length)];
        return gen;
    }
}
