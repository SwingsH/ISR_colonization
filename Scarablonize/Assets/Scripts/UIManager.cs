﻿using UnityEngine;using System.Collections;public class UIManager : MonoBehaviour {	public GameObject Menu, Levels, InGame, Staff, Back;	public TurnUI Turn;	public CenterMsg Msg;	public GameLifeUI ScarabLife, HumanLife;	public Result GameResult;	static UIManager _instance;	EnumType.UIType _currentShowUI;	public static UIManager Instance	{		get {return _instance;}	}	void Awake()	{		_instance = this;		Menu.SetActive(false);		Levels.SetActive(false);		InGame.SetActive(false);		Staff.SetActive(false);		Back.SetActive(false);	}	// Use this for initialization	void Start()	{	}		// Update is called once per frame	void Update () {		}	void OnDestroy()	{		_instance = null;	}		public void Open(EnumType.UIType uiType)	{		if (uiType != EnumType.UIType.Staff)			_currentShowUI = uiType;		switch (uiType)		{			case EnumType.UIType.Menu:				Menu.SetActive(true);				Levels.SetActive(false);				InGame.SetActive(false);				Staff.SetActive(false);				Back.SetActive(false);				break;			case EnumType.UIType.Levels:				Menu.SetActive(false);				Levels.SetActive(true);				InGame.SetActive(false);				Staff.SetActive(false);				Back.SetActive(true);				break;			case EnumType.UIType.InGame:				Menu.SetActive(false);				Levels.SetActive(false);				InGame.SetActive(true);				Staff.SetActive(false);				Back.SetActive(true);				break;			case EnumType.UIType.Staff:				Menu.SetActive(false);				Levels.SetActive(false);				InGame.SetActive(false);				Staff.SetActive(true);				Back.SetActive(true);				break;		}	}	public void CloseStaff()	{		Staff.SetActive(false);		Open(_currentShowUI);	}	public void ShowCenterMsg(string msg)	{		Msg.ShowMsg(msg);	}	public int HumanCount	{		set {HumanLife.Count = value;}	}	public int ScarabCount	{		set {ScarabLife.Count = value;}	}	public void ShowResult()	{		GameResult.ShowResult();	}}