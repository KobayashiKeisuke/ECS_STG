using UnityEngine;
using UnityEngine.SceneManagement;

namespace GAME.TITLE.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class TitleButton : MonoBehaviour
    {
        //------------------------------------------
        // 定数関連
        //------------------------------------------
        #region ===== CONSTS =====

        #endregion //) ===== CONSTS =====
        //------------------------------------------
        // メンバ変数
        //------------------------------------------
        #region ===== MEMBER_VARIABLES =====

        /// <summary> 連打防止用 </summary>
        bool m_isClicked = false;
        #endregion //) ===== MEMBER_VARIABLES =====

        //------------------------------------------
        // 初期化周り
        //------------------------------------------
        #region ===== INITIALIZE =====

        #endregion //) ===== INITIALIZE =====

        //------------------------------------------
        // ボタンアクション
        //------------------------------------------
        #region ===== BUTTON_ACTION =====

        public void OnButtonClicked()
        {
            if( m_isClicked )
            {
                return;
            }
            m_isClicked = true;
            SceneManager.LoadScene("IngameScene", LoadSceneMode.Single);
        }
        #endregion //) ===== BUTTON_ACTION =====
    }
}