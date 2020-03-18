using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    [ProviderAlias(nameof(Terminal))]
    public sealed class TerminalLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        readonly TerminalLoggerProcessor _processor = new TerminalLoggerProcessor();

        readonly ConcurrentDictionary<string, TerminalLogger> _loggers =
            new ConcurrentDictionary<string, TerminalLogger>();

        readonly IOptionsMonitor<TerminalLoggerOptions> _options;

        readonly IDisposable _reload;

        IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

        public TerminalLoggerProvider(IOptionsMonitor<TerminalLoggerOptions> options)
        {
            _options = options;
            _reload = options.OnChange(opts =>
            {
                foreach (var logger in _loggers)
                    logger.Value.Options = opts;
            });
        }

        public void Dispose()
        {
            _reload.Dispose();
            _processor.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName ?? throw new ArgumentNullException(categoryName),
                name => new TerminalLogger(_options.CurrentValue, _scopeProvider, _processor));
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));

            foreach (var logger in _loggers)
                logger.Value.ScopeProvider = _scopeProvider;
        }
    }
}