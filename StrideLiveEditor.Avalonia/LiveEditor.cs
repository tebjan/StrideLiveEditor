using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia
{
    public class LiveEditor
    {
        private readonly Game _game;
        private readonly string[] _args;
        private CancellationToken _cancellationToken;

        public LiveEditor(Game game, string[] args)
        {
            _game = game;
            _args = args;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .Start(AppMain, _args);
        }

        // Application entry point. Avalonia is completely initialized.
        private void AppMain(Application app, string[] args)
        {
            // Do you startup code here
            new LiveEditorMainWindow(_game).Show();

            // Start the main loop
            app.Run(_cancellationToken);
        }
    }
}
