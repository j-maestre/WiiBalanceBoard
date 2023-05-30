using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager instance;

    [Header("Project Details")]
    public string ProjectName;
    public string NextSceneAfterCalibration = string.Empty;

    [Header("Controller Settings")]
    public DirectionType ControllerType;
    [Range(0, 7)] public int CentreOfBalanceDecimalOutput;

    private int currentScene = -1;

    void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is already an instance of project manager");
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }

    void Start() {
        BoardController.SetCentreOfBalanceDecimalOutput(CentreOfBalanceDecimalOutput);
        BoardController.SetControllerDirectionType(ControllerType);
        if (NextSceneAfterCalibration == string.Empty) {
            Debug.Log("In Side NextSceneAfterCalibration ");
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }
    }

    public int GetCurrentSceenIndex() {
        return currentScene;
    }
}
