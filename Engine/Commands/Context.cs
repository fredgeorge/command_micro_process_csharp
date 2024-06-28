/*
 * Copyright (c) 2024 by Fred George
 * @author Fred George  fredgeorge@acm.org
 * Licensed under the MIT License; see LICENSE file in root.
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
        get => throw new InvalidOperationException("Use Int(), String(), or Double() to extract Context values");
        set => _parameters[label] = value;
    }

    public int Int(ParameterLabel label) => (int)_parameters[label];

    public string String(ParameterLabel label) => (string)_parameters[label];

    public double Double(ParameterLabel label) => (double)_parameters[label];

    internal Context Subset(List<ParameterLabel> labels) => 
        new(_parameters.Where(kvp => labels.Contains(kvp.Key)).ToDictionary());

    internal void UpdateFrom(Context subContext, List<ParameterLabel> changedLabels) => 
        changedLabels.ForEach(label => this._parameters[label] = subContext._parameters[label]);
}

// ReSharper disable once InconsistentNaming
public interface ParameterLabel {
    String Name { get; }
    static virtual ParameterLabel? LabelFor(string label) => null;
}