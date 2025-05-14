using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CubeMap : MonoBehaviour
{
    // キューブの今の状態を持っているクラス
    private CubeState cubeState;

    // 各面に対応する UI（3×3の色つきマスがある）
    public Transform up;    // 上
    public Transform down;  // 下
    public Transform left;  // 左
    public Transform right; // 右
    public Transform front; // 前
    public Transform back;  // 後ろ

    // ゲーム開始時に一度だけ呼ばれる（今は使っていない）
    void Start()
    {
    }

    // 毎フレーム呼ばれる（今は使っていない）
    void Update()
    {
    }

    // CubeState から各面の情報をもらって、UI の色を変える
    public void Set()
    {
        // キューブの状態を持っているスクリプトを見つける
        cubeState = FindFirstObjectByType<CubeState>();

        // 各面の状態を UI に反映する（3×3マスの色を更新）
        UpdateMap(cubeState.up, up);       // 上面
        UpdateMap(cubeState.down, down);   // 下面
        UpdateMap(cubeState.left, left);   // 左面
        UpdateMap(cubeState.right, right); // 右面
        UpdateMap(cubeState.front, front); // 前面
        UpdateMap(cubeState.back, back);   // 後ろ
    }

    // ある面（例：上や前）の 3×3 の色を UI 上で塗り分ける
    void UpdateMap(List<GameObject> face, Transform side)
    {
        int index = 0;

        // UI 側のマス目（Image）をひとつずつ見ていく
        foreach (Transform imageTransform in side)
        {
            // 今見ている面のブロック名（例："F1"）の先頭文字（例：'F'）を取り出す
            char faceChar = face[index].name[0];

            // 色を変えるための Image パーツを取得
            Image image = imageTransform.GetComponent<Image>();

            // ブロックの名前の頭文字に応じて、UI の色を塗り分ける
            if (faceChar == 'F') // 前面
            {
                image.color = new Color(1f, 0.4117647f, 0.7058823f, 1f); // ピンク
            }
            else if (faceChar == 'B') // 後ろ
            {
                image.color = Color.red;
            }
            else if (faceChar == 'U') // 上
            {
                image.color = Color.yellow;
            }
            else if (faceChar == 'D') // 下
            {
                image.color = Color.white;
            }
            else if (faceChar == 'L') // 左
            {
                image.color = new Color(0.5647059f, 0.9333334f, 0.5647059f, 1f); // 明るい緑
            }
            else if (faceChar == 'R') // 右
            {
                image.color = Color.blue;
            }

            index++; // 次のマスに進む
        }
    }
}
