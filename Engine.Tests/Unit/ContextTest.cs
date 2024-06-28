/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
 */

using System;
using System.Collections.Generic;
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
    public void Access() {
        Assert.Equal(45, _c.Int(Age));
        Assert.Equal("Jennifer", _c.String(Name));
        Assert.Equal(450_000.0, _c.Double(Wealth));
        Assert.Throws<KeyNotFoundException>(() => _c.String(Spouse));  // No such element in Context
        Assert.Throws<InvalidCastException>(() => _c.Double(Age));  // Attempt to extract wrong type
        Assert.Throws<InvalidCastException>(() => _c.Int(Name));  // Attempt to extract wrong type
        _c[Spouse] = "Harold";
        Assert.Equal("Harold", _c.String(Spouse));
        Assert.Throws<InvalidCastException>(() => _c.Double(Spouse));  // Attempt to extract wrong type
        Assert.Throws<InvalidOperationException>(() => _c[Age]);  // Cannot access value directly
    }

    [Fact]
    public void ExtractSubContext() {
        Context subContext = _c.Subset(Age, Name);
        Assert.Equal(45, subContext.Int(Age));
        Assert.Equal("Jennifer", subContext.String(Name));
        Assert.Throws<KeyNotFoundException>(() => subContext.Double(Wealth));  // No such element in Context
    }
}

internal class DataLabel(string name) : ParameterLabel {
    internal static readonly DataLabel Age = new("Age");
    internal static readonly DataLabel Wealth = new("Wealth");
    internal static readonly DataLabel Name = new("Name");
    internal static readonly DataLabel Spouse = new("Spouse");
    public string Label => name;
}
