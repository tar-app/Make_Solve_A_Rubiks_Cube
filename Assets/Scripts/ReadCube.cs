using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ReadCube : MonoBehaviour
{
    // キューブの6つの面の中心（ここから光を飛ばす）
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;

    // それぞれの面から光を飛ばす9つの場所（3×3マス分）
    private List<GameObject> upRays = new List<GameObject>();
    private List<GameObject> downRays = new List<GameObject>();
    private List<GameObject> leftRays = new List<GameObject>();
    private List<GameObject> rightRays = new List<GameObject>();
    private List<GameObject> frontRays = new List<GameObject>();
    private List<GameObject> backRays = new List<GameObject>();

    // 飛ばした光が当たった面の記録（あとで使う）
    private List<GameObject> facesHit = new List<GameObject>();

    // 光が当たる対象を「レイヤー8」に限定する設定
    private int layerMask = 1 << 8;

    // 他のスクリプトとのやりとり用
    private CubeState cubeState;
    private CubeMap cubeMap;

    // 光の出どころとして使う空のオブジェクト（あらかじめ作っておく）
    public GameObject emptyGo;

    // ゲームが始まったときに一度だけ実行される
    void Start()
    {
        SetRayTransforms(); // 光を飛ばす準備をする

        cubeState = FindFirstObjectByType<CubeState>(); // キューブの色状態を持ってるスクリプト
        cubeMap = FindFirstObjectByType<CubeMap>();     // 見た目に反映するスクリプト

        ReadState(); // キューブの色を読み取って保存する

        CubeState.started = true; // 準備ができたことを他のスクリプトに知らせる
    }

    // 毎フレーム呼ばれる（今回は使ってない）
    void Update()
    {
    }

    // キューブの6つの面の色を読み取る
    public void ReadState()
    {
        // 念のため、スクリプトをもう一度探して確認
        cubeState = FindFirstObjectByType<CubeState>();
        cubeMap = FindFirstObjectByType<CubeMap>();

        // 各面から光を飛ばして、どんな色があるかを読み取る
        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);

        cubeMap.Set(); // 読み取った情報を見た目に反映
    }

    // 各面の中央から光を飛ばすための起点を9つ作る
    void SetRayTransforms()
    {
        upRays = BuildRays(tUp, new Vector3(90f, 90f, 0f));      // 上：下に光を飛ばす
        downRays = BuildRays(tDown, new Vector3(270f, 90f, 0f)); // 下：上に光を飛ばす
        leftRays = BuildRays(tLeft, new Vector3(0f, 180f, 0f));  // 左：右に光を飛ばす
        rightRays = BuildRays(tRight, new Vector3(0f, 0f, 0f));  // 右：左に光を飛ばす
        frontRays = BuildRays(tFront, new Vector3(0f, 90f, 0f)); // 前：奥に光を飛ばす
        backRays = BuildRays(tBack, new Vector3(0f, 270f, 0f));  // 後：手前に光を飛ばす
    }

    // 指定された位置から3×3＝9か所に光を飛ばす起点を作る
    List<GameObject> BuildRays(Transform rayTransform, Vector3 directionEuler)
    {
        int rayCount = 0;
        List<GameObject> rays = new List<GameObject>();

        // 上から下、左から右に順番に作っていく
        for (int y = 1; y >= -1; y--)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector3 startPos = new Vector3(
                    rayTransform.localPosition.x + x,
                    rayTransform.localPosition.y + y,
                    rayTransform.localPosition.z
                );

                // 空オブジェクトを置いて、そこから光を飛ばすようにする
                GameObject rayStart = Instantiate(emptyGo, startPos, Quaternion.identity, rayTransform);
                rayStart.name = rayCount.ToString();

                rays.Add(rayStart);
                rayCount++;
            }
        }

        // 光の向きを決めておく
        rayTransform.localRotation = Quaternion.Euler(directionEuler);

        return rays;
    }

    // 指定された面の9か所から光を飛ばして、ぶつかったパネルを調べる
    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> facesHit = new List<GameObject>();

        foreach (GameObject rayStart in rayStarts)
        {
            Vector3 rayOrigin = rayStart.transform.position;

            RaycastHit hit;

            // forward方向（まっすぐ）に光を飛ばす
            bool didHit = Physics.Raycast(rayOrigin, rayTransform.forward, out hit, Mathf.Infinity, layerMask);

            if (didHit)
            {
                // 何かに当たったら黄色い線を描く（見た目用）
                Debug.DrawRay(rayOrigin, rayTransform.forward * hit.distance, Color.yellow);

                // 当たった面を記録する
                facesHit.Add(hit.collider.gameObject);
            }
            else
            {
                // 何も当たらなかったら緑色の長い線を出す（デバッグ用）
                Debug.DrawRay(rayOrigin, rayTransform.forward * 1000f, Color.green);
            }
        }

        return facesHit;
    }
}
