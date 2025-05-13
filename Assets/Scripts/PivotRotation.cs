using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PivotRotation : MonoBehaviour
{
    // 現在回転対象となっている面（9個の小Cube）
    private List<GameObject> activeSide;

    // マウスの基準位置（前フレーム）
    private Vector2 mouseRef;

    // マウスドラッグ中かどうか
    private bool dragging = false;

    // 自動回転処理中かどうか
    private bool autoRotating = false;

    // マウス感度（回転の速さを決める）
    private float sensitivity = 0.4f;

    // 自動回転スピード
    private float speed = 300f;

    // このフレームでの回転量
    private Vector3 rotation;

    // 自動回転の目標角度
    private Quaternion targetQuaternion;

    // 外部クラスへの参照（キューブ状態とリーダー）
    private ReadCube readCube;
    private CubeState cubeState;

    // 初期化処理：他クラスを検索して参照を取得
    void Start()
    {
        readCube = FindFirstObjectByType<ReadCube>();
        cubeState = FindFirstObjectByType<CubeState>();
    }

    // 毎フレーム末尾に実行される処理（回転制御）
    void LateUpdate()
    {
        // プレイヤーによるドラッグ回転
        if (dragging == true && autoRotating == false)
        {
            SpinSide(activeSide); // マウスに応じて面回転

            // マウスボタンを離したらスナップ処理開始
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                dragging = false;
                RotateToRightAngle(); // 90度単位に調整
            }
        }

        // 自動回転中の補間処理
        if (autoRotating == true)
        {
            AutoRotate();
        }
    }

    // プレイヤーのマウス移動による回転処理
    private void SpinSide(List<GameObject> side)
    {
        // 回転ベクトル初期化
        rotation = Vector3.zero;

        // 現在のマウス位置と前回の差分を取得
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        Vector2 mouseOffset = currentMousePos - mouseRef;

        // XとYの合計移動量に感度をかける
        float combinedOffset = mouseOffset.x + mouseOffset.y;
        float rotationAmount = combinedOffset * sensitivity;

        // 面の種類に応じて回転軸を決定
        if (side == cubeState.up)
        {
            rotation.y = rotationAmount;
        }
        else if (side == cubeState.down)
        {
            rotation.y = -rotationAmount;
        }
        else if (side == cubeState.left)
        {
            rotation.z = rotationAmount;
        }
        else if (side == cubeState.right)
        {
            rotation.z = -rotationAmount;
        }
        else if (side == cubeState.front)
        {
            rotation.x = -rotationAmount;
        }
        else if (side == cubeState.back)
        {
            rotation.x = rotationAmount;
        }

        // 自身をローカル軸で回転させる
        transform.Rotate(rotation, Space.Self);

        // マウス基準位置を更新
        mouseRef = currentMousePos;
    }

    // プレイヤーの操作によって回転を開始
    public void Rotate(List<GameObject> side)
    {
        // 回転対象を記録
        activeSide = side;

        // 現在のマウス位置を記録
        mouseRef = Mouse.current.position.ReadValue();

        // ドラッグ開始
        dragging = true;
    }

    // 指定角度で自動回転を開始する（シャッフルや解法用）
    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        // 対象の小Cubeを一時的にPivotの子にする
        cubeState.PickUp(side);

        // 回転軸を中央キューブの逆ベクトルで決定
        Vector3 centerPosition = side[4].transform.parent.transform.localPosition;
        Vector3 rotationAxis = Vector3.zero - centerPosition;

        // 指定角度の回転クォータニオンを作成
        Quaternion rotationQuat = Quaternion.AngleAxis(angle, rotationAxis);

        // 現在の回転に掛けて目標回転を決定
        targetQuaternion = rotationQuat * transform.localRotation;

        activeSide = side;
        autoRotating = true;
    }

    // 現在の回転を最も近い90度単位に調整する処理
    public void RotateToRightAngle()
    {
        // 現在の角度（オイラー角）を取得
        Vector3 currentEuler = transform.localEulerAngles;

        // 各軸の角度を90度単位で丸める
        currentEuler.x = Mathf.Round(currentEuler.x / 90f) * 90f;
        currentEuler.y = Mathf.Round(currentEuler.y / 90f) * 90f;
        currentEuler.z = Mathf.Round(currentEuler.z / 90f) * 90f;

        // 丸めた角度から目標回転を作成
        targetQuaternion = Quaternion.Euler(currentEuler);

        autoRotating = true;
    }

    // 自動回転の補間処理（毎フレーム呼ばれる）
    private void AutoRotate()
    {
        // 手動操作を禁止
        dragging = false;

        // 回転補間ステップ量を計算
        float step = speed * Time.deltaTime;

        // 現在の回転を目標回転へ近づける
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        // 目標角との差が1度以下なら完了
        float angleDifference = Quaternion.Angle(transform.localRotation, targetQuaternion);
        if (angleDifference <= 1f)
        {
            // 正確な角度に補正
            transform.localRotation = targetQuaternion;

            // 小Cubeたちを元の親に戻す
            cubeState.PutDown(activeSide, transform.parent);

            // 状態を再読み込み（面構成の更新）
            readCube.ReadState();

            // 全フラグを解除して終了
            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
        }
    }
}
