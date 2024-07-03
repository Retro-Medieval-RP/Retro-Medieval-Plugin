using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroMedieval.Utils;

public class Switcher<T, T2>
{
    private List<Decision> Cases { get; set; } = [];

    public Switcher<T, T2> Case(Predicate<T> condition, Action<T2> branch)
    {
        var @case = new Decision { Condition = condition, Branch = branch };

        Cases.Add(@case);
        return this;
    }

    public void Default(Action<T2> branch) =>
        Case(x => true, branch);

    public void Switch(T toSwitch, T2 instance) =>
        Switch(toSwitch, instance, Cases);

    public void Switch(T toSwitch, T2 instance, params Decision[] cases) =>
        Switch(toSwitch, instance, cases.ToList());

    public void Switch(T toSwitch, T2 instance, IEnumerable<Decision> cases)
    {
        foreach (var @case in cases.Where(@case => @case.Condition(toSwitch)))
        {
            @case.Branch(instance);
            break;
        }
    }

    public class Decision
    {
        public Predicate<T> Condition { get; set; }
        public Action<T2> Branch { get; set; }
    }
}