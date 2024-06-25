/*
 * Copyright (c) 2024 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

namespace Engine.Commands;

// Understands something that can be done (and undone)
// ReSharper disable once InconsistentNaming
public interface Command {
    ExecutionResult Execute(Context c);
    ExecutionResult Undo(Context c);
}

public enum ExecutionResult {
    NotExecuted, Succeeded, Failed, Suspended, Reversed, ReveralFailed
}

internal interface Task {
    List<ParameterLabel> ReferencedLabels { get; }
    List<ParameterLabel> UpdatedLabels { get;  }
    TaskResult execute(Context c);
}

public enum TaskResult {
    TaskSucceeded, TaskFailed, Task_Suspended
}