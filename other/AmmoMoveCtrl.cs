﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 子弹运动控制.
/// </summary>
public class AmmoMoveCtrl : MonoBehaviour
{
    /// <summary>
    /// 子弹数据信息.
    /// </summary>
    public class AmmoDt
    {
        public AmmoType AmmoState = AmmoType.Null;
        /// <summary>
        /// 子弹要击中的目标.
        /// </summary>
        public Transform AimObj;
        /// <summary>
        /// 子弹击中的坐标.
        /// </summary>
        public Vector3 PosHit;
    }

    /// <summary>
    /// Null 普通子弹.
    /// GenZongDan 跟踪子弹.
    /// </summary>
    public enum AmmoType
    {
        Null,
        GenZongDan,
    }
    AmmoType AmmoState = AmmoType.Null;
    /// <summary>
    /// 跟踪弹跟随的目标.
    /// </summary>
    Transform GenZongTr;
    /// <summary>
    /// 子弹速度.
    /// </summary>
    public float AmmoSpeed;
    bool IsDestoryThis = false;
    /// <summary>
    /// 子弹爆炸粒子特效,需要挂上自销毁脚本.
    /// </summary>
    public GameObject LiZiPrefab;
    AmmoDt mAmmoInfo;

    // Update is called once per frame
    void Update()
    {
	    if (AmmoState == AmmoType.GenZongDan)
        {
            float mvDis = AmmoSpeed * Time.deltaTime;
            if (Vector3.Distance(GenZongTr.position, transform.position) <= mvDis)
            {
                //子弹击中目标.
                OnDestoryThis();
                return;
            }
            Vector3 vecForward = Vector3.Normalize(GenZongTr.position - transform.position);
            transform.position += vecForward * mvDis;
            transform.forward = Vector3.Lerp(transform.forward, vecForward, 15f * Time.deltaTime);
        }
	}

    void OnDestoryThis()
    {
        if (IsDestoryThis)
        {
            return;
        }

        if (LiZiPrefab != null)
        {
            Instantiate(LiZiPrefab, transform.position, transform.rotation);
        }
    }

    public void InitMoveAmmo(AmmoDt ammoInfo)
    {
        mAmmoInfo = ammoInfo;
        switch (ammoInfo.AmmoState)
        {
            case AmmoType.Null:
                {
                    Vector3[] posArray = new Vector3[2];
                    posArray[0] = transform.position;
                    posArray[1] = ammoInfo.PosHit;
                    iTween.MoveTo(gameObject, iTween.Hash("path", posArray,
                                                       "speed", AmmoSpeed,
                                                       "orienttopath", true,
                                                       "easeType", iTween.EaseType.linear,
                                                       "oncomplete", "MoveAmmoOnCompelteITween"));
                    break;
                }
            case AmmoType.GenZongDan:
                {
                    GenZongTr = ammoInfo.AimObj;
                    break;
                }
        }
        AmmoState = ammoInfo.AmmoState;
    }

    void MoveAmmoOnCompelteITween()
    {
        if (AmmoType.Null == AmmoState)
        {
            if (mAmmoInfo.AimObj != null)
            {
                DaoJuCtrl daoJuCom = mAmmoInfo.AimObj.GetComponent<DaoJuCtrl>();
                if (daoJuCom.DaoJuState == DaoJuCtrl.DaoJuType.ZhangAiWu)
                {
                    //障碍物爆炸. 
                    daoJuCom.OnDestoryThis();
                }
            }
        }
        OnDestoryThis();
    }
}