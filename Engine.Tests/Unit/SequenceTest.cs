/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using Engine.Commands;
using static Engine.Tests.Util.TestTaskLabel;
using static Engine.Commands.ExecutionResult;
using Xunit;

namespace Engine.Tests.Unit;

// Ensures SequenceCommands work correctly
public class SequenceTest {
    private readonly Context _c = new();

    [Fact]
    public void SingleTask() {
        var sequence = Sequence.First(SuccessfulTask).Otherwise(SuccessfulRecovery);
        Assert.Equal(Succeeded, sequence.Execute(_c));
    }
}