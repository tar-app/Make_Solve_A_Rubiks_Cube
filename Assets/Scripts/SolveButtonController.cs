using UnityEngine;
using UnityEngine.UI;

public class SolveButtonController : MonoBehaviour
{
    // 「解くボタン」の部品（Unity エディタからセットする）
    [SerializeField] private Button solveButton;

    // ゲーム開始時に一度だけ呼ばれる
    void Start()
    {
        // ボタンがセットされていれば、最初は押せないようにする
        if (solveButton != null)
        {
            solveButton.interactable = false;
        }
    }

    // ボタンを押せるようにする（外から呼び出せる）
    public void EnableSolveButton()
    {
        if (solveButton != null)
        {
            solveButton.interactable = true;
        }
    }

    // ボタンを押せないようにする（外から呼び出せる）
    public void DisableSolveButton()
    {
        if (solveButton != null)
        {
            solveButton.interactable = false;
        }
    }
}
