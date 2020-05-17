using System;
using System.Threading;
using Stride.Engine;

#if NETFRAMEWORK
using StrideLiveEditor;
#else
using System.Threading.Tasks;
using StrideLiveEditor.Avalonia;
#endif

namespace StrideLiveEditorContainerProject.Windows
{
    internal class StrideLiveEditorContainerProjectApp
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var game = new Game())
            using (var cts = new CancellationTokenSource())
            {
#if DEBUG
#if NETFRAMEWORK
                var window = new LiveEditorMainWindow(game);
                window.Show();
#else
                Task.Run(() => StartLiveEditor(args, game, cts));
#endif
#endif
                game.Run();
#if DEBUG
#if NETFRAMEWORK
                if (window != null)
                    window.Close();
#else
                cts.Cancel();
#endif
#endif
            }
        }

        private static void StartLiveEditor(string[] args, Game game, CancellationTokenSource cts)
        {
#if NETCOREAPP
            var liveEditor = new LiveEditor(game, args);
            liveEditor.Start(cts.Token);
#endif
        }
    }
}
