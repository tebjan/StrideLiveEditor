using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrideLiveEditorContainerProject
{
    public class AutoAddEntitiesScript : AsyncScript
    {
        public override async Task Execute()
        {
            await Task.Delay(10000);

            TestScript testScript = null;

            var rand = new Random();
            while (Game.IsRunning)
            {
                if (testScript == null)
                {
                    testScript = new TestScript();
                    Entity.Add(testScript);
                }

                double chance = 1.0 / SceneSystem.SceneInstance.RootScene.Entities.Sum(e => CountEntity(e));
                var randomEntity = GetRandomEntity(rand, chance, SceneSystem.SceneInstance.RootScene.Entities);

                if (rand.NextDouble() < 0)
                {
                    SceneSystem.SceneInstance.RootScene.Entities.Remove(randomEntity);
                }
                else
                {
                    var entity = new Entity($"Test Entity {rand.Next()}");
                    
                    if (randomEntity != null)
                        randomEntity.Transform.Children.Add(entity.Transform);
                    else
                        SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
                }

                await Task.Delay(2000);
            }
        }
        
        private int CountEntity(Entity entity)
        {
            int count = 1;
            foreach (var e in entity.Transform.Children)
            {
                count += CountEntity(e.Entity);
            }

            return count;
        }

        private Entity GetRandomEntity(Random random, double chance, IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (random.NextDouble() < chance)
                    return entity;

                var result = GetRandomEntity(random, chance, entity.Transform.Children.Select(e => e.Entity));
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
