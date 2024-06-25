/*
 * Copyright (c) 2024 by Fred George
 * May be used freely except for training; license required for training.
 * @author Fred George  fredgeorge@acm.org
 */

namespace Engine.Commands;

// Understands information to be shared amongst Tasks
public class Context {
    private readonly Dictionary<ParameterLabel, object> _parameters;

    internal Context(Dictionary<ParameterLabel, object> parameters) {
        _parameters = parameters;
    }

    internal Context() : this(new()) { }
}

// ReSharper disable once InconsistentNaming
interface ParameterLabel {
    String Name { get; }
}