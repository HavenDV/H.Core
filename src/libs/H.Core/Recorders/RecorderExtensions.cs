﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Core.Recorders
{
    /// <summary>
    /// 
    /// </summary>
    public static class RecorderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recorder"></param>
        /// <param name="timeout"></param>
        /// <param name="format"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IRecording> StartWithTimeoutAsync(
            this IRecorder recorder, 
            TimeSpan timeout,
            AudioFormat format,
            CancellationToken cancellationToken = default)
        {
            recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
            if (format is AudioFormat.None)
            {
                throw new ArgumentException("format is None.");
            }
            
            using var recording = await recorder.StartAsync(format, cancellationToken).ConfigureAwait(false);

            await Task.Delay(timeout, cancellationToken).ConfigureAwait(false);

            await recording.StopAsync(cancellationToken).ConfigureAwait(false);

            return recording;
        }
    }
}
