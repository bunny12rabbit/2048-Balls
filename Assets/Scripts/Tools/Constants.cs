using System.Collections.Generic;

public static class Constants
{
    public struct TagsNames
    {
        public enum Tags : byte
        {
            Respawn,
            Ball,
        }

        public const string RESPAWN = "Respawn";
        public const string BALL = "Ball";

        private static readonly Dictionary<Tags, string> _tags = new Dictionary<Tags, string>
        {
            {Tags.Respawn, RESPAWN},
            {Tags.Ball, BALL}
        };

        public static string GetTag(Tags tag) => _tags[tag];
    }
    
    public struct SceneNames
    {
        public enum Scene
        {
            Menu, Game
        }

        public const string MENU = "Menu";
        public const string GAME = "Game";
        
        private static readonly Dictionary<Scene, string> _scenes = new Dictionary<Scene, string>
        {
            {Scene.Menu, MENU},
            {Scene.Game, GAME}
        };

        public static string GetSceneName(Scene scene) => _scenes[scene];

    }
}