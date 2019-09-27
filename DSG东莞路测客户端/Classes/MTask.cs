using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DSG东莞路测客户端
{
    class MTask
    {
        private TaskFlag Flag { get; set; }
        private Task Task { get; set; }

        public MTask()
        {
            Flag = new TaskFlag();
        }
        public MTask(Action action)
        {
            Flag = new TaskFlag();
            SetAction(action);
        }
        public void SetAction(Action action)
        {
            Task = new Task(action, Flag.Token);
        }
        public void SetTimeout(int ms, Action timeoutCallback)
        {

        }
        public async void Start(int timeoutInterval = 0, Action timeoutCallback = null)
        {
            if (Task == null) return;
            Task.Start();
            if (timeoutInterval > 0)
            {
                using (var timeoutCancellationTokenSource = new CancellationTokenSource())
                {
                    var completedTask = await Task.WhenAny(this.Task, Task.Delay(timeoutInterval, timeoutCancellationTokenSource.Token));
                    if (completedTask == this.Task)
                    {
                        timeoutCancellationTokenSource.Cancel();
                    }
                    else
                    {
                        timeoutCallback?.Invoke();
                    }
                }
            }

        }
        public void Cancel()
        {
            if (Flag == null) return;
            Flag.Cancel();
        }
        public bool IsCancelled()
        {
            if (Flag == null) return false;
            return this.Flag.IsCancelled();
        }
        public void WaitOne()
        {
            if (Flag == null) return;
            Flag.ResetEvent.WaitOne();
        }
        public void Pause()
        {
            if (Flag == null) return;
            Flag.ResetEvent.Reset();
        }
        public void Continue()
        {
            if (Flag == null) return;
            Flag.ResetEvent.Set();
        }

        public class TaskFlag
        {
            private CancellationTokenSource tokenSource;
            public CancellationToken Token { get; set; }

            public ManualResetEvent ResetEvent { get; set; }
            public TaskFlag()
            {
                tokenSource = new CancellationTokenSource();
                Token = tokenSource.Token;
                ResetEvent = new ManualResetEvent(true);
            }
            public void Cancel()
            {
                tokenSource.Cancel();
            }
            public bool IsCancelled()
            {
                return Token.IsCancellationRequested;
            }
            public void WaitOne()
            {
                ResetEvent.WaitOne();
            }
        }
    }
}
