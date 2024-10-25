using System.Diagnostics.CodeAnalysis;
namespace RestApia.Shared.Common.Models;

public record TemplatedStringModel
{
    private readonly List<Part> _parts = new ();
    public IReadOnlyCollection<Part> Parts => _parts;

    public TemplatedStringModel()
    {
    }

    public TemplatedStringModel(string value)
    {
        _parts.Add(new Part(value));
    }

    public TemplatedStringModel(IReadOnlyCollection<Part> parts)
    {
        _parts = [..parts];
    }

    public int Length => AsString.Length;

    public override string ToString() => string.Join(string.Empty, _parts.Select(x => x.Text));
    public string AsString => ToString();
    public bool HasUnresolvedTemplates => _parts.Any(x => x.IsTemplatedVariable);

    public static implicit operator string(TemplatedStringModel templatedStringModel) => templatedStringModel.ToString();
    public static implicit operator TemplatedStringModel(string templatedString) => new () { _parts = { new Part(templatedString) } };

    public static TemplatedStringModel operator +(TemplatedStringModel templatedStringModel, string value)
    {
        if (string.IsNullOrEmpty(value)) return templatedStringModel;

        templatedStringModel._parts.Add(new Part(value));
        return templatedStringModel;
    }

    public static TemplatedStringModel operator +(TemplatedStringModel templatedStringModel, Part value)
    {
        if (string.IsNullOrEmpty(value.Text)) return templatedStringModel;

        templatedStringModel._parts.Add(value);
        return templatedStringModel;
    }

    public static TemplatedStringModel operator +(TemplatedStringModel templatedStringModel, (string, bool) value)
    {
        if (string.IsNullOrEmpty(value.Item1)) return templatedStringModel;

        templatedStringModel._parts.Add(new Part(value.Item1, value.Item2));
        return templatedStringModel;
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter")]
    public record Part(string Text, bool IsTemplatedVariable = false)
    {
        public string Text { get; private set; } = Text;
        public bool IsTemplatedVariable { get; private set; } = IsTemplatedVariable;

        public void ResolveTemplate(string content)
        {
            Text = content;
            IsTemplatedVariable = false;
        }
    }
}
