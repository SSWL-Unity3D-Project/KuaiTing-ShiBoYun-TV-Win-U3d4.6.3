﻿using UnityEngine;

/// <summary>
/// 排行榜UI控制.
/// </summary>
public class RankListUICtrl : MonoBehaviour
{
    /// <summary>
    /// 头像列表.
    /// TouXiangImgArray[x] -> 0 猪猪侠, 1 波比, 2 超人强, 3 菲菲.
    /// </summary>
    public Texture[] TouXiangImgArray;
    /// <summary>
    /// 排名列表头像.
    /// </summary>
    public UITexture[] RankTouXiangArray;
    /// <summary>
    /// 排名数据UI列表控制.
    /// </summary>
    public RankDtUI[] mRankDtUIArray;
    /// <summary>
    /// 积分父级Tr列表.
    /// </summary>
    public Transform[] mJiFenTrArray;
    /// <summary>
    /// 积分Tr.
    /// </summary>
    public Transform mJiFenTr;
    public UISprite[] JiFenSpriteArray;

    /// <summary>
    /// 显示排行榜UI.
    /// </summary>
    public void ShowRankListUI()
    {
        int indexVal = 0;
        RankManage.RankData rankDt = null;
        for (int i = 0; i < RankTouXiangArray.Length; i++)
        {
            if (rankDt.IsPlayerData)
            {
                mJiFenTr.parent = mJiFenTrArray[i];
                mJiFenTr.localPosition = Vector3.zero;
            }
            rankDt = PlayerController.GetInstance().RankDtManage.RankDtList[i];
            indexVal = (int)rankDt.RankType;
            RankTouXiangArray[i].mainTexture = TouXiangImgArray[indexVal];
            mRankDtUIArray[i].ShowTimeUsedVal((int)rankDt.TimeUsedVal);
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 显示玩家积分.
    /// </summary>
    public void ShowJiFenInfo(int jiFen)
    {
        int jiFenTmp = 0;
        string jiFenStr = jiFen.ToString();
        for (int i = 0; i < 6; i++)
        {
            if (jiFenStr.Length > i)
            {
                jiFenTmp = jiFen % 10;
                JiFenSpriteArray[i].spriteName = jiFenTmp.ToString();
                jiFen = (int)(jiFen / 10f);
                JiFenSpriteArray[i].enabled = true;
            }
            else
            {
                JiFenSpriteArray[i].enabled = false;
            }
        }
    }
}