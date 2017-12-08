﻿using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.Grains.Core
{
    public interface IResource<in TState, TSummary> : IStateful<TState, TSummary> where TState : ISummarizableTo<TSummary>, IExist
    {
        Task Create();
        Task<bool> Exits();
    }
    
    public interface IResource<in TState> : IStateful<TState> where TState : IExist
    {
        Task Create();
        Task<bool> Exits();
    }
}