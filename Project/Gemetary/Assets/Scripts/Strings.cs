using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Strings
{
    // MAGIC STRING SEARCH REGEX: [^((?!const).)*".+".+$]

    #region Scene Names

    public const string OPTIONS_SCENE_NAME = "OptionsScene";
    public const string TITLE_SCENE_NAME = "TitleScene";
    public const string MAZE_SCENE_NAME = "MazeScene";
    public const string GAME_OVER_SCENE_NAME = "GameOverScene";
    public const string HOW_TO_PLAY_SCENE_NAME = "HowToPlayScene";

    #endregion

    #region Object Tags

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
    public const string HINTER_TAG = "Hinter";
    public const string EXPLOSION_AUDIO_TAG = "ExplosionAudio";
    public const string THUNDER_SFX_TAG = "Thunder";
    public const string GEM_SFX_TAG = "GemSFX";
    public const string BUTTON_CLICK_SFX_TAG = "ButtonClickSFX";
    public const string BUTTON_HOVER_SFX_TAG = "ButtonHoverSFX";

    #endregion

    #region Defaults

    public const string DEFAULT_NAME = "Your Name";

    #endregion

    #region Animation Triggers

    public const string BEGIN_MOVE_ANIM = "BeginMove";
    public const string END_MOVE_ANIM = "EndMove";
    public const string JUMP_ANIM = "Jump";
    public const string DEATH_ANIM = "Die";

    #endregion

    #region Hints

    public const string DODGE_HINT = "Impressive!\nYou've collected enough\n gems to dodge!\n Try pressing 'Space'!";
    public const string TOMBSTONE_HINT = "Watch out!\nTombstones here seem\nrigged to explode!";
    public const string LANTERN_HINT = "My eyes!\nOh wait, I'm a hint.\nThe bright light scares\nthe enemies!\nGet them!";
    public const string ANNOYING_START_HINT = "Hi!\nI'm an annoying hint!\nI'll go away eventually.\nOr you can make me\nby pressing 'escape'!\nAnnoying hints can be disabled\nin the game options.";
    public const string SECTION_CLEAR_HINT_PREFIX = "Run ";
    public const string SECTION_CLEAR_HINT_POSTFIX = "!\nYou've gained temporary\nsuper-gem-powers!\nYou can destroy stuff!";
    #endregion

    #region Cheat Codes

    public const string CHEAT_NAME = "IThinkICheated";
    public const string HARD_NAME = "2Ez4Me";

    #endregion
   
    #region Buttons

    public const string ENEMY_WALK_ANIM_NAME = "walk";
    public const string HORIZ_AXIS_NAME = "Horizontal";
    public const string VERT_AXIS_NAME = "Vertical";
    public const string MOUSE_SCROLL_AXIS_NAME = "Mouse ScrollWheel";
    public const string CANCEL_BUTTON = "Cancel";
    public const string DODGE_BUTTON = "Jump";
    public const int CANCEL_INDEX = 0, DODGE_INDEX = 1;

    #endregion

    #region Object Names

    public const string FLOOR_HOLDER_NAME = "FloorHolder";
    public const string POWERUP_HOLDER_NAME = "PowerupHolder";
    public const string FENCE_HOLDER_NAME = "FenceHolder";
    public const string PILLAR_HOLDER_NAME = "PillarHolder";
    public const string GEM_HOLDER_NAME = "GemHolder";
    public const string ENEMY_HOLDER_NAME = "EnemyHolder";
    public const string RESTORE_HOLDER_NAME = "RestorerHolder";
    public const string BOMB_HOLDER_NAME = "BombHolder";
    public const string FOG_HOLDER_NAME = "FogHolder";
    public const string FENCE_NAME = "Fence";
    public const string PILLAR_NAME = "Pillar";
    public const string GEM_NAME = "Gem";

    public const string MAZE_SECTION_NAME_PREFIX = "MazeSection[";
    public const string MAZE_SECTION_NAME_POSTFIX = "]";

    #endregion

    #region Delimiters

    public const string COMMA = ",";
    public const string COLON = ":";
    public const string SPACE_COLON_SPACE = " : ";
    public const string NEWLINE = "\n";
    public const string PLUS_SYMBOL = "+";

    #endregion

    #region Keys

    public const string KEY_PREFIX = "X:";
    public const string KEY_MIDDLE = "-Z:";
    public const string SCORE_PREFIX = "Score: ";
    public const string BASE_HIGH_SCORE_TEXT = "High Scores:\n";
    public const string DEFAULT_SCORE_TEXT = DEFAULT_NAME + ":0";
    public const string YOUR_SCORE_TEXT_PREFIX = "Your Score: ";
    public const string YOUR_SCORE_CHEATER_POSTFIX = ", cheater!";
    public const string YOUR_SCORE_HARDCORE_POSTFIX = ", boss!";
    public static readonly string[] HIGH_SCORE_KEYS = { "1st", "2nd", "3rd", "4th", "5th" };
    public static readonly string[] AXIS_KEYS = { CANCEL_BUTTON, DODGE_BUTTON };

    #endregion

    #region Text Colorization

    public const string OPEN_COLOR_PREFIX = "<color=\"#";
    public const string OPEN_COLOR_POSTFIX = "\">";
    public const string CLOSE_COLOR = "</color>\n";

    #endregion

}

