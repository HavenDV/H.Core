﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core.Recorders;
using H.Core.Utilities;

namespace H.Core.Recognizers
{
    /// <summary>
    /// 
    /// </summary>
    public static class StreamRecognitionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recognition"></param>
        /// <param name="recording"></param>
        /// <param name="exceptionsBag"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static async Task BindRecordingAsync(
            this IStreamingRecognition recognition, 
            IRecording recording,
            ExceptionsBag? exceptionsBag = null,
            CancellationToken cancellationToken = default)
        {
            recognition = recognition ?? throw new ArgumentNullException(nameof(recognition));
            recording = recording ?? throw new ArgumentNullException(nameof(recording));

            if (recording.Format is RecordingFormat.None)
            {
                throw new ArgumentException("recording.Format is None.");
            }
            if (recording.Format is not RecordingFormat.Raw)
            {
                if (!recording.Header.Any())
                {
                    throw new ArgumentException("recording.Header is empty.");
                }

                await recognition.WriteAsync(recording.Header, cancellationToken).ConfigureAwait(false);
            }

            if (recording.Data.Any())
            {
                await recognition.WriteAsync(recording.Data, cancellationToken).ConfigureAwait(false);
            }

            async void OnDataReceived(object? _, byte[] bytes)
            {
                try
                {
                    await recognition.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    exceptionsBag?.OnOccurred(exception);
                }
            }

            void OnStopped(object? o, EventArgs eventArgs)
            {
                try
                {
                    recording.DataReceived -= OnDataReceived;
                    recording.Stopped -= OnStopped;
                }
                catch (Exception exception)
                {
                    exceptionsBag?.OnOccurred(exception);
                }
            }

            recording.DataReceived += OnDataReceived;
            recording.Stopped += OnStopped;
        }

        /// <summary>
        /// Dispose is required!.
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="recorder"></param>
        /// <param name="exceptionsBag"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task<IStreamingRecognition> StartStreamingRecognitionAsync(
            this IRecognizer recognizer, 
            IRecorder recorder,
            ExceptionsBag? exceptionsBag = null,
            CancellationToken cancellationToken = default)
        {
            recognizer = recognizer ?? throw new ArgumentNullException(nameof(recognizer));
            recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));

            if (recognizer.StreamingFormat is RecordingFormat.None)
            {
                throw new ArgumentException("Recognizer does not support streaming recognition.");
            }
            
            var recording = await recorder.StartAsync(recognizer.StreamingFormat, cancellationToken)
                .ConfigureAwait(false);
            var recognition = await recognizer.StartStreamingRecognitionAsync(cancellationToken)
                .ConfigureAwait(false);
            recognition.Stopping += async (_, _) =>
            {
                try
                {
                    await recording.StopAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    exceptionsBag?.OnOccurred(exception);
                }
                finally
                {
                    recording.Dispose();
                }
            };

            await recognition.BindRecordingAsync(recording, exceptionsBag, cancellationToken).ConfigureAwait(false);

            return recognition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="bytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> ConvertOverStreamingRecognition(
            this IRecognizer recognizer,
            byte[] bytes, 
            CancellationToken cancellationToken = default)
        {
            recognizer = recognizer ?? throw new ArgumentNullException(nameof(recognizer));
            bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));

            using var recognition = await recognizer.StartStreamingRecognitionAsync(cancellationToken)
                .ConfigureAwait(false);
            
            var response = string.Empty;
            recognition.FinalResultsReceived += (_, value) => response = value;

            await recognition.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            await recognition.StopAsync(cancellationToken).ConfigureAwait(false);

            return response;
        }
    }
}
