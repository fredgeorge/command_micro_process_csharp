/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using System.Data;

namespace Engine.Commands;

// Understands something that can be done (and undone)
// ReSharper disable once InconsistentNaming
public interface Command {
    ExecutionResult Execute(Context c);
    ExecutionResult Undo(Context c);
    void Accept(CommandVisitor visitor);
}

public enum ExecutionResult {
    NotExecuted, Succeeded, Failed, Suspended, Reversed, ReversalFailed
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

public interface CommandVisitor {
    void PreVisit(SequenceCommand command);
    void PostVisit(SequenceCommand command);
    void Visit(SimpleCommand command);
}