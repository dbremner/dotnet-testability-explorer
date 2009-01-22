using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Thinklouder.Testability.Metrics
{
    public interface WhiteList
    {
        bool IsClassWhiteListed(string className);
    }

    public class RegExpWhiteList : WhiteList
    {
        class Predicate
        {
            private readonly System.Text.RegularExpressions.Regex pattern;

            public Predicate(string regExp)
            {
                pattern = new Regex(regExp);
            }

            public virtual bool IsClassWhitelisted(string className)
            {
                return pattern.IsMatch(className);
            }
        }

        class NotPredicate : Predicate
        {

            public NotPredicate(string regExp)
                : base(regExp)
            {
            }

            public override bool IsClassWhitelisted(string className)
            {
                return !base.IsClassWhitelisted(className);
            }

        }

        private readonly List<Predicate> patterns = new List<Predicate>();

        public RegExpWhiteList(params string[] regexps)
        {
            foreach ( string regExp in regexps )
            {
                AddPackage(regExp);
            }
        }

        public bool IsClassWhiteListed(String className)
        {
            foreach ( Predicate predicate in this.patterns )
            {
                if ( predicate.IsClassWhitelisted(className) )
                {
                    return true;
                }
            }
            return false;
        }

        public void AddPackage(String regExp)
        {
            if ( regExp.StartsWith("!") )
            {
                patterns.Add(new NotPredicate(regExp.Substring(1)));
            }
            else
            {
                patterns.Add(new Predicate(regExp));
            }
        }
    }
}