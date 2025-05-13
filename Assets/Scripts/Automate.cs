using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Automate : MonoBehaviour
{
    // 自動回転に使う手順リスト
    public static List<string> moveList = new List<string>();

    // 使用可能な全ての回転手
    private readonly List<string> allMoves = new List<string>()
    {
        "U", "D", "L", "R", "F", "B",
        "U2", "D2", "L2", "R2", "F2", "B2",
        "U'", "D'", "L'", "R'", "F'", "B2'"
    };

    // キューブ状態読み取りと面保持の参照
    private ReadCube readCube;
    private CubeState cubeState;

    // 初期化
    void Start()
    {
        readCube = FindFirstObjectByType<ReadCube>();
        cubeState = FindFirstObjectByType<CubeState>();
    }

    // 毎フレームの更新処理：moveList を順次実行
    void Update()
    {
        // moveList に手があり、自動回転中でなく、ゲーム開始済なら
        if (moveList.Count > 0 && !CubeState.autoRotating && CubeState.started)
        {
            DoMove(moveList[0]);  // 最初の手を実行
            moveList.RemoveAt(0); // 実行済みの手を削除
        }
    }

    // シャッフル用の手順をランダムで生成
    public void Shuffle()
    {
        List<string> moves = new List<string>();

        // ランダムな長さ（10から29手）で回転手を作る
        int shuffleLength = Random.Range(10, 30);

        for (int i = 0; i < shuffleLength; i++)
        {
            int randomMove = Random.Range(0, allMoves.Count);
            moves.Add(allMoves[randomMove]);
        }

        moveList = moves;
    }

    // 指定の手（文字列）に応じて該当面を回転
    void DoMove(string move)
    {
        readCube.ReadState();            // 状態読み取り
        CubeState.autoRotating = true;  // 回転開始フラグON

        if (move == "U") RotateSide(cubeState.up, -90f);
        if (move == "U'") RotateSide(cubeState.up, 90f);
        if (move == "U2") RotateSide(cubeState.up, -180f);

        if (move == "D") RotateSide(cubeState.down, -90f);
        if (move == "D'") RotateSide(cubeState.down, 90f);
        if (move == "D2") RotateSide(cubeState.down, -180f);

        if (move == "L") RotateSide(cubeState.left, -90f);
        if (move == "L'") RotateSide(cubeState.left, 90f);
        if (move == "L2") RotateSide(cubeState.left, -180f);

        if (move == "R") RotateSide(cubeState.right, -90f);
        if (move == "R'") RotateSide(cubeState.right, 90f);
        if (move == "R2") RotateSide(cubeState.right, -180f);

        if (move == "F") RotateSide(cubeState.front, -90f);
        if (move == "F'") RotateSide(cubeState.front, 90f);
        if (move == "F2") RotateSide(cubeState.front, -180f);

        if (move == "B") RotateSide(cubeState.back, -90f);
        if (move == "B'") RotateSide(cubeState.back, 90f);
        if (move == "B2") RotateSide(cubeState.back, -180f);
    }

    // 回転対象の面と角度を受け取って、自動回転を呼び出す
    void RotateSide(List<GameObject> side, float angle)
    {
        PivotRotation pivot = side[4].transform.parent.GetComponent<PivotRotation>();
        pivot.StartAutoRotate(side, angle);
    }
}
