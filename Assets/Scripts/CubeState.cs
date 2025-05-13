using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CubeState : MonoBehaviour
{
    // キューブの6面（各面9つの小キューブ）を格納するリスト
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();
    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();

    // 自動回転中かどうかのフラグ（全体で共有される）
    public static bool autoRotating = false;

    // キューブの状態初期読み取りが完了したかのフラグ
    public static bool started = false;

    // 初期化処理（未使用）
    void Start()
    {
    }

    // 毎フレームの更新処理（未使用）
    void Update()
    {
    }

    // 特定の面（9個）の小キューブを、中央のキューブを軸にして親オブジェクトにまとめる
    public void PickUp(List<GameObject> cubeSide)
    {
        // すべての小キューブに対して処理
        foreach (GameObject face in cubeSide)
        {
            // 真ん中のキューブ（インデックス4）以外のみ処理
            if (face != cubeSide[4])
            {
                // 2階層上のTransformを中央のTransformに設定（回転軸を揃える）
                face.transform.parent.transform.parent = cubeSide[4].transform.parent;
            }
        }
    }

    // 回転が終わった後、小キューブをPivotから元の親に戻す
    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        // すべての小キューブに対して処理
        foreach (GameObject littleCube in littleCubes)
        {
            // 中央のキューブ以外のみ処理
            if (littleCube != littleCubes[4])
            {
                // 親の親をPivotに戻す（全体の位置を戻す）
                littleCube.transform.parent.transform.parent = pivot;
            }
        }
    }

    // 指定された面のキューブの名前（先頭1文字）を文字列として連結して返す
    string GetSidesString(List<GameObject> side)
    {
        // 面の状態を表す文字列を初期化
        string sideString = "";

        // 各キューブの名前の先頭1文字を取得して結合
        foreach (GameObject face in side)
        {
            char firstChar = face.name[0];
            sideString += firstChar.ToString();
        }

        return sideString;
    }

    // 全体のキューブの状態を文字列として出力（Kociembaの入力フォーマット用）
    public string GetStateString()
    {
        // 状態全体の文字列を初期化
        string stateString = "";

        // 各面の文字列を順番に取得・連結
        stateString += GetSidesString(up);     // 上
        stateString += GetSidesString(right);  // 右
        stateString += GetSidesString(front);  // 前
        stateString += GetSidesString(down);   // 下
        stateString += GetSidesString(left);   // 左
        stateString += GetSidesString(back);   // 後

        return stateString;
    }
}
