using Stride.Core.Mathematics;
using Stride.Engine;

namespace StrideLiveEditorContainerProject
{
    public class TestScript : SyncScript
    {
        public override void Update()
        {
            var elapsed = (float)Game.UpdateTime.Elapsed.TotalSeconds;

            Entity.Transform.Position += new Vector3(1, 1, 1) * elapsed;
        }
    }
}
