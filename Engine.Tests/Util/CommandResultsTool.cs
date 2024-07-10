/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using System;
using System.Collections.Generic;
using Engine.Commands;

namespace Engine.Tests.Util;

// Solicits the current state of a Command structure
internal class CommandResultsTool : CommandVisitor {
    private static readonly List<string> allStates = new()
        { "NotExecuted", "Successful", "Failure", "Suspension", "ReversalSuccess", "ReversalFailure" };
    private readonly Dictionary<string, int> _taskCounts = new();
    
    internal CommandResultsTool(Command command) {
        foreach (var state in allStates) _taskCounts[state] = 0;
        command.Accept(this);
    }
    
    public void Visit(SimpleCommand command, string state, Task primaryTask, Task reversingTask) => 
        _taskCounts[state] += 1;

    internal int this[string state] {
        get {
            if (!_taskCounts.ContainsKey(state)) throw new ArgumentException("Count for invalid state requested");
            return _taskCounts.GetValueOrDefault(state);
        }
    }

    public override string ToString() => _taskCounts.ToString();
}