using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalibrateController : MonoBehaviour {
    public TMP_Text TitleText;
    public Button connectButton;
    public GameObject slider;
    public GameObject[] Sliders;
    public GameObject[] horizontalSliders;
    public GameObject[] verticalSliders;
    public TextMeshProUGUI instruct;
    public RawImage BalanceBoardImage;
    public RawImage NoFeetImage;
    public RawImage FingerImage;
    public GameObject FeetImage;
    public GameObject LoadingImage;

    public bool IsLoading = false;

    private bool isConnected;
    private Calibrate calibration;
    private int lastExp = -1;
    private ProjectManager pm;

    private CallBack callback;
    private bool IsRunOnce = false;
    private bool isLoadingGame = false;

    void Start() {
        instruct.text = "Welcome " + PlayerPrefs.GetString("USERNAME", string.Empty);
        pm = ProjectManager.instance;
        TitleText.text = pm.ProjectName;
        ReadyButton(connectButton);
        slider.SetActive(false);
        BalanceBoardImage.gameObject.SetActive(false);
        NoFeetImage.gameObject.SetActive(false);
        FingerImage.gameObject.SetActive(false);
        foreach (GameObject sl in horizontalSliders) {
            sl.SetActive(false);
        }
        foreach (GameObject sl in verticalSliders) {
            sl.SetActive(false);
        }
        calibration = Calibrate.CONNECT;
        isConnected = false;
        LoadingImage.SetActive(false);
    }

    void Update() {
        if (IsLoading) {
            LoadingImage.SetActive(true);
        }
        if (Input.GetKey(KeyCode.Escape)) {
            Quit();
        }
        isConnected = BoardController.bb.IsActive;
        if (BoardController.bb.IsSwitchedOff) {
            FingerImage.gameObject.SetActive(true);
            NoFeetImage.gameObject.SetActive(false);
        } else {
            FingerImage.gameObject.SetActive(false);
            NoFeetImage.gameObject.SetActive(true);
        }
        if (!isConnected) {
            BalanceBoardImage.gameObject.SetActive(true);
            ReadyButton(connectButton);
        } else if (isConnected && calibration == Calibrate.CONNECT) {
            NextCalibration();
            StartCoroutine(Coroutine_WaitForSecond(EmptyCalibration, 2));
        } else if (isConnected && calibration != Calibrate.EMPTY && calibration != Calibrate.CONNECT) {
            NoFeetImage.gameObject.SetActive(false);
            if (BoardController.bb.ExpType == 3) {
                if (!BoardController.bb.IsCalibrating) {
                    if (calibration == Calibrate.STAND) {
                        FeetImage.gameObject.SetActive(true);
                        CompleteButton(connectButton, "CONNECTED");
                        if (BoardController.TotalWeight > 10.00f && !IsRunOnce) {
                            IsRunOnce = true;
                            SetInstruction("Stand Straight Test");
                            StartCoroutine(Coroutine_WaitForSecond(StandButtonGo, 3));
                        }
                    } else if (calibration == Calibrate.LEAN) {
                        FeetImage.gameObject.SetActive(false);
                        CompleteButton(connectButton, "Complete");
                        if (BoardController.IsOnTheBoard) {
                            SetInstruction("Lean Test");
                            if (!BoardController.bb.IsCalibrating && !IsRunOnce) {
                                IsRunOnce = true;
                                StartCoroutine(Coroutine_WaitForSecond(LeanButtonGo, 1));
                            }
                        }
                    } else if (calibration == Calibrate.READY && !isLoadingGame) {
                        isLoadingGame = true;
                        SetInstruction("Starting Game");
                        CompleteButton(connectButton, "All Done! READY");
                        BoardController.SetIsKeyboard(false);
                        StartCoroutine(Coroutine_WaitForSecond(ReadyButtonGo, 3));
                    }
                    Debug.Log("calibration" + calibration);
                    Debug.Log("IsGameLoading" + isLoadingGame);
                }
            } else {
                if (lastExp != BoardController.bb.ExpType) {
                    lastExp = BoardController.bb.ExpType;
                    Debug.LogError("SNAP! Something broke, expansion type is not valid #" + BoardController.bb.ExpType);
                    SetInstruction("SNAP! Something broke... Please restart the game and try again.");
                }
            }
        }
    }

    private IEnumerator Coroutine_WaitForSecond(CallBack cb, int sec) {
        yield return new WaitForSeconds(sec);
        cb();
    }

    private void SetInstruction(string message) {
        instruct.text = message;
    }

    public void ConnectButtonGo() {
        BoardController.bb.ConnectBalanceBoard();
        calibration = Calibrate.STAND;
    }

    private void EmptyCalibration() {
        if (isConnected && !BoardController.bb.IsCalibrating) {
            BoardController.CalibrateOffset(ref slider, calibration, NextCalibration);
        }
    }

    public void StandButtonGo() {
        if (isConnected && !BoardController.bb.IsCalibrating) {
            BoardController.CalibrateOffset(ref slider, calibration, NextCalibration);
        }
    }

    public void LeanButtonGo() {
        if (isConnected && !BoardController.bb.IsCalibrating) {
            BoardController.CalibrateLeanOffset(ref Sliders, ref instruct, NextCalibration);
        }
    }

    private void CompleteButton(Button btn, string text = "") {
        btn.interactable = false;
        ColorBlock cb = btn.colors;
        cb.disabledColor = Color.green;
        btn.colors = cb;
        if (text != "") {
            btn.GetComponentInChildren<Text>().text = text;
        }
    }

    private void ReadyButton(Button btn) { 
        btn.interactable = true;
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.yellow;
        btn.colors = cb;
    }

    public void NextCalibration() {
        bool isCurrent = false;
        foreach (Calibrate item in Calibrate.GetValues(typeof(Calibrate))) {
            Debug.Log("Value of is Current" + isCurrent);
            if (isCurrent) {
                calibration = item;
            }
            if (item == calibration) {
                isCurrent = !isCurrent;
            }
        }
        IsRunOnce = false;
    }

    public void ReadyButtonGo() {
        Debug.Log("Inside RBG" + pm.GetCurrentSceenIndex());
        if (pm.GetCurrentSceenIndex() != -1) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(pm.GetCurrentSceenIndex() + 1);
        } else {
            //PCarController.ControlTypeOption = PCarController.ControlType.Balanceboard;
            //UnityEngine.SceneManagement.SceneManager.LoadScene(pm.NextSceneAfterCalibration);
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }

    public void ReadyIsKeyboard() {
        BoardController.SetIsKeyboard(true);
        ReadyButtonGo();
    }

    public void Quit() {
        Application.Quit();
    }
}
