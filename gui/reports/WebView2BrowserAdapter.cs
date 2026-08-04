using Microsoft.Web.WebView2.Wpf;
using System.Threading.Tasks;

namespace Castor.gui.reports
{
    /// <summary>
    /// Адаптер для совместимости ReportCalculator с WebView2
    /// </summary>
    public class WebView2BrowserAdapter : IBrowserAdapter
    {
        private readonly WebView2 _browser;

        public WebView2BrowserAdapter(WebView2 browser)
        {
            _browser = browser;
        }

        public async Task NavigateToStringAsync(string html)
        {
            if (_browser.CoreWebView2 == null)
            {
                await _browser.EnsureCoreWebView2Async();
            }

            _browser.NavigateToString(html);
        }

        public async Task InvokeScriptAsync(string script)
        {
            if (_browser.CoreWebView2 == null)
            {
                await _browser.EnsureCoreWebView2Async();
            }

            await _browser.CoreWebView2.ExecuteScriptAsync(script);
        }

        public async Task<object> EvaluateScriptAsync(string script)
        {
            if (_browser.CoreWebView2 == null)
            {
                await _browser.EnsureCoreWebView2Async();
            }

            var result = await _browser.CoreWebView2.ExecuteScriptAsync(script);
            return result;
        }
    }
}