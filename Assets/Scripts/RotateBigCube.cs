using UnityEngine;
using UnityEngine.InputSystem;

public class RotateBigCube : MonoBehaviour
{
    // 右クリックでのスワイプ開始地点
    private Vector2 firstPressPos = Vector2.zero;

    // スワイプ終了地点
    private Vector2 secondPressPos = Vector2.zero;

    // スワイプ方向（正規化された2Dベクトル）
    private Vector2 currentSwipe = Vector2.zero;

    // 前回のマウス位置（ドラッグ用）
    private Vector3 previousMousePosition = Vector3.zero;

    // マウスの動き（前回との差分）
    private Vector3 mouseDelta = Vector3.zero;

    // 回転対象のCube（ゲームオブジェクト）
    public GameObject target;

    // 補間回転スピード
    private float speed = 200f;

    // 毎フレーム実行される処理（スワイプとドラッグを監視）
    void Update()
    {
        Swipe(); // スワイプ入力を検知してCubeを瞬時に回転
        Drag();  // ドラッグ中はCube全体を自由に回転
    }

    // マウス右ボタンを押してドラッグした場合のCube回転処理
    void Drag()
    {
        // 右ボタンを押している間
        if (Mouse.current.rightButton.isPressed)
        {
            // 現在のマウス位置を取得
            Vector3 currentMousePosition = Mouse.current.position.ReadValue();

            // 現在位置と前回位置の差分を計算（＝マウスの動き）
            mouseDelta = currentMousePosition - previousMousePosition;

            // X・Y軸ごとの動き
            float deltaX = mouseDelta.x;
            float deltaY = mouseDelta.y;

            // Cubeの回転量を計算（小さくするために0.1倍）
            float rotationX = deltaY * 0.1f;       // 上下の動き → X軸回転
            float rotationY = -deltaX * 0.1f;      // 左右の動き → Y軸回転（反転）

            // 計算した回転量からクォータニオンを作る
            Quaternion newRotation = Quaternion.Euler(rotationX, rotationY, 0f);

            // 新しい回転を現在の回転に加算する（回し続ける）
            transform.rotation = newRotation * transform.rotation;
        }
        else
        {
            // 右ボタンを離した後、Cubeをtarget方向にゆっくり補間
            if (transform.rotation != target.transform.rotation)
            {
                float step = speed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
            }
        }

        // 毎フレーム前回マウス位置を更新
        previousMousePosition = Mouse.current.position.ReadValue();
    }

    // スワイプ入力の開始・終了・方向の検出と、それに応じたCubeの回転
    void Swipe()
    {
        // 右クリックを押した瞬間 → スワイプの始点を記録
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            firstPressPos = Mouse.current.position.ReadValue();
        }

        // 右クリックを離した瞬間 → 終点を記録し、スワイプ方向を判定
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            secondPressPos = Mouse.current.position.ReadValue();

            // スワイプベクトルを計算
            float swipeX = secondPressPos.x - firstPressPos.x;
            float swipeY = secondPressPos.y - firstPressPos.y;

            // ベクトルの長さを求める（距離）
            float swipeLength = Mathf.Sqrt(swipeX * swipeX + swipeY * swipeY);

            // 距離が0でなければ正規化する（方向だけ抽出）
            if (swipeLength != 0f)
            {
                currentSwipe.x = swipeX / swipeLength;
                currentSwipe.y = swipeY / swipeLength;
            }
            else
            {
                currentSwipe = Vector2.zero;
            }

            // スワイプの方向に応じてCubeを90度単位で回転
            if (IsLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0f, 90f, 0f, Space.World);
            }
            else if (IsRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0f, -90f, 0f, Space.World);
            }
            else if (IsUpLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(90f, 0f, 0f, Space.World);
            }
            else if (IsUpRightSwipe(currentSwipe))
            {
                target.transform.Rotate(0f, 0f, -90f, Space.World);
            }
            else if (IsDownLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0f, 0f, 90f, Space.World);
            }
            else if (IsDownRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-90f, 0f, 0f, Space.World);
            }
        }
    }

    // 左方向のスワイプかどうか
    bool IsLeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0f && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    // 右方向のスワイプかどうか
    bool IsRightSwipe(Vector2 swipe)
    {
        return swipe.x > 0f && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    // 左上方向のスワイプかどうか
    bool IsUpLeftSwipe(Vector2 swipe)
    {
        return swipe.y > 0f && swipe.x < 0f;
    }

    // 右上方向のスワイプかどうか
    bool IsUpRightSwipe(Vector2 swipe)
    {
        return swipe.y > 0f && swipe.x > 0f;
    }

    // 左下方向のスワイプかどうか
    bool IsDownLeftSwipe(Vector2 swipe)
    {
        return swipe.y < 0f && swipe.x < 0f;
    }

    // 右下方向のスワイプかどうか
    bool IsDownRightSwipe(Vector2 swipe)
    {
        return swipe.y < 0f && swipe.x > 0f;
    }
}
