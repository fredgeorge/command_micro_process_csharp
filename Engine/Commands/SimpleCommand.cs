/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using static Engine.Commands.ExecutionResult;
using static Engine.Commands.TaskResult;
namespace Engine.Commands;

// Understands something that can be done (and undone)
public class SimpleCommand : Command {
    private readonly Task _primaryTask;
    private readonly Task _reversingTask;

    public SimpleCommand(Task primaryTask, Task reversingTask) {
        _primaryTask = primaryTask;
        _reversingTask = reversingTask;
    }

    public ExecutionResult Execute(Context c) {
        var subContext = c.Subset(_primaryTask.ReferencedLabels);
        var taskResult = _primaryTask.Execute(subContext);
        c.UpdateFrom(subContext, _primaryTask.UpdatedLabels);
        return taskResult switch {
            TaskSucceeded => Succeeded,
            TaskFailed => Failed,
            TaskSuspended => Suspended,
            _ => throw new NotImplementedException("Unexpected TaskResult value")
        };
    }

    public ExecutionResult Undo(Context c) {
        throw new NotImplementedException();
    }
}