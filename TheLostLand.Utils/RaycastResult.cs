using System.Linq;
using SDG.Unturned;
using UnityEngine;

namespace TheLostLand.Utils;

public class RaycastResult
{
    public RaycastHit Raycast;
    public InteractableVehicle Vehicle;
    public BarricadeData Barricade;
    public BarricadeRegion BarricadeRegion;
    public StructureData Structure;
    public StructureRegion StructureRegion;
    public bool RaycastHit = false;

    public byte BarricadeX;
    public byte BarricadeY;
    public ushort BarricadePlant;
    public ushort BarricadeIndex;


    public byte StructureX;
    public byte StructureY;
    public ushort StructureIndex;

    public Transform BarricadeRootTransform;
    public Transform StructureRootTransform;

    public RaycastResult(RaycastHit info, bool hit)
    {
        RaycastHit = hit;
        if (!hit)
        {
            return;
        }
        
        Raycast = info;
        Vehicle = TryGetEntity<InteractableVehicle>();
        var target = Raycast.collider?.transform;
        if (target == null)
        {
            return;
        }
        if (target.CompareTag("Barricade"))
        {
            target = DamageTool.getBarricadeRootTransform(target);
            BarricadeRootTransform = target;
            if (!BarricadeManager.tryGetInfo(target, out var x, out var y, out var plant, out var index,
                    out var region, out var drop))
            {
                return;
            }
            BarricadeRegion = region;
            BarricadeX = x;
            BarricadeY = y;
            BarricadePlant = plant;
            BarricadeIndex = index;
            var b = region.barricades.FirstOrDefault(d => d.instanceID == drop.instanceID);
            if (b != null)
            {
                Barricade = b;
            }
        }
        else if (target.CompareTag("Structure"))
        {
            target = DamageTool.getStructureRootTransform(target);
            StructureRootTransform = target;
            if (StructureManager.tryGetInfo(target, out var x, out var y, out var index,
                    out var region))
            {
                StructureX = x;
                StructureY = y;
                StructureIndex = index;
                StructureRegion = region;
                var b = region.structures[index];
                if (b != null)
                {
                    Structure = b;
                }
            }
        }
    }

    public T TryGetEntity<T>()
    {
        return Raycast.transform.GetComponentInParent<T>();
    }

    public T[] TryGetEntities<T>()
    {
        return Raycast.transform.GetComponentsInParent<T>();
    }

    public bool ParentHasComponent<T>()
    {
        return TryGetEntity<T>() != null;
    }

    public bool HasComponent<T>()
    {
        return Raycast.transform.GetComponent<T>() != null;
    }

    public T GetComponent<T>()
    {
        return Raycast.transform.GetComponent<T>();
    }
}