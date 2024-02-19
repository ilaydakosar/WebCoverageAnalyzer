using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using PuppeteerSharp.PageCoverage;


namespace Home.Controllers
{
    public class HomeController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Analyze(string mainUrl)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            var options = new LaunchOptions { Headless = true };

            using var workbook = new XLWorkbook();
            var wsCSS = workbook.Worksheets.Add("CSS Coverage");
            var wsJS = workbook.Worksheets.Add("JS Coverage");

            wsCSS.Cell(1, 1).Value = "URL";
            wsCSS.Cell(1, 2).Value = "CSS Varlık URL";
            wsCSS.Cell(1, 3).Value = "Kullanılmayan Yüzde";

            wsJS.Cell(1, 1).Value = "URL";
            wsJS.Cell(1, 2).Value = "JS Varlık URL";
            wsJS.Cell(1, 3).Value = "Kullanılmayan Yüzde";

            int cssRow = 2, jsRow = 2;

            using (var browser = await Puppeteer.LaunchAsync(options))
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(mainUrl, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle2 } });
                    await page.EvaluateFunctionAsync(@"
                () => {
            return new Promise((resolve, reject) => {
                var totalHeight = 0;
                var distance = 100;
                var timer = setInterval(() => {
                    var scrollHeight = document.body.scrollHeight;
                    window.scrollBy(0, distance);
                    totalHeight += distance;

                    if(totalHeight >= scrollHeight){
                        clearInterval(timer);
                        resolve();
                    }
                }, 50);
            });
        }
    ");

                    await page.Coverage.StartJSCoverageAsync();
                    await page.Coverage.StartCSSCoverageAsync();

                    var cssCoverage = await page.Coverage.StopCSSCoverageAsync();
                    var jsCoverage = await page.Coverage.StopJSCoverageAsync();

                    foreach (var entry in cssCoverage)
                    {
                        var unusedPercentage = CalculateUnusedPercentage(entry);
                        if (unusedPercentage >= 97)
                        {
                            wsCSS.Cell(cssRow, 1).Value = mainUrl;
                            wsCSS.Cell(cssRow, 2).Value = entry.Url;
                            wsCSS.Cell(cssRow, 3).Value = unusedPercentage;
                            cssRow++;
                        }
                    }

                    // JS Analizi
                    foreach (var entry in jsCoverage)
                    {
                        var unusedPercentage = CalculateUnusedPercentage(entry);
                        if (unusedPercentage >= 97)
                        {
                            wsJS.Cell(jsRow, 1).Value = mainUrl;
                            wsJS.Cell(jsRow, 2).Value = entry.Url;
                            wsJS.Cell(jsRow, 3).Value = unusedPercentage;
                            jsRow++;
                        }
                    }
                }
            }

            var fileName = $"CoverageResults-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var tempFilePath = Path.GetTempPath() + fileName;
            workbook.SaveAs(tempFilePath);

            return PhysicalFile(tempFilePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private double CalculateUnusedPercentage(CoverageEntry entry)
        {
            var totalBytes = entry.Text.Length;
            var usedBytes = entry.Ranges.Sum(range => range.End - range.Start);
            var unusedBytes = totalBytes - usedBytes;
            return ((double)unusedBytes / totalBytes) * 100;
        }
    }
}

