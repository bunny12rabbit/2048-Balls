using System.Collections.Generic;

public static class TagsManager
{
    public enum Tags : byte
    {
        Respawn,
        Ball,
    }

    public const string RESPAWN = "Respawn";
    public const string BALL = "Ball";

    private static Dictionary<Tags, string> _tags = new Dictionary<Tags, string>
    {
        {Tags.Respawn, RESPAWN},
        {Tags.Ball, BALL}
    };

    public static string GetTag(Tags tag) => _tags[tag];
}