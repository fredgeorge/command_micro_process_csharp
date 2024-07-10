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
    private State _state = NotExecuted.Instance;

    public SimpleCommand(Task primaryTask, Task reversingTask) {
        _primaryTask = primaryTask;
        _reversingTask = reversingTask;
    }

    public ExecutionResult Execute(Context c) => _state.Execute(c, this);

    public ExecutionResult Undo(Context c) => _state.Undo(c, this);

    private ExecutionResult ExecuteTask(Context c) {
        try {
            var subContext = c.Subset(_primaryTask.ReferencedLabels);
            var taskResult = _primaryTask.Execute(subContext);
            c.UpdateFrom(subContext, _primaryTask.UpdatedLabels);
            _state = taskResult switch {
                TaskSucceeded => Successful.Instance,
                TaskFailed => Failure.Instance,
                TaskSuspended => Suspension.Instance,
                _ => throw new NotImplementedException("Unexpected TaskResult value")
            };
            return _state.Result;
        }
        catch (Exception) {
            _state = Failure.Instance;
            return Failed;
        }
    }

    private ExecutionResult UndoTask(Context c) {
        try {
            var subContext = c.Subset(_reversingTask.ReferencedLabels);
            var taskResult = _reversingTask.Execute(subContext);
            c.UpdateFrom(subContext, _reversingTask.UpdatedLabels);
            _state = taskResult switch {
                TaskSucceeded => ReversalSuccess.Instance,
                TaskFailed => ReversalFailure.Instance,
                TaskSuspended => ReversalFailure.Instance, // Cannot suspend a reversal
                _ => throw new NotImplementedException("Unexpected reversal TaskResult value")
            };
            return _state.Result;
        }
        catch (Exception) {
            _state = ReversalFailure.Instance;
            return ReversalFailed;
        }
    }

    public void Accept(CommandVisitor visitor) {
        var stateClass = _state.GetType().ToString();
         var state = stateClass.Substring(stateClass.IndexOf("+") + 1);
        visitor.Visit(this, state, _primaryTask, _reversingTask);
    }

    private interface State {
        ExecutionResult Result { get; }
        ExecutionResult Execute(Context c, SimpleCommand command);
        ExecutionResult Undo(Context c, SimpleCommand command);
    }
    
    private class NotExecuted : State {
        internal static readonly NotExecuted Instance = new();

        public ExecutionResult Result => 
            throw new InvalidOperationException("Cannot get result of a Command that was never executed");

        public ExecutionResult Execute(Context c, SimpleCommand command) => command.ExecuteTask(c);

        public ExecutionResult Undo(Context c, SimpleCommand command) => 
            throw new Exception("Cannot undo a Command that was never executed");
    }
    
    private class Successful : State {
        internal static readonly Successful Instance = new();

        public ExecutionResult Result => Succeeded;

        public ExecutionResult Execute(Context c, SimpleCommand command) => Succeeded;

        public ExecutionResult Undo(Context c, SimpleCommand command) => command.UndoTask(c);
    }
    
    private class Failure : State {
        internal static readonly Failure Instance = new();

        public ExecutionResult Result => Failed;

        public ExecutionResult Execute(Context c, SimpleCommand command) => Failed;

        public ExecutionResult Undo(Context c, SimpleCommand command) => 
            throw new Exception("Cannot undo a Command that failed");
    }
    
    private class Suspension : State {
        internal static readonly Suspension Instance = new();

        public ExecutionResult Result => Suspended;

        public ExecutionResult Execute(Context c, SimpleCommand command) => command.ExecuteTask(c);

        public ExecutionResult Undo(Context c, SimpleCommand command) => 
            throw new Exception("Cannot undo a Command that was suspended");
    }
    
    private class ReversalSuccess : State {
        internal static readonly ReversalSuccess Instance = new();

        public ExecutionResult Result => Reversed;

        public ExecutionResult Execute(Context c, SimpleCommand command) => Failed; // Cannot re-execute a Command

        public ExecutionResult Undo(Context c, SimpleCommand command) => Reversed; // Already reversed succesfully
    }
    
    private class ReversalFailure : State {
        internal static readonly ReversalFailure Instance = new();

        public ExecutionResult Result => ReversalFailed;

        public ExecutionResult Execute(Context c, SimpleCommand command) => Failed; // Cannot re-execute a Command

        public ExecutionResult Undo(Context c, SimpleCommand command) => ReversalFailed; // Already unsuccessful
    }
}