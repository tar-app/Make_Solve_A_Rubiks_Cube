using UnityEngine;
using UnityEngine.UI;

public class ShuffleButtonController : MonoBehaviour
{
    // シャッフルボタンの部品をセットする（Unityエディタから指定する）
    [SerializeField] private Button shuffleButton;

    // ゲーム開始時に一度だけ呼ばれる
    void Start()
    {

        // ボタンがちゃんと指定されていれば、有効にする
        if (shuffleButton != null)
        {
            shuffleButton.interactable = true;
        }
    }

    // ボタンを押せるようにする（外から呼び出せる）
    public void EnableShuffleButton()
    {
        if (shuffleButton != null)
        {
            shuffleButton.interactable = true;
        }
    }

    // ボタンを押せなくする（外から呼び出せる）
    public void DisableShuffleButton()
    {
        if (shuffleButton != null)
        {
            shuffleButton.interactable = false;
        }
    }
}
