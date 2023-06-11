using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AvatarManager
{
    public static List<Avatar> Avatars = new List<Avatar>();
    public static void LoadAvatars()
    {
        var sprites = Resources.LoadAll<Sprite>("Avatar/Sprites");
        Avatars = new List<Avatar>();
        for (int x = 0; x < sprites.Length; x++)
        {
            Avatars.Add(new Avatar()
            {
                Id = x,
                Name = sprites[x].name,
                Sprite = sprites[x],
            });
        }
    }

    public static Sprite GetAvatarByName(string name)
    {
        if (Avatars.Count == 0)
            LoadAvatars();
        return Avatars.SingleOrDefault(t => t.Name == name).Sprite;
    }

    public static Sprite GetAvatar(int id)
    {
        if (Avatars.Count == 0)
            LoadAvatars();
        return Avatars.SingleOrDefault(t => t.Id == id).Sprite;
    }
}
