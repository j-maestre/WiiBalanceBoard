using System;
using System.Collections.Generic;
using UnityEngine;

public class BB_User {
    public BB_Quadrant max;
    public BB_Quadrant min;
}

public class BB_Scales {
    public BB_Quadrant Quad;
    public float AverageWeight;

    public float left, right, front, back, total;

    /// <summary>
    /// Weight offset after empty calibration
    /// </summary>
    private BB_Quadrant empty;

    /// <summary>
    /// Weight offset after standing calibration
    /// </summary>
    private BB_Quadrant stand;

    /// <summary>
    /// an array to store x seconds of weights for each scale
    /// </summary>
    private List<BB_Quadrant> calibrationQuad;
    private List<float> minX, minY, maxX, maxY;
    private float minXavg, minYavg, maxXavg, maxYavg;

    public int CentreOfBalanceOutput;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public BB_Scales() {
        Quad = new BB_Quadrant();
        empty = new BB_Quadrant();
        stand = new BB_Quadrant();
    }

    /// <summary>
    /// Set the current weight values.
    /// </summary>
    /// <param name="weights">Unity.Vector4 Current Raw Weights</param>
    public void SetWeights(Vector4 weights) {
        Quad.Front_Right = weights.x;
        Quad.Front_Left = weights.y;
        Quad.Back_Right = weights.z;
        Quad.Back_Left = weights.w;
    }

    /// <summary>
    /// Set the current weight values while removing the offset values
    /// </summary>
    /// <param name="weights"></param>
    public void SetQuadWeightsWithOffset(Vector4 weights) {
        Quad.Front_Right = RoundOff(weights.x - empty.Front_Right - stand.Front_Right);
        Quad.Front_Left = RoundOff(weights.y - empty.Front_Left - stand.Front_Left);
        Quad.Back_Right = RoundOff(weights.z - empty.Back_Right - stand.Back_Right);
        Quad.Back_Left = RoundOff(weights.w - empty.Back_Left - stand.Back_Left);
        SetAxisValues();
    }

    public void SetCenterWithOffset(Vector2 c) {
        Quad.CenterRaw = c - empty.CenterRaw;
        Quad.Center = c - empty.Center - stand.Center;

        if (Quad.Center.x <= -0.1 && minXavg != 0) {
            Quad.Center.x *= minXavg;
        } else if (Quad.Center.x >= 0.1 && maxXavg != 0) {
            Quad.Center.x *= maxXavg;
        }
        if (Quad.Center.y <= -0.1 && minYavg != 0) {
            Quad.Center.y *= minYavg;
        } else if (Quad.Center.y >= 0.1 && maxYavg != 0) {
            Quad.Center.y *= maxYavg;
        }
        Quad.CenterNoRound = Quad.Center;

        Quad.Center.x = (float)Math.Round((double)Quad.Center.x, CentreOfBalanceOutput);
        Quad.Center.y = (float)Math.Round((double)Quad.Center.y, CentreOfBalanceOutput);

        if (Quad.Center.x < -1.0f) {
            Quad.Center.x = -1.0f;
        } else if (Quad.Center.x > 1.0f) {
            Quad.Center.x = 1.0f;
        }
        if (Quad.Center.y < -1.0f) {
            Quad.Center.y = -1.0f;
        } else if (Quad.Center.y > 1.0f) {
            Quad.Center.y = 1.0f;
        }
    }

    public void SetWeightWithOffset(float w) {
        Quad.Weight = w - empty.Weight;
    }

    public int RoundOff(float i) {
        return ((int)Math.Round(i / 10.0)) * 10;
    }

    private void SetAxisValues() {
        left = Quad.Front_Left + Quad.Back_Left;
        front = Quad.Front_Left + Quad.Front_Right;
        right = Quad.Front_Right + Quad.Back_Right;
        back = Quad.Back_Left + Quad.Back_Right;
        total = left + right;
    }

    /// <summary>
    /// To start a calibration we need to reset the values first
    /// Should be called once at the beginning of every new calibration
    /// </summary>
    public void ResetOffCalibration(Calibrate selected) {
        if (selected == Calibrate.EMPTY) {
            empty = new BB_Quadrant();
        } else if (selected == Calibrate.STAND) {
            stand = new BB_Quadrant();
        }
        calibrationQuad = new List<BB_Quadrant>();
        minX = new List<float>();
        minY = new List<float>();
        maxX = new List<float>();
        maxY = new List<float>();
    }

    /// <summary>
    /// Called in an update loop and accumulated over a period of time
    /// </summary>
    /// <param name="bb">Weight values from the balance board either kg or raw</param>
    public void UpdateCalibration(Vector4 bb, Vector2 c, float w, Calibrate cal) {
        if (cal == Calibrate.STAND) {
            c = c - empty.Center;
        }
        calibrationQuad.Add(new BB_Quadrant(bb, c, w));
    }

    public void FinishOffCalibration(Calibrate selected) {
        if (selected == Calibrate.EMPTY) {
            FinalizeCalibration(empty);
        } else if (selected == Calibrate.STAND) {
            FinalizeCalibration(stand, true);
        } else if (selected == Calibrate.LEAN) {
            FinalizeCalibration(minX, minY, maxX, maxY);
        }
    }

    public void UpdateLeanAbility(float v, Direction dir) {
        switch (dir) {
            case Direction.LEFT:
                minX.Add(v);
                break;
            case Direction.RIGHT:
                maxX.Add(v);
                break;
            case Direction.FORWARD:
                maxY.Add(v);
                break;
            case Direction.BACK:
                minY.Add(v);
                break;
            default:
                break;
        };
    }

    public void FinalizeCalibration(BB_Quadrant final, bool avgWeight = false) {
        foreach (BB_Quadrant q in calibrationQuad) {
            final.Front_Left += q.Front_Left;
            final.Front_Right += q.Front_Right;
            final.Back_Left += q.Back_Left;
            final.Back_Right += q.Back_Right;
            final.Center += q.Center;
            final.CenterRaw += q.CenterRaw;
            final.Weight += q.Weight;
        }
        int count = calibrationQuad.Count;
        final.Front_Left /= count;
        final.Front_Right /= count;
        final.Back_Left /= count;
        final.Back_Right /= count;
        final.Center /= count;
        final.CenterRaw /= count;
        final.Weight /= count;
        if (avgWeight) {
            AverageWeight = final.Weight - empty.Weight;
        }
    }

    public void FinalizeCalibration(List<float> _minX, List<float> _minY, List<float> _maxX, List<float> _maxY) {
        if (_minX.Count > 0)
        {
            foreach (float x in _minX)
            {
                minXavg += x;
            }
            minXavg /= _minX.Count;
            minXavg = 1.0f / Math.Abs(minXavg);

        }

        if (_minY.Count > 0)
        {
            foreach (float y in _minY)
            {
                minYavg += y;
            }
            minYavg /= _minY.Count;
            minYavg = 1.0f / Math.Abs(minYavg);
        }

        if (_maxX.Count > 0)
        {
            foreach (float x in _maxX)
            {
                maxXavg += x;
            }
            maxXavg /= _maxX.Count;
            maxXavg = 1.0f / Math.Abs(maxXavg);
        }

        if (_maxY.Count > 0)
        {
            foreach (float y in _maxY)
            {
                maxYavg += y;
            }
            maxYavg /= _maxY.Count;
            maxYavg = 1.0f / Math.Abs(maxYavg);
        }
    }
}
