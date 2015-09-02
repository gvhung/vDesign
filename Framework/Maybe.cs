using System;

namespace Framework.Maybe
{
    public static class MaybeExtesions
    {
        public static TResult With<TInput, TResult>(this TInput input, Func<TInput, TResult> evaluator)
            where TInput : class 
            where TResult : class
        {
            if (input == null) return null;

            return evaluator(input);
        }

        public static TResult? With<TInput, TResult>(this TInput input, Func<TInput, TResult?> evaluator)
            where TInput : class
            where TResult : struct 
        {
            if (input == null) return null;

            return evaluator(input);
        }

        public static TResult WithStruct<TInput, TResult>(this TInput input, Func<TInput, TResult> evaluator, TResult defValue)
            where TInput : class 
            where TResult : struct 
        {
            if (input == null) return defValue;

            return evaluator(input);
        }


        public static TResult With<TInput, TResult>(this TInput input, Func<TInput, TResult> evaluator, TResult defaultResult)
            where TInput : class
            where TResult : class
        {
            if (input == null) return defaultResult;

            return evaluator(input);
        }

        public static TResult ReturnOrDefault<TInput, TResult>(this TInput input, Func<TInput, TResult> evaluator)
            where TInput : class
        {
            if (input == null) return default(TResult);

            return evaluator(input);
        }

        public static TResult ReturnOrDefault<TInput, TResult>(this TInput input, Func<TInput, TResult> evaluator, TResult def)
            where TInput : class
        {
            if (input == null) return def;

            return evaluator(input);
        }

        public static bool ReturnSuccess<TInput>(this TInput input)
            where TInput : class
        {
            return input != null;
        }

        public static TResult IfNotNullReturn<TInput, TResult>(this TInput input, Func<TInput, TResult> func)
            where TInput : class
            where TResult : class 
        {
            if (input != null) return func(input);

            return null;
        }

        public static TInput IfNotNull<TInput>(this TInput input, Action<TInput> action)
            where TInput : class
        {
            if (input != null) action(input);

            return input;
        }

        public static TOutput IfTrue<TOutput>(this bool input, Func<TOutput> func)
            where TOutput : class
        {
            if (input)
                return func();

            return null;
        }
    }
}
