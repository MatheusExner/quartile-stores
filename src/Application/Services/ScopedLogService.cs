using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ScopedLogService : BaseLogService, IScopedLogService
    {
        private readonly Serilog.IDiagnosticContext _diagnosticContext;

        public ScopedLogService(
            ILogger<ScopedLogService> logger,
            Serilog.IDiagnosticContext diagnosticContext
        )
            : base(logger, diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public void EnrichDiagnosticContext(string propertyName, string value)
        {
            _diagnosticContext.Set(propertyName, value);
        }
    }
}
