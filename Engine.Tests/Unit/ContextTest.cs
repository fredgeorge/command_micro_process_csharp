/*
 * Copyright (c) 2024 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

using Engine.Commands;
using Xunit;
using static Engine.Tests.Unit.DataLabel;

namespace Engine.Tests.Unit;

// Ensures Context manipulation works correctly
public class ContextTest {
    private readonly Context _c = new();
    public ContextTest() {
        _c[Age] = 45;
        _c[Name] = "Jennifer";
        _c[Wealth] = 0.45e6;
    }
    [Fact]
    void Access() {
        Assert.Equal(45, _c.Int(Age));
        Assert.Equal("Jennifer", _c.String(Name));
        Assert.Equal(450_000.0, _c.Double(Wealth));
    }
}

internal class DataLabel(string name) : ParameterLabel {
    internal static readonly DataLabel Age = new("Age");
    internal static readonly DataLabel Wealth = new("Wealth");
    internal static readonly DataLabel Name = new("Name");
    internal static readonly DataLabel Spouse = new("Spouse");
    public string Label => name;
}
