﻿using Generics;
 using Interfaces;
 using UnityEngine;

 public interface IMatchableItem<T> where T : struct
 { 
     Transform transform { get; }
    
     CollisionHandlerGeneric<T> CollisionHandler { get; }
     
    IMatchableData<T> Data { get; }
    
    bool CheckMatch(IMatchableItem<T> matchableItem);

    void OnMatch(IMatchableItem<T> matchedObject);
}