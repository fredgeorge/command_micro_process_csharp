/*
 * Copyright (c) 2024 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using System;
using Engine.Commands;
using Engine.Tests.Util;
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
        var analysis = new CommandResultsTool(sequence);
        Assert.Equal(1, analysis["Suspension"]);
        Assert.Equal(2, analysis["Successful"]);
        Assert.Equal(Succeeded, sequence.Execute(_c)); // Second execution should be successful
        analysis = new CommandResultsTool(sequence);
        Assert.Equal(0, analysis["Suspension"]);
        Assert.Equal(3, analysis["Successful"]);
    }
}