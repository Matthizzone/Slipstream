using UnityEngine;

public class FootstepMaterial : MonoBehaviour
{
    public enum Materials { Generic, Cement, Grass, Gravel, Leaves, Metal, Sand, Water, Wood }
    public Materials material;

    public string GetClip()
    {
        if (material == Materials.Generic)
        {
            return "Footstep/Generic";
        }
        else if (material == Materials.Cement)
        {
            return "Footstep/Cement";
        }
        else if (material == Materials.Grass)
        {
            return "Footstep/Grass";
        }
        else if (material == Materials.Gravel)
        {
            return "Footstep/Gravel";
        }
        else if (material == Materials.Leaves)
        {
            return "Footstep/Leaves";
        }
        else if (material == Materials.Metal)
        {
            return "Footstep/Metal";
        }
        else if (material == Materials.Sand)
        {
            return "Footstep/Sand";
        }
        else if (material == Materials.Water)
        {
            return "Footstep/Water";
        }
        else if (material == Materials.Wood)
        {
            return "Footstep/Wood";
        }

        return "Footstep/Generic";
    }
}
