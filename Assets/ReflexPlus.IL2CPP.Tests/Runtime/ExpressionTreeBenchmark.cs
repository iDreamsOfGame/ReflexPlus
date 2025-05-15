using System;
using UnityEngine;
using Domain.Generics;
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;

// ReSharper disable UnusedVariable

internal class ExpressionTreeBenchmark : MonoBehaviour
{
    private const int Iterations = 10000;

    private const int SampleCount = 64;

    private readonly Stopwatch stopwatch = new Stopwatch();

    private readonly RingBuffer<long> normalGetterBuffer = new RingBuffer<long>(SampleCount);

    private readonly RingBuffer<long> normalSetterBuffer = new RingBuffer<long>(SampleCount);

    private readonly RingBuffer<long> reflectionGetterBuffer = new RingBuffer<long>(SampleCount);

    private readonly RingBuffer<long> reflectionSetterBuffer = new RingBuffer<long>(SampleCount);

    private readonly RingBuffer<long> expressionGetterBuffer = new RingBuffer<long>(SampleCount);

    private readonly RingBuffer<long> expressionSetterBuffer = new RingBuffer<long>(SampleCount);

    private FieldInfo fieldInfo;

    private Func<object, object> fieldGetter;

    private Action<object, object> fieldSetter;

    private int num;

    private GUIStyle guiStyle;

    private void Start()
    {
        guiStyle = new GUIStyle("label")
        {
            fontSize = 64,
            alignment = TextAnchor.MiddleCenter
        };
        var type = GetType();
        fieldInfo = type.GetField("_number", BindingFlags.Instance | BindingFlags.NonPublic);
        fieldGetter = CompileFieldGetter(type, fieldInfo);
        fieldSetter = CompileFieldSetter(type, fieldInfo);
    }

    private void OnGUI()
    {
        BenchmarkNormalGetter();
        BenchmarkNormalSetter();
        BenchmarkReflectionGetter();
        BenchmarkReflectionSetter();
        BenchmarkExpressionGetter();
        BenchmarkExpressionSetter();

        var cellHeight = (float)Screen.height / 6;
        GUILabel(new Rect(0, 0 * cellHeight, Screen.width, cellHeight), $"Normal Getter: {Average(normalGetterBuffer)}");
        GUILabel(new Rect(0, 1 * cellHeight, Screen.width, cellHeight), $"Normal Setter: {Average(normalSetterBuffer)}");
        GUILabel(new Rect(0, 2 * cellHeight, Screen.width, cellHeight), $"Reflection Getter: {Average(reflectionGetterBuffer)}");
        GUILabel(new Rect(0, 3 * cellHeight, Screen.width, cellHeight), $"Reflection Setter: {Average(reflectionSetterBuffer)}");
        GUILabel(new Rect(0, 4 * cellHeight, Screen.width, cellHeight), $"Expression Getter: {Average(expressionGetterBuffer)}");
        GUILabel(new Rect(0, 5 * cellHeight, Screen.width, cellHeight), $"Expression Setter: {Average(expressionSetterBuffer)}");
    }

    private void GUILabel(Rect area, string content)
    {
        GUI.Label(area, content, guiStyle);
    }

    private void BenchmarkNormalGetter()
    {
        stopwatch.Restart();
        for (var i = 0; i < Iterations; i++)
        {
            var number = num;
        }

        stopwatch.Stop();
        normalGetterBuffer.Push(stopwatch.ElapsedTicks);
    }

    private void BenchmarkNormalSetter()
    {
        stopwatch.Restart();
        for (var i = 0; i < Iterations; i++)
        {
            num = 42;
        }

        stopwatch.Stop();
        normalSetterBuffer.Push(stopwatch.ElapsedTicks);
    }

    private void BenchmarkReflectionGetter()
    {
        stopwatch.Restart();
        for (var i = 0; i < Iterations; i++)
        {
            var number = (int)fieldInfo.GetValue(this);
        }

        stopwatch.Stop();
        reflectionGetterBuffer.Push(stopwatch.ElapsedTicks);
    }

    private void BenchmarkReflectionSetter()
    {
        stopwatch.Restart();
        for (var i = 0; i < Iterations; i++)
        {
            fieldInfo.SetValue(this, 42);
        }

        stopwatch.Stop();
        reflectionSetterBuffer.Push(stopwatch.ElapsedTicks);
    }

    private void BenchmarkExpressionGetter()
    {
        stopwatch.Restart();
        for (var i = 0; i < Iterations; i++)
        {
            var number = (int)fieldGetter(this);
        }

        stopwatch.Stop();
        expressionGetterBuffer.Push(stopwatch.ElapsedTicks);
    }

    private void BenchmarkExpressionSetter()
    {
        stopwatch.Restart();
        for (var i = 0; i < Iterations; i++)
        {
            fieldSetter(this, 123);
        }

        stopwatch.Stop();
        expressionSetterBuffer.Push(stopwatch.ElapsedTicks);
    }

    private static Func<object, object> CompileFieldGetter(Type type, FieldInfo fieldInfo)
    {
        var ownerParameter = Expression.Parameter(typeof(object));

        var fieldExpression = Expression.Field(Expression.Convert(ownerParameter, type), fieldInfo);

        return Expression.Lambda<Func<object, object>>(Expression.Convert(fieldExpression, typeof(object)), ownerParameter).Compile();
    }

    private static Action<object, object> CompileFieldSetter(Type type, FieldInfo fieldInfo)
    {
        var ownerParameter = Expression.Parameter(typeof(object));
        var fieldParameter = Expression.Parameter(typeof(object));

        var fieldExpression = Expression.Field(Expression.Convert(ownerParameter, type), fieldInfo);

        return Expression.Lambda<Action<object, object>>(
                Expression.Assign(fieldExpression, Expression.Convert(fieldParameter, fieldInfo.FieldType)),
                ownerParameter,
                fieldParameter)
            .Compile();
    }

    private static long Average(RingBuffer<long> buffer)
    {
        long total = 0;

        for (var i = 0; i < buffer.Length; i++)
        {
            total += buffer[i];
        }

        return (total / buffer.Length) / Iterations;
    }
}