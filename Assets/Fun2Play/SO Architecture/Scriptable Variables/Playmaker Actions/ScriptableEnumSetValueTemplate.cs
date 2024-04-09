using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    //[Placeholder][ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Get the value from a ScriptableEnumTemplate Object.")]
    public class ScriptableEnumTemplateSetValue : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ScriptableEnumTemplate))]
        [Tooltip("Select the Scriptable Enum Object.\n\nTip: Use a variable rather than hard reference so it can be found easily when searching for it.")]
        public FsmObject scriptableEnum;

        [Tooltip("Store the value in a bool variable")]
        [UIHint(UIHint.Variable)]
        public FsmEnum value;

        private ScriptableEnumTemplate _scriptableEnum;

        public override void Reset()
        {
            scriptableEnum = new FsmObject { UseVariable = true };
            value = null;
        }

        public override void OnEnter()
        {
            DoSetScriptableEnumValue();
            Finish();
        }

        private void DoSetScriptableEnumValue()
        {
            _scriptableEnum = scriptableEnum.Value as ScriptableEnumTemplate;
            //[Placeholder]_scriptableEnum.SetValue(value.Value);
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return scriptableEnum + ": Set Value";
        }
#endif
    }
}
