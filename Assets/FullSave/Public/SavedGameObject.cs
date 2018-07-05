namespace pointcache.FullSave
{
    using UnityEngine;
    using System.Collections;


    /// <summary>
    /// Add this component to a gameObject which you want to save on runtime
    /// </summary>
    public class SavedGameObject : MonoBehaviour
    {
        [DebugOnly, SerializeField] private string _guid;

        public string guid 
        {
            get
            {
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            for (int i = 0; i < 100; i++)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            }
        }
#endif
    }

}