using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;                    //에디터 라이브러리 사용 

public class SpriteInfo
{
    public float time;
    public string spriteName;
}
public class AnimationScript : EditorWindow
{
    private AnimationClip animationClip;
    private List<SpriteInfo> spriteInfoList = new List<SpriteInfo>();

    [MenuItem("Window/Animation Sprite Extractor")]

    public static void ShowWindow()
    {
        GetWindow<AnimationScript>("Animation Sprite Extractor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Extract Sprites Info form Animation Clip", EditorStyles.boldLabel);

        animationClip = EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), true) as AnimationClip;

        if(animationClip != null)
        {
            if(GUILayout.Button("Extract Sprites Info"))
            {
                ExtractSpritesInfo(animationClip);
            }

            if(spriteInfoList.Count > 0)
            {
                GUILayout.Label("Sprites Info : ", EditorStyles.boldLabel);
                foreach( var spriteInfo in spriteInfoList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Time :", GUILayout.Width(50));
                    GUILayout.Label(spriteInfo.time.ToString() , GUILayout.Width(100));
                    GUILayout.Label("Sprite :", GUILayout.Width(50));
                    GUILayout.Label(spriteInfo.spriteName , GUILayout.Width(200));
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    private void ExtractSpritesInfo(AnimationClip Clip)
    {
        spriteInfoList.Clear();
        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(Clip);

        foreach(var binding in bindings)
        {
            if(binding.propertyName.Contains("Sprite"))
            {
                var keyframes = AnimationUtility.GetObjectReferenceCurve(Clip, binding);

                foreach(var keyframe in keyframes)
                {
                    Sprite sprite = keyframe.value as Sprite;
                    if(sprite != null)
                    {
                        spriteInfoList.Add(new SpriteInfo { time = keyframe.time, spriteName = sprite.name });
                    }
                }
            }
        }
    }
}
