using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ============= Event dicts ===========================
[CustomPropertyDrawer(typeof(Animation_Trigger_Dict))]
[CustomPropertyDrawer(typeof(Int_var_to_string))]
[CustomPropertyDrawer(typeof(Float_var_to_string))]
[CustomPropertyDrawer(typeof(Bool_var_to_string))]
public class Custom_AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

//[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class Custom_AnySerializableDictionaryStoragePropertyDrawer : SerializableDictionaryStoragePropertyDrawer { }