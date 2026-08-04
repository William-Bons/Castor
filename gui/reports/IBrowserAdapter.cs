using System.Threading.Tasks;

namespace Castor.gui.reports
{
    public interface IBrowserAdapter
    {
        Task NavigateToStringAsync(string html);
        Task InvokeScriptAsync(string script);
        Task<object> EvaluateScriptAsync(string script);
    }
}