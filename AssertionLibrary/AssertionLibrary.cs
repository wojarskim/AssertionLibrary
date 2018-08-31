using System;
using System.Linq.Expressions;

namespace AssertionLibrary
{
    public class ExpectationFailedExceptin : Exception { }

    public static class AssertionLibrary
    {
        public static Action<Predicate<TSubject>> Expect<TSubject>(this TSubject subject)
        {
            return (Predicate<TSubject> predicate) =>
            {
                if (!predicate.Invoke(subject))
                {
                    throw new ExpectationFailedExceptin();
                }
            };
        }

        public static void Eq<TSubject>(this Action<Predicate<TSubject>> assertion, object other)
        {
            assertion.Invoke((subject) =>
            {
                return subject.Equals(other);
            });
        }

        public static void IsGreater<TSubject>(this Action<Predicate<TSubject>> assertion, TSubject other) where TSubject : IComparable
        {
            assertion.Invoke((subject) =>
            {
                return subject.CompareTo(other) > 0;
            });
        }

        public static Action<Predicate<TSubject>> Not<TSubject>(this Action<Predicate<TSubject>> assertion)
        {
            return (Predicate<TSubject> predicate) =>
            {
                assertion.Invoke((subject) =>
                {
                    return !predicate.Invoke(subject);
                });
            };
        }

        public static void RaiseError<TSubject>(this Action<Predicate<TSubject>> assertion) 
        {
            assertion.Invoke((subject) =>
            {
                try
                {
                    (subject as Action)?.Invoke();
                }
                catch
                {
                    return true;
                }
                return false;
            });
        }

        public static Action<Predicate<AssertionSubjectProperties>> Properties<TSubject>(this Action<Predicate<TSubject>> assertion)
        {
            return (Predicate<AssertionSubjectProperties> predicate) =>
            {
                assertion.Invoke((subject) =>
                {
                    var newSubject = new AssertionSubjectProperties(subject);
                    return predicate.Invoke(newSubject);
                });
            };
        }

        public static Action<Predicate<AssertionSubjectProperties>> PropertiesWithout<TSubject, TResult>(this Action<Predicate<TSubject>> assertion, Expression<Func<TSubject, TResult>> excluder)
        {
            return (Predicate<AssertionSubjectProperties> predicate) =>
            {
                assertion.Invoke((subject) =>
                {
                    var excludedPropertyName = (excluder.Body as MemberExpression)?.Member?.Name;
                    var newSubject = new AssertionSubjectProperties(subject, excludedPropertyName);
                    return predicate.Invoke(newSubject);
                });
            };
        }
    }
}