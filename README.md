# Custom tags and text animation for TMP

![Example](https://drive.google.com/uc?id=1HEhZUus5WSkSyWsOthn82qQsIBYOOfQU)

This package aims to implement a more clean and robust way to extend TMP with custom tags. 

Instead of using [`<link>`](https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichTextLink.html) tags (which is really clunky), the idea is to use a [`ITextPreprocessor`](https://docs.unity3d.com/Packages/com.unity.textmeshpro@1.5/api/TMPro.ITextPreprocessor.html) to parse tags, and then update text mesh using that information.

This repository is a proof-of-concept, so don't expect it to be stable.

## Install
I recommend to fork or clone this repository, for when something inevitably needs to be fixed.

But if you want to install it as package:
1. Select in UPM "Add package from git URL..."
2. Install via link
```
https://github.com/oneir0mancer/tmpro-custom-tags.git
```
## Custom Tags
### Creating custom text effect
To create custom text effect inherit from `BaseTextEffect`, which lets you modify vertices' information every frame.

<details>
  <summary>Code Example</summary>

```csharp
public class CustomTextEffect : BaseTextEffect
{
    public override string Tag => "custom_tag";
    
    protected override void ApplyToCharacter(TMP_Text text, TMP_CharacterInfo charInfo)
    {
        // YOUR IMPLEMENTATION
    }
}
```
</details>

In order to reference this new tag in Inspector, you will also need to create a ScriptableObject wrapper for this class.

<details>
  <summary>Code Example</summary>
  
```csharp
[CreateAssetMenu(menuName = "TMP Custom Tags/Custom Effect")]
public class CustomTextEffectSo : TextEffectSoWrapper<CustomTextEffect>
{ }
```
</details>

### Using custom text effect
In order to use custom tags, add `TagParser` compontent to your gameobject with TMP_Text, and supply ScriptableObjects for all custom tags used. 

You can preview them if you RMB the component and choose corresponding context menu option.
Just note that while in editor mode Unity doesn't redraw scene or game view every frame.

## Typewriter Animation
This component will animate text appearing character by character. Since every step will regenerate text mesh, interfering with effects, we cache vertex data right before the step, and restore it after mesh was regenerated.

## Limitations
* Tag preprocessor is very bare-bones, so any unknown tag will interfere with where other tags start and end.
* You need to create a ScriptableObject wrapper for every tag to use them in Inspector. 
  ~~I'm working on a component that would eliminate that by using `[SerializedReference]`.~~
