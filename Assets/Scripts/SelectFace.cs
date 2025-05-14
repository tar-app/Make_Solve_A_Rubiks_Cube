using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor;

public class SelectFace : MonoBehaviour
{
    // キューブの状態を読み取るためのスクリプト
    private ReadCube readCube;

    // キューブの各面の情報を持っているスクリプト
    private CubeState cubeState;

    // クリックしたときに調べる対象を「レイヤー8」に限定する設定
    private int layerMask = 1 << 8;

    // ゲームが始まったときに一度だけ実行される
    void Start()
    {
        // ReadCube を見つけて使えるようにする
        readCube = FindFirstObjectByType<ReadCube>();

        // CubeState を見つけて使えるようにする
        cubeState = FindFirstObjectByType<CubeState>();
    }

    // 毎フレーム実行される（クリックを監視している）
    void Update()
    {
        // 左クリックされた瞬間、かつ自動回転していないときだけ反応する
        if (Mouse.current.leftButton.wasPressedThisFrame && !CubeState.autoRotating)
        {
            // キューブの最新の状態を読み取る（Rayで確認するため）
            readCube.ReadState();

            // マウスの画面上の位置を取得
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // マウスの位置から「まっすぐ前」に光線（Ray）を飛ばす
            Ray rayFromCamera = Camera.main.ScreenPointToRay(mousePosition);

            // Ray がキューブの面に当たったかを調べる（距離100以内、レイヤー8のみ）
            RaycastHit hitInfo;
            bool didHit = Physics.Raycast(rayFromCamera, out hitInfo, 100.0f, layerMask);

            if (didHit)
            {
                // 当たった面のオブジェクトを取得する
                GameObject hitFace = hitInfo.collider.gameObject;

                // すべての面（上・下・左・右・前・後）をまとめてリストにする
                List<List<GameObject>> allSides = new List<List<GameObject>>();
                allSides.Add(cubeState.up);
                allSides.Add(cubeState.down);
                allSides.Add(cubeState.left);
                allSides.Add(cubeState.right);
                allSides.Add(cubeState.front);
                allSides.Add(cubeState.back);

                // どの面に当たったかを順番に調べる
                foreach (List<GameObject> side in allSides)
                {
                    // この面の中に、当たったブロックが含まれているか？
                    bool containsFace = side.Contains(hitFace);

                    if (containsFace)
                    {
                        // この面をまとめて動かせるように準備する（Pivot に集める）
                        cubeState.PickUp(side);

                        // 中央のブロックの親オブジェクトから PivotRotation を取得して、
                        // プレイヤー操作による回転を開始する
                        PivotRotation pivot = side[4].transform.parent.GetComponent<PivotRotation>();
                        pivot.Rotate(side);
                    }
                }
            }
        }
    }
}
