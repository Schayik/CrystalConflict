using UnityEngine;

public class InteractableObject : MonoBehaviour {

    [SerializeField] private new AudioClip audio;
    [SerializeField] private string trigger;

    public virtual void Interact(Player player)
    {
        SoundManager.instance.RandomizeSfx(audio);
        player.SetAnimatorTrigger(trigger);
    }

    

}
