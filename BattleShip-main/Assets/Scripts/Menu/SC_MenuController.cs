using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MenuController : MonoBehaviour
{
    #region Buttons
    public void Btn_Menu_SinglePlayer()
    {
        SC_MenuLogic.Instance.Btn_Menu_SinglePlayer();
    }
    public void Btn_Menu_MultiPlayer()
    {
        SC_MenuLogic.Instance.Btn_Menu_MultiPlayer();
    }
    public void Btn_Menu_Options()
    {
        SC_MenuLogic.Instance.Btn_Menu_Options();
    }
    public void Btn_Loading_Back()
    {
        SC_MenuLogic.Instance.Btn_Loading_Back();
    }
    public void Btn_MultiPlayer_Back()
    {
        SC_MenuLogic.Instance.Btn_MultiPlayer_Back();  
    }
    public void Btn_Menu_StudentInfo()
    {
        SC_MenuLogic.Instance.Btn_Menu_StudentInfo();
    }
    public void Btn_StudentInfo_Back()
    {
        SC_MenuLogic.Instance.Btn_StudentInfo_Back();
    }
    public void Btn_StudentInfo_Options()
    {
        SC_MenuLogic.Instance.Btn_StudentInfo_Options();
    }
    public void Btn_Options_Back()
    {
        SC_MenuLogic.Instance.Btn_Options_Back();
    }
    public void Btn_StudentInfo_CV()
    {
        SC_MenuLogic.Instance.Btn_StudentInfo_CV();
    }
    public void Btn_MultiPlayer_Play()
    {
        SC_MenuLogic.Instance.Btn_MultiPlayer_Play();
    }
    public void Btn_MultiPlayer_EnterRoom()
    {
        SC_MenuLogic.Instance.Btn_MultiPlayer_EnterRoom();
    }

    #endregion
    #region Sliders
    public void Slider_OnValueChanged()
    {
        SC_MenuLogic.Instance.Slider_OnValueChanged();
    }
    public void Slider_MusicVolChange()
    {
        SC_MenuLogic.Instance.Slider_MusicVolChange();
    }
    #endregion
}
