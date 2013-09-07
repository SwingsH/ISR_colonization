﻿public class Const{	public const int MapSize = 10;
    public const string FILENAME_LEVEL_PREFIX = "level"; // levelxxx.txt 的檔名
    public const string MAP_NAME_STARTSCENE = "Main"; //should be useless...    public const string MAP_NAME_EMPTY = "Empty";
    public const string DIR_LEVEL = "levels/";
    // Extension name
    public const string EXT_LEVEL_CONFIG = ".txt";}public class EnumType{	public enum UIType	{		Menu,		Levels,		InGame,		Staff	}}


//遊戲進行狀態
public enum PlayStatus
{
    GameTitle,
    GameChooseChapter,
    GameChoosePlaymode,
    MapGenerating, // Map is generating
    RoundScarabTurn,
    ScarabTurnAnimating, // 蟲族方動畫撥放中, 無法操作
    RoundHumanTurn,
    HumanTurnAnimating, // 人類方動畫撥放中, 無法操作
    BattleResult
}

public enum PlayMode
{
    SinglePlay,
    TwoPlayer
}public class TagConst{	public const string SinglePlay = "1P";	public const string TwoPlayers = "2P";	public const string Staff = "Staff";}