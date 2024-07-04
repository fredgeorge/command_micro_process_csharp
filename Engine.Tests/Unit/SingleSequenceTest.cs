/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using Engine.Commands;
using static Engine.Tests.Util.TestTaskBuilder;
using static Engine.Commands.ExecutionResult;
using Xunit;

namespace Engine.Tests.Unit;

// Ensures SequenceCommands work correctly
public class SingleSequenceTest {
    private readonly Context _c = new();

    [Fact]
    public void SingleSuccessfulTask() {
        var sequence = Sequence.First(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Succeeded, sequence.Execute(_c));
    }

    [Fact]
    public void SingleFailingTask() {
        var sequence = Sequence.First(FailedTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Failed, sequence.Execute(_c));
    }

    [Fact]
    public void MultipleSuccessfulTask() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Succeeded, sequence.Execute(_c));
    }

    [Fact]
    public void FailingMiddleTask() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(FailedTask).Otherwise(SuccessfulRecovery)
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Reversed, sequence.Execute(_c));
    }

    [Fact]
    public void FailingReversal() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(SuccessfulTask).Otherwise(FailedRecovery)
            .Then(FailedTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(ReversalFailed, sequence.Execute(_c));
    }

    [Fact]
    public void CrashingTask() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(CrashedTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Reversed, sequence.Execute(_c));
    }
}