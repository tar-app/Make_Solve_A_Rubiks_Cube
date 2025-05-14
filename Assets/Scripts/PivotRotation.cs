using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PivotRotation : MonoBehaviour
{
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

    // キューブの状態を読み取ったり管理したりするスクリプト
    private ReadCube readCube;
    private CubeState cubeState;

    // ゲーム開始時に一度だけ呼ばれる
    void Start()
    {
        // 他のスクリプトを見つけて使えるようにする
        readCube = FindFirstObjectByType<ReadCube>();
        cubeState = FindFirstObjectByType<CubeState>();
    }

    // 毎フレームの最後に呼ばれる（回転処理をここで実行）
    void LateUpdate()
    {
        // マウスでドラッグ中かつ自動回転していない場合
        if (dragging == true && autoRotating == false)
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
        if (autoRotating == true)
        {
            AutoRotate(); // 目標の角度に近づけていく
        }
    }

    // マウスを動かして回す処理（プレイヤー操作）
    private void SpinSide(List<GameObject> side)
    {
        rotation = Vector3.zero; // 回転の初期化

        // 今のマウス位置と前の位置の差を計算
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        Vector2 mouseOffset = currentMousePos - mouseRef;

        // 動いた量（横＋縦）×感度で回転の強さを決める
        float combinedOffset = mouseOffset.x + mouseOffset.y;
        float rotationAmount = combinedOffset * sensitivity;

        // どの面を回すかによって、回す軸を変える
        if (side == cubeState.up)
            rotation.y = rotationAmount;
        else if (side == cubeState.down)
            rotation.y = -rotationAmount;
        else if (side == cubeState.left)
            rotation.z = rotationAmount;
        else if (side == cubeState.right)
            rotation.z = -rotationAmount;
        else if (side == cubeState.front)
            rotation.x = -rotationAmount;
        else if (side == cubeState.back)
            rotation.x = rotationAmount;

        // 自分自身（Pivot）を回転させる
        transform.Rotate(rotation, Space.Self);

        // マウス位置を更新して次回比較できるようにする
        mouseRef = currentMousePos;
    }

    // プレイヤーが面を選んで回そうとしたときに呼ばれる
    public void Rotate(List<GameObject> side)
    {
        activeSide = side; // 今回回す面を記録
        mouseRef = Mouse.current.position.ReadValue(); // 今のマウス位置を記録
        dragging = true; // ドラッグ開始！
    }

    // 解く処理やシャッフルで自動的に回すときに呼ばれる
    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        // 今回回す面のブロックたちをひとつにまとめる（回しやすくする）
        cubeState.PickUp(side);

        // 中央のブロックの位置を使って、回す方向を決める
        Vector3 centerPosition = side[4].transform.parent.transform.localPosition;
        Vector3 rotationAxis = Vector3.zero - centerPosition;

        // 回す角度と方向から、目標の向きを作る
        Quaternion rotationQuat = Quaternion.AngleAxis(angle, rotationAxis);
        targetQuaternion = rotationQuat * transform.localRotation;

        activeSide = side;
        autoRotating = true; // 自動回転をスタート
    }

    // 回し終わるときに、ちょうど90度にピタッとそろえる
    public void RotateToRightAngle()
    {
        // 今の角度を数値で取得
        Vector3 currentEuler = transform.localEulerAngles;

        // それぞれの軸（X,Y,Z）を90度の倍数に丸める
        currentEuler.x = Mathf.Round(currentEuler.x / 90f) * 90f;
        currentEuler.y = Mathf.Round(currentEuler.y / 90f) * 90f;
        currentEuler.z = Mathf.Round(currentEuler.z / 90f) * 90f;

        // 目標となる角度を作る
        targetQuaternion = Quaternion.Euler(currentEuler);

        autoRotating = true; // 自動でその角度に近づけていく
    }

    // 自動回転中の処理（毎フレーム呼ばれてだんだん近づける）
    private void AutoRotate()
    {
        dragging = false; // 手動操作はできなくする

        float step = speed * Time.deltaTime; // 今回の回転量を計算

        // 今の向きを、目標の向きに少しずつ近づける
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        // 角度の差が1度以下になったら完了とみなす
        float angleDifference = Quaternion.Angle(transform.localRotation, targetQuaternion);
        if (angleDifference <= 1f)
        {
            // 最終的な位置を目標にピタッと合わせる
            transform.localRotation = targetQuaternion;

            // 回していた面を元の場所に戻す
            cubeState.PutDown(activeSide, transform.parent);

            // キューブ全体の情報をもう一度読み取り直す
            readCube.ReadState();

            // フラグをリセット（操作できるように戻す）
            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
        }
    }
}
