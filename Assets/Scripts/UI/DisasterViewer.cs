using Unity.VisualScripting;
using UnityEngine;

public class DisasterViewer : Singleton<DisasterViewer>
{
    [SerializeField] public SpriteRenderer  spriteRenderer;
    [SerializeField] public Sprite  spriteStub;

    public void ChangeSprite(DisasterData  disaster)
    {
        if (disaster == null)
            spriteRenderer.sprite = spriteStub;
        else
            spriteRenderer.sprite = disaster.Icon;
    }
    
}