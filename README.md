# FullSave
Easy to use system that saves whole gamestate.

FullSave saves all gameObjects you need, with their transforms, and any components you inherit from **SavedComponent**.
Currently saving runtime created (not from prefab) gameObjects is not supported, but its easy to implement.
Conceptually any entity in game has to be described by a prefab, as its usually with most games.

![fs rocks](https://i.imgur.com/1UrL5Ij.png)


**Created with unity 2018.1.6f1**

This project relies on FullSerializer https://github.com/jacobdufault/fullserializer

# Glossary

**Tracked Object** - a gameObject that has a prefab, has `SavedGameObject` component attached, **or** a scriptable that inherits `SavedScriptable` and is stored in `FullSaveStorage`.

# Components

**FullSaveSystem** - Monobehavior through which you interface with the system. Needs a reference to a `FullSaveStorage` asset, which you must manually create and reference. 

**FullSaveStorage** - asset that tracks prefabs and scriptables to lookup them by guid on runtime.

**SavedComponent** - base class for any component you want to save on runtime,
any fields withing those components marked with `[RuntimeSave]` attribute will be saved.


**SavedGameObject** - any tracked gameObject you want to save has to have this attached. https://github.com/git/git/blob/master/README#L18

**SavedScriptable** - base class for scriptables. 
any fields withing those components marked with `[RuntimeSave]` attribute will be saved.

# Serialization

When serializing all tracked objects will be saved with their InstanceID and guid of the backing asset.

All SavedComponent fields and properties marked with `[RuntimeSave]` attribute will be saved.
This includes:
- Any type supported by FullSerializer
- References to other GameObjects (these gameobjects **have to be tracked**, but 
the field itself can be of GameObject type.
- Component references to any UnityEngine.Component, that are attached to a **tracked** gameObject.

FullSerializer will take care of cyclical references and correctly serialize objects without duplicates.
GameObject fields will be serialized as instance id int.

Serilization happens in 2 steps : 
  1. Serialize all gameObjects
  2. Serialize all components separately, each serialized component holds a field `ownerID` which is used to find 
  target go later.
  
Whole pipe uses fsConverters and custom code.

When deserializing:
  1. We delete all existing SaveGameobjects in scene
  2. We recreate all gameObjects, for each object in save file we lookup its prefab in storage, and instantiate it (disabled)
  3. We deserialize all components, look up corresponding object in scene and Get/Add(if missing) a component
  4. For each component we overwrite fields that were previously serialized.
  5. For some types we use custom fsConverter to deserialize integer ids into actual references.

# Usage

Check Example scenes.

Any GameObject you want to save has to have a backing prefab with **SavedGameObject** component attached to it and 
be tracked by **FullSaveStorage** asset. To automatically collect all of the prefabs use `FullSave/StoreObjects` 
from the menu.

  1. Create new gameObject.
  2. Attach **SavedGameObject** component to it.
  3. Make a prefab out of it.
  4. Run `FullSave/StoreObjects` command to track this prefab
  5. Create new script, inherit **SavedComponent**.
  6. Mark required fields with `[RuntimeSave]`
  7. Make instances of tracked gameObject in scene.
  8. On runtime call `Save(path)` / `Load(path)` on **FullSaveSystem**
  
# Limitations

- Multiple SavedComponents per object are not supported (currently)
- Runtime created (without a prefab) gameobjects are not supported (currently)
- If component was serialized, but prefab changed and now its missing, system will still add it to the gameobject
- When saving *all* SavedGameObjects in scene will be saved, it is possible you have a specific loaded scene you want to save, or a group of objects, in that case some changes to the code has to be made.
- Fully relies on FullSerializer (limitation or blessing?)
- Clearly readable save format in json, easy to tinker with (might not be what you want)
- Slow compared to other serializers, but the purpose is to have a Save/Load functionality for singleplayer games, where its not crucial.
- Might not work on all platforms. FullSerializer is made to work everywhere, but i could on accident break something in AOT, tested 
- Lacks unit tests.
