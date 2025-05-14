using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CubeState : MonoBehaviour
{
    // キューブの6つの面。それぞれの面に9個の小さいブロックがある
    public List<GameObject> up = new List<GameObject>();    // 上の面
    public List<GameObject> down = new List<GameObject>();  // 下の面
    public List<GameObject> left = new List<GameObject>();  // 左の面
    public List<GameObject> right = new List<GameObject>(); // 右の面
    public List<GameObject> front = new List<GameObject>(); // 前の面
    public List<GameObject> back = new List<GameObject>();  // 後ろの面

    // キューブが自動で回っているかどうか（他のスクリプトと共有するため static）
    public static bool autoRotating = false;

    // キューブの準備が終わったかどうか（trueになれば操作できる）
    public static bool started = false;

    // ゲームスタート時に呼ばれるが、今は使っていない
    void Start()
    {
    }

    // 毎フレーム呼ばれるが、今は使っていない
    void Update()
    {
    }

    // 指定された面（9個）の小ブロックを、真ん中のブロックの下にまとめる
    public void PickUp(List<GameObject> cubeSide)
    {
        Transform centerPivot = cubeSide[4].transform.parent;

        foreach (GameObject face in cubeSide)
        {
            Transform parent = face.transform.parent;
            parent.SetParent(centerPivot); // 中心ピースの親にくっつける
        }
    }

    // 回し終わったあとの小ブロックたちを、元の場所に戻す
    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        foreach (GameObject littleCube in littleCubes)
        {
            if (littleCube != littleCubes[4])
            {
                var t = littleCube.transform.parent;
                t.transform.parent = pivot;

                // わずかなズレ補正だけ追加（回転は触らない）
                t.localPosition = new Vector3(
                    Mathf.Round(t.localPosition.x),
                    Mathf.Round(t.localPosition.y),
                    Mathf.Round(t.localPosition.z)
                );
            }
        }
    }

    // 指定された面のブロック名の「最初の1文字」だけをつなげた文字列を作る
    string GetSidesString(List<GameObject> side)
    {
        string sideString = "";

        foreach (GameObject face in side)
        {
            // 名前の最初の文字（例: "U1" → "U"）を取り出して追加する
            char firstChar = face.name[0];
            sideString += firstChar.ToString();
        }

        return sideString;
    }

    // 全6面の状態をひとつの長い文字列にまとめて返す（解くときのデータとして使う）
    public string GetStateString()
    {
        string stateString = "";

        // 解く順番にそって、それぞれの面の状態をつなげていく
        stateString += GetSidesString(up);     // 上
        stateString += GetSidesString(right);  // 右
        stateString += GetSidesString(front);  // 前
        stateString += GetSidesString(down);   // 下
        stateString += GetSidesString(left);   // 左
        stateString += GetSidesString(back);   // 後ろ

        return stateString;
    }
}
