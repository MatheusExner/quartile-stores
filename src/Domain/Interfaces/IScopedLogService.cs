namespace Domain.Interfaces
{
    public interface IScopedLogService : ILogService
    {
        void EnrichDiagnosticContext(string propertyName, string value);
    }
}
