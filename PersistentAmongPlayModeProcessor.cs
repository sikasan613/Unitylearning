using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
///   PersistentAmongPlayMode�̏��������ۂɂ���N���X
/// </summary>
[InitializeOnLoad] //�G�f�B�^�[�N�����ɏ����������悤��
public class PersistentAmongPlayModeProcessor
{

    //�G�f�B�^��~���O�̒l���L�^���邽�߂�Dict(InstanceID�ƃt�B�[���h����Key�ɂ��A���̒l��ݒ肷�銴��)
    private static readonly Dictionary<int, Dictionary<string, object>> _valueDictDict = new Dictionary<int, Dictionary<string, object>>();

    //=================================================================================
    //������
    //=================================================================================

    static PersistentAmongPlayModeProcessor()
    {

        //�v���C���[�h���ύX���ꂽ���̏�����ݒ�
        EditorApplication.playModeStateChanged += state => {

            //�I���{�^�������������ɁA���̎��̒l��ۑ�
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _valueDictDict.Clear();
                ExecuteProcessToAllMonoBehaviour(SaveValue);
            }
            //���ۂɏI��������(�V�[���Đ��O�̒l�ɖ߂�����)�ɁA�ۑ����Ă��l�𔽉f
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                ExecuteProcessToAllMonoBehaviour(ApplyValue);
            }
        };

    }

    //�SMonoBehaviour���擾���A�w�肵�����������s����
    private static void ExecuteProcessToAllMonoBehaviour(Action<MonoBehaviour> action)
    {
        Object.FindObjectsOfType(typeof(MonoBehaviour)).ToList().ForEach(o => action((MonoBehaviour)o));
    }

    //=================================================================================
    //����
    //=================================================================================

    //PersistentAmongPlayMode���t���Ă�S�t�B�[���h�ɏ��������s����
    private static void ExecuteProcessToAllPersistentAmongPlayModeField(MonoBehaviour component, Action<FieldInfo> action)
    {
        //Public�Ƃ���ȊO�̃t�B�[���h�ɑ΂��ď��������s
        ExecuteProcessToAllPersistentAmongPlayModeField(component, action, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        ExecuteProcessToAllPersistentAmongPlayModeField(component, action, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
    }

    //PersistentAmongPlayMode���t���Ă�A���ABindingFlags�Ŏw�肵���S�t�B�[���h�ɏ��������s����
    private static void ExecuteProcessToAllPersistentAmongPlayModeField(MonoBehaviour component, Action<FieldInfo> action, BindingFlags bindingFlags)
    {
        //�R���|�[�l���g����S�t�B�[���h���擾
        component.GetType()
          .GetFields(bindingFlags)
          .ToList()
          .ForEach(fieldInfo => {
          //PersistentAmongPlayMode���t���Ă���̂ɂ������������s
          if (fieldInfo.GetCustomAttributes(typeof(PersistentAmongPlayModeAttribute), true).Length != 0)
                  action(fieldInfo);
          });
    }

    //=================================================================================
    //�ۑ�
    //=================================================================================

    //PersistentAmongPlayMode�̑������t�����l��ۑ�
    private static void SaveValue(MonoBehaviour component)
    {
        //�e�t�B�[���h�̒l��ۑ����邽�߂�Dict
        var valueDict = new Dictionary<string, object>();

        //PersistentAmongPlayMode�̑������t�����l������Dict�ɓo�^
        ExecuteProcessToAllPersistentAmongPlayModeField(component, fieldInfo => { valueDict.Add(fieldInfo.Name, fieldInfo.GetValue(component)); });

        //�C���X�^���XID��Key�ɂ��āA�l���܂Ƃ߂�Dict��ǉ�
        _valueDictDict.Add(component.GetInstanceID(), valueDict);
    }

    //=================================================================================
    //���f
    //=================================================================================

    //PersistentAmongPlayMode�̑������t�����l�𔽉f
    private static void ApplyValue(MonoBehaviour component)
    {
        //�I���{�^�������������ɑ��݂��Ȃ�����(�V�[���Đ����ɍ폜���ꂽ�Ƃ���)��̓X���[
        if (!_valueDictDict.ContainsKey(component.GetInstanceID()))
        {
            return;
        }

        //�e�t�B�[���h�̒l��ۑ�����Dict���擾
        var valueDict = _valueDictDict[component.GetInstanceID()];

        //PersistentAmongPlayMode�̑������t�����l�������f
        var isChangedValue = false; //�l�ɕύX����������

        ExecuteProcessToAllPersistentAmongPlayModeField(component, fieldInfo => {
            var fieldName = fieldInfo.Name;

            //�l���ω��������𔻒�(�l�̕ύX�𒼐ڔ�r����Ƃ��܂��s���Ȃ��̂�String�ɂ��Ĕ�r)
            isChangedValue = isChangedValue || fieldInfo.GetValue(component).ToString() != valueDict[fieldName].ToString();

            //�l�̔��f
            fieldInfo.SetValue(component, valueDict[fieldName]);
        });

        //�l�̕ύX����������ۑ��o����悤�ɂ��邽�߁A�V�[���ɕύX������������(�Ĉ�)��ݒ�
        if (isChangedValue)
        {
            EditorSceneManager.MarkAllScenesDirty();
        }
    }

}