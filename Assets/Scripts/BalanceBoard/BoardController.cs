using Michsky.UI.ModernUIPack;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public enum Calibrate {
    CONNECT,
    EMPTY,
    STAND,
    LEAN,
    READY,
    LEFT,
    RIGHT,
    FORWARD,
    BACK,
    NONE
}

public delegate void CallBack();

public class BoardController : MonoBehaviour {
    public static BB_BalanceBoard bb;
    public static BB_Quadrant theBalanceBoard;
    public static Vector2 CenterOfBalance;
    public static Vector2 CenterOfBalanceNoRoundOrLimit;
    public static Vector2 CenterOfBalanceRaw;
    public static BB_Quadrant theRawBalanceBoard;
    public static float AverageWeight;
    public static float TotalWeight;
    public static bool IsOnTheBoard = false;

    public static Direction currentDirection;   
    public static bool isKeyboard = true;

    public static Calibrate currentCalibration;

    // Info shown in inspector

    // INSPECTOR
    [SerializeField] public float maxWeight_ = -5.0f;
    [SerializeField] private string status, remote, battery, expType, totalWeight;
    [SerializeField] public BB_Quadrant bbRaw;
    [SerializeField] public Vector2 bbCenter;
    [SerializeField] private Vector2 bbCenterRaw;
    ///////////////////

    private const float DURATION = 5.0f;
    private static float[] countdown, duration;
    private static float[] startTimer, currentTimer, lastTimer, countTimer;
    private static GameObject slider;
    private static GameObject[] sliders;
    private static GameObject[] leanImages;
    private static TextMeshProUGUI instruct;
    private bool dontUpdate = false;
    public static bool isWaitingForResponse = false;
    private static bool[] isDone, isNotNeeded = {false, false};

    private int setDecimalOffset = 1;

    private static CallBack callback;
    

    void Awake() {
        DontDestroyOnLoad(this.gameObject);
        bb = new BB_BalanceBoard();
        bb.SetOnEnabledEvents();
    }

    void OnDisable() {
        bb.UnsetOnDisabledEvents();
    }

    void Start() {
        currentCalibration = Calibrate.EMPTY;
    }

    void Update() {

        if(Input.GetButton("Fire1")){
            //SceneManager.LoadScene(1);
        }
        if (bb.IsActive) {
            UpdateActiveStats();
            if (bb.ExpType == 3) {
                UpdateBalanceBoardStats();
                if (bb.IsCalibrating) {
                    if (!isWaitingForResponse) {
                        Calibration();
                    } else {
                         Debug.Log("WAITING");
                    }
                } else {
                    if (!isKeyboard) {
                        //UpdateDirection();
                    }
                }
            } else {
                bb.ConnectBalanceBoard();
                status = "Wrong device " + bb.ExpType;
            }
        } else {
            bb.ConnectBalanceBoard();
            status = "Not Active: " + bb.IsActive;
        }
    }

    public static void SetControllerDirectionType(DirectionType dir) {
        if (bb == null)
        {
            Debug.LogError("Balance Board instance missing");
            return;
        }
        bb.ControlType = dir;
    }

    public static void SetCentreOfBalanceDecimalOutput(int value) {
        if (bb == null)
        {
            Debug.LogError("Balance Board instance missing");
            return;
        }
        bb.SetCenterOfBalanceDescimalRound(value);
    }

    public static void UpdateDirection() {
        currentDirection = bb.GetCurrentDirection;
    }

    private void Calibration() {
        if (currentCalibration == Calibrate.EMPTY || currentCalibration == Calibrate.STAND) {
            StandingCalibration();
        } else if (currentCalibration == Calibrate.LEFT || currentCalibration == Calibrate.RIGHT) {
            LeaningCalibration();
        } else if (currentCalibration == Calibrate.FORWARD || currentCalibration == Calibrate.BACK) {
            LeaningCalibration();
        }
    }

    private float timer = 0.0f;
    private bool timeToCollect = false;

    private void StandingCalibration() {
        slider.SetActive(true);
        float duration = DURATION;
        if (currentCalibration == Calibrate.STAND) {
            Debug.Log("Inside Stand Shubham");
            duration = DURATION * 2;
        }
        if (countTimer[0] > 0.0f) {
            slider.GetComponent<RadialSlider>().SliderValue = (((currentTimer[0] - startTimer[0]) / duration) * 100);
            slider.GetComponent<RadialSlider>().UpdateUI();
            if(bbRaw.Weight >= maxWeight_){
                maxWeight_ = bbRaw.Weight;
            }
        }
        countTimer[0] += Time.timeSinceLevelLoad - currentTimer[0];
        currentTimer[0] = Time.timeSinceLevelLoad;
        if (countTimer[0] > 1) {
            countdown[0]--;
            countTimer[0] = 0;
        }
        timer += Time.deltaTime;
        if (timer > 0.05f) {
            timeToCollect = true;
            timer = 0.0f;
        } else {
            timeToCollect = false;
        }
        if (currentTimer[0] - startTimer[0] < duration) {
            bb.CalibrationUpdate(currentCalibration);
        } else {
            slider.GetComponent<RadialSlider>().SliderValue = 100;
            slider.GetComponent<RadialSlider>().UpdateUI();
            bb.IsCalibrating = false;
            bb.CalibrationComplete(currentCalibration);
            slider.SetActive(false);
            if (currentCalibration == Calibrate.STAND) {
            } else {
                callback();
            }
        }
    }

    private static float currentSessionResult, currentSessionWeight;
    private static string currentSessionName;

    private static string currentLevelName;

    private bool CheckIfComplete(bool[] isDone) {
        bool complete = false;
        foreach (bool done in isDone) {
            if (done) {
                complete = true;
            } else {
                complete = false;
                return complete;
            }
        }
        return complete;
    }

    private int GetCurrentCalibrationIndex() {
        return (int)currentCalibration - (int)Calibrate.LEFT;
    }

    private void SetNextCalibration(string message)
    {
        if (bb.ControlType == DirectionType.LEFT_RIGHT) {
            if (currentCalibration == Calibrate.LEFT) {
                Debug.Log("Inside Left Shubham");
                currentCalibration = Calibrate.RIGHT;
                Debug.Log("Calibration - Leaning Test Right");
                Debug.Log("Inside 1Right Shubham" + currentCalibration);
            } else if (currentCalibration == Calibrate.RIGHT) {
                Debug.Log("Check Else if after 1 RIght ht Shubham");
                currentCalibration = Calibrate.READY;
            }
        } else if (bb.ControlType == DirectionType.FORWARD_BACK) {
            if (currentCalibration == Calibrate.FORWARD) {
                currentCalibration = Calibrate.BACK;
                Debug.Log("Calibration - Leaning Test Back");
            } else if (currentCalibration == Calibrate.BACK) {
                currentCalibration = Calibrate.READY;
            }
        } else if (bb.ControlType == DirectionType.FOUR_WAY) {
            if (currentCalibration == Calibrate.LEFT) {
                currentCalibration = Calibrate.RIGHT;
                Debug.Log("Calibration - Leaning Test Right");
                Debug.Log("Inside 2Rig ht Shubham");
            } else if (currentCalibration == Calibrate.RIGHT) {
                currentCalibration = Calibrate.FORWARD;
                Debug.Log("Calibration - Leaning Test Forward");
            } else if (currentCalibration == Calibrate.FORWARD) {
                currentCalibration = Calibrate.BACK;
                Debug.Log("Calibration - Leaning Test Back");
            } else if (currentCalibration == Calibrate.BACK) {
                currentCalibration = Calibrate.READY;
            }
        }
        if (currentCalibration != Calibrate.READY) {
            startTimer[GetCurrentCalibrationIndex()] = Time.timeSinceLevelLoad;
        }
    }

    private void LeaningCalibration() {
        if (CheckIfComplete(isDone)) {
            bb.CalibrationComplete(Calibrate.LEAN);
            currentCalibration = Calibrate.READY;
            bb.IsCalibrating = false;
            foreach (GameObject s in sliders) {
                s.SetActive(false);
            }
            Debug.Log("LEAN COMPLETED");
            callback();
            return;
        }
        int index = GetCurrentCalibrationIndex();
        for (int i = 0; i < sliders.Length; i++) {
            if (isDone[i] && !isNotNeeded[i]) {
                sliders[i].SetActive(true);
                sliders[i].GetComponentInChildren<Animation>().Rewind();
                sliders[i].GetComponentInChildren<Animation>().Stop();
                foreach (Image img in sliders[i].GetComponentsInChildren<Image>()) {
                    img.color = Color.green;
                }
            } else if (index == i) {
                sliders[i].SetActive(true);
            } else {
                sliders[i].SetActive(false);
            }
        }
        dontUpdate = false;
        float sliderValue = 0.0f;
        if (currentCalibration == Calibrate.LEFT && CenterOfBalance.x < -0.2) {
            sliderValue = Mathf.Abs(CenterOfBalance.x);
        } else if (currentCalibration == Calibrate.RIGHT && CenterOfBalance.x > 0.2) {
            sliderValue = Mathf.Abs(CenterOfBalance.x);
        } else if (currentCalibration == Calibrate.BACK && CenterOfBalance.y < -0.2) {
            sliderValue = Mathf.Abs(CenterOfBalance.y);
        } else if (currentCalibration == Calibrate.FORWARD && CenterOfBalance.y > 0.2) {
            sliderValue = Mathf.Abs(CenterOfBalance.y);
        } else {
            sliderValue = 0.0f;
            dontUpdate = true;
        }
        if (!dontUpdate) {
            sliderValue = (sliderValue > 1.0f) ? 1.0f : sliderValue;
            sliders[index].GetComponent<Slider>().value = sliderValue * 100.0f;
        }
        timer += Time.deltaTime;
        if (timer > 0.2f) {
            timeToCollect = true;
            timer = 0.0f;
        } else {
            timeToCollect = false;
        }
        if (duration[index] < DURATION) {
            if (!dontUpdate) {
                duration[index] += Time.deltaTime;
                bb.UpdateMargins(currentCalibration);
            }
        } else {
            if (!isDone[index]) {
                isDone[index] = true;
                if (!CheckIfComplete(isDone)) {
                    SetNextCalibration("good");
                }
               
            }
        }
    }

    public static void CalibrateOffset(ref GameObject givenSlider, Calibrate selected, CallBack cb)
    {
        callback = cb;
        currentCalibration = selected;
        slider = givenSlider;
        countdown = new[] { 0.0f };
        duration = new[] { DURATION };
        startTimer = new[] { 0.0f };
        currentTimer = new[] { 0.0f };
        lastTimer = new[] { 0.0f };
        countTimer = new[] { 0.0f };
        ResetUpdateStats(currentCalibration, 0);
        if (currentCalibration == Calibrate.STAND)  {
            cb();
        }
    }

    public static void CalibrateLeanOffset(ref GameObject[] givenSliders, ref TextMeshProUGUI instructions, CallBack cb)
    {

        callback = cb;
        instruct = instructions;
        sliders = givenSliders;
        countdown = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
        duration = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
        startTimer = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
        currentTimer = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
        lastTimer = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
        countTimer = new[] { 0.0f, 0.0f, 0.0f, 0.0f };
        if (bb.ControlType == DirectionType.LEFT_RIGHT || bb.ControlType == DirectionType.FOUR_WAY) {
            currentCalibration = Calibrate.LEFT;
            Debug.Log("Calibration - Leaning Test Left");
            ResetUpdateStats(Calibrate.LEFT, 0);
            ResetUpdateStats(Calibrate.RIGHT, 1);
            isDone = new[] { false, false, true, true };
            isNotNeeded = new[] { false, false, true, true };
            if (bb.ControlType == DirectionType.FOUR_WAY) {
                ResetUpdateStats(Calibrate.FORWARD, 2);
                ResetUpdateStats(Calibrate.BACK, 3);
                isDone = new[] { false, false, false, false };
                isNotNeeded = new[] { false, false, false, false };
            }
        } else if (bb.ControlType == DirectionType.FORWARD_BACK) {
            currentCalibration = Calibrate.FORWARD;
            Debug.Log("Calibration - Leaning Test Forward");
            ResetUpdateStats(Calibrate.FORWARD, 2);
            ResetUpdateStats(Calibrate.BACK, 3);
            isDone = new[] { true, true, false, false };
            isNotNeeded = new[] { true, true, false, false };
        }
    }

    private static void ResetUpdateStats(Calibrate current, int index) {
        bb.CalibrationReset(current);
        countdown[index] = DURATION;
        startTimer[index] = Time.timeSinceLevelLoad;
        currentTimer[index] = 0.0f;
        bb.IsCalibrating = true;
    }

    public void UpdateActiveStats() {
        remote = "Remote #" + bb.Remote;
        expType = "Active: " + bb.ExpType;
        battery = bb.Battery.ToString();
        totalWeight = bb.TotalWeight.ToString();
        TotalWeight = bb.TotalWeight;
        AverageWeight = bb.AvgWeight;
        if (TotalWeight > AverageWeight / 2 && AverageWeight > 0) {
            IsOnTheBoard = true;
        } else {
            IsOnTheBoard = false;
        }
    }

    private void UpdateBalanceBoardStats() {
        status = "Connected!";
        bbCenter = CenterOfBalance = bb.CenterOfBalance;
        CenterOfBalanceNoRoundOrLimit = bb.CenterOfBalanceNoRound;
        bbCenterRaw = CenterOfBalanceRaw = bb.CenterOfBalanceRaw;
        bbRaw = theBalanceBoard = theRawBalanceBoard = bb.GetWeights;
        bb.UpdateWeights();
        totalWeight = bb.TotalWeight + " kg";
    }

    public static void SetIsKeyboard(bool value) {
        isKeyboard = value;
    }

    public static bool GetIsKeyboard() {
        return isKeyboard;
    }
}
