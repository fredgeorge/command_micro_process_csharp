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

    public Context() : this(new()) { }

    public object this[ParameterLabel label] {
        get { throw new InvalidOperationException("Use Int(), String(), or Double() to extract Context values"); }
        set { _parameters[label] = value; }
    }

    public int Int(ParameterLabel label) => (int)SafeParameter(label);

    public string String(ParameterLabel label) => (string)SafeParameter(label);

    public double Double(ParameterLabel label) => (double)SafeParameter(label);

    private object SafeParameter(ParameterLabel label) => 
        _parameters[label] ?? throw new ArgumentException("No context value found for {label}");
}

// ReSharper disable once InconsistentNaming
public interface ParameterLabel {
    String Label { get; }
    static virtual ParameterLabel? LabelFor(string label) => null;
}