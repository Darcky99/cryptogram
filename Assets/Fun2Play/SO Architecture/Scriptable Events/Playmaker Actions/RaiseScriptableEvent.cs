using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Fun2Play: SO Architecture")]
    [Tooltip("Raise a Scriptable Event. Optionally pass up to 3 parameters. Supported variables: GameObject, Int, Float, String, Bool, Vector2, Vector3, Sprite. This is used in combination with a ScriptableEvent scriptable object.")]
    public class RaiseScriptableEvent : FsmStateAction
    {
        public enum ParameterType
        {
            None,
            GameObject,
            Int,
            Float,
            String,
            Bool,
            Vector2,
            Vector3,
            Sprite
        }

        [RequiredField]
        [ObjectType(typeof(ScriptableEvent))]
        [Tooltip("Select the Scriptable Event scriptable object")]
        public FsmObject scriptableEvent;

        [Tooltip("Optionally, select the variable type to pass for parameter 1.")]
        [Title("Parameter 1")]
        public ParameterType parameterType1;

        // Parameters for Parameter 1
        [HideIf(nameof(HideMyGameObject1))]
        [Title("Game Object Parameter")]
        public FsmGameObject gameObjectParameter1;
        [HideIf(nameof(HideMyInt1))]
        [Title("Int Parameter")]
        public FsmInt intParameter1;
        [HideIf(nameof(HideMyFloat1))]
        [Title("Float Parameter")]
        public FsmFloat floatParameter1;
        [HideIf(nameof(HideMyString1))]
        [Title("String Parameter")]
        public FsmString stringParameter1;
        [HideIf(nameof(HideMyBool1))]
        [Title("Bool Parameter")]
        public FsmBool boolParameter1;
        [HideIf(nameof(HideMyVector2_1))]
        [Title("Vector2 Parameter")]
        public FsmVector2 vector2Parameter1;
        [HideIf(nameof(HideMyVector3_1))]
        [Title("Vector3 Parameter")]
        public FsmVector3 vector3Parameter1;
        [HideIf(nameof(HideMySprite1))]
        [Title("Sprite Parameter")]
        [ObjectType(typeof(Sprite))]
        public FsmObject spriteParameter1;

        [Tooltip("Optionally, select the variable type to pass for parameter 2.")]
        [Title("Parameter 2")]
        public ParameterType parameterType2;

        // Parameters for Parameter 2
        [HideIf(nameof(HideMyGameObject2))]
        [Title("Game Object Parameter")]
        public FsmGameObject gameObjectParameter2;
        [HideIf(nameof(HideMyInt2))]
        [Title("Int Parameter")]
        public FsmInt intParameter2;
        [HideIf(nameof(HideMyFloat2))]
        [Title("Float Parameter")]
        public FsmFloat floatParameter2;
        [HideIf(nameof(HideMyString2))]
        [Title("String Parameter")]
        public FsmString stringParameter2;
        [HideIf(nameof(HideMyBool2))]
        [Title("Bool Parameter")]
        public FsmBool boolParameter2;
        [HideIf(nameof(HideMyVector2_2))]
        [Title("Vector2 Parameter")]
        public FsmVector2 vector2Parameter2;
        [HideIf(nameof(HideMyVector3_2))]
        [Title("Vector3 Parameter")]
        public FsmVector3 vector3Parameter2;
        [HideIf(nameof(HideMySprite2))]
        [Title("Sprite Parameter")]
        [ObjectType(typeof(Sprite))]
        public FsmObject spriteParameter2;

        [Tooltip("Optionally, select the variable type to pass for parameter 3.")]
        [Title("Parameter 3")]
        public ParameterType parameterType3;

        // Parameters for Parameter 3
        [HideIf(nameof(HideMyGameObject3))]
        [Title("Game Object")]
        public FsmGameObject gameObjectParameter3;
        [HideIf(nameof(HideMyInt3))]
        [Title("Int Parameter")]
        public FsmInt intParameter3;
        [HideIf(nameof(HideMyFloat3))]
        [Title("Float Parameter")]
        public FsmFloat floatParameter3;
        [HideIf(nameof(HideMyString3))]
        [Title("String Parameter")]
        public FsmString stringParameter3;
        [HideIf(nameof(HideMyBool3))]
        [Title("Bool Parameter")]
        public FsmBool boolParameter3;
        [HideIf(nameof(HideMyVector2_3))]
        [Title("Vector2 Parameter")]
        public FsmVector2 vector2Parameter3;
        [HideIf(nameof(HideMyVector3_3))]
        [Title("Vector3 Parameter")]
        public FsmVector3 vector3Parameter3;
        [HideIf(nameof(HideMySprite3))]
        [Title("Sprite Parameter")]
        [ObjectType(typeof(Sprite))]
        public FsmObject spriteParameter3;

        private ScriptableEvent _scriptableEvent;
        private object parameter1;
        private object parameter2;
        private object parameter3;

        public override void Reset()
        {
            scriptableEvent = null;
            parameterType1 = ParameterType.None;
            parameterType2 = ParameterType.None;
            parameterType3 = ParameterType.None;
        }

        public override void OnEnter()
        {
            parameter1 = GetParameterValue(parameterType1, gameObjectParameter1, intParameter1, floatParameter1, stringParameter1, boolParameter1, vector2Parameter1, vector3Parameter1, spriteParameter1);
            parameter2 = GetParameterValue(parameterType2, gameObjectParameter2, intParameter2, floatParameter2, stringParameter2, boolParameter2, vector2Parameter2, vector3Parameter2, spriteParameter2);
            parameter3 = GetParameterValue(parameterType3, gameObjectParameter3, intParameter3, floatParameter3, stringParameter3, boolParameter3, vector2Parameter3, vector3Parameter3, spriteParameter3);

            _scriptableEvent = scriptableEvent.Value as ScriptableEvent;
            _scriptableEvent.Raise(parameter1, parameter2, parameter3);
            Finish();
        }

        private object GetParameterValue(ParameterType parameterType, FsmGameObject gameObjectParameter, FsmInt intParameter, FsmFloat floatParameter, FsmString stringParameter, FsmBool boolParameter, FsmVector2 vector2Parameter, FsmVector3 vector3Parameter, FsmObject spriteParameter)
        {
            switch (parameterType)
            {
                case ParameterType.None:
                    return null;
                case ParameterType.GameObject:
                    return gameObjectParameter.Value;
                case ParameterType.Int:
                    return intParameter.Value;
                case ParameterType.Float:
                    return floatParameter.Value;
                case ParameterType.String:
                    return stringParameter.Value;
                case ParameterType.Bool:
                    return boolParameter.Value;
                case ParameterType.Vector2:
                    return vector2Parameter.Value;
                case ParameterType.Vector3:
                    return vector3Parameter.Value;
                case ParameterType.Sprite:
                    return spriteParameter.Value as Sprite;
                default:
                    return null;
            }
        }

        // Hiding conditions for parameter fields
        public bool HideMyGameObject1() => parameterType1 != ParameterType.GameObject;
        public bool HideMyGameObject2() => parameterType2 != ParameterType.GameObject;
        public bool HideMyGameObject3() => parameterType3 != ParameterType.GameObject;

        public bool HideMyInt1() => parameterType1 != ParameterType.Int;
        public bool HideMyInt2() => parameterType2 != ParameterType.Int;
        public bool HideMyInt3() => parameterType3 != ParameterType.Int;

        public bool HideMyFloat1() => parameterType1 != ParameterType.Float;
        public bool HideMyFloat2() => parameterType2 != ParameterType.Float;
        public bool HideMyFloat3() => parameterType3 != ParameterType.Float;

        public bool HideMyString1() => parameterType1 != ParameterType.String;
        public bool HideMyString2() => parameterType2 != ParameterType.String;
        public bool HideMyString3() => parameterType3 != ParameterType.String;

        public bool HideMyBool1() => parameterType1 != ParameterType.Bool;
        public bool HideMyBool2() => parameterType2 != ParameterType.Bool;
        public bool HideMyBool3() => parameterType3 != ParameterType.Bool;

        public bool HideMyVector2_1() => parameterType1 != ParameterType.Vector2;
        public bool HideMyVector2_2() => parameterType2 != ParameterType.Vector2;
        public bool HideMyVector2_3() => parameterType3 != ParameterType.Vector2;

        public bool HideMyVector3_1() => parameterType1 != ParameterType.Vector3;
        public bool HideMyVector3_2() => parameterType2 != ParameterType.Vector3;
        public bool HideMyVector3_3() => parameterType3 != ParameterType.Vector3;

        public bool HideMySprite1() => parameterType1 != ParameterType.Sprite;
        public bool HideMySprite2() => parameterType2 != ParameterType.Sprite;
        public bool HideMySprite3() => parameterType3 != ParameterType.Sprite;

#if UNITY_EDITOR
        public override string AutoName()
        {
            return "Raise: " + scriptableEvent;
        }
#endif
    }
}
