namespace MagicWords.Core
{
    public enum GameState
    {
        None,
        Loading,
        MainMenu,
        GameSetup,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public enum GameMode
    {
        SinglePlayer,
        MultiPlayer
    }

    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum PowerUpType
    {
        ChangeCell,
        ChangeLetter,
        FreezeTrap,
        TimeBonus,
        ScoreBooster
    }

    public enum AudioClipId
    {
        // UI Sounds
        ButtonClick,
        MenuMusic,

        // Game Sounds
        GameMusic,
        TileSelect,
        TileDeselect,
        ValidWord,
        InvalidWord,
        PowerUpActivated,
        Victory,
        Defeat
    }

    public enum Language
    {
        English,
        Spanish
    }

    public enum TileState
    {
        Normal,
        Selected,
        Frozen,
        Objective
    }
}