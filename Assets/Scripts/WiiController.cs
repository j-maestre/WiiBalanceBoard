
//using UnityEngine;
//using UnityEngine.UI;

//public class WiiController : MonoBehaviour
//{
//    public static WiiController Instance;

//    public bool allowSending;
//    // Remote identifier, needed for WiiBuddy's functions
//    public static int remote;
//    // Offset for weight distribution on the four sensors of the board
//    public Vector4 weightOffset;
//    // Offset of balance in terms of forward/backward 
//    public float balanceOffset;
//    // Min amount of leaning needed to start pulling data from the board (leaning values goes from 0 to 1)
//    public float minZone = 0.3f;


//    // Max needed values to go max speed
//    public float maxZoneForward;
//    public float maxZoneBackward;

//    public Vector2 lean;


//    void Awake()
//    {
//        if (Instance != null && Instance != this)
//            Destroy(gameObject);
//        Instance = this;
//        DontDestroyOnLoad(this.gameObject);
//    }

//    // Use this for initialization
//    void OnEnable()
//    {
//        // Connects to a board 
//        if (!Wii.IsActive(remote))
//        {
//            Debug.Log("Start searching");
//            Wii.StartSearch();
//        }
//        Wii.OnDiscoveryFailed += OnDiscoveryFailed;
//        Wii.OnWiimoteDiscovered += OnWiimoteDiscovered;
//        Wii.OnWiimoteDisconnected += OnWiimoteDisconnected;
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // If connected to a board, send data
//        if (Wii.IsActive(remote))
//        {
//            if (allowSending)
//            {
//                SendWheelRatio();
//            }
//        }
//    }

//    public void OnDiscoveryFailed(int i)
//    {
//        Debug.Log("Error:" + i + ". Try Again.");
//        //searching = false;
//    }
//    // Assign found board to "remote"
//    public void OnWiimoteDiscovered(int thisRemote)
//    {
//        Debug.Log("found this one: " + thisRemote);
//        if (!Wii.IsActive(remote))
//            remote = thisRemote;
//    }
//    // Debug log when connection to a board is dropped
//    public void OnWiimoteDisconnected(int whichRemote)
//    {
//        Debug.Log("lost this one: " + whichRemote);
//    }

//    public void SendWheelRatio()
//    {
//        // Add offset to bring weight distribution on the board to zero while standing still
//        Vector4 board = Wii.GetBalanceBoard(remote) + weightOffset;

//        // Calculates distribution of front and back sensors on both sides independently
//        float rightWheel = (board.x - board.z) / (board.x + board.z);
//        float leftWheel = (board.y - board.w) / (board.y + board.w);

//        // Normalize values to start from the min need lean
//        rightWheel = Normalize(rightWheel, minZone);
//        leftWheel = Normalize(leftWheel, minZone);

//        // Send data to the NewRobotController
//        //CarController.Instance.leftTarget = leftWheel;
//        //CarController.Instance.rightTarget = rightWheel;
//    }

//    // Function for normalizing value between given min and max needed lean values
//    public float Normalize(float value, float min)
//    {
//        if (Mathf.Abs(value) < min || Wii.GetTotalWeight(remote) < 40)
//        {
//            return 0;
//        }
//        if (value > 0)
//        {
//            value -= min;
//            return Mathf.Min(1, value / (maxZoneForward - min));
//        }
//        else
//        {
//            value += min;
//            return -Mathf.Min(1, value / (maxZoneBackward + min));
//        }
//    }

//    // Zeroes center of balance to the position on which a person is standing on the board
//    public void CalibrateCenterOfBalance()
//    {
//        float weight = Wii.GetTotalWeight(remote) / 4;
//        Vector4 weigthDistribution = Wii.GetBalanceBoard(remote);
//        weightOffset = new Vector4(weight, weight, weight, weight) - weigthDistribution;

//        balanceOffset = (float)Wii.GetCenterOfBalance(remote).y;
//    }

//}
