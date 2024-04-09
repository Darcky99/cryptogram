using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: Helper Tools")]
    [Tooltip("Retrieve a Global Manager from the GlobalManagerRegistry dictionary.")]
    public class GetGlobalManagerByKey : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The Key of the Manager to retrieve.")]
        public FsmString key;

        [RequiredField]
        [Tooltip("The GameObject Variable to store the Manager in.")]
        public FsmGameObject gameObject;

        public override void Reset()
        {
            key = "ManagerKey";
            gameObject = new FsmGameObject { UseVariable = true };
        }

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(key.Value))
            {
                if (GlobalManagerRegistry.managerDictionary.TryGetValue(key.Value, out GameObject manager))
                {
                    // Manager found, assign to the GameObject variable
                    gameObject.Value = manager;
                }
                else
                {
                    // Manager not found, log a warning
                    Debug.LogError($"The Manager with key '{key.Value}' was not found in the GlobalManagerRegistry dictionary. Delete the action or fix the key.");
                }
            }

            Finish();
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ("Get Global Manager By Key: " + gameObject.Name);
        }
#endif
    }
}