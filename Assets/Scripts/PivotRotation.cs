using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PivotRotation : MonoBehaviour
{
    private ReadCube readCube;
    private CubeState cubeState;
    private ShuffleButtonController shuffleButtonController;
    private SolveButtonController solveButtonController;

    // 今回回す面（9つのブロック）
    private List<GameObject> activeSide;

    // 前のマウスの位置（動きを調べるために記録しておく）
    private Vector2 mouseRef;

    // プレイヤーがマウスでドラッグしているかどうか
    private bool dragging = false;

    // 自動的に回している最中かどうか
    private bool autoRotating = false;

    // マウスをどれくらい動かしたらどれくらい回るか（感度）
    private float sensitivity = 0.4f;

    // 自動で回るときの速さ
    private float speed = 300f;

    // 1フレーム分の回転量（X, Y, Z）
    private Vector3 rotation;

    // 回すべき目標の角度（ここに向かって回していく）
    private Quaternion targetQuaternion;

    // ゲーム開始時に一度だけ呼ばれる
    void Start()
    {
        readCube = FindFirstObjectByType<ReadCube>();
        cubeState = FindFirstObjectByType<CubeState>();
        shuffleButtonController = FindFirstObjectByType<ShuffleButtonController>();
        solveButtonController = FindFirstObjectByType<SolveButtonController>();
    }

    // 毎フレームの最後に呼ばれる（回転処理をここで実行）
    void LateUpdate()
    {
        // マウスでドラッグ中かつ自動回転していない場合
        if (dragging && !autoRotating)
        {
            SpinSide(activeSide); // マウスの動きに応じて面を回す

            // マウスを離したら、自動で90度にピッタリ調整する
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                dragging = false;
                RotateToRightAngle();
            }
        }

        // 自動回転しているとき（シャッフルや解くとき）
        if (autoRotating)
        {
            AutoRotate(); // 目標の角度に近づけていく
        }
    }

    // マウスを動かして回す処理（プレイヤー操作）
    private void SpinSide(List<GameObject> side)
    {
        rotation = Vector3.zero;

        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        Vector2 mouseOffset = currentMousePos - mouseRef;

        float combinedOffset = mouseOffset.x + mouseOffset.y;
        float rotationAmount = combinedOffset * sensitivity;

        if (side == cubeState.up) rotation.y = rotationAmount;
        else if (side == cubeState.down) rotation.y = -rotationAmount;
        else if (side == cubeState.left) rotation.z = rotationAmount;
        else if (side == cubeState.right) rotation.z = -rotationAmount;
        else if (side == cubeState.front) rotation.x = -rotationAmount;
        else if (side == cubeState.back) rotation.x = rotationAmount;

        transform.Rotate(rotation, Space.Self);

        mouseRef = currentMousePos;
    }

    // プレイヤーが面を選んで回そうとしたときに呼ばれる
    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        mouseRef = Mouse.current.position.ReadValue();
        dragging = true;
    }

    // 解く処理やシャッフルで自動的に回すときに呼ばれる
    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        cubeState.PickUp(side);

        Vector3 centerPosition = side[4].transform.parent.transform.localPosition;
        Vector3 rotationAxis = Vector3.zero - centerPosition;

        Quaternion rotationQuat = Quaternion.AngleAxis(angle, rotationAxis);
        targetQuaternion = rotationQuat * transform.localRotation;

        activeSide = side;
        autoRotating = true;
    }

    // 回し終わるときに、ちょうど90度にピタッとそろえる
    public void RotateToRightAngle()
    {
        Vector3 currentEuler = transform.localEulerAngles;

        currentEuler.x = Mathf.Round(currentEuler.x / 90f) * 90f;
        currentEuler.y = Mathf.Round(currentEuler.y / 90f) * 90f;
        currentEuler.z = Mathf.Round(currentEuler.z / 90f) * 90f;

        targetQuaternion = Quaternion.Euler(currentEuler);
        autoRotating = true;

        // プレイヤーが面を回したあとにSolveボタンを有効にする（仕様通り）
        solveButtonController?.EnableSolveButton();
    }

    // 自動回転中の処理（毎フレーム呼ばれてだんだん近づける）
    private void AutoRotate()
    {
        dragging = false;

        float step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        float angleDifference = Quaternion.Angle(transform.localRotation, targetQuaternion);
        if (angleDifference <= 1f)
        {
            transform.localRotation = targetQuaternion;

            cubeState.PutDown(activeSide, transform.parent);
            readCube.ReadState();

            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
        }
    }
}
