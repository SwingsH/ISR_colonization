﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//遊戲邏輯主控台, 各系統溝通銜接口
public class GameControl{
    private static GameControl _instance;
    private GameMain _main;
    private GameLogic _logic;
    private PlayStatus _currentPlayStatus;
	private PlayStatus _lastPlayStatus;
    private PlayMode _currentPlayMode;
    private ushort _currentChapterID = 0; // 0 = no chapter
	private bool _statusChanged;
    private MapBlockData _currentChoosedBlock = null; // null 表示現在沒有選取 操作中 block 

	public delegate void OnStatusChanged(PlayStatus lastStatus, PlayStatus currentStatus);
	public OnStatusChanged NotifyStatusChanged;

    // private GUI Manager 預定地, for Sadwx

    private GameControl(GameMain main)
    {
        // GUIManager.ShowGameTitle
        _main = main;
        _logic = new GameLogic();

        TriggerGameEnter();
    }

    public static GameControl Instance
    {
        get
        {
            if (_instance == null)
            {
                GameMain main = GameObject.FindObjectOfType(typeof(GameMain)) as GameMain;
                _instance = new GameControl(main);
            }

            return _instance;
        }
    }

    public PlayStatus GameStatus
    {
        get
        {
            return _currentPlayStatus;
        }
    }

    public PlayMode GameMode
    {
        get
        {
            return _currentPlayMode;
        }
    }

    public GameLogic Logic
    {
        get{ return _logic;}
    }

    // 玩家從 title 畫面, 觸發遊戲開始
    public void TriggerGameEnter()
    {
        if(_currentPlayStatus != PlayStatus.GameTitle)
        {
            DebugLog("Status error.");
            return;
        }

        _currentPlayStatus = PlayStatus.GameChoosePlaymode;

        // NGUI show ui
		UIManager.Instance.Open(EnumType.UIType.Menu);
    }

    // 玩家從 mode 選擇觸發
    public void TriggerChoosePlayMode(PlayMode mode)
    {
        if (_currentPlayStatus != PlayStatus.GameChoosePlaymode)
        {
            DebugLog("Status error.");
            return;
        }

        _currentPlayMode = mode;
        _currentPlayStatus = PlayStatus.GameChooseChapter;

        // NGUI show ui
		UIManager.Instance.Open(EnumType.UIType.Levels);
    }

    // 玩家從 關卡選擇 UI 畫面, 觸發遊戲開始
    public void TriggerChooseChapter(ushort chapterID)
    {
        if (_currentPlayStatus != PlayStatus.GameChooseChapter)
        {
            DebugLog("Status error.");
            return;
        }

        _currentChapterID = chapterID;
        _currentPlayStatus = PlayStatus.MapGenerating;

        MapGenerator.Generate( _currentChapterID );

        // NGUI show ui
		UIManager.Instance.Open(EnumType.UIType.InGame);
    }

    // Map 產生完畢後呼叫, PS: 因為 load level 是 async, 所以 is done 由 map generator 判定
    public void TriggerMapGenerateDone()
    {
        if (_currentPlayStatus != PlayStatus.MapGenerating)
        {
            DebugLog("Status error. Should be MapGenerating");
            return;
        }

        DebugLog("蟲族先攻");

        _logic.InitialMap(MapGenerator.GeneratedData, MapGenerator.GeneratedHoleData);
        _currentPlayStatus = PlayStatus.RoundScarabTurn; //蟲族先攻

        DebugLog("PlayStatus: " + _currentPlayStatus.ToString());
    }

    /// <summary>
    /// 現在上場的打擊者
    /// </summary>
    public Creature NowHitter
    {
        get
        {
            if(_currentPlayStatus == PlayStatus.RoundHumanTurn)
                return Creature.People;
            else if(_currentPlayStatus == PlayStatus.RoundScarabTurn)
                return Creature.Scarab;
            else
                return Creature.None;
        }
    }
    //------------  Map 控制相關 -------------------
    // player click a tile in Map, Top Left is 0, 0
    public void MapTileClick(MapBlockData data)
    {
        switch(_currentPlayStatus)
        {
            case PlayStatus.RoundHumanTurn:
            case PlayStatus.RoundScarabTurn:
                if (NowHitter == Creature.None)
                {
                    DebugLog(" Status error : not round turn status. " + _currentPlayStatus.ToString() );
                    return;
                }

				UIManager.Instance.ScarabCount = _logic.ScarabCount;				UIManager.Instance.HumanCount = _logic.PeopleCount;                IVector2 vec = new IVector2();                vec.x = data.Column;                vec.y = data.Row;                ControlMessage controlMsg = _logic.CanControl(vec, NowHitter);                if (controlMsg != ControlMessage.OK)                {
					UIManager.Instance.ShowCenterMsg("you can't do it !");
                    //todo:                    //GameControl.Instance.DebugLog(" controlMsg " + controlMsg.ToString() );                    return;                }
                // todo: some click effect                // ready click 2                if (NowHitter == Creature.People)
                {                    _currentPlayStatus = PlayStatus.RoundHumanReadyMove;                }                else if (NowHitter == Creature.Scarab)                {
                    _currentPlayStatus = PlayStatus.RoundScarabReadyMove;
                }
                _currentChoosedBlock = data;

                DebugLog("PlayStatus: " + _currentPlayStatus.ToString());
                break;
            case PlayStatus.RoundHumanReadyMove:
                bool legal = _logic.IsLegalMove(_currentChoosedBlock.Block.Pos, data.Block.Pos);
                List<IVector2> infectPositions = new List<IVector2>();
                IVector2 realEnd;
                
                MoveType moveType = _logic.Move(_currentChoosedBlock.Block.Pos, data.Block.Pos, out realEnd, out infectPositions);

                if (moveType == MoveType.Move)
                {
                    for (int i = 0; i < infectPositions.Count; i++)
                    {
                        MapBlock block = _logic.GetMapBlock(infectPositions[i]);
                        MapGenerator.HumanInfectBlock(block);
                    }

                    DebugLog(" infect nums. " + infectPositions.Count.ToString());
                }
                else if (moveType == MoveType.Clone)
                {
                    MapBlock block = _logic.GetMapBlock(realEnd);
                    MapGenerator.HumanInfectBlock(block);
                    DebugLog("MoveType.Clone " + realEnd.ToString());
                }
                else
                {
                    DebugLog("MoveType.None");
                }

                break;
            case PlayStatus.RoundScarabReadyMove:
                bool legal_2 = _logic.IsLegalMove(_currentChoosedBlock.Block.Pos, data.Block.Pos);
                List<IVector2> infectPositions_2 = new List<IVector2>();
                IVector2 realEnd_2;
                MoveType moveType_2 = _logic.Move(_currentChoosedBlock.Block.Pos, data.Block.Pos, out realEnd_2, out infectPositions_2);

                DebugLog(" Move Start : " + _currentChoosedBlock.Block.Pos.x.ToString() + "," + _currentChoosedBlock.Block.Pos.y.ToString() +
                          "    End: " + data.Block.Pos.x.ToString() + "," + data.Block.Pos.y.ToString() +
                        "   Real End " + realEnd_2.x.ToString() + "," + realEnd_2.y.ToString() + "   MoveType : " + moveType_2.ToString());

                if (moveType_2 == MoveType.Move)
                {
                    for (int i = 0; i < infectPositions_2.Count; i++)
                    {
                        MapBlock block = _logic.GetMapBlock(infectPositions_2[i]);
                        MapGenerator.ScarabInfectBlock(block);
                    }

                    DebugLog("MoveType.Move. infect nums. " + infectPositions_2.Count.ToString());
                }
                else if (moveType_2 == MoveType.Clone)
                {
                    MapBlock block = _logic.GetMapBlock(realEnd_2);
                    MapGenerator.ScarabInfectBlock(block);

                    DebugLog("MoveType.Clone " + realEnd_2.ToString());
                }
                else
                {
                    DebugLog("MoveType.None");
                }

                break;

            default:
                DebugLog("Click Tile valid. " + _currentPlayStatus.ToString());
                break;
        }
    }

    // update per-frame
    public void Update()
    {
		if (_lastPlayStatus != _currentPlayStatus)
		{
			if (NotifyStatusChanged != null)
				NotifyStatusChanged(_lastPlayStatus, _currentPlayStatus);

			_lastPlayStatus = _currentPlayStatus;
		}

        switch(_currentPlayStatus)
        {
            case PlayStatus.GameChooseChapter:
                //todo
                break;
            case PlayStatus.GameChoosePlaymode:
                //todo
                break;

            case PlayStatus.GameTitle:
                //todo
                break;
            case PlayStatus.MapGenerating:
                //todo
                break;
            default:
                // todo
                break;
        }
    }

    public void StartCoroutine(IEnumerator routine)
    {
        _main.StartCoroutine(routine);
    }

    // message for debug use.
    public void DebugLog(string msg)
    {
        Debug.Log(msg);
    }
}
