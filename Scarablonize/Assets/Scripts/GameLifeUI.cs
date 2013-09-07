﻿using UnityEngine;using System.Collections;using System.Collections.Generic;public class GameLifeUI : MonoBehaviour {	public UISprite LifeBG;	public UILabel LifeDisplay;	public float PlayTime = .5f;	Queue<int> _lifeValues = new Queue<int>();	float _barLength;	float _lifeDisplaytopPos;	int _currentLife = 100;	bool IsAnimating {get; set;}	int TotalCreatureCounts {get {return Const.MapSize * Const.MapSize;}}	IEnumerator PlayLifeAnimation(int newLife)	{		if (IsAnimating)			yield break;		IsAnimating = true;		//caculate play rate		int lifeOffset = newLife >= _currentLife ? 1 : -1;		float differAmount = (float)(newLife - _currentLife) / (float)TotalCreatureCounts;		float deltaAmount = differAmount / PlayTime;		while (_currentLife != newLife)		{			_currentLife += lifeOffset;			LifeBG.fillAmount += deltaAmount;			LifeDisplay.text = _currentLife.ToString();			LifeDisplay.cachedTransform.localPosition = new Vector3(0, _lifeDisplaytopPos - (_barLength * (1 - LifeBG.fillAmount)), 0);			yield return new WaitForEndOfFrame();		}//		Count = newLife;		LifeBG.fillAmount = (float)newLife / (float)(Const.MapSize * Const.MapSize);		LifeDisplay.text = newLife.ToString();		LifeDisplay.cachedTransform.localPosition = new Vector3(0, _lifeDisplaytopPos - (_barLength * (1 - LifeBG.fillAmount)), 0);		IsAnimating = false;	}		void Initialize()	{		Count = 0;	}		void Awake()	{		_barLength = LifeBG.cachedTransform.localScale.y;		_lifeDisplaytopPos = LifeDisplay.cachedTransform.localPosition.y;		Initialize();	}	// Use this for initialization	void Start () 	{	}		// Update is called once per frame	void Update ()	{		if (_lifeValues.Count > 0 && !IsAnimating)			StartCoroutine(PlayLifeAnimation(_lifeValues.Count));	}	void OnDisable()	{		StopAllCoroutines();	}		public int Count	{		set		{			_lifeValues.Enqueue(value);//			LifeBG.fillAmount = (float)value / (float)(Const.MapSize * Const.MapSize);//			LifeDisplay.text = value.ToString();//			LifeDisplay.cachedTransform.localPosition = new Vector3(0, _lifeDisplaytopPos - (_barLength * (1 - LifeBG.fillAmount)), 0);		}	}	void OnGUI()	{		if (GUILayout.Button("add value"))			Count = _currentLife + 1;		if (GUILayout.Button("dec value"))			Count = _currentLife - 1;	}}