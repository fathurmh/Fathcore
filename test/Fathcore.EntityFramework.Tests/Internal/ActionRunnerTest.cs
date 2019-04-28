using System;
using System.Threading;
using Fathcore.EntityFramework.Internal;
using Xunit;

namespace Fathcore.EntityFramework.Tests.Internal
{
    public class ActionRunnerTest
    {
        [Fact]
        public void ActionRunner_ExecutesAction()
        {
            bool run = false;
            var runner = new ActionRunner();
            var token = new CancellationToken();

            runner.ExecuteIfNotCancelled(token, () => run = true);

            Assert.True(run);
        }

        [Fact]
        public void ActionRunner_DoesnotExecutesActionIfCancelled()
        {
            bool run = false;
            var runner = new ActionRunner();
            var token = new CancellationToken(true);

            Assert.Throws<OperationCanceledException>(() => runner.ExecuteIfNotCancelled(token, () => run = true));
            Assert.False(run);
        }
    }
}
