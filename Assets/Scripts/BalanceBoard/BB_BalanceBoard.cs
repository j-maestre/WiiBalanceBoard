using UnityEngine;

public class Margin {
    public float left = 500.0f;
    public float right = 500.0f;
    public float forward = 500.0f;
    public float back = 500.0f;
}

public class BB_BalanceBoard {

    public bool IsCalibrating = false;
    public DirectionType ControlType { get => controlType; set => controlType = value; }

    public Direction GetCurrentDirection => UpdateDirection();  
 
    public int ExpType => Wii.GetExpType(remote);
    public float TotalWeight => scales.Quad.Weight;
    public float AvgWeight => scales.AverageWeight;
    public BB_Quadrant GetWeights => scales.Quad;
    public bool IsActive => Wii.IsActive(remote);
    public bool IsSwitchedOff = false;
    public int Remote => remote;
    public float Battery => Wii.GetBattery(remote);
    public Vector2 CenterOfBalance => scales.Quad.Center;
    public Vector2 CenterOfBalanceNoRound => scales.Quad.CenterNoRound;
    public Vector2 CenterOfBalanceRaw => scales.Quad.CenterRaw;

    private int remote = -1;
    private BB_Scales scales;
    private DirectionType controlType = DirectionType.FOUR_WAY;
    private Margin margins;
    private Vector2 min, max = new Vector2();

    public BB_BalanceBoard() {
        scales = new BB_Scales();
        margins = new Margin();
    }
    
    public void UpdateWeights() {
        scales.SetQuadWeightsWithOffset(Wii.GetRawBalanceBoard(remote));
        scales.SetCenterWithOffset(Wii.GetCenterOfBalance(remote));
        scales.SetWeightWithOffset(Wii.GetTotalWeight(remote));
    }

    public void ConnectBalanceBoard() {
        if (!Wii.IsSearching())  {
            Wii.ResetPlugin();
            Wii.StartSearch();
            Time.timeScale = 1f;
        }
    }

    public void SetCenterOfBalanceDescimalRound(int value) {
        scales.CentreOfBalanceOutput = value;
    }

    public void CalibrationReset(Calibrate calibrate) {
        scales.ResetOffCalibration(calibrate);
    }

    public void CalibrationUpdate(Calibrate calibrate) {
        scales.UpdateCalibration(Wii.GetRawBalanceBoard(remote), Wii.GetCenterOfBalance(remote), Wii.GetTotalWeight(remote), calibrate);
    }

    public bool IsLeaning(Calibrate method) {
        if (method == Calibrate.LEFT) {
            return GetCurrentDirection == Direction.LEFT;
        }
        if (method == Calibrate.RIGHT) {
            return GetCurrentDirection == Direction.RIGHT;
        }
        return false;
    }

    public void UpdateMargins(Calibrate method) {
        if (method == Calibrate.LEFT) {
            scales.UpdateLeanAbility(CenterOfBalance.x, Direction.LEFT);
        } else if (method == Calibrate.RIGHT) {
            scales.UpdateLeanAbility(CenterOfBalance.x, Direction.RIGHT);
        } else if (method == Calibrate.FORWARD) {
            scales.UpdateLeanAbility(CenterOfBalance.y, Direction.FORWARD);
        } else if (method == Calibrate.BACK) {
            scales.UpdateLeanAbility(CenterOfBalance.y, Direction.BACK);
        }
    }

    public void CalibrationComplete(Calibrate calibrate) {
        scales.FinishOffCalibration(calibrate);
    }

    private Direction UpdateDirection() {
        if (scales.total < -500) {
            return Direction.OFF;
        }
        if (controlType == DirectionType.LEFT_RIGHT) {
            if (TestDirection(scales.left, scales.right, margins.left)) {
                return Direction.LEFT;
            } else if (TestDirection(scales.right, scales.left, margins.right)) {
                return Direction.RIGHT;
            }
        }
        if (controlType == DirectionType.FORWARD_BACK) {
            if (TestDirection(scales.front, scales.back, margins.forward)) {
                return Direction.FORWARD;
            } else if (TestDirection(scales.back, scales.front, margins.back)) {
                return Direction.BACK;
            }
        }
        if (controlType == DirectionType.FOUR_WAY) {
            if (TestDirection(scales.left, scales.right, scales.front, scales.back)) {
                return Direction.LEFT;
            } else if (TestDirection(scales.right, scales.left, scales.front, scales.back)) {
                return Direction.RIGHT;
            } else if (TestDirection(scales.front, scales.back, scales.left, scales.right)) {
                return Direction.FORWARD;
            } else if (TestDirection(scales.back, scales.front, scales.left, scales.right)) {
                return Direction.BACK;
            }
        }
        return Direction.NONE;
    }

    private bool TestDirection(float a, float b, float margin) {
        if (a > b && a - b > margin) {
            return true;
        }
        return false;
    }

    private bool TestDirection(float a, float b, float c, float d, float margin = 2000) {
        if (a > b && a - b > margin) {
            if (a > c && a - c > margin) {
                if (a > d && a - d > margin) {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetOnEnabledEvents() {
        Wii.OnDiscoveryFailed += OnDiscoveryFailed;
        Wii.OnWiimoteDiscovered += OnWiimoteDiscovered;
        Wii.OnWiimoteDisconnected += OnWiimoteDisconnected;
    }

    public void UnsetOnDisabledEvents() {
        Wii.OnDiscoveryFailed -= OnDiscoveryFailed;
        Wii.OnWiimoteDiscovered -= OnWiimoteDiscovered;
        Wii.OnWiimoteDisconnected -= OnWiimoteDisconnected;
    }

    private void OnDiscoveryFailed(int i) {
        Debug.LogWarning("Discovery Failed:" + i + ". Try Again.");
        IsSwitchedOff = true;
    }

    private void OnWiimoteDiscovered(int thisRemote) {
        Debug.Log("Discovered Wii Device: " + thisRemote);
        IsSwitchedOff = false;
        if (!Wii.IsActive(remote))
            remote = thisRemote;
    }

    public void OnWiimoteDisconnected(int whichRemote) {
        Debug.LogWarning("Wii Device Disconnected: " + whichRemote);
    }
}
