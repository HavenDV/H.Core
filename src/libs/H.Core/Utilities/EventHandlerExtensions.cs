﻿//Copyright (C) 2017 Pedro Lamas, http://www.pedrolamas.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace H.Core.Utilities
{
    public static class EventHandlerExtensions
    {
        private static readonly Task CompletedTask = Task.FromResult(0);

        public static Task InvokeAsync<T>(this EventHandler<T> eventHandler, object sender, T eventArgs)
            where T : DeferredEventArgs
        {
            return InvokeAsync(eventHandler, sender, eventArgs, CancellationToken.None);
        }

        public static Task InvokeAsync<T>(this EventHandler<T> eventHandler, object sender, T eventArgs, CancellationToken cancellationToken)
            where T : DeferredEventArgs
        {
            if (eventHandler == null)
            {
                return CompletedTask;
            }

            var tasks = eventHandler.GetInvocationList()
                .OfType<EventHandler<T>>()
                .Select(invocationDelegate =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    invocationDelegate(sender, eventArgs);

                    var deferral = eventArgs.GetCurrentDeferralAndReset();

                    return deferral?.WaitForCompletion(cancellationToken) ?? CompletedTask;
                })
                .ToArray();

            return Task.WhenAll(tasks);
        }
    }
}