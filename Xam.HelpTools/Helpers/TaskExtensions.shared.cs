using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xam.HelpTools.Helpers
{
    public static class TaskExtensions
    {
        public static async void RunAsync(this Task task, bool continueInTheSamContext, Action<Exception> OnException)
        {
            try
            {
                await task.ConfigureAwait(continueInTheSamContext);
            }
            catch (Exception e)
            {
                OnException?.Invoke(e);
            }
        }

        public static async void RunAsync(this ValueTask vt, bool continueOnTheSameContext, Action<Exception> onException)
        {
            try
            {
                await vt.ConfigureAwait(continueOnTheSameContext);
            }
            catch (Exception e)
            {
                onException?.Invoke(e);
            }
        }
    }
}
