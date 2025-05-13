using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor;

public class SelectFace : MonoBehaviour
{
    // キューブの現在状態を読み取るスクリプト
    private ReadCube readCube;

    // キューブの6面状態を保持するクラス
    private CubeState cubeState;

    // Raycastで検出対象とするレイヤーマスク（8番レイヤー）
    private int layerMask = 1 << 8;

    // 初期化時に実行される処理
    void Start()
    {
        // ReadCube スクリプトをシーン内から探して取得
        readCube = FindFirstObjectByType<ReadCube>();

        // CubeState スクリプトをシーン内から探して取得
        cubeState = FindFirstObjectByType<CubeState>();
    }

    // 毎フレーム実行される処理
    void Update()
    {
        // 左クリックが押された瞬間かつ、自動回転中でなければ処理開始
        if (Mouse.current.leftButton.wasPressedThisFrame && !CubeState.autoRotating)
        {
            // 現在の6面の状態を更新（Rayの読み直し）
            readCube.ReadState();

            // マウスの位置を取得
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // 画面のマウス位置からRay（カメラから前方へ）を生成
            Ray rayFromCamera = Camera.main.ScreenPointToRay(mousePosition);

            // Raycastでキューブの面にヒットするか調べる（100距離以内、指定レイヤーのみ）
            RaycastHit hitInfo;
            bool didHit = Physics.Raycast(rayFromCamera, out hitInfo, 100.0f, layerMask);

            if (didHit)
            {
                // 当たったオブジェクト（面）を取得
                GameObject hitFace = hitInfo.collider.gameObject;

                // キューブの6面をまとめたリストを作成
                List<List<GameObject>> allSides = new List<List<GameObject>>();

                allSides.Add(cubeState.up);
                allSides.Add(cubeState.down);
                allSides.Add(cubeState.left);
                allSides.Add(cubeState.right);
                allSides.Add(cubeState.front);
                allSides.Add(cubeState.back);

                // どの面に属しているかを1つずつ調べる
                foreach (List<GameObject> side in allSides)
                {
                    // 該当面のリストにクリックした面が含まれているかチェック
                    bool containsFace = side.Contains(hitFace);

                    if (containsFace)
                    {
                        // 回転のためにこの面をPickUp（Pivotに再配置）
                        cubeState.PickUp(side);

                        // 中央キューブ（index 4）の親から PivotRotation を取得し、回転開始
                        PivotRotation pivot = side[4].transform.parent.GetComponent<PivotRotation>();
                        pivot.Rotate(side);
                    }
                }
            }
        }
    }
}
