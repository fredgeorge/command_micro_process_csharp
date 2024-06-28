/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
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

// ReSharper disable once InconsistentNaming
public interface Task {
    List<ParameterLabel> ReferencedLabels { get; }
    List<ParameterLabel> UpdatedLabels { get;  }
    TaskResult Execute(Context c);
}

public enum TaskResult {
    TaskSucceeded, TaskFailed, TaskSuspended
}