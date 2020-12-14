﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Core.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICall
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        ICommand Command { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string[] Arguments { get; }

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        event EventHandler? Running;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler? Ran;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RunAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
