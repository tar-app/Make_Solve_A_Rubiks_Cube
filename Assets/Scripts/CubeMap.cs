using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CubeMap : MonoBehaviour
{
    // キューブの状態を保持するクラス
    private CubeState cubeState;

    // 各面に対応する UI 上の Transform（3×3のImageオブジェクトが子として並んでいる）
    public Transform up;
    public Transform down;
    public Transform left;
    public Transform right;
    public Transform front;
    public Transform back;

    // 初期化処理（未使用）
    void Start()
    {
    }

    // 毎フレーム処理（未使用）
    void Update()
    {
    }

    // CubeState から各面の状態を取得して、UIに色を反映させる
    public void Set()
    {
        // 現在のシーン内から CubeState を検索して取得
        cubeState = FindFirstObjectByType<CubeState>();

        // 各面の状態（GameObjectリスト）を取得し、UI面に反映
        UpdateMap(cubeState.up, up);       // 上面
        UpdateMap(cubeState.down, down);   // 下面
        UpdateMap(cubeState.left, left);   // 左面
        UpdateMap(cubeState.right, right); // 右面
        UpdateMap(cubeState.front, front); // 前面
        UpdateMap(cubeState.back, back);   // 背面
    }

    // 指定された面の GameObject リストを UI 側に色で反映する処理
    void UpdateMap(List<GameObject> face, Transform side)
    {
        // ループ用のインデックス
        int index = 0;

        // side の子要素（Image）を順番に処理
        foreach (Transform imageTransform in side)
        {
            // face[index] の名前から1文字目を取得（F, B, U, D, L, R）
            char faceChar = face[index].name[0];

            // Image コンポーネントを取得
            Image image = imageTransform.GetComponent<Image>();

            // 名前の先頭文字に応じて色を設定（完全な if 分岐）
            if (faceChar == 'F')
            {
                image.color = new Color(1f, 0.4117647f, 0.7058823f, 1f); // ピンク
            }
            else if (faceChar == 'B')
            {
                image.color = Color.red;
            }
            else if (faceChar == 'U')
            {
                image.color = Color.yellow;
            }
            else if (faceChar == 'D')
            {
                image.color = Color.white;
            }
            else if (faceChar == 'L')
            {
                image.color = new Color(0.5647059f, 0.9333334f, 0.5647059f, 1f); // ライトグリーン
            }
            else if (faceChar == 'R')
            {
                image.color = Color.blue;
            }

            // 次の面へ
            index++;
        }
    }
}
