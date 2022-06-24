using UnityEngine;
using UnityEditor;

/// <summary>
/// PersistentAmongPlayModeAttribute�̑�����t�^��������Inspector�̕\����ς��邽�߂̃N���X
/// </summary>
[CustomPropertyDrawer(typeof(PersistentAmongPlayModeAttribute))]
public class PersistentAmongPlayModeDrawer : PropertyDrawer
{

    /// <summary>
    /// GUI�̍������擾
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //�ʏ���3�{�̍������m��(EditorGUIUtility.singleLineHeight�͈�s�̃f�t�H���g�̍���)
        return EditorGUIUtility.singleLineHeight * 3;
    }

    /// <summary>
    /// GUI�̕\���ݒ�
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var size = new Vector2(0, EditorGUIUtility.singleLineHeight);
        position.size = position.size - size;

        //���ߕ\��
        EditorGUI.HelpBox(position, label.text + "�́A�Đ����ɕύX���Ă���~���ɖ߂�܂���", MessageType.Info);

        //���l��ݒ肷�邽�߂�GUI�ݒ�
        position.size = position.size - size;
        position.y += EditorGUIUtility.singleLineHeight * 2;
        EditorGUI.PropertyField(position, property, label);
    }

}