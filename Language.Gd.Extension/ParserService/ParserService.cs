#region Using Directives 

using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.Logging;

using Task = System.Threading.Tasks.Task;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.ParserService {

    public delegate SyntaxNode ParseMethod(string text, string filePath = null, CancellationToken cancellationToken = default);

    sealed class ParserService: IDisposable {

        static readonly Logger Logger = Logger.Create<ParserService>();

        static readonly object      ParseMethodKey = new Object();
        readonly        IDisposable _parserObs;
        SyntaxTreeAndSnapshot       _syntaxTreeAndSnapshot;
        bool                        _waitingForAnalysis;

        ParserService(ITextBuffer textBuffer) {
            TextBuffer = textBuffer;

            WeakEventDispatcher.Connect(this, textBuffer);

            _parserObs = Observable.FromEventPattern<EventArgs>(
                                        handler => RebuildTriggered += handler,
                                        handler => RebuildTriggered -= handler)
                                   .Select(_ => CreateBuildResultArgs())
                                   .Throttle(ServiceProperties.ParserServiceThrottleTime)
                                   .Select(args => Observable.DeferAsync(async token => {
                                        var parseResult = await BuildAsync(args, token).ConfigureAwait(false);

                                        return Observable.Return(parseResult);
                                    }))
                                   .Switch()
                                   .ObserveOn(SynchronizationContext.Current)
                                   .Subscribe(TrySetResult);

            _waitingForAnalysis = true;
            // Initiales Parsen antriggern
            Invalidate();
        }

        public void Dispose() {
            _parserObs.Dispose();
        }

        public event EventHandler<EventArgs> ParseResultChanging;

        public event EventHandler<SnapshotSpanEventArgs> ParseResultChanged;

        // Dieses Event feuern wir um den Observer zu "füttern".
        event EventHandler<EventArgs> RebuildTriggered;

        [NotNull]
        public ITextBuffer TextBuffer { get; }

        public bool WaitingForAnalysis {
            get { return _waitingForAnalysis; }
        }

        [CanBeNull]
        public SyntaxTreeAndSnapshot SyntaxTreeAndSnapshot {
            get { return _syntaxTreeAndSnapshot; }
        }

        public static ParseMethod GetParseMethod(ITextBuffer textBuffer) {
            textBuffer.Properties.TryGetProperty(ParseMethodKey, out ParseMethod parseMethod);
            return parseMethod ?? Syntax.ParseGuiDescriptionSyntax;
        }

        public static ParserService GetOrCreateSingelton(ITextBuffer textBuffer) {
            return textBuffer.Properties.GetOrCreateSingletonProperty(
                typeof(ParserService),
                () => new ParserService(textBuffer));
        }

        public static ParserService TryGet(ITextBuffer textBuffer) {
            textBuffer.Properties.TryGetProperty<ParserService>(typeof(ParserService), out var parserService);
            return parserService;
        }

        public static void SetParseMethod(ITextBuffer textBuffer, ParseMethod parseMethod) {
            textBuffer.Properties.AddProperty(ParseMethodKey, parseMethod);
        }

        public void Invalidate() {
            OnParseResultChanging(EventArgs.Empty);
            OnRebuildTriggered(EventArgs.Empty);
        }

        public SyntaxTreeAndSnapshot UpdateSynchronously(CancellationToken cancellationToken = default) {
            var syntaxTreeAndSnapshot = SyntaxTreeAndSnapshot;
            if (syntaxTreeAndSnapshot != null && syntaxTreeAndSnapshot.IsCurrent(TextBuffer)) {
                return syntaxTreeAndSnapshot;
            }

            syntaxTreeAndSnapshot = BuildSynchronously(CreateBuildResultArgs(), cancellationToken);
            TrySetResult(syntaxTreeAndSnapshot);

            return syntaxTreeAndSnapshot;
        }

        void OnRebuildTriggered(EventArgs e) {
            RebuildTriggered?.Invoke(this, e);
        }

        void OnParseResultChanging(EventArgs e) {
            _waitingForAnalysis = true;
            ParseResultChanging?.Invoke(this, e);
        }

        void OnParseResultChanged(SnapshotSpanEventArgs e) {
            _waitingForAnalysis = false;
            ParseResultChanged?.Invoke(this, e);
        }

        struct BuildResultArgs {

            public ITextSnapshot Snapshot    { get; set; }
            public string        Text        { get; set; }
            public string        FilePath    { get; set; }
            public ParseMethod   ParseMethod { get; set; }

        }

        /// <summary>
        /// Diese Methode muss im GUI Thread aufgerufen werden!
        /// </summary>
        BuildResultArgs CreateBuildResultArgs() {
            var args = new BuildResultArgs {
                Snapshot    = TextBuffer.CurrentSnapshot,
                Text        = TextBuffer.CurrentSnapshot.GetText(),
                FilePath    = TextBuffer.GetTextDocument()?.FilePath,
                ParseMethod = GetParseMethod(TextBuffer)
            };

            return args;
        }

        /// <summary>
        /// Achtung: Diese Methode wird bereits in einem Background Thread aufgerufen. Also vorischt bzgl. thread safety!
        /// Deshalb werden die BuildResultArgs bereits vorab im GUI Thread erstellt.
        /// </summary>
        static async Task<SyntaxTreeAndSnapshot> BuildAsync(BuildResultArgs args, CancellationToken cancellationToken) {

            return await Task.Run(() => {

                using (Logger.LogBlock(nameof(BuildAsync))) {
                    return BuildSynchronously(args, cancellationToken);
                }

            }, cancellationToken).ConfigureAwait(false);
        }

        static SyntaxTreeAndSnapshot BuildSynchronously(BuildResultArgs args, CancellationToken cancellationToken) {

            var syntaxTree = args.ParseMethod(args.Text, args.FilePath, cancellationToken).SyntaxTree;

            return new SyntaxTreeAndSnapshot(syntaxTree, args.Snapshot);
        }

        void TrySetResult(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot) {

            // Der Puffer wurde zwischenzeitlich schon wieder geändert. Dieses Ergebnis brauchen wir nicht,
            // da bereits ein neues berechnet wird.
            if (!syntaxTreeAndSnapshot.IsCurrent(TextBuffer)) {
                return;
            }

            _syntaxTreeAndSnapshot = syntaxTreeAndSnapshot;

            var snapshotSpan = syntaxTreeAndSnapshot.Snapshot.GetFullSpan();
            OnParseResultChanged(new SnapshotSpanEventArgs(snapshotSpan));
        }

        // Irgend jemand scheint den ITextBuffer länger als erhofft im Speicher zu halten
        // Damit der Parserservice nicht genauso lange im Speicher verbleibt, verknüpfen wir
        // hier die Events "weak".
        sealed class WeakEventDispatcher {

            readonly WeakReference _target;

            WeakEventDispatcher(ParserService service) {
                _target = new WeakReference(service);
            }

            public static void Connect(ParserService service, ITextBuffer textBuffer) {
                var dispatcher = new WeakEventDispatcher(service);
                textBuffer.Changed += dispatcher.OnTextBufferChanged;
            }

            void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) {
                var textBuffer = (ITextBuffer) sender;
                if (_target.Target is ParserService target) {
                    target.Invalidate();
                } else {
                    textBuffer.Changed -= OnTextBufferChanged;
                }
            }

        }

    }

}