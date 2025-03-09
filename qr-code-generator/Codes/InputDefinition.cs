using System.Collections.Generic;

public class InputDefinition
{
    public string Name { get; set; }
    public InputType Type { get; set; }
    public string Placeholder { get; set; }
    public string Description { get; set; }
    public List<ValidationRule> ValidationRules { get; set; } = new List<ValidationRule>();
}

public enum InputType
{
    String,
    Checkbox,
    Dropdown,
    Toggle
}

public class ValidationRule
{
    public string Rule { get; set; }
    public string ErrorMessage { get; set; }
}