using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ReadCube : MonoBehaviour
{
    // 各面の中心Transform（この位置からRayを出す）
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;

    // 各面に9本ずつRayを出す起点オブジェクトリスト
    private List<GameObject> upRays = new List<GameObject>();
    private List<GameObject> downRays = new List<GameObject>();
    private List<GameObject> leftRays = new List<GameObject>();
    private List<GameObject> rightRays = new List<GameObject>();
    private List<GameObject> frontRays = new List<GameObject>();
    private List<GameObject> backRays = new List<GameObject>();

    // Rayがヒットしたキューブ面（デバッグ用）
    private List<GameObject> facesHit = new List<GameObject>();

    // レイヤーマスク（レイヤー8のオブジェクトのみに当たる）
    private int layerMask = 1 << 8;

    // 他のクラスを参照
    private CubeState cubeState;
    private CubeMap cubeMap;

    // Ray起点として使う空のGameObjectプレハブ
    public GameObject emptyGo;

    // ゲーム開始時に一度だけ実行される処理
    void Start()
    {
        // 各面のRay発射準備をする
        SetRayTransforms();

        // CubeStateとCubeMapの参照を取得
        cubeState = FindFirstObjectByType<CubeState>();
        cubeMap = FindFirstObjectByType<CubeMap>();

        // キューブの状態を読み取って保存
        ReadState();

        // キューブの準備完了フラグをON
        CubeState.started = true;
    }

    // 毎フレーム実行（ここでは使わない）
    void Update()
    {
    }

    // 6面すべての色状態を読み取る
    public void ReadState()
    {
        // 念のため再取得（冗長だけど安全）
        cubeState = FindFirstObjectByType<CubeState>();
        cubeMap = FindFirstObjectByType<CubeMap>();

        // 各面の色を読み取って CubeState に保存
        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);

        // 読み取った状態をUI上に反映する
        cubeMap.Set();
    }

    // 各面にRay起点（9個）を作り、向きを設定する
    void SetRayTransforms()
    {
        upRays = BuildRays(tUp, new Vector3(90f, 90f, 0f));      // 上面 → 真下に向ける
        downRays = BuildRays(tDown, new Vector3(270f, 90f, 0f)); // 下面 → 真上に向ける
        leftRays = BuildRays(tLeft, new Vector3(0f, 180f, 0f));  // 左面 → 右向きにする
        rightRays = BuildRays(tRight, new Vector3(0f, 0f, 0f));  // 右面 → 左向きにする
        frontRays = BuildRays(tFront, new Vector3(0f, 90f, 0f)); // 前面 → 後ろ向き
        backRays = BuildRays(tBack, new Vector3(0f, 270f, 0f));  // 背面 → 手前向き
    }

    // 指定されたTransformを基準に、3x3＝9個のRay発射地点を作る
    List<GameObject> BuildRays(Transform rayTransform, Vector3 directionEuler)
    {
        int rayCount = 0;
        List<GameObject> rays = new List<GameObject>();

        // 上から下（Y=1→-1）、左から右（X=-1→1）に走査
        for (int y = 1; y >= -1; y--)
        {
            for (int x = -1; x <= 1; x++)
            {
                // 起点のローカル座標を作成
                Vector3 startPos = new Vector3(
                    rayTransform.localPosition.x + x,
                    rayTransform.localPosition.y + y,
                    rayTransform.localPosition.z
                );

                // 空のオブジェクトを生成してRay起点にする
                GameObject rayStart = Instantiate(emptyGo, startPos, Quaternion.identity, rayTransform);
                rayStart.name = rayCount.ToString();

                rays.Add(rayStart);
                rayCount++;
            }
        }

        // Rayの向きを決める回転をTransformに反映
        rayTransform.localRotation = Quaternion.Euler(directionEuler);

        return rays;
    }

    // 1面分（9個）のRayを飛ばし、それぞれヒットしたキューブ面を返す
    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> facesHit = new List<GameObject>();

        // 9個すべてのRay起点に対してループ
        foreach (GameObject rayStart in rayStarts)
        {
            // Rayを飛ばすスタート位置を取得
            Vector3 rayOrigin = rayStart.transform.position;

            // Rayのヒット情報を格納する変数
            RaycastHit hit;

            // RayをTransform.forward方向に無限長で飛ばす（レイヤー制限付き）
            bool didHit = Physics.Raycast(rayOrigin, rayTransform.forward, out hit, Mathf.Infinity, layerMask);

            if (didHit)
            {
                // Rayが何かに当たった場合 → 線を黄色で描画（デバッグ用）
                Debug.DrawRay(rayOrigin, rayTransform.forward * hit.distance, Color.yellow);

                // ヒットしたGameObject（面）をリストに追加
                facesHit.Add(hit.collider.gameObject);
            }
            else
            {
                // 何も当たらなかった場合 → 線を緑色で長く描画
                Debug.DrawRay(rayOrigin, rayTransform.forward * 1000f, Color.green);
            }
        }

        return facesHit;
    }
}
