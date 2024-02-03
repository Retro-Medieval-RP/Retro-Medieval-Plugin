using System.Reflection;
using SDG.Unturned;
using TheLostLand.Modules.Attributes;
using UnityEngine;

namespace TheLostLand.Modules.EntitySpawns;

[ModuleInformation("Entity Spawns")]
public class EntitySpawnsModule : Module
{
    public override void Load()
    {
    }

    public void SpawnEntity(Vector3 player_loc)
    {
        if (!GetStorage<EntitySpawnsStorage>(out var storage))
        {
            return;
        }

        var add_animal_method = typeof(AnimalManager)
            .GetMethod("addAnimal", BindingFlags.NonPublic | BindingFlags.Instance);
        
        foreach (var e in storage.StorageItem)
        {
            if (!(Vector3.Distance(player_loc, new Vector3(e.LocationX, e.LocationY, e.LocationZ)) >= 20))
            {
                continue;
            }
            
            var animal = add_animal_method?.Invoke(this, [
                e.EntitySpawnID,
                new Vector3(e.LocationX, e.LocationY, e.LocationZ),
                0f,
                false
            ]) as Animal;
                
            AnimalManager.sendAnimalAlive(animal, new Vector3(e.LocationX, e.LocationY, e.LocationZ), 0);
        }
    }
    
    public override void Unload()
    {
    }
}