using UnityEngine;

public class GameCMD : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Util.DeleteString("Rank");
            Util.Log(" 清理完成 ");
        }
    }
}
