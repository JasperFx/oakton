using System.Collections.Generic;
using System.Reflection;
using Baseline.Reflection;

namespace Oakton.Parsing
{
    public abstract class TokenHandlerBase : ITokenHandler
    {
        private readonly MemberInfo _member;

        protected TokenHandlerBase(MemberInfo member)
        {
            _member = member;
        }

        protected void setValue(object target, object value)
        {
            (_member as PropertyInfo)?.SetValue(target, value);
            (_member as FieldInfo)?.SetValue(target, value);
        }

        public string Description
        {
            get
            {
                var name = _member.Name;
                _member.ForAttribute<DescriptionAttribute>(att => name = att.Description);

                return name;
            }
        }

        public string MemberName => _member.Name;

        public abstract bool Handle(object input, Queue<string> tokens);
        public abstract string ToUsageDescription();

        public bool Equals(TokenHandlerBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._member.Equals(_member);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TokenHandlerBase)) return false;
            return Equals((TokenHandlerBase) obj);
        }

        public override int GetHashCode()
        {
            return (_member != null ? _member.GetHashCode() : 0);
        }
    }
}