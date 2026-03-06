using UnityEngine;

[ExecuteAlways]
public class DistributeInGrid3D : MonoBehaviour
{
    [SerializeField] private Vector3Int gridSpaces = new Vector3Int(3, 3, 3);
    [SerializeField] private Vector3Int gridSizes = new Vector3Int(3, 3, 3);
    [SerializeField] private bool run;

    [Header("Randomize")]
    [SerializeField] private int seed;
    [Header("Randomized Position Offsets")]
    [SerializeField] private bool addRandomOffset = true;
    [SerializeField] private Vector3 minPosOffset = new Vector3(-0.25f, 0, -0.25f);
    [SerializeField] private Vector3 maxPosOffset = new Vector3(0.25f, 0, 0.25f);


    [Header("Randomized Rotation Offsets")]
    [SerializeField] private bool addRandomRotationOffset = true;
    [SerializeField] private Vector3 minRotOffset = new Vector3(0, -180, 0);
    [SerializeField] private Vector3 maxRotOffset = new Vector3(0, 180, 0);

    private System.Random rand = new System.Random();
    private void Start()
    {
        if (seed == 0)
        {
            seed = Random.Range(0, int.MaxValue);
        }
        Distribute();
    }

    private void Update()
    {
        if (run)
        {
            run = false;
            Distribute();
        }
    }

    private void Distribute()
    {
        rand = new System.Random(seed);
        int index = 0;
        for (int z = 0; z < gridSpaces.z; z++)
        {
            for (int y = 0; y < gridSpaces.y; y++)
            {
                for (int x = 0; x < gridSpaces.x; x++)
                {
                    if (index < this.transform.childCount)
                    {
                        var pos = new Vector3(
                            x * gridSizes.x,
                            y * gridSizes.y,
                            z * gridSizes.z
                        );

                        if (addRandomOffset)
                        {
                            var randX = minPosOffset.x + (rand.NextDouble() * (maxPosOffset.x - minPosOffset.x));
                            var randY = minPosOffset.y + (rand.NextDouble() * (maxPosOffset.y - minPosOffset.y));
                            var randZ = minPosOffset.z + (rand.NextDouble() * (maxPosOffset.z - minPosOffset.z));
                            pos += new Vector3((float)randX, (float)randY, (float)randZ);
                        }

                        var rot = new Vector3();
                        if (addRandomRotationOffset)
                        {
                            var randX = minRotOffset.x + (rand.NextDouble() * (maxRotOffset.x - minRotOffset.x));
                            var randY = minRotOffset.y + (rand.NextDouble() * (maxRotOffset.y - minRotOffset.y));
                            var randZ = minRotOffset.z + (rand.NextDouble() * (maxRotOffset.z - minRotOffset.z));
                            rot = new Vector3((float)randX, (float)randY, (float)randZ);
                        }

                        var child = this.transform.GetChild(index);
                        child.transform.position = pos;
                        child.transform.rotation = Quaternion.Euler(rot);
                        index++;
                    }
                }
            }
        }
    }
}
