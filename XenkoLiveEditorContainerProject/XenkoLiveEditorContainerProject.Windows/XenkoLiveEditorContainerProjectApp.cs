using Xenko.Engine;

namespace XenkoLiveEditorContainerProject
{
    class XenkoLiveEditorContainerProjectApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
