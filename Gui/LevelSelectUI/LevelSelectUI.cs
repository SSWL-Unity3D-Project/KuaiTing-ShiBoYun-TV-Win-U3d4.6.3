﻿using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    /// <summary>
    /// 开始按键.
    /// </summary>
    public GameObject StartBtObj;
    /// <summary>
    /// 选择关卡的动画.
    /// </summary>
    public Animator mAnimator;
    /// <summary>
    /// 已完成图集.
    /// </summary>
    public GameObject[] YiWanChengUI = new GameObject[4];
    int _mSelectLevel = 1;
    /// <summary>
    /// 选择的游戏关卡[1, 4].
    /// </summary>
    public int mSelectLevel
    {
        set
        {
            _mSelectLevel = value;
            Debug.Log("LevelSelectUI -> Level " + _mSelectLevel);
        }
        get
        {
            return _mSelectLevel;
        }
    }
    float TimeLastSelect = 0f;
    /// <summary>
    /// 循环动画UI总控.
    /// </summary>
    Loading mLoadingCom;

    public void Init(Loading loadingCom)
    {
        bool isActiveStartBt = false;
        mLoadingCom = loadingCom;
        isActiveStartBt = true;
        /*switch (NetworkRootMovie.GetInstance().eNetState)
        {
            case NetworkRootMovie.GameNetType.NoLink:
                {
                    isActiveStartBt = true;
                    break;
                }
            case NetworkRootMovie.GameNetType.Link:
                {
                    if (loadingCom.mGameModeSelect.eGameMode == NetworkRootMovie.GameMode.Link)
                    {
                        isActiveStartBt = true;
                    }
                    else
                    {
                        isActiveStartBt = true;
                    }
                    break;
                }

        }*/
        SetActiveStartBt(isActiveStartBt);

        for (int i = 0; i < 4; i++)
        {
            if (YiWanChengUI[i] != null)
            {
                YiWanChengUI[i].SetActive(false);
                //if (GlobalData.GetInstance().YiWanChengLvList.Contains(i + 1))
                //{
                //    YiWanChengUI[i].SetActive(true);
                //}
                //else
                //{
                //    YiWanChengUI[i].SetActive(false);
                //}
            }
        }
        gameObject.SetActive(true);

        /*if (loadingCom.mGameLinkPlayer != null)
        {
            loadingCom.mGameLinkPlayer.SetAcitveStartBt(false);
            loadingCom.mGameLinkPlayer.SetActiveLinkNameParent(false);
        }*/
    }

    void Update()
    {
        float steerVal = pcvr.GetInstance().mGetSteer;
        if (steerVal > 0f && Time.realtimeSinceStartup - TimeLastSelect > 0.5f)
        {
            if (mLoadingCom != null && mLoadingCom.m_LevelSource != null)
            {
                mLoadingCom.m_LevelSource.Play();
            }
            TimeLastSelect = Time.realtimeSinceStartup;
            int level = mSelectLevel;
            level++;
            if (level > 4)
            {
                level = 1;
            }
            mSelectLevel = level;

            //向右转动.
            string trigger = "right0" + mSelectLevel;
            mAnimator.SetTrigger(trigger);
            Debug.Log("aniName -> " + trigger);
        }

        if (steerVal < 0f && Time.realtimeSinceStartup - TimeLastSelect > 0.5f)
        {
            if (mLoadingCom != null && mLoadingCom.m_LevelSource != null)
            {
                mLoadingCom.m_LevelSource.Play();
            }
            TimeLastSelect = Time.realtimeSinceStartup;
            int level = mSelectLevel;
            level--;
            if (level < 1)
            {
                level = 4;
            }
            mSelectLevel = level;

            //向左转动.
            string trigger = "left0" + mSelectLevel;
            mAnimator.SetTrigger(trigger);
            Debug.Log("aniName -> " + trigger);
        }
    }

    void SetActiveStartBt(bool isActive)
    {
        StartBtObj.SetActive(isActive);
    }

    public void HiddenSelf()
    {
        gameObject.SetActive(false);
        mLoadingCom.mLoadSceneCount = mSelectLevel;
    }

    bool IsRemoveSelf = false;
    public void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            Destroy(gameObject);
        }
    }
}