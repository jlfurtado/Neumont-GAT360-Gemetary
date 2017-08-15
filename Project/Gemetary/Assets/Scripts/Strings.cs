using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Strings
{
    public const string OPTIONS_SCENE_NAME = "OptionsScene";
    public const string TITLE_SCENE_NAME = "TitleScene";
    public const string MAZE_SCENE_NAME = "MazeScene";
    public const string GAME_OVER_SCENE_NAME = "GameOverScene";
    public const string HOW_TO_PLAY_SCENE_NAME = "HowToPlayScene";
    public const string SCORE_MANAGER_TAG = "ScoreManager";
    public const string PLAYER_TAG = "Player";
    public const string MAZE_TAG = "Maze";
    public const string TITLE_MUSIC_TAG = "TitleMusic";
    public const string SCENE_MOVER_TAG = "SceneMover";
    public const string ENEMY_TAG = "Enemy";
    public const string FLASHER_TAG = "Flasher";
    public const string PAUSE_MENU_TAG = "PauseMenu";
    public const string AXIS_INPUT_HELPER_TAG = "AxisInputHelper";

    public const string PLAYER_NAME_INPUT_TAG = "PlayerNameInput";
    public const string DEFAULT_NAME = "Your Name";

    public const string BEGIN_MOVE_ANIM = "BeginMove";
    public const string END_MOVE_ANIM = "EndMove";
    public const string JUMP_ANIM = "Jump";
    public const string DEATH_ANIM = "Die";

    public const string CANCEL_BUTTON = "Cancel";
    public const string DODGE_BUTTON = "Jump";

    public const string CHEAT_NAME = "IThinkICheated";
    public const string HARD_NAME = "2Ez4Me";

    public const string HINTER_TAG = "Hinter";
    public const string DODGE_HINT = "Impressive!\nYou've collected enough\n gems to dodge!\n Try pressing 'Space'!";
    public const string TOMBSTONE_HINT = "Watch out!\nTombstones here seem\nrigged to explode!";
    public const string LANTERN_HINT = "My eyes!\nOh wait, I'm a hint.\nThe bright light scares\nthe enemies!\nGet them!";
    public static readonly string[] HIGH_SCORE_KEYS = { "1st", "2nd", "3rd", "4th", "5th" };
    public static readonly string[] AXIS_KEYS = { CANCEL_BUTTON, DODGE_BUTTON };
    public const int CANCEL_INDEX = 0, DODGE_INDEX = 1;
}

