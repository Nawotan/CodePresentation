// ***  Draw Parabolic Ball 3D Trajectory ***
//
//  1. Add LineRender to the GameObject
//  2. Create StartPoint- and FinishPoint-GameObject
//  3. Enjoy!
//
//  Created 2021 by Anton Chura
//
//  anton4405anton@yandex.ru
//

using UnityEngine;

/// <summary>
/// This class draws a parabola of a ball's flight depending on two points (start and finish) and a shooting angle.
/// </summary>
public class BallTrajectory : MonoBehaviour {

    public static BallTrajectory ballTrajectory;

    [Header("Points")]
    [Tooltip("If you load Start point in the inspector, and not in the code")]
    public bool UseInspectorStartPoint = false;

    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(UseInspectorStartPoint))]
    [Tooltip("The starting point of an inverted parabola")]
    public Transform StartTransform;

    [Tooltip("If you load Finish point in the inspector, and not in the code")]
    public bool UseInspectorFinishPoint = false;

    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(UseInspectorFinishPoint))]
    [Tooltip("The finish point of the inverted parabola")]
    public Transform FinishTransform;

    [Header("Parameters")]
    [Tooltip("The number of parabola points")]
    public int N = 100;
    [Tooltip("Angle with a horizontal plane")]
    public float Angle = 45;
    [Tooltip("Gravitational constant")]
    public float g = 10;

    private LineRenderer lineRenderer;
    private float AngleTangent;
    private float QuadraticConstant;// QuadraticConstant is the constant A in Ax^2 + Bx + C = 0
    private bool protocol = true;
    private Vector3 StartPoint = new Vector3(), FinishPoint = new Vector3();
    private Vector3 StartVelocity;
    private float StartSpeed;// StartVelocity magnitude

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        ballTrajectory = this;
    }

    void Start() {
        AngleTangent = Mathf.Tan(Angle * Mathf.PI / 180);

        if (UseInspectorStartPoint && StartTransform == null) {
            protocol = false;
            Debug.Log("BallTrajectory: Start point is not defined in Inspector!");
        }
        if (UseInspectorFinishPoint && FinishTransform == null) {
            protocol = false;
            Debug.Log("BallTrajectory: Finish point is not defined in Inspector!");
        }
        if (lineRenderer == null) {
            protocol = false;
            Debug.Log("BallTrajectory: LineRender is not added to the GameObject");
        } else lineRenderer.positionCount = N;
    }

    void Update() {
        if (protocol) {
            if (UseInspectorStartPoint) StartPoint = StartTransform.position;
            if (UseInspectorFinishPoint) FinishPoint = FinishTransform.position;

            // Horizontal direction between StartPoint and FinishPoint
            Vector3 HorizontalDirection = new Vector3();
            HorizontalDirection.x = FinishPoint.x - StartPoint.x;
            HorizontalDirection.z = FinishPoint.z - StartPoint.z;

            // Horizontal Direction with magnitude = 1
            Vector3 NormalizedHorizontalDirection = HorizontalDirection / (N - 1);

            // Finish Point in vertical plane 
            Vector2 FinishPoint2D = new Vector2();
            FinishPoint2D.x = Vector3.Magnitude(HorizontalDirection);
            FinishPoint2D.y = FinishPoint.y - StartPoint.y;

            QuadraticConstant = (FinishPoint2D.x * AngleTangent - FinishPoint2D.y) / (FinishPoint2D.x * FinishPoint2D.x);

            // DrawCurve
            for (int n = 0; n < N; n++) {
                lineRenderer.SetPosition(n, StartPoint + n * NormalizedHorizontalDirection + new Vector3(0, getLocalHeight(Vector3.Magnitude(n * NormalizedHorizontalDirection)), 0));
            }

            // Start Speed
            StartSpeed = Mathf.Sqrt(g * (1 + AngleTangent * AngleTangent) / (2 * QuadraticConstant));

            // Start Velocity
            Vector3 VelocityDirection = NormalizedHorizontalDirection + new Vector3(0, getLocalHeight(Vector3.Magnitude(NormalizedHorizontalDirection)), 0);
            Vector3 NormVelocityDirection = VelocityDirection / Vector3.Magnitude(VelocityDirection);
            StartVelocity = StartSpeed * NormVelocityDirection;
        }
    }

    private void OnDestroy() {
        ballTrajectory = null;
    }

    private float getLocalHeight(float x) {
        return x * AngleTangent - QuadraticConstant * x * x;
    }

    /// <summary>
    /// Get the start point of the inverted parabola
    /// </summary>
    public Vector3 GetStartPoint() {
        if (UseInspectorStartPoint) {
            if (StartTransform != null) return StartTransform.position;
            else {
                Debug.Log("BallTrajectory: Start point is not defined in Inspector!");
                return new Vector3();
            }
        }
        return StartPoint;
    }

    /// <summary>
    /// Set the start point of the inverted parabola
    /// </summary>
    public bool SetStartPoint(Vector3 StartPoint) {
        if (!UseInspectorStartPoint) this.StartPoint = StartPoint;
        return !UseInspectorStartPoint;
    }

    /// <summary>
    /// Get the endpoint of the inverted parabola
    /// </summary>
    public Vector3 GetFinishPoint() {
        if (UseInspectorFinishPoint) {
            if (FinishTransform != null) return FinishTransform.position;
            else {
                Debug.Log("BallTrajectory: Finish point is not defined in Inspector!");
                return new Vector3();
            }
        }
        return FinishPoint;
    }

    /// <summary>
    /// Set the endpoint of the inverted parabola
    /// </summary>
    public bool SetFinishPoint(Vector3 FinishPoint) {
        if (!UseInspectorFinishPoint) this.FinishPoint = FinishPoint;
        return !UseInspectorFinishPoint;
    }

    /// <summary>
    /// Get the starting Velocity of the ball thrown on the parabola
    /// </summary>
    public Vector3 getStartVelocity() {
        if (StartVelocity != null) return StartVelocity;
        Debug.Log("BallTrajectory: StartVelocity is not defined!");
        return new Vector3();
    }

    /// <summary>
    /// Get the starting speed of the ball thrown on the parabola
    /// </summary>
    public float getStartSpeed() {
        if (StartVelocity != null) return StartSpeed;
        Debug.Log("BallTrajectory: StartVelocity is not defined!");
        return 0;
    }
}
