# Web Asset Coverage Analyzer

The Web Asset Coverage Analyzer is an ASP.NET Core application designed to analyze the usage of CSS and JavaScript resources on websites, identifying unused assets. It provides valuable insights to web developers and performance analysts to optimize resource usage on their pages.

## Features

- Analyzes the usage of CSS and JavaScript resources from web pages.
- Reports the percentage of unused CSS and JavaScript files.
- Exports analysis results to Excel (.xlsx) format.
- Uses PuppeteerSharp for real-time page analysis via headless browser.
- Developed using the ASP.NET Core MVC architecture.

## Installation

To run this project in your local development environment, follow these steps:

1. Clone the project :https://github.com/ilaydakosar/WebCoverageAnalyzer

2. Install dependencies:
cd web-asset-coverage-analyzer
dotnet restore

3. Run the application:
dotnet run

4. Access the application by navigating to `http://localhost:5000` in your browser.

## Usage

Once the application is running, enter the URL of the web page you want to analyze and click the "Start Analysis" button. Upon completion, an Excel file containing the analysis results will be downloaded.

## Contributing

If you would like to contribute to this project, please follow these steps:

1. Fork and clone the project.
2. Create a new branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a Pull Request.


## Acknowledgments

- Thanks to the creators and contributors of the [PuppeteerSharp](https://github.com/hardkoded/puppeteer-sharp) library.
- Thanks to everyone who has contributed to the development of the [ClosedXML](https://github.com/ClosedXML/ClosedXML) library.

