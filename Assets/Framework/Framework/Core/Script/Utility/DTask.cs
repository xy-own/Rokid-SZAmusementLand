using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using D.Define;
using D.Utility;

namespace D
{
    public static class DTask
    {
        // cancellationToken: this.GetCancellationTokenOnDestroy()
        private static CancellationTokenSource cancellationTokenSource = null;
        public async static void YieldAction(float time, Action action)
        {
            TaskCancel();
            cancellationTokenSource = new CancellationTokenSource();
            bool cancel = await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cancellationTokenSource.Token).SuppressCancellationThrow();
            if (!cancel)
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
                action?.Invoke();
            }

        }

        // public async static void YieldAction(float time, Action action)
        // {
        //     TaskCancel();
        //     cancellationTokenSource = new CancellationTokenSource();
        //     try
        //     {
        //         await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(time), cancellationTokenSource.Token);
        //         action?.Invoke();
        //     }
        //     catch (OperationCanceledException)
        //     {
        //         Type staticClassType = typeof(DTask);
        //         Log($" {staticClassType.Namespace} 任务被取消 ", 3);
        //     }
        //     catch (Exception)
        //     {
        //     }
        // }

        public static void TaskCancel()
        {
            if (cancellationTokenSource != null)
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                    cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        public static void Log(string log, int level = 0)
        {
            Util.Log(log, (LogColor)level);
        }
    }
}