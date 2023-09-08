using UnityEditor;

using UnityEngine;
namespace misc.editor
{
    public class ResetSaves : ScriptableObject
    {
        [MenuItem("Fudog.org/Reset saves")]
        private static void DoIt()
        {
            var saver = new Saver(true, "options");
            saver.Save();
            Debug.Log("saves reseted");
            saver = new Saver(true, "optionsp");
            saver.Save();
            Debug.Log("publish saves reseted");
        }
    }
}
