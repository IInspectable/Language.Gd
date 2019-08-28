#region Using Directives

using System;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Logging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class ComEventSink {

        public static IDisposable Advise<T>(object obj, T sink) where T : class {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (!typeof(T).IsInterface) {
                throw new InvalidOperationException();
            }

            if (!(obj is IConnectionPointContainer connectionPointContainer)) {
                throw new ArgumentException("Not an IConnectionPointContainer", nameof(obj));
            }

            connectionPointContainer.FindConnectionPoint(typeof(T).GUID, out var connectionPoint);
            if (connectionPoint == null) {
                throw new InvalidOperationException("Could not find connection point for " + typeof(T).FullName);
            }

            connectionPoint.Advise(sink, out var cookie);

            return new ComEventSinkImpl(connectionPoint, cookie);
        }

        sealed class ComEventSinkImpl: IDisposable {

            static readonly Logger Logger = Logger.Create(typeof(ComEventSinkImpl));

            readonly IConnectionPoint _connectionPoint;
            readonly uint             _cookie;
            bool                      _unadvised;

            public ComEventSinkImpl(IConnectionPoint connectionPoint, uint cookie) {
                _connectionPoint = connectionPoint;
                _cookie          = cookie;
            }

            public void Dispose() {

                ThreadHelper.ThrowIfNotOnUIThread();

                if (_unadvised) {
                    Logger.Error("Already unadvised.");
                    return;
                }

                _connectionPoint.Unadvise(_cookie);
                _unadvised = true;
            }

        }

    }

}