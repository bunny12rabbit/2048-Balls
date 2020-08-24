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

    public enum SceneIndexes
    {
        AppStart,
        MainMenu,
        Game
    }
}