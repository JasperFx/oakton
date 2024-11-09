using System.Collections.Generic;
using System.Reflection;
using JasperFx.Core.Reflection;

namespace Oakton.Parsing;

#nullable disable annotations // FIXME

public abstract class TokenHandlerBase : ITokenHandler
{
    protected TokenHandlerBase(MemberInfo member)
    {
        Member = member;
    }

    public MemberInfo Member { get; }

    public string Description
    {
        get
        {
            var name = Member.Name;
            Member.ForAttribute<DescriptionAttribute>(att => name = att.Description);

            return name;
        }
    }

    public string MemberName => Member.Name;

    public abstract bool Handle(object input, Queue<string> tokens);
    public abstract string ToUsageDescription();

    protected void setValue(object target, object value)
    {
        (Member as PropertyInfo)?.SetValue(target, value);
        (Member as FieldInfo)?.SetValue(target, value);
    }

    protected object getValue(object target)
    {
        return (Member as PropertyInfo)?.GetValue(target)
               ?? (Member as FieldInfo)?.GetValue(target);
    }

    public bool Equals(TokenHandlerBase other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other.Member.Equals(Member);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != typeof(TokenHandlerBase))
        {
            return false;
        }

        return Equals((TokenHandlerBase)obj);
    }

    public override int GetHashCode()
    {
        return Member != null ? Member.GetHashCode() : 0;
    }
}