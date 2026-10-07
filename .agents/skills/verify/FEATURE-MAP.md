# Equibles Feature Map

Generated: 2026-10-07 (Melbourne). Source of truth: `src/Equibles.Web/Controllers/*.cs` (routes), `tests/Equibles.FunctionalTests/Tests/*.cs` (selectors/tests), `docs/guide/*.md` (nav paths), `docs/technical/mcp-tools.md` + `src/*/*.Mcp/Tools/*.cs` (MCP tools). Routes are ASP.NET attribute templates; URLs are lowercased at runtime (`LowercaseUrls = true`). Base route for template-less actions is `{controller=Home}/{action=Index}` (BaseController), so `Index` actions resolve to `/{controller}`.

## Launch

```bash
docker compose up                                  # full stack
# db      PostgreSQL/ParadeDB  localhost:5432
# web     Web portal           http://localhost:8080
# mcp     MCP server           http://localhost:9090/mcp   (compose maps 9090:8080 —
#         committed local deploy override; upstream default + all docs
#         (README, faq, hosts.md) still say 8081)
# worker  scrapers             (no port)
dotnet run --project src/Equibles.Web              # fallback: http://localhost:5000
dotnet run --project src/Equibles.Mcp.Server
dotnet run --project src/Equibles.Worker.Host      # run against `docker compose up db -d`
```

- Auth env vars: `Auth__Username` / `Auth__Password` (set in compose via `AUTH_USERNAME`/`AUTH_PASSWORD` in `.env`). Both set → fallback policy requires login on every page; unset → open access. Logout: `POST /auth/logout`.
- `/healthz` — anonymous health check, body contains `Healthy`.
- `McpApiKey` env → `Authorization: Bearer <key>` required on `/mcp` when set; open when empty.

## Areas

Groups: Stocks · Institutions & 13F profiles · 13F holdings (market-wide) · Insider trading · Congress · Investment advisers · Market-wide short data · Economic data (FRED) · Futures (CFTC) · Market indicators (CBOE) · Search · Status & ops · Home, auth, changelog & misc.

Top-nav shape (from `_Layout.cshtml` + guides): logo → Home; **Stocks**, **Institutions**; **More** dropdown (13F Statistics, Latest 13F Filings, 13F Trends, Double-Down Report, Conviction Heat Map, Overlap Matrix, Smart Money Index, Insider Trading, Investment Advisers, Economic Data, Futures, Market, Largest Short Volume, Most Shorted); **MCP** (= `/home/connect`); **Status**; search box (submits to `/search`).

### Stocks

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Stock browser (list, search, sort, min-market-cap, paging) | `/stocks` | Home → **Stocks** | `h1`; `input[name='search']`; `select[name='sort']`; `tbody tr.stock-row td:first-child a` | StocksIndexTests, StocksIndexSeededTests, StocksIndexSortFilterSeededTests, StocksIndexPaginationTests | docs/guide/tutorial-explore-stock.md |
| Stock profile (redirects to Price tab; 404 on unknown) | `/stocks/{ticker}` | Home → Stocks → click ticker row | none in tests (URL/status asserts only) | StocksShowTests | docs/guide/tutorial-explore-stock.md |
| Price tab (chart, OHLCV table, badges) | `/stocks/{ticker}/price` | Home → Stocks → ticker → tab **Price** (default) | `h2`; `div.font-mono.font-semibold`; `div.font-mono.text-xs`; `.badge` | StocksPriceTests, StocksPriceSeededTests, StocksShowFilingActivityBadgeTests | docs/guide/tutorial-explore-stock.md |
| 13F Holdings tab (holder table, top buyers/sellers, buckets, ownership-trend chart, CSV) | `/stocks/{ticker}/holdings` | Home → Stocks → ticker → tab **13F Holdings** | `[data-testid='holdings-top-buyers']`; `[data-testid='holdings-top-sellers']`; `[data-testid^='holdings-bucket-']`; `[data-testid='holdings-bucket-unchanged'] .collapse-title .badge`; `#ownership-trend-chart` | StocksHoldingsTests, StocksHoldingsSeededTests, StocksHoldingsOwnershipTrendSeededTests | docs/guide/tutorial-explore-stock.md |
| Short volume tab (FINRA; chart + table) | `/stocks/{ticker}/short-volume` | Home → Stocks → ticker → tab **Short Volume** | `#short-volume-chart`; `h2`; `table tbody tr` | StocksShortVolumeTests | docs/guide/tutorial-explore-stock.md |
| Short interest tab | `/stocks/{ticker}/short-interest` | Home → Stocks → ticker → tab **Short Interest** | `#short-interest-chart`; `h2`; `table tbody tr` | StocksShortInterestTests | docs/guide/tutorial-explore-stock.md |
| Fails-to-deliver (FTD) tab (SEC FTD) | `/stocks/{ticker}/ftd` | Home → Stocks → ticker → tab **Fails to Deliver** | `h2`; `table tbody tr` | StocksFtdTests, StocksFtdSeededTests | docs/guide/tutorial-explore-stock.md |
| Financials tab (XBRL income/balance/cash-flow) | `/stocks/{ticker}/financials` | Home → Stocks → ticker → tab **Financials** | `#financials-statement`; `#financials-period`; `table tbody tr` | StocksFinancialsSeededTests | docs/guide/how-to-view-financial-statements.md |
| SEC filings/documents tab | `/stocks/{ticker}/documents` | Home → Stocks → ticker → tab **SEC Filings** | `h2` | StocksDocumentsTests | docs/guide/how-to-ask-about-sec-filings.md |
| Filing document view (rendered filing content) | `/stocks/{ticker}/documents/{id:guid}` | Home → Stocks → ticker → SEC Filings → filing row | none in tests (URL/status asserts only) | StocksShowDocumentTests | docs/guide/tutorial-explore-stock.md |
| Insider trading tab (Forms 3/4/5) | `/stocks/{ticker}/insider-trading` | Home → Stocks → ticker → tab **Insider Trades** | `h2`; `table tbody tr` | StocksInsiderTradingTests, StocksInsiderTradingSeededTests | docs/guide/how-to-view-insider-activity.md |
| Proposed sales tab (Form 144) | `/stocks/{ticker}/proposed-sales` | Home → Stocks → ticker → tab **Proposed Sales** | none in tests | — | docs/guide/how-to-view-proposed-sales.md |
| Exempt offerings tab (Form D) | `/stocks/{ticker}/exempt-offerings` | Home → Stocks → ticker → tab **Exempt Offerings** | none in tests | — | docs/guide/how-to-view-exempt-offerings.md |
| Fund operations tab (N-CEN) | `/stocks/{ticker}/fund-operations` | Home → Stocks → ticker → tab **Fund Operations** | none in tests | — | docs/guide/how-to-view-fund-operations.md |
| Fund holdings tab (N-PORT) | `/stocks/{ticker}/fund-holdings` | Home → Stocks → ticker → tab **Fund Holdings** | none in tests | — | docs/guide/how-to-view-fund-holdings.md |
| Congressional trades tab | `/stocks/{ticker}/congressional-trades` | Home → Stocks → ticker → tab **Congressional Trades** | `h2`; `table tbody tr`; `td.text-right.font-mono`; `.badge` | StocksCongressionalTradesTests, StocksCongressionalTradesSeededTests | docs/guide/how-to-ask-about-congressional-trades.md |
| Per-stock holder detail (institution × stock) | `/stocks/{ticker}/holders/{cik}` | Home → Stocks → ticker → 13F Holdings → click holder name | `.breadcrumbs li`; `h1` | StocksShowHolderTests | docs/guide/tutorial-explore-stock.md |

### Institutions & 13F profiles

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Institutions browser (name/CIK search, state/city/value filters, fund scores) | `/institutions` | Home → **Institutions** | `[data-testid='institutions-table']`; `input[name='search'][placeholder='Institution name...']`; `button[type='submit']`; `a:has-text('Clear')`; `[data-testid='institution-fund-score']`; `[data-testid='fund-score-alpha']`; `[data-testid='institution-alpha']` | InstitutionsIndexSeededTests, InstitutionsFundScoreSeededTests | docs/guide/how-to-browse-institutions.md |
| Institution profile (summary, industry allocation, quarterly activity, charts) | `/institutions/{cik}` | Home → Institutions → click institution row (row link `a[href='/institutions/{cik}']`) | `[data-testid='institution-summary']`; `[data-testid='institution-backtest-link']`; `[data-testid='institution-export-csv']`; `h1` | InstitutionProfileSeededTests | docs/guide/how-to-browse-institutions.md |
| Clone-portfolio backtest (benchmark, from/to) | `/institutions/{cik}/backtest` | Home → Institutions → profile → **Backtest** link | `[data-testid='backtest-filters']`; `[data-testid='backtest-heading']`; `input[name='from']`; `input[name='to']`; `select[name='benchmark']`; `button[type='submit']` | InstitutionsBacktestSeededTests, InstitutionsBacktestReportDateOverflowTests | docs/guide/how-to-browse-institutions.md |
| Side-by-side comparison (2–4 filers) | `/institutions/compare` (`?ciks=A&ciks=B…&date=YYYY-MM-DD`) | Direct URL (guide: go to `/institutions/compare`, pick institutions via autocomplete chips) | `[data-testid='compare-overlap-summary']`; `[data-testid='compare-overlap-table']` | InstitutionsCompareSeededTests | docs/guide/how-to-compare-institutions-side-by-side.md |
| Overlap matrix (2–10 filers) | `/institutions/overlap-matrix` (`?ciks=A&ciks=B…`) | Home → More → **Overlap Matrix** | `[data-testid='overlap-matrix-table']`; `table[aria-label='Fund summaries']` | InstitutionsOverlapMatrixSeededTests, InstitutionsOverlapMatrixMaxCiksTests | docs/guide/how-to-compare-institution-overlap.md |
| Combined consensus portfolio (2–25 filers) | `/institutions/combined` | Direct URL (guide: go to `/institutions/combined`, Combine) | `[data-testid='combined-summary']`; `[data-testid='combined-portfolio-table']` | InstitutionsCombinedSeededTests | docs/guide/how-to-view-combined-institution-portfolio.md |
| Smart Money Index (top-fund consensus signal) | `/institutions/smart-money-index` | Home → More → **Smart Money Index** | none in tests (view test ids: `smart-money-heading`/`-chart`/`-constituents`) | — | docs/guide/how-to-view-smart-money-index.md |
| Institution autocomplete JSON (picker backend) | `/institutions/search` (`?q=&limit=`) | Used by Compare / Overlap Matrix / Combined / Screener pickers | none in tests | — | docs/guide/how-to-compare-institution-overlap.md |

### 13F holdings (market-wide)

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Holdings activity leaderboards (Top Buys / Top Sells / New / Sold Out) | `/holdings/activity` (`?date=&combined=`) | Direct URL (guide: go to `/holdings/activity`) | `[data-testid='activity-top-buys']`; `[data-testid='activity-top-sells']`; `[data-testid='activity-new-positions']`; `[data-testid='activity-sold-out-positions']`; `[data-testid='activity-export-csv']` | HoldingsActivitySeededTests | docs/guide/how-to-view-holdings-activity.md |
| Holdings screener (filters + QoQ compare) | `/holdings/screener` | Direct URL (guide: go to `/holdings/screener`) | `[data-testid='screener-filters']`; `[data-testid='screener-results']`; `input[name='MinFilerCount']`; `button:has-text('Apply filters')` | HoldingsScreenerSeededTests | docs/guide/how-to-use-holdings-screener.md |
| Screener CSV export | `/holdings/screener/export.csv` | Home → screener → **Download CSV** | none in tests (link `[data-testid='screener-export-csv']` checked for presence only) | — | docs/guide/how-to-use-holdings-screener.md |
| Latest 13F filings (paged) | `/holdings/latest-13f-filings` | Home → More → **Latest 13F Filings** | `[data-testid='latest-filings-table']`; `.badge`; `tbody tr` | HoldingsLatestFilingsSeededTests | docs/guide/how-to-view-latest-13f-filings.md |
| 13F statistics | `/holdings/13f-statistics` | Home → More → **13F Statistics** | `[data-testid='stats-table']`; `.font-bold`; `h2` | HoldingsStatsSeededTests | docs/guide/how-to-view-13f-statistics.md |
| 13F trends (charts) | `/holdings/13f-trends` | Home → More → **13F Trends** | `canvas#aum-chart`; `canvas#filer-chart` | HoldingsTrendsSeededTests | docs/guide/how-to-view-13f-trends.md |
| Double-Down report (paged) | `/holdings/double-down-report` | Home → More → **Double-Down Report** | `[data-testid='double-down-table']`; `[data-testid='double-down-pager']` | HoldingsDoubleDownSeededTests | docs/guide/how-to-view-double-down-report.md |
| Most held (breadth leaderboard) | `/holdings/most-held` | Direct URL (guide documents MCP only, not this page) | `[data-testid='most-held-table']`; `[data-testid='most-held-pager']`; `select#most-held-date`; `select#most-held-sort` | HoldingsMostHeldSeededTests | — (MCP twin: docs/guide/how-to-ask-about-most-held-stocks.md) |
| Conviction heat map (bubble chart) | `/holdings/conviction-heat-map` | Home → More → **Conviction Heat Map** | `canvas[aria-label='13F conviction heat map bubble chart']` | HoldingsHeatMapSeededTests | docs/guide/how-to-view-conviction-heat-map.md |
| CSV export endpoints (holders / institution / activity) | `/holdings/export/holders` · `/holdings/export/institution` · `/holdings/export/activity` | **Download CSV** buttons on stock-holdings / institution-profile / activity pages | none in tests (HttpClient-level tests; no DOM) | HoldingsExportHoldersCsvTests, HoldingsExportInstitutionCsvTests, HoldingsExportActivityCsvTests | docs/guide/how-to-browse-institutions.md |

### Insider trading

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Insider dashboard (top buys / top sells / biggest, 90-day) | `/insider-trading/dashboard` | Home → More → **Insider Trading** | `[data-testid='insider-top-buys']`; `[data-testid='insider-top-sells']`; `[data-testid='insider-biggest']` | InsiderDashboardSeededTests | docs/guide/how-to-view-insider-activity.md |
| Insider profile (transactions across companies) | `/insiders/{ownerCik}` | Home → search box → insiders category → click insider name | `h1`; `table tbody tr`; `text=CEO` | InsiderProfileSeededTests | docs/guide/how-to-view-insider-profile.md |

### Congress

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Congress member profile (trades, disclosures) | `/congress/{id:guid}` | Home → search box → Congress category → click member name | `h1`; `table tbody tr`; `text=Member of Congress` | CongressMemberProfileSeededTests | docs/guide/how-to-view-congress-member-trades.md |

### Investment advisers

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Advisers browser (Form ADV list, search, paging) | `/advisers` | Home → More → **Investment Advisers** | `h1`; GetByRole(AriaRole.Link, new() { Name = "BNY MELLON SECURITIES CORPORATION" }) | AdvisersIndexTests, AdvisersIndexPageOverflowTests | docs/guide/how-to-browse-investment-advisers.md |
| Adviser profile (AUM, SEC number, fees) | `/advisers/{crd:int}` | Advisers list → click adviser name | none in tests (`body` contains asserts via index test) | AdvisersIndexTests | docs/guide/how-to-browse-investment-advisers.md |

### Market-wide short data

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Most shorted (FINRA short-interest leaderboard) | `/most-shorted` | Home → More → **Most Shorted** | none in tests (view test id: `[data-testid='most-shorted-table']`) | — | docs/guide/how-to-browse-short-data.md |
| Largest short volume (FINRA daily leaderboard) | `/short-volume` | Home → More → **Largest Short Volume** | none in tests (view test id: `[data-testid='short-volume-table']`) | — | docs/guide/how-to-browse-short-data.md |

### Economic data (FRED)

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Economic data browser (series grouped by category) | `/economicdata` | Home → More → **Economic Data** | `h1`; `a.font-mono` | EconomicDataIndexTests | docs/guide/how-to-browse-economic-data.md |
| Series detail (chart + observations) | `/economicdata/{seriesId}` | Economic Data landing → click series (or direct `/economicdata/FEDFUNDS`) | `.breadcrumbs li`; `#economy-chart` | EconomicDataShowTests | docs/guide/how-to-browse-economic-data.md |

### Futures (CFTC)

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Futures browser (contracts overview) | `/futures` | Home → More → **Futures** | `h1`; `p` | CftcIndexTests | docs/guide/how-to-browse-futures.md |
| Contract detail (positioning charts) | `/futures/{marketCode}` | Futures landing → click contract row (or `/futures/{market-code}`) | `h1`; `h2` | CftcShowTests | docs/guide/how-to-browse-futures.md |

### Market indicators (CBOE)

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Market overview (indicator cards) | `/market` | Home → More → **Market** | `h2.card-title` | MarketIndexTests | docs/guide/how-to-browse-market-indicators.md |
| Put/call ratio detail (404 on bad type) | `/market/putcallratio/{type}` | Market overview → click put/call card | `#pcr-chart`; `.breadcrumbs li`; `tbody tr` | MarketPutCallRatioTests, MarketPutCallRatioInvalidTypeTests | docs/guide/how-to-browse-market-indicators.md |
| VIX chart page | `/market/vix` | Market overview → click VIX card | `#vix-chart`; `.breadcrumbs li` | MarketVixTests | docs/guide/how-to-browse-market-indicators.md |

### Search

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Global search (7 categories, sort, date range; exact ticker redirects to stock) | `/search` · `/search/results` | Home → search box in nav (or magnifier icon → `/Search`) | `#global-search-form input[name='q']`; `#global-search-form button[type='submit']`; `#search-results`; `#search-sort` | SearchIndexSeededTests, SearchInvertedDateRangeTests | docs/guide/how-to-search.md |

### Status & ops

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Status dashboard (worker states, data counts, MCP key) | `/status` | Home → **Status** | `[data-status-interval='5']`; `h1` | StatusIndexTests | docs/guide/how-to-view-status-and-errors.md |
| Data counts JSON | `/status/data` | Direct URL | none in tests (JSON asserts only) | StatusDataTests | docs/guide/how-to-view-status-and-errors.md |
| Error detail + Mark as seen / Delete (POST, antiforgery) | `/status/show/{id:guid}` (GET); POST actions: `[HttpPost("{id:guid}/markasseen")]`, `[HttpPost("{id:guid}/delete")]`, `[HttpPost("deleteall")]` — combined with base route `{controller}/{action}` (exact resolved POST URLs `?`) | Status → error row | `button` (presence); `.alert-error` (404 case) | StatusShowTests, StatusShowNotFoundTests | docs/guide/how-to-view-status-and-errors.md |
| Live activity feed (SSE page) | `/status/activity` | Status → **Activity** | `[data-activity-status-text]`; `h1` | StatusActivityTests | docs/guide/how-to-view-status-and-errors.md |
| Activity SSE stream | `/status/activity/stream` | N/A (EventSource endpoint backing Activity page) | none in tests | — | docs/guide/how-to-view-status-and-errors.md |

### Home, auth, changelog & misc

| Feature | Route(s) | Navigate from start page | Key selectors / test ids | Functional tests | Guide |
|---|---|---|---|---|---|
| Landing page | `/` | Is the start page | `h1`; `a.btn-primary` | HomeIndexTests | docs/guide/tutorial-install.md |
| Error page (`{statusCode?}`, clamps to 500) | `/home/error/{statusCode?}` | Automatic on 404/429/500 | `h1` | HomeErrorTests | — |
| Connect AI assistant page (MCP URL + key) | `/home/connect` | Home → **MCP** in top nav | `h1`; `code`; `text=MCP server is open to everyone` | HomeConnectTests | docs/guide/tutorial-connect-ai-assistant.md |
| Login (env-based auth; GET form + POST submit) | `/auth/login` (GET + POST) | Redirect from any page when auth enabled | `form[action*='/auth/login']`; `h1` | AuthLoginTests | docs/guide/how-to-enable-authentication.md |
| Changelog (renders CHANGELOG.md) | `/changelog` | Update-available version banner → **View changelog** | `.changelog-content`; `h1` | ChangelogIndexTests | docs/guide/how-to-upgrade.md |

## MCP Tools

All tools are exposed via one route: `POST /mcp` (streamable HTTP; `tools/list` + `tools/call`), mounted by `Equibles.Mcp.Server` (docker `mcp` service). `Fixture` = xUnit fixture used by every test class below. 61 tools total.

| Tool | Method/route | Fixture | Tests |
|---|---|---|---|
| GetTopHolders | tools/call, POST /mcp | McpServerAppFixture | McpServerEndpointTests, McpServerToolCorrectnessTests, GetTopHoldersNegativeMaxResultsTests |
| GetInstitutionalOwnershipHistory | tools/call, POST /mcp | McpServerAppFixture | none |
| GetInstitutionPortfolio | tools/call, POST /mcp | McpServerAppFixture | none |
| SearchInstitutions | tools/call, POST /mcp | McpServerAppFixture | SearchInstitutionsNegativeMaxResultsTests |
| GetTopInstitutionalBuyersSellers | tools/call, POST /mcp | McpServerAppFixture | none |
| GetMostHeldStocks | tools/call, POST /mcp | McpServerAppFixture | GetMostHeldStocksNegativeMaxResultsTests |
| GetInstitutionSummary | tools/call, POST /mcp | McpServerAppFixture | none |
| GetInstitutionSectorAllocation | tools/call, POST /mcp | McpServerAppFixture | none |
| GetInstitutionQuarterlyActivity | tools/call, POST /mcp | McpServerAppFixture | none |
| CompareInstitutionPortfolios | tools/call, POST /mcp | McpServerAppFixture | none |
| GetInstitutionConsensusHoldings | tools/call, POST /mcp | McpServerAppFixture | none |
| GetInstitutionCloneBacktest | tools/call, POST /mcp | McpServerAppFixture | none |
| GetInsiderTransactions | tools/call, POST /mcp | McpServerAppFixture | McpServerToolCorrectnessTests, InsiderTransactionsNegativeMaxResultsTests |
| GetInsiderOwnership | tools/call, POST /mcp | McpServerAppFixture | none |
| SearchInsiders | tools/call, POST /mcp | McpServerAppFixture | SearchInsidersNegativeMaxResultsTests |
| GetForm144ProposedSales | tools/call, POST /mcp | McpServerAppFixture | ProposedSalesNegativeMaxResultsTests |
| GetCongressionalTrades | tools/call, POST /mcp | McpServerAppFixture | McpServerToolCorrectnessTests, CongressionalTradesNegativeMaxResultsTests |
| GetMemberTrades | tools/call, POST /mcp | McpServerAppFixture | GetMemberTradesNegativeMaxResultsTests |
| SearchCongressMembers | tools/call, POST /mcp | McpServerAppFixture | SearchCongressMembersNegativeMaxResultsTests |
| GetMemberNetWorth | tools/call, POST /mcp | McpServerAppFixture | none |
| GetEconomicIndicator | tools/call, POST /mcp | McpServerAppFixture | McpServerToolCorrectnessTests, EconomicIndicatorNegativeMaxResultsTests |
| GetLatestEconomicIndicators | tools/call, POST /mcp | McpServerAppFixture | none |
| SearchEconomicIndicators | tools/call, POST /mcp | McpServerAppFixture | SearchEconomicIndicatorsNegativeMaxResultsTests |
| GetEconomicCalendar | tools/call, POST /mcp | McpServerAppFixture | none |
| GetStockPrices | tools/call, POST /mcp | McpServerAppFixture | McpServerToolCorrectnessTests, StockPricesNegativeMaxResultsTests |
| GetLatestClosingPrices | tools/call, POST /mcp | McpServerAppFixture | none |
| GetDividendHistory | tools/call, POST /mcp | McpServerAppFixture | none |
| GetStochasticOscillator | tools/call, POST /mcp | McpServerAppFixture | none |
| GetAverageTrueRange | tools/call, POST /mcp | McpServerAppFixture | none |
| GetOnBalanceVolume | tools/call, POST /mcp | McpServerAppFixture | none |
| GetBollingerBands | tools/call, POST /mcp | McpServerAppFixture | none |
| GetShortVolume | tools/call, POST /mcp | McpServerAppFixture | ShortVolumeNegativeMaxResultsTests |
| GetShortInterest | tools/call, POST /mcp | McpServerAppFixture | ShortInterestNegativeMaxResultsTests |
| GetShortInterestSnapshot | tools/call, POST /mcp | McpServerAppFixture | ShortInterestSnapshotNegativeMaxResultsTests |
| GetLargestShortVolume | tools/call, POST /mcp | McpServerAppFixture | LargestShortVolumeNegativeMaxResultsTests |
| GetShortSqueezeScores | tools/call, POST /mcp | McpServerAppFixture | none |
| GetOffExchangeVolume | tools/call, POST /mcp | McpServerAppFixture | none |
| GetCftcPositioning | tools/call, POST /mcp | McpServerAppFixture | GetCftcPositioningNegativeMaxResultsTests |
| GetLatestCftcPositioning | tools/call, POST /mcp | McpServerAppFixture | none |
| SearchCftcMarkets | tools/call, POST /mcp | McpServerAppFixture | SearchCftcMarketsNegativeMaxResultsTests |
| GetPutCallRatios | tools/call, POST /mcp | McpServerAppFixture | PutCallRatiosNegativeMaxResultsTests |
| GetVixHistory | tools/call, POST /mcp | McpServerAppFixture | VixHistoryNegativeMaxResultsTests |
| ReadDocumentLines | tools/call, POST /mcp | McpServerAppFixture | none |
| SearchDocuments | tools/call, POST /mcp | McpServerAppFixture | none |
| SearchDocument | tools/call, POST /mcp | McpServerAppFixture | none |
| ListFilings | tools/call, POST /mcp | McpServerAppFixture | none |
| GetFailsToDeliver | tools/call, POST /mcp | McpServerAppFixture | FailsToDeliverNegativeMaxResultsTests |
| GetFormDOfferings | tools/call, POST /mcp | McpServerAppFixture | ExemptOfferingsNegativeMaxResultsTests |
| GetFundsHoldingStock | tools/call, POST /mcp | McpServerAppFixture | none |
| SearchFunds | tools/call, POST /mcp | McpServerAppFixture | none |
| GetFundProfile | tools/call, POST /mcp | McpServerAppFixture | none |
| GetFundNcenReports | tools/call, POST /mcp | McpServerAppFixture | GetFundOperationsNegativeMaxResultsTests |
| SearchInvestmentAdvisers | tools/call, POST /mcp | McpServerAppFixture | SearchInvestmentAdvisersNegativeMaxResultsTests |
| GetInvestmentAdviser | tools/call, POST /mcp | McpServerAppFixture | none |
| GetFinancialFact | tools/call, POST /mcp | McpServerAppFixture | none |
| CompareFinancialFact | tools/call, POST /mcp | McpServerAppFixture | none |
| GetFinancialStatement | tools/call, POST /mcp | McpServerAppFixture | none |
| GetRevenueBreakdown | tools/call, POST /mcp | McpServerAppFixture | none |
| GetGovernmentContracts | tools/call, POST /mcp | McpServerAppFixture | none |
| GetTopGovernmentContractors | tools/call, POST /mcp | McpServerAppFixture | none |
| GetFdaAdvisoryCommitteeMeetings | tools/call, POST /mcp | McpServerAppFixture | none |

## Selector conventions

The Playwright tests use plain **CSS locators** via `page.Locator(...)` — headings (`h1`, `h2`), row counts on `table tbody tr` / `tbody tr`, forms addressed by input `name` or element `id` (`input[name='search']`, `#financials-statement`, `select#most-held-sort`), charts by `canvas#…` or `canvas[aria-label='…']`, and semantic text (`text=CEO`, `button:has-text('Apply filters')`). Major data cards/tables carry stable kebab-case `data-testid` attributes in the Razor views (`[data-testid='holdings-top-buyers']`, `[data-testid='screener-results']`, `[data-testid='institution-summary']`, …) which tests select with attribute selectors; role-based `GetByRole(AriaRole.Link, new() { Name = "…" })` appears exactly once (AdvisersIndexTests) and `GetByTestId` is never used. **Convention for future tests:** prefer `[data-testid='<feature>-<element>']` attribute selectors (kebab-case, defined in `src/Equibles.Web/Views/**`), fall back to `table tbody tr`-style structural CSS; use `GetByRole`/`GetByTestId` only with justification.

## Gaps

- **No functional tests (web):** `/stocks/{ticker}/proposed-sales`, `/exempt-offerings`, `/fund-operations`, `/fund-holdings` tabs (guides exist; agent must discover selectors manually from `Views/Stocks/_ProposedSalesTab.cshtml` etc.); market-wide `/most-shorted` and `/short-volume` (view test ids available but unused: `[data-testid='most-shorted-table']`, `[data-testid='short-volume-table']`); `/institutions/smart-money-index` (view test ids `smart-money-*`); `/holdings/screener/export.csv` (only link-presence checked); `/institutions/search` JSON autocomplete; Status POST admin actions (only button presence asserted; full mark-as-seen/delete/deleteall flows untested); `/status/activity/stream` SSE stream itself (only page shell tested).
- **No functional tests (MCP): 37 of 61 tools** — all SEC/document/fund-directory/financial-facts/congress-net-worth/government-contracts/FDA-catalyst tools except the maxResults-negative cases: ListFilings, ReadDocumentLines, SearchDocuments, SearchDocument, SearchFunds, GetFundProfile, GetFundsHoldingStock, GetInvestmentAdviser, GetFinancialFact, CompareFinancialFact, GetFinancialStatement, GetRevenueBreakdown, GetMemberNetWorth, GetLatestEconomicIndicators, GetEconomicCalendar, GetLatestClosingPrices, GetDividendHistory, GetStochasticOscillator, GetAverageTrueRange, GetOnBalanceVolume, GetBollingerBands, GetShortSqueezeScores, GetOffExchangeVolume, GetLatestCftcPositioning, GetGovernmentContracts, GetTopGovernmentContractors, GetFdaAdvisoryCommitteeMeetings, GetInstitutionalOwnershipHistory, GetInstitutionPortfolio, GetTopInstitutionalBuyersSellers, GetInstitutionSummary, GetInstitutionSectorAllocation, GetInstitutionQuarterlyActivity, CompareInstitutionPortfolios, GetInstitutionConsensusHoldings, GetInstitutionCloneBacktest, GetInsiderOwnership. Seeded-value correctness coverage exists for only 5 tools (GetTopHolders, GetInsiderTransactions, GetCongressionalTrades, GetEconomicIndicator, GetStockPrices).
- **Selectors agent must discover manually:** test-id-free pages — `/stocks` sort controls are CSS only (`select[name='sort']`); put/call & VIX pages assert `#pcr-chart`/`#vix-chart`; Smart Money, most-shorted and short-volume test ids exist only in views (not exercised); Status POST forms use antiforgery (need the generated action URL + hidden token), exact URLs unverified (`?` above).
- **Doc drift to verify at runtime:** `/holdings/most-held` web page documented as "AI assistants only" in `docs/guide/how-to-ask-about-most-held-stocks.md` although a live web route + test exist; nav wording "Futures/Market in the top navigation" actually renders inside the **More** dropdown.