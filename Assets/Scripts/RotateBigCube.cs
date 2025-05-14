using UnityEngine;
using UnityEngine.InputSystem;

public class RotateBigCube : MonoBehaviour
{
    // 右クリックを押した最初の場所（スワイプの始まり）
    private Vector2 firstPressPos = Vector2.zero;

    // 右クリックを離した場所（スワイプの終わり）
    private Vector2 secondPressPos = Vector2.zero;

    // スワイプの方向（どっちに動かしたか）
    private Vector2 currentSwipe = Vector2.zero;

    // 前のフレームのマウスの位置（動きを比べる用）
    private Vector3 previousMousePosition = Vector3.zero;

    // マウスの動き（前と今の位置の差）
    private Vector3 mouseDelta = Vector3.zero;

    // 回したいキューブ（このゲームオブジェクト）
    public GameObject target;

    // 自動的に回すときの速さ
    private float speed = 200f;

    // 毎フレーム呼ばれる（ずっと監視してる）
    void Update()
    {
        Swipe(); // スワイプでキューブをパッと回す
        Drag();  // ドラッグ（押したまま動かす）でキューブをグルグル動かす
    }

    // 右クリックしながらマウスを動かすとキューブが回る
    void Drag()
    {
        if (Mouse.current.rightButton.isPressed) // 右クリック押してる間
        {
            Vector3 currentMousePosition = Mouse.current.position.ReadValue();
            mouseDelta = currentMousePosition - previousMousePosition;

            float deltaX = mouseDelta.x; // 横の動き
            float deltaY = mouseDelta.y; // 縦の動き

            // マウスの動きにあわせて、キューブを回す量を決める（0.1倍でちょっとだけ回す）
            float rotationX = deltaY * 0.1f;
            float rotationY = -deltaX * 0.1f;

            // 回す方向を作る（上に動かせば上に回る、みたいな）
            Quaternion newRotation = Quaternion.Euler(rotationX, rotationY, 0f);

            // 今の回転にさらに加える（連続して回っていく）
            transform.rotation = newRotation * transform.rotation;
        }
        else // ボタン離したとき
        {
            // 目標の向きに自動で戻るように少しずつ回す
            if (transform.rotation != target.transform.rotation)
            {
                float step = speed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
            }
        }

        // マウスの位置を毎回記録しておく
        previousMousePosition = Mouse.current.position.ReadValue();
    }

    // スワイプ操作（右クリック→スッと動かす）でキューブを90度カクッと回す
    void Swipe()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // 右クリックした瞬間のマウス位置
            firstPressPos = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            // 離したときの位置
            secondPressPos = Mouse.current.position.ReadValue();

            // どっちにどれくらい動いたか
            float swipeX = secondPressPos.x - firstPressPos.x;
            float swipeY = secondPressPos.y - firstPressPos.y;

            // 動いた距離を計算
            float swipeLength = Mathf.Sqrt(swipeX * swipeX + swipeY * swipeY);

            // 距離が0じゃなければ、方向だけを取り出す
            if (swipeLength != 0f)
            {
                currentSwipe.x = swipeX / swipeLength;
                currentSwipe.y = swipeY / swipeLength;
            }
            else
            {
                currentSwipe = Vector2.zero;
            }

            // スワイプの方向に応じて、キューブをそれぞれの方向に90度回す
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

    // 以下は「どっち方向にスワイプしたか」を判断する関数たち
    bool IsLeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0f && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool IsRightSwipe(Vector2 swipe)
    {
        return swipe.x > 0f && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool IsUpLeftSwipe(Vector2 swipe)
    {
        return swipe.y > 0f && swipe.x < 0f;
    }

    bool IsUpRightSwipe(Vector2 swipe)
    {
        return swipe.y > 0f && swipe.x > 0f;
    }

    bool IsDownLeftSwipe(Vector2 swipe)
    {
        return swipe.y < 0f && swipe.x < 0f;
    }

    bool IsDownRightSwipe(Vector2 swipe)
    {
        return swipe.y < 0f && swipe.x > 0f;
    }
}
