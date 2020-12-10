﻿using System.Threading;
using System.Threading.Tasks;

namespace H.Core.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConverter : IModule
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsStreamingRecognitionSupported { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> ConvertAsync(byte[] bytes, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IStreamingRecognition> StartStreamingRecognitionAsync(CancellationToken cancellationToken = default);
    }
}
