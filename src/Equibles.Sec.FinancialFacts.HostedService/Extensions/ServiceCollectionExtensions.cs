using Equibles.Core.AutoWiring;
using Equibles.Sec.FinancialFacts.BusinessLogic.Parsers;
using Equibles.Sec.FinancialFacts.HostedService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Equibles.Sec.FinancialFacts.HostedService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecFinancialFactsWorker(this IServiceCollection services)
    {
        // Service registrations only — the ISharesOutstandingProvider (BusinessLogic)
        // and XBRL parsers are consumed by the Yahoo price importer and the SEC
        // filing pipeline. Keep the AutoWire calls; the heavy hosted scrapers
        // (FinancialFactsScraper, ConceptMetadata, XbrlFactsExtraction,
        // ReportedStatementsCapture/Parse, SplitShareCountEvidence) are disabled
        // for insider + congressional tracking and not registered here.
        services.AutoWireServicesFrom<FinancialFactsImportService>();
        // The XBRL parsers live in the BusinessLogic assembly; wire them so the
        // dimensional-fact extraction sweep can resolve them.
        services.AutoWireServicesFrom<InlineXbrlParser>();
        return services;
    }
}
