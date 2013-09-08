﻿using UnityEngine;using System.Collections;using System.Collections.Generic;public class GameLifeUI : MonoBehaviour {	public UISprite LifeBG, IconBG;	public UILabel LifeDisplay;	public float PlayTime = .5f;	Queue<int> _lifeValues = new Queue<int>();	float _barLength;	float _lifeDisplaytopPos;	float _iconDisplayTopPos;	int _currentLife = 0;	bool _isAnimating;	int TotalCreatureCounts {get {return Const.MapSize * Const.MapSize;}}	void OnStatusChanged(PlayStatus last, PlayStatus current)	{		switch (current)		{			case PlayStatus.RoundHumanTurn:			case PlayStatus.RoundScarabTurn:            case PlayStatus.BattleResult:				if (tag == TagConst.Human)					Count = GameControl.Instance.Logic.PeopleCount;				else					Count = GameControl.Instance.Logic.ScarabCount;				break;		}	}	IEnumerator PlayLifeAnimation(int newLife)	{		if (_isAnimating /*|| newLife < 0 || newLife > TotalCreatureCounts*/)			yield break;		_isAnimating = true;		//caculate play rate		int lifeOffset = newLife >= _currentLife ? 1 : -1;		float differAmount = (float)(newLife - _currentLife) / (float)TotalCreatureCounts;		float deltaAmount = differAmount / PlayTime;		while (_currentLife != newLife)		{			_currentLife += lifeOffset;			LifeBG.fillAmount += deltaAmount;			LifeDisplay.text = _currentLife.ToString();			LifeDisplay.cachedTransform.localPosition = new Vector3(0, _lifeDisplaytopPos - (_barLength * (1 - LifeBG.fillAmount)), -5f);			IconBG.cachedTransform.localPosition = new Vector3(0, _iconDisplayTopPos - (_barLength * (1 - LifeBG.fillAmount)), 0);			yield return new WaitForEndOfFrame();		}		_currentLife = newLife;		LifeBG.fillAmount = (float)newLife / (float)(Const.MapSize * Const.MapSize);		LifeDisplay.text = newLife.ToString();		LifeDisplay.cachedTransform.localPosition = new Vector3(0, _lifeDisplaytopPos - (_barLength * (1 - LifeBG.fillAmount)), -5f);		IconBG.cachedTransform.localPosition = new Vector3(0, _iconDisplayTopPos - (_barLength * (1 - LifeBG.fillAmount)), 0);		_isAnimating = false;	}		void Initialize()	{		Count = 0;	}		void Awake()	{		_barLength = LifeBG.cachedTransform.localScale.y;		_lifeDisplaytopPos = LifeDisplay.cachedTransform.localPosition.y;		_iconDisplayTopPos = IconBG.cachedTransform.localPosition.y;		Initialize();	}	// Use this for initialization	void Start () 	{		GameControl.Instance.NotifyStatusChanged += OnStatusChanged;	}		// Update is called once per frame	void Update ()	{		if (_lifeValues.Count > 0 && !_isAnimating)			StartCoroutine(PlayLifeAnimation(_lifeValues.Dequeue()));	}	void OnDisable()	{		StopAllCoroutines();		_isAnimating = false;	}//	void OnEnable()//	{//		if (tag == TagConst.Scarab)//			Count = GameControl.Instance.Logic.ScarabCount;//		else//			Count = GameControl.Instance.Logic.PeopleCount;//	}	void OnDestroy()	{		StopAllCoroutines();	}		public int Count	{		set		{			_lifeValues.Enqueue(value);		}	}//	void OnGUI()//	{//		if (GUILayout.Button("add value"))//			Count = _currentLife + 1;////		if (GUILayout.Button("dec value"))//			Count = _currentLife - 1;//	}}