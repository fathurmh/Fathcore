using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Internal;
using Xunit;

namespace Fathcore.EntityFramework.Tests.Internal
{
    public class ActionRunnerTest
    {
        [Fact]
        public void ExecuteIfNotCancelled_ExecutesAction()
        {
            var list = new List<int>();
            var runner = new ActionRunner();
            var token = new CancellationToken();

            runner.ExecuteIfNotCancelled(token, () => Modify(list));

            Assert.Single(list);
        }

        [Fact]
        public void ExecuteIfNotCancelled_ExecutesAction_ReturnResult()
        {
            var x = 5;
            var y = 3;
            var runner = new ActionRunner();
            var token = new CancellationToken();

            var result = runner.ExecuteIfNotCancelled(token, () => Add(x, y));

            Assert.Equal(x + y, result);
        }

        [Fact]
        public void ExecuteIfNotCancelled_DoesnotExecutesActionIfCancelled()
        {
            var list = new List<int>();
            var runner = new ActionRunner();
            var token = new CancellationToken(true);

            Assert.Throws<OperationCanceledException>(() => runner.ExecuteIfNotCancelled(token, () => Modify(list)));
            Assert.Empty(list);
        }

        [Fact]
        public void ExecuteIfNotCancelled_DoesnotExecutesActionIfCancelled_ReturnResult()
        {
            var x = 5;
            var y = 3;
            var result = 0;
            var runner = new ActionRunner();
            var token = new CancellationToken(true);

            Assert.Throws<OperationCanceledException>(() => result = runner.ExecuteIfNotCancelled(token, () => Add(x, y)));
            Assert.NotEqual(x + y, result);
        }

        [Fact]
        public async Task ExecuteIfNotCancelledAsync_ExecutesAction()
        {
            var list = new List<int>();
            var runner = new ActionRunner();
            var token = new CancellationToken();

            await runner.ExecuteIfNotCancelledAsync(token, () => ModifyAsync(list));

            Assert.Single(list);
        }

        [Fact]
        public async Task ExecuteIfNotCancelledAsync_ExecutesAction_ReturnResult()
        {
            var x = 5;
            var y = 3;
            var runner = new ActionRunner();
            var token = new CancellationToken();

            var result = await runner.ExecuteIfNotCancelledAsync(token, () => AddAsync(x, y));

            Assert.Equal(x + y, result);
        }

        [Fact]
        public async Task ExecuteIfNotCancelledAsync_DoesnotExecutesActionIfCancelled()
        {
            var list = new List<int>();
            var runner = new ActionRunner();
            var token = new CancellationToken(true);

            await Assert.ThrowsAsync<OperationCanceledException>(() => runner.ExecuteIfNotCancelledAsync(token, () => ModifyAsync(list)));
            Assert.Empty(list);
        }

        [Fact]
        public async Task ExecuteIfNotCancelledAsync_DoesnotExecutesActionIfCancelled_ReturnResult()
        {
            var x = 5;
            var y = 3;
            var result = 0;
            var runner = new ActionRunner();
            var token = new CancellationToken(true);

            await Assert.ThrowsAsync<OperationCanceledException>(async () => result = await runner.ExecuteIfNotCancelledAsync(token, () => AddAsync(x, y)));
            Assert.NotEqual(x + y, result);
        }

        int Add(int x, int y)
        {
            return x + y;
        }

        void Modify(List<int> list)
        {
            list.Add(list.Count);
        }

        async Task<int> AddAsync(int x, int y)
        {
            return await Task.FromResult(Add(x, y));
        }

        async Task ModifyAsync(List<int> list)
        {
            await Task.Run(() => Modify(list));
        }
    }
}
