/*
 * Copyright (c) 2024 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using static Engine.Commands.ExecutionResult;

namespace Engine.Commands;

// Understands a sequential series of Commands
public class SequenceCommand : Command {
    private readonly List<Command> _commands = new();

    internal SequenceCommand() { }

    public ExecutionResult Execute(Context c) => Execute(c, _commands.ToList());

    // Tail recursion on the sequence of Commands
    private ExecutionResult Execute(Context c, List<Command> commands) {
        if (commands.Count == 0) return Succeeded;
        var currentCommand = commands.First();
        return currentCommand.Execute(c) switch {
            NotExecuted => throw new Exception("Invalid result of NotExecuted for current Command"),
            Succeeded =>
                Execute(c, commands.Skip(1).ToList()) switch {
                    NotExecuted => throw new Exception("Invalid result of NotExecuted for remaining Commands"),
                    Succeeded => Succeeded,
                    Failed => currentCommand.Undo(c),
                    Suspended => Suspended,
                    Reversed => currentCommand.Undo(c),
                    ReversalFailed => ChildReversalFailed(currentCommand),
                    _ => throw new Exception("Unexpected flow from Command execution")
                },
            Failed => Failed,
            Suspended => Suspended,
            Reversed => currentCommand.Undo(c),
            ReversalFailed => ReversalFailed,
            _ => throw new Exception("Unexpected flow from Command execution")
        };
    }

    public ExecutionResult Undo(Context c) {
        throw new NotImplementedException();
    }
    
    public void Accept(CommandVisitor visitor) {
        visitor.PreVisit(this, _commands.Count);
        _commands.ForEach(command => command.Accept(visitor));
        visitor.PostVisit(this);
    }

    internal void Add(Command command) => _commands.Add(command);
    
    private static ExecutionResult ChildReversalFailed(Command command) {
        command.Undo(new Context());
        return ReversalFailed;
    }

    // DSL support
    public SimpleCommandBuilder Then(TaskBuilder taskBuilder) =>
        new(taskBuilder.Task(), this);
}

// DSL syntactic sugar to kick off SequenceCommand creation
public static class Sequence {
    public static SimpleCommandBuilder First(TaskBuilder taskBuilder) =>
        new(taskBuilder.Task(), new SequenceCommand());
}

// DSL support class; defers creation of SimpleCommand until all build parameters are known
public class SimpleCommandBuilder {
    private readonly Task _task;
    private readonly SequenceCommand _sequence;

    internal SimpleCommandBuilder(Task task, SequenceCommand sequence) {
        _task = task;
        _sequence = sequence;
    }

    public SequenceCommand Otherwise(TaskBuilder taskBuilder) {
        _sequence.Add(new SimpleCommand(_task, taskBuilder.Task()));
        return _sequence;
    }
}

// ReSharper disable once InconsistentNaming
public interface TaskBuilder {
    Task Task();
}