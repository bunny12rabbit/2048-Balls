﻿using Generics;
 using Interfaces;
 using UnityEngine;

 // Remove this interface
 public interface IMatchableItem<T> where T : struct
 { 
     Transform transform { get; }
    
     CollisionHandlerGeneric<T> CollisionHandler { get; }
     
    IMatchableData<T> Data { get; }
    
    bool CheckMatch(IMatchableItem<T> matchableItem);

    void OnMatch(IMatchableItem<T> matchedObject);

    // move to IData
    void UpdateData(T criteria = default);
 }