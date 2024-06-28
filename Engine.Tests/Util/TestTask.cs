/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Xunit;
using static Engine.Commands.TaskResult;
using static Engine.Tests.Util.TestParameterLabel;

namespace Engine.Tests.Util;

// Understands a flexible Task for assertions
internal class TestTask : Task {
    private readonly TaskResult _result;
    private readonly List<ParameterLabel> _referencedLabels;
    private readonly List<ParameterLabel> _updatedLabels;

    internal TestTask(TaskResult result, List<ParameterLabel> referencedLabels, List<ParameterLabel> updatedLabels) {
        _result = result;
        _referencedLabels = referencedLabels;
        _updatedLabels = updatedLabels;
    }
    
    internal TestTask(TaskResult result) : this(result, new(), new()) { }

    public List<ParameterLabel> ReferencedLabels => _referencedLabels.ToList();
    
    public List<ParameterLabel> UpdatedLabels => _updatedLabels.ToList();
    
    public TaskResult Execute(Context c) {
        ReferencedLabels.ForEach(label => Assert.Equal(label.Name, c.String(label)));
        TestParameterLabel.AllLabels.ForEach(label => c[label] = label.Name + "+");
        return _result;
    }
}

internal class CrashingTask: Task {
    internal static CrashingTask Instance = new();
    public List<ParameterLabel> ReferencedLabels => new();
    public List<ParameterLabel> UpdatedLabels => new();
    public TaskResult Execute(Context c) => throw new Exception("Deliberate crash for testing purposes");
}

internal class TestTaskLabel : TaskBuilder {
    internal static readonly TestTaskLabel SuccessfulTask = new(() => new TestTask(TaskSucceeded));
    internal static readonly TestTaskLabel SuccessfulRecovery = new(() => new TestTask(TaskSucceeded));
    internal static readonly TestTaskLabel FailedTask = new(() => new TestTask(TaskFailed));
    internal static readonly TestTaskLabel FailedRecovery = new(() => new TestTask(TaskFailed));
    internal static readonly TestTaskLabel SuspendedTask = new(() => new TestTask(TaskSuspended));
    internal static readonly TestTaskLabel CrashedTask = new(() => CrashingTask.Instance);
    internal static readonly TestTaskLabel ReadAbCwriteD = new(() => 
        new TestTask(TaskSucceeded, new List<ParameterLabel>{A, B, C}, new List<ParameterLabel>{D}));
    internal static readonly TestTaskLabel ReadAbCwriteBd = new(() => 
        new TestTask(TaskSucceeded, new List<ParameterLabel>{A, B, C}, new List<ParameterLabel>{B, D}));
    
    private readonly Func<Task> _taskBuilder;

    private TestTaskLabel(Func<Task> taskBuilder) {
        _taskBuilder = taskBuilder;
    }
    
    public Task Task() => _taskBuilder();
}

internal class TestParameterLabel : ParameterLabel {

    internal static List<TestParameterLabel> AllLabels = new();
    
    internal static TestParameterLabel A = new("A");
    internal static TestParameterLabel B = new("B");
    internal static TestParameterLabel C = new("C");
    internal static TestParameterLabel D = new("D");
    internal static TestParameterLabel E = new("E");

    public string Name { get; }
    
    private TestParameterLabel(string name) {
        AllLabels.Add(this);
        Name = name;
    }
}