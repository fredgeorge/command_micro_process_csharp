/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using Engine.Commands;
using Engine.Tests.Util;
using Xunit;
using static Engine.Tests.Util.TestTaskBuilder;
using static Engine.Commands.ExecutionResult;

namespace Engine.Tests.Unit;

// Ensures sequences of Sequences work correctly
public class CompoundSequenceTest {
    private readonly Context _c = new();
    private CommandResultsTool _analysis = null!;

    [Fact]
    public void EmbeddedSequence() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(Sequence
                .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
                .Then(SuccessfulTask).Otherwise(SuccessfulRecovery))
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Succeeded, sequence.Execute(_c));
        _analysis = new CommandResultsTool(sequence);
        Assert.Equal(4, _analysis["Successful"]);
    }

    [Fact]
    public void ReferencedSequence() {
        var subsequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery);
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(subsequence)
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Succeeded, sequence.Execute(_c));
        _analysis = new CommandResultsTool(sequence);
        Assert.Equal(4, _analysis["Successful"]);
    }

    [Fact]
    public void FailingSubSequence() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(Sequence
                .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
                .Then(FailedTask).Otherwise(SuccessfulRecovery)
                .Then(SuccessfulTask).Otherwise(SuccessfulRecovery))
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Reversed, sequence.Execute(_c));
        _analysis = new CommandResultsTool(sequence);
        Assert.Equal(0, _analysis["Successful"]);
        Assert.Equal(1, _analysis["Failure"]);
        Assert.Equal(2, _analysis["NotExecuted"]);
        Assert.Equal(2, _analysis["ReversalSuccess"]);
    }

    [Fact]
    public void FailingReversalSubSequence() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(Sequence
                .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
                .Then(SuccessfulTask).Otherwise(FailedRecovery)
                .Then(FailedTask).Otherwise(SuccessfulRecovery))
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(ReversalFailed, sequence.Execute(_c));
        _analysis = new CommandResultsTool(sequence);
        Assert.Equal(0, _analysis["Successful"]);
        Assert.Equal(1, _analysis["Failure"]);
        Assert.Equal(1, _analysis["NotExecuted"]);
        Assert.Equal(2, _analysis["ReversalSuccess"]);
        Assert.Equal(1, _analysis["ReversalFailure"]);
    }
}