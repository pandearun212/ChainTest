using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public List<CubeCollisionHandler> cubes;
    public Rigidbody cube;
    public GameObject collidingCube1;
    public GameObject collidingCube2;
    public bool isDragginCube;

    [SerializeField]
    Renderer indicator;
    [SerializeField]
    GameObject beam;
    [SerializeField]
    GameObject confettiPrefab;

    [Space]
    [Header("Parameters to control cube")]
    [SerializeField]
    float cubeBoundaryLimit = 3.5f;
    [SerializeField]
    float cubeLaunchForce = 20.0f;

    [Space]
    [Header("Materials for Cube")]
    public List<Material> cubeMaterials;
    public List<Color> confettiColors;


    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        InstantiateCube();
        StartCoroutine(AdjustCameraView());
    }

    IEnumerator AdjustCameraView()
    {
        bool isAdjustmentRequired = false;

        while (!indicator.isVisible)
        {
            isAdjustmentRequired = true;
            Camera.main.fieldOfView++;
            yield return new WaitForEndOfFrame();
        }

        // the board should be well within the field of view so increase field of view by a little
        if (isAdjustmentRequired)
        {
            Camera.main.fieldOfView += 10;
        }
    }

    void InstantiateCube()
    {
        GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.position = new Vector3(0, 0.5f, -0.9f);
        newCube.transform.eulerAngles = Vector3.zero;
        newCube.AddComponent<Rigidbody>();
        newCube.AddComponent<BoxCollider>();
        newCube.AddComponent<CubeCollisionHandler>();
        newCube.GetComponent<BoxCollider>().material = new PhysicMaterial();

        cube = newCube.GetComponent<Rigidbody>();
        AssignRandomCubeNumber();

        beam.transform.parent = newCube.transform;
        beam.transform.localPosition = new Vector3(0, 0, 6);
        beam.transform.eulerAngles = Vector3.zero;
        beam.SetActive(true);
    }

    GameObject InstantiateCombinedCube(Vector3 pos, int number)
    {
        GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.position = pos;
        newCube.transform.eulerAngles = Vector3.zero;
        newCube.AddComponent<Rigidbody>();
        newCube.AddComponent<BoxCollider>();
        newCube.AddComponent<CubeCollisionHandler>();
        newCube.GetComponent<BoxCollider>().material = new PhysicMaterial();

        CubeCollisionHandler cubeCollisionHandler = newCube.GetComponent<CubeCollisionHandler>();
        cubeCollisionHandler.cubeNumber = number;
        cubeCollisionHandler.cubeManager = this;
        GameObject confetti = Instantiate(confettiPrefab, newCube.transform);
        ParticleSystem ps = confetti.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psmain = ps.main;
        psmain.startColor = confettiColors[(int)Mathf.Log(number, 2) - 1];

        newCube.GetComponent<Renderer>().material = cubeMaterials[(int)Mathf.Log(number, 2) - 1];

        return newCube;
    }

    void AssignRandomCubeNumber()
    {
        int randomNum = Mathf.Abs(Random.Range(-2, 6));

        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        CubeCollisionHandler cubeCollisionHandler = cube.GetComponent<CubeCollisionHandler>();
        cubeCollisionHandler.cubeManager = this;

        cubeCollisionHandler.cubeNumber = (int)Mathf.Pow(2, randomNum + 1);
        cubeRenderer.material = cubeMaterials[randomNum];
    }

    public void OnSameNumberedCubeCollision(GameObject obj, int num)
    {
        if (collidingCube1 == null)
        {
            collidingCube1 = obj;
        }
        else if (collidingCube2 == null)
        {
            collidingCube2 = obj;
            CombineTwoCubes(obj.transform.position, num);
        }
    }

    void CombineTwoCubes(Vector3 pos, int number)
    {
        cubes.Remove(collidingCube1.GetComponent<CubeCollisionHandler>());
        cubes.Remove(collidingCube2.GetComponent<CubeCollisionHandler>());
        Destroy(collidingCube1.gameObject);
        Destroy(collidingCube2.gameObject);
        collidingCube1 = null;
        collidingCube2 = null;
        GameObject obj = InstantiateCombinedCube(pos, number * 2);
        CubeCollisionHandler cubeCollisionHandler = obj.GetComponent<CubeCollisionHandler>();
        cubeCollisionHandler.SetCombinedCubeAttributes();

        //update score
        ScoreManager.Instance.UpdateScore(number * 2);
    }

    IEnumerator InstantiateCubeAfterDelay()
    {
        cube = null;
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.CheckForInterstitialAds();
        InstantiateCube();
    }

    // Update is called once per frame
    void Update()
    {
        // if over ui element then do not run
        if (!MouseInputUIBlocker.BlockedByUI && !GoogleAdMobManager.Instance.isShowingAd && cube != null)
        {
            // this will be better to change to touch input later.
            if (Input.GetMouseButton(0))
            {
                isDragginCube = true;
                float multiplier = 0.1f;

                // quick fix for sensitivity in mobile
#if UNITY_EDITOR
                multiplier = 1.0f;
#endif

                float newCubePosition = cube.transform.position.x + (Input.GetAxis("Mouse X") * multiplier);

                // check if cube if out of boundary.
                if (Mathf.Abs(newCubePosition) > cubeBoundaryLimit)
                {
                    Vector3 cubePosition = cube.transform.position;
                    cubePosition.x = Mathf.Sign(cubePosition.x) * cubeBoundaryLimit;
                    cube.transform.position = cubePosition;
                }
                else
                {
                    cube.transform.position += new Vector3(Input.GetAxis("Mouse X") * multiplier, 0, 0);
                }
            }
        }

        // launch the cube
        if (Input.GetMouseButtonUp(0) && isDragginCube)
        {
            cube.AddForce(Vector3.forward * cubeLaunchForce);
            cubes.Add(cube.GetComponent<CubeCollisionHandler>());
            StartCoroutine(InstantiateCubeAfterDelay());
            isDragginCube = false;
            beam.transform.parent = null;
            beam.SetActive(false);
            GameManager.Instance.collisionCount++;

        }
    }
}
