using System.Collections.Generic;
using UnityEngine;

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
    
    public struct LayersNames
    {
        public enum Layers
        {
            Ball,
            IgnorePhysics
        }
        
        public const string IGNORE_PHYSICS = "IgnorePhysics";
        public const string BALL = "Ball";
        
        private static readonly Dictionary<Layers, int> _layers = new Dictionary<Layers, int>
        {
            {Layers.Ball, LayerMask.NameToLayer(BALL)},
            {Layers.IgnorePhysics, LayerMask.NameToLayer(IGNORE_PHYSICS)}
        };
        
        public static int GetLayer(Layers layer) => _layers[layer];
    }

    public enum SceneIndexes
    {
        AppStart,
        MainMenu,
        Game
    }
}