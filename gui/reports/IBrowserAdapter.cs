using Microsoft.Web.WebView2.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Castor.gui.reports
{
    /// <summary>
    /// Интерфейс адаптера для работы с разными браузерами
    /// </summary>
    public interface IBrowserAdapter
    {
        Task NavigateToStringAsync(string html);
        Task InvokeScriptAsync(string script);
        Task<object> EvaluateScriptAsync(string script);
    }

    /// <summary>
    /// Адаптер для старого WebBrowser (WPF)
    /// </summary>
    public class WebBrowserAdapter : IBrowserAdapter
    {
        private readonly WebBrowser _browser;

        public WebBrowserAdapter(WebBrowser browser)
        {
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
        }

        public async Task NavigateToStringAsync(string html)
        {
            if (_browser.Dispatcher.CheckAccess())
            {
                _browser.NavigateToString(html);
            }
            else
            {
                await _browser.Dispatcher.InvokeAsync(() =>
                {
                    _browser.NavigateToString(html);
                });
            }
        }

        public async Task InvokeScriptAsync(string script)
        {
            if (_browser.Dispatcher.CheckAccess())
            {
                _browser.InvokeScript("eval", script);
            }
            else
            {
                await _browser.Dispatcher.InvokeAsync(() =>
                {
                    _browser.InvokeScript("eval", script);
                });
            }
        }

        public async Task<object> EvaluateScriptAsync(string script)
        {
            object result = null;

            if (_browser.Dispatcher.CheckAccess())
            {
                result = _browser.InvokeScript("eval", script);
            }
            else
            {
                await _browser.Dispatcher.InvokeAsync(() =>
                {
                    result = _browser.InvokeScript("eval", script);
                });
            }

            return result;
        }
    }

    /// <summary>
    /// Адаптер для WebView2
    /// </summary>
    public class WebView2BrowserAdapter : IBrowserAdapter
    {
        private readonly WebView2 _browser;

        public WebView2BrowserAdapter(WebView2 browser)
        {
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
        }

        public async Task NavigateToStringAsync(string html)
        {
            if (_browser.CoreWebView2 == null)
                await _browser.EnsureCoreWebView2Async();

            _browser.NavigateToString(html);
        }

        public async Task InvokeScriptAsync(string script)
        {
            if (_browser.CoreWebView2 == null)
                await _browser.EnsureCoreWebView2Async();

            await _browser.CoreWebView2.ExecuteScriptAsync(script);
        }

        public async Task<object> EvaluateScriptAsync(string script)
        {
            if (_browser.CoreWebView2 == null)
                await _browser.EnsureCoreWebView2Async();

            return await _browser.CoreWebView2.ExecuteScriptAsync(script);
        }
    }
}