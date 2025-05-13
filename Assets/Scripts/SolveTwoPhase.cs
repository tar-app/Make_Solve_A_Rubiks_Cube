using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Kociemba; // 2段階解法ライブラリ
using JetBrains.Annotations; // （実行には不要。エディタ補助用）

public class SolveTwoPhase : MonoBehaviour
{
    // キューブ状態読み取り用
    public ReadCube readCube;

    // キューブ面情報保持クラス
    public CubeState cubeState;

    // Solver() を1回だけ実行するためのフラグ
    public bool doOnce = true;

    // 初期化：各クラスの参照を取得
    void Start()
    {
        readCube = FindFirstObjectByType<ReadCube>();
        cubeState = FindFirstObjectByType<CubeState>();
    }

    // 毎フレーム実行される処理
    void Update()
    {
        // キューブの状態読み取りが完了していて、かつ未実行ならSolverを呼ぶ
        if (CubeState.started == true && doOnce == true)
        {
            // 1回限りにするためにフラグを倒す
            doOnce = false;

            // 解法を求める処理を実行
            Solver();
        }
    }

    // 2段階法による解法を計算する処理
    public void Solver()
    {
        // 最新のキューブ状態を取得
        readCube.ReadState();

        // 文字列形式の状態を取得（54文字: URFDLB順）
        string moveString = cubeState.GetStateString();
        print(moveString); // デバッグ表示

        // 解法情報（解析メッセージ）を格納する変数
        string info = "";

        // 解法を取得（KociembaのSearchクラスを使用）
        // string solution = SearchRunTime.solution(moveString, out info, buildTables: true); ←必要なら切替
        string solution = Search.solution(moveString, out info);

        // 解法文字列をリスト形式に変換（例: "U F2 R'" → [U, F2, R']）
        List<string> solutionList = StringToList(solution);

        // 解法リストを Automate に渡して自動回転を開始させる
        Automate.moveList = solutionList;

        // 解法情報をデバッグ表示（手数や時間など）
        print(info);
    }

    // 解法文字列を List<string> に分割する処理
    List<string> StringToList(string solution)
    {
        // 空白で区切られた文字列を配列に分割
        string[] splitMoves = solution.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);

        // 分割した配列からリストを生成して返す
        List<string> solutionList = new List<string>(splitMoves);
        return solutionList;
    }
}
