using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Automate : MonoBehaviour
{
    private ReadCube readCube;
    private CubeState cubeState;
    private ShuffleButtonController shuffleButtonController;
    private SolveButtonController solveButtonController;
    private bool isShuffling = false;　// シャッフル実行中かどうかを示す内部フラグ（解法と区別するため）

    // 自動で回すときの手順（"U", "R'", などの指示）が入るリスト
    public static List<string> moveList = new List<string>();

    // キューブを回すときに使えるパターン（90度／180度／逆回転）
    private readonly List<string> allMoves = new List<string>()
    {
        "U", "D", "L", "R", "F", "B",
        "U2", "D2", "L2", "R2", "F2", "B2",
        "U'", "D'", "L'", "R'", "F'", "B'"
    };

    // ゲーム開始時に一度だけ呼ばれる（準備）
    void Start()
    {
        readCube = FindFirstObjectByType<ReadCube>();
        cubeState = FindFirstObjectByType<CubeState>();
        shuffleButtonController = FindFirstObjectByType<ShuffleButtonController>();
        solveButtonController = FindFirstObjectByType<SolveButtonController>();
    }

    // 毎フレーム呼ばれる（登録された動きを1つずつ実行していく）
    void Update()
    {
        // まだ回す手順が残っていて、現在回転中でなければ、次の手を実行する
        if (moveList.Count > 0 && !CubeState.autoRotating && CubeState.started)
        {
            DoMove(moveList[0]); // 次の手を実行
            moveList.RemoveAt(0); // 実行した手をリストから削除
        }

        // 全ての回転が終わったあと（最後の手を回し終えた瞬間）に一度だけ呼ばれる
        if (moveList.Count == 0 && !CubeState.autoRotating && CubeState.started)
        {
            // シャッフルボタンを再び有効にする（操作可能に戻す）
            shuffleButtonController?.EnableShuffleButton();

            // シャッフルの完了時のみ Solveボタンを再び有効にする
            if (isShuffling)
            {
                solveButtonController?.EnableSolveButton();
                isShuffling = false;
            }
        }
    }

    // シャッフル用：ランダムな手順を作って moveList に入れる
    public void Shuffle()
    {
        List<string> moves = new List<string>();

        // 手の数は 10から29 の間でランダム
        int shuffleLength = Random.Range(10, 30);

        for (int i = 0; i < shuffleLength; i++)
        {
            int randomMove = Random.Range(0, allMoves.Count);
            moves.Add(allMoves[randomMove]); // ランダムに手を追加
        }

        moveList = moves; // シャッフル手順を登録
        CubeState.autoRotating = false; // キューブが自動で回っている最中かどうかを示すフラグ（false = 手動）
        isShuffling = true; // ← シャッフル中かどうかを示すフラグ（true = シャッフル中）

        // シャッフル開始と同時にボタン無効化
        shuffleButtonController?.DisableShuffleButton();

        // シャッフル開始と同時にボタン無効化
        solveButtonController?.DisableSolveButton();
    }

    // 指定された動き（例："L'"）に応じて、対応する面を回す
    void DoMove(string move)
    {
        readCube.ReadState();            // 状態を読み取って最新にする
        CubeState.autoRotating = true;  // 自動で回しているフラグをONにする

        // 動きの種類に応じて回す面と方向を決める
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

    // 面と角度を指定して、実際に自動回転させる（Pivot に回転を頼む）
    void RotateSide(List<GameObject> side, float angle)
    {
        // 真ん中のブロックの親から回転処理をするスクリプトを取得
        PivotRotation pivot = side[4].transform.parent.GetComponent<PivotRotation>();

        // 指定の面と角度で回転スタート
        pivot.StartAutoRotate(side, angle);
    }
}
