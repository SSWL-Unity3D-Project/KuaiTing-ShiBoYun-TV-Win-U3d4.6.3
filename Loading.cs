﻿using UnityEngine;
using System.Collections;
using System;

public class Loading : SSGameMono
{
    /// <summary>
    /// UI摄像机.
    /// </summary>
    public UICamera mUICamera;
    /// <summary>
    /// 加密校验中.
    /// </summary>
    //public GameObject m_JiaMiJiaoYanZhong;
    [HideInInspector]
    public SSLedByAudioCtrl mLedAudioScript;
    public LogoAnimation mLogoAni;
    private string CoinNumSet = "1";
	private string InsertCoinNum = "";

	public UITexture m_BeginTex;
	//public Texture   m_LoadingTex;
	public UITexture m_InsertTex;
	public UISprite CoinNumSetTex;
	public UISprite m_InsertNumS;
	public UISprite m_InsertNumG;
	private int m_InserNum = 0;
	private int m_CoinNumSet = 0;
    private bool m_IsBeginOk = false;

	private float m_PressTimmer = 0.0f;
	private float m_InserTimmer = 0.0f;
	private bool m_IsStartGame =false;
	public AudioSource m_TbSource;
	public AudioSource m_BeginSource;
    public AudioSource m_LevelSource;
    public GameObject m_Loading;
	public GameObject m_Tishi;
	private string GameMode = "";

	public UITexture m_pTishiTexture;
	public Texture[] m_pTexture;

	public MovieTexture m_MovieTex;
	public UITexture m_FreeTexture;
	public GameObject m_ToubiObj;

	private bool timmerstar = false;
	private float timmerforstar = 0.0f;
	public static bool m_HasBegin = false;
	public bool IsLuPingTest;
    internal SSUICenter m_SSUICenterCom;
    void Start ()
	{
		if (IsLuPingTest)
        {
			gameObject.SetActive(false);
		}

		m_HasBegin = false;
		GameMode = ReadGameInfo.GetInstance ().ReadGameStarMode();
		if(GameMode == ReadGameInfo.GameMode.Oper.ToString())
		{
			m_FreeTexture.enabled = false;
			CoinNumSet = ReadGameInfo.GetInstance ().ReadStarCoinNumSet();
			InsertCoinNum = ReadGameInfo.GetInstance ().ReadInsertCoinNum();
			CoinNumSetTex.spriteName = CoinNumSet;
			m_InserNum = Convert.ToInt32(InsertCoinNum);
            m_CoinNumSet = Convert.ToInt32(CoinNumSet);
            UpdateInsertCoin();
			UpdateTex();
		}
		else
		{
			m_ToubiObj.SetActive(false);
			m_FreeTexture.enabled = true;
		}
		m_Loading.SetActive(false);
		InputEventCtrl.GetInstance().mListenPcInputEvent.ClickSetEnterBtEvent += ClickSetEnterBtEvent;
        if (GameMode == ReadGameInfo.GameMode.Free.ToString())
        {
            InputEventCtrl.GetInstance().mListenPcInputEvent.ClickCloseDongGanBtEvent += ClickStartBtOneEvent;
        }
        else
        {
            InputEventCtrl.GetInstance().mListenPcInputEvent.ClickTVYaoKongEnterBtEvent += ClickStartBtOneEvent;
            InputEventCtrl.GetInstance().mListenPcInputEvent.ClickTVYaoKongExitBtEvent += ClickTVYaoKongExitBtEvent;
        }

        if (m_InserNum >= m_CoinNumSet && GameMode == ReadGameInfo.GameMode.Oper.ToString())
        {
            UpdateTex();
            //ClickStartBtOneEvent(InputEventCtrl.ButtonState.DOWN); 关闭自动开始游戏.
        }

        m_SSUICenterCom = gameObject.AddComponent<SSUICenter>();
        if (mUICamera != null)
        {
            m_SSUICenterCom.Init(mUICamera.transform);
        }
        
        InputEventCtrl.GetInstance().OnCaiPiaJiChuPiaoEvent += OnCaiPiaJiChuPiaoEvent;
        InputEventCtrl.GetInstance().OnCaiPiaJiWuPiaoEvent += OnCaiPiaJiWuPiaoEvent;
    }

    private void ClickTVYaoKongExitBtEvent(InputEventCtrl.ButtonState val)
    {
        if (val == InputEventCtrl.ButtonState.UP)
        {
            if (m_SSUICenterCom != null)
            {
                m_SSUICenterCom.SpawnExitGameDlg();
            }
        }
    }

    void OnCaiPiaJiWuPiaoEvent(pcvrTXManage.CaiPiaoJi val)
    {
        Debug.Log(val + ":: CaiPiaoJi wuPiao!");
    }

    void OnCaiPiaJiChuPiaoEvent(pcvrTXManage.CaiPiaoJi val)
    {
        GlobalData.GetInstance().CaiPiaoCur--;
        Debug.Log("CaiPiaoCur == " + GlobalData.GetInstance().CaiPiaoCur);
    }

	//float m_LastJiaoYanTime = 0f;
    void Update ()
	{
		/*if (m_JiaMiJiaoYanZhong.activeSelf)
		{
			if (VerifyEnvironmentObj.VerifyIsSucceedShowImg
			    || (Time.time - m_LastJiaoYanTime > 8f && GameRoot.m_VerifyEnvironmentObj == null))
			{
				//隐藏校验中.
				SetActiveJiaMiJiaoYan(false);
			}
		}*/

		if (!m_IsStartGame) {
			UpdateTex();
		}
        
        if (pcvr.bIsHardWare)
        {
			if (GlobalData.GetInstance().CoinCur != m_InserNum && GameMode == ReadGameInfo.GameMode.Oper.ToString())
            {
				m_InserNum = GlobalData.GetInstance().CoinCur - 1;
				OnClickInsertBt();
			}
        }
		else
		{
			//if (pcvr.IsTestNoInput)
			//{
			//	return;
			//}

			//if(Input.GetKeyDown(KeyCode.T) && GameMode == ReadGameInfo.GameMode.Oper.ToString())
			//{
			//	OnClickInsertBt();
			//}
		}
		OnLoadingClicked();
	}

	void ClickStartBtOneEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.UP)
        {
			return;
		}

        if (m_SSUICenterCom != null && m_SSUICenterCom.m_ExitGameUI != null)
        {
            return;
        }

        if (m_IsStartGame == true)
        {
            return;
        }

        if (mLevelSelectUI == null)
        {
            SpawnLevelSelectUI();
        }
        else
        {
            RemoveLevelSelectUI();
            OnClickBeginBt();
        }
	}

	void ClickSetEnterBtEvent(InputEventCtrl.ButtonState val)
	{
		if (val == InputEventCtrl.ButtonState.UP) {
			return;
		}

		if (m_HasBegin) {
			return;
		}
		
		XkGameCtrl.IsLoadingLevel = true;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		Application.LoadLevel(6);
	}

	void UpdateInsertCoin()
	{
		int n = 1;
		int num = m_InserNum;
		int temp = num;
		while(num > 9)
		{
			num /= 10;
			n++;
		}
		if(n > 2)
		{
			m_InsertNumS.spriteName = "9";
			m_InsertNumG.spriteName = "9";
		}
		else if(n==2)
		{
			int shiwei = (int)(temp/10);
			int gewei = (int)(temp-shiwei*10);
			m_InsertNumS.spriteName = shiwei.ToString();
			m_InsertNumG.spriteName = gewei.ToString();
		}
		else if(n == 1)
		{
			m_InsertNumS.spriteName = "0";
			m_InsertNumG.spriteName = temp.ToString();
		}
	}

	void UpdateTex()
	{
		if(GameMode == ReadGameInfo.GameMode.Free.ToString() || m_InserNum >= Convert.ToInt32(CoinNumSet))
		{
			m_InserTimmer = 0.0f;
			m_IsBeginOk = true;
			m_InsertTex.enabled = false;
			m_BeginTex.enabled =true;

            if (m_pTishiTexture.gameObject.activeInHierarchy == true)
            {
                m_pTishiTexture.enabled = true;
            }
			m_PressTimmer+=(Time.deltaTime / Time.timeScale);
			if(m_PressTimmer >= 0.0f && m_PressTimmer <= 0.5f)
			{
				m_BeginTex.enabled =true;
                if (m_pTishiTexture.gameObject.activeInHierarchy == true)
                {
                    m_pTishiTexture.mainTexture = m_pTexture[0];
                }
			}
			else if(m_PressTimmer > 0.5f && m_PressTimmer <= 1.0f)
			{
				m_BeginTex.enabled =false;
                if (m_pTishiTexture.gameObject.activeInHierarchy == true)
                {
                    m_pTishiTexture.mainTexture = m_pTexture[1];
                }
			}
			else
			{
				m_PressTimmer = 0.0f;
			}
			//pcvr.StartBtLight = StartLightState.Shan;
		}
		else
		{
			//pcvr.StartBtLight = StartLightState.Mie;
			m_InserTimmer+=(Time.deltaTime / Time.timeScale);
			m_IsBeginOk = false;
			m_InsertTex.enabled = true;
			m_BeginTex.enabled =false;
			m_pTishiTexture.enabled = false;
			m_PressTimmer = 0.0f;
			if(m_InserTimmer >= 0.0f && m_InserTimmer <= 0.4f)
			{
				m_InsertTex.enabled = true;
			}
			else if(m_InserTimmer > 0.4f && m_InserTimmer <= 0.8f)
			{
				m_InsertTex.enabled = false;
			}
			else
			{
				m_InserTimmer = 0.0f;
			}
		}
	}
	IEnumerator loadScene(int num)   
	{
		XkGameCtrl.IsLoadingLevel = true;
		Resources.UnloadUnusedAssets();
		GC.Collect();
		AsyncOperation async = Application.LoadLevelAsync(num);   
		yield return async;		
	}

	void OnClickInsertBt()
    {
        m_InserNum++;
        m_TbSource.Play();
        ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());
        UpdateInsertCoin();
        if (m_InserNum >= m_CoinNumSet)
        {
            UpdateTex();
            ClickStartBtOneEvent(InputEventCtrl.ButtonState.DOWN);
        }
    }
	void OnClickBeginBt()
	{
		if (PlayerControllerForMoiew.IsLoadMovieLevel) {
			return;
		}

		if(m_IsBeginOk && !m_HasBegin)
		{
			m_BeginSource.Play();
			m_IsStartGame = true;
			if(GameMode == ReadGameInfo.GameMode.Oper.ToString())
			{
				m_InserNum -= Convert.ToInt32(CoinNumSet);
				UpdateInsertCoin();
				ReadGameInfo.GetInstance().WriteInsertCoinNum(m_InserNum.ToString());

				if (pcvr.bIsHardWare) {
					pcvr.GetInstance().mPcvrTXManage.SubPlayerCoin(Convert.ToInt32(CoinNumSet), pcvrTXManage.PlayerCoinEnum.player01);
				}
			}
			m_Tishi.SetActive(false);
			m_Loading.SetActive(true);
			timmerstar = true;
			m_HasBegin = true;
		}
	}
    
    int _mLoadSceneCount = 0;
    /// <summary>
    /// 加载游戏的关卡信息.
    /// </summary>
    [HideInInspector]
    public int mLoadSceneCount
    {
        set
        {
            Debug.Log("Loading -> mLoadSceneCount == " + value);
            _mLoadSceneCount = value;
        }
        get
        {
            return _mLoadSceneCount;
        }
    }
    //static int LoadSceneCount;
	void OnLoadingClicked()
	{
		if(timmerstar)
		{
			timmerforstar += Time.deltaTime;
			if(timmerforstar > 1.5f)
			{
                int sceneCount = (mLoadSceneCount % (Application.levelCount - 3)) + 2;
                Debug.Log("OnLoadingClicked -> sceneCount =================== " + sceneCount);
                StartCoroutine (loadScene(sceneCount));
				timmerstar = false;
                //LoadSceneCount++;
            }
		}
	}

    /*public void SetActiveJiaMiJiaoYan(bool isActive)
	{
		m_LastJiaoYanTime = Time.time;
		m_JiaMiJiaoYanZhong.SetActive(isActive);
	}*/
    
    /// <summary>
    /// 游戏场景选择控制脚本.
    /// </summary>
    [HideInInspector]
    public LevelSelectUI mLevelSelectUI;
    /// <summary>
    /// 产生选择游戏场景UI.
    /// </summary>
    void SpawnLevelSelectUI()
    {
        if (mUICamera == null)
        {
            SSDebug.LogWarning("SpawnLevelSelectUI -> mUICamera was null");
            return;
        }

        if (mLevelSelectUI == null)
        {
            GameObject objPrefab = (GameObject)Resources.Load("Prefab/Gui/LevelSelect/LevelSelect");
            if (objPrefab != null)
            {
                SSDebug.Log("SpawnLevelSelectUI...");
                GameObject obj = (GameObject)Instantiate(objPrefab, mUICamera.transform);
                mLevelSelectUI = obj.GetComponent<LevelSelectUI>();
                mLevelSelectUI.Init(this);

                if (m_pTishiTexture != null)
                {
                    m_pTishiTexture.gameObject.SetActive(false);
                }
            }
            else
            {
                SSDebug.LogWarning("SpawnLevelSelectUI -> objPrefab was null");
            }
        }
    }

    void RemoveLevelSelectUI()
    {
        if (mLevelSelectUI != null)
        {
            mLevelSelectUI.RemoveSelf();
            mLevelSelectUI = null;
            Resources.UnloadUnusedAssets();
        }
    }
}
