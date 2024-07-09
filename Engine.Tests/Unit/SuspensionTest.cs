/*
 * Copyright (c) 2024 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using Engine.Commands;
using static Engine.Tests.Util.TestTaskBuilder;
using static Engine.Commands.ExecutionResult;
using Xunit;

namespace Engine.Tests.Unit;

// Ensures suspended commands work correctly
public class SuspensionTest {
    private readonly Context _c = new();
    
    [Fact]
    public void FirstTaskSuspended() {
        var sequence = Sequence
            .First(SuspendedTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Suspended, sequence.Execute(_c));
    }
    
    [Fact]
    public void LastTaskSuspendedOnce() {
        var sequence = Sequence
            .First(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(SuccessfulTask).Otherwise(SuccessfulRecovery)
            .Then(SuspendedTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Suspended, sequence.Execute(_c));
        Assert.Equal(Succeeded, sequence.Execute(_c)); // Second execution should be successful
    }
}