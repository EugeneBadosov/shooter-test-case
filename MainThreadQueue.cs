using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Очередь команд для выполнения в главном потоке. Каждая командная функция (например, New GameObject)
/// принимает параметр результата, который изначально не имеет значения, но получает значение после выполнения команды.
/// </summary>
public class MainThreadQueue
{
	/// <summary>
	/// Результат команды, поставленной в очередь. Будет иметь значение, когда оно будет Готово.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Result<T>
	{
		private T value;
		private bool hasValue;
		private AutoResetEvent readyEvent;

		public Result()
        {
			readyEvent = new AutoResetEvent(false);
        }

		/// <summary>
		/// Значение результата. Блокируется до тех пор, пока значение isReady не станет истинным.
		/// </summary>
		public T Value
        {
            get
            {
				readyEvent.WaitOne();
				return value;
            }
        }

		/// <summary>
		/// Проверяет, готово ли результирующее значение.
		/// </summary>
		public bool IsReady
        {
            get
            {
				return hasValue;
            }
        }

		public void Ready(T value)
        {
			this.value = value;
			hasValue = true;
			readyEvent.Set();
        }

		public void Reset()
        {
			value = default(T);
			hasValue = false;
        }
	}

	public class Result
    {
		private bool hasValue;
		private AutoResetEvent readyEvent;

		public Result()
        {
			readyEvent = new AutoResetEvent(false);
        }

		public bool IsReady
        {
            get
            {
				return hasValue;
            }
        }

		public void Ready()
        {
			hasValue = true;
			readyEvent.Set();
        }

		public void Wait()
        {
			readyEvent.WaitOne();
        }

		public void Reset()
        {
			hasValue = false;
        }
    }

	private enum CommandType
    {
		GetTransform,

		SetRotation
    }

	private abstract class BaseCommand
    {
		public CommandType Type;
    }

	private class GetTransformCommand : BaseCommand
    {
		public GameObject GameObject;

		public Result<Transform> Result;

		public GetTransformCommand()
        {
			Type = CommandType.GetTransform;
        }
    }

	private class SetRotationCommand : BaseCommand
    {
		public Transform Transform;

		public float ZAngle;

		public Result Result;

		public SetRotationCommand()
        {
			Type = CommandType.SetRotation;
        }
    }

	private Stack<GetTransformCommand> getTransformPool;
	private Stack<SetRotationCommand> setRotationPool;

	private Queue<BaseCommand> commandQueue;

	private Stopwatch executeLimitStopwatch;

	public MainThreadQueue()
    {
		getTransformPool = new Stack<GetTransformCommand>();
		setRotationPool = new Stack<SetRotationCommand>();
		commandQueue = new Queue<BaseCommand>();
		executeLimitStopwatch = new Stopwatch();
    }

	private static T GetFromPool<T>(Stack<T> pool)
		where T : new()
    {
        lock (pool)
        {
			if(pool.Count > 0)
            {
				return pool.Pop();
            }
        }
		return new T();
    }

	private static void ReturnToPool<T>(Stack<T> pool, T obj)
    {
        lock (pool)
        {
			pool.Push(obj);
        }
    }

	private void QueueCommand(BaseCommand cmd)
    {
        lock (commandQueue)
        {
			commandQueue.Enqueue(cmd);
        }
    }

	public void GetTransform(GameObject go, Result<Transform> result)
    {
		Assert.IsTrue(go != null);
		Assert.IsTrue(result != null);

		result.Reset();
		GetTransformCommand cmd = GetFromPool(getTransformPool);
		cmd.GameObject = go;
		cmd.Result = result;
		QueueCommand(cmd);
    }

	public void SetRotation(Transform transform, float zAngle, Result result)
    {
		Assert.IsTrue(transform != null);
		Assert.IsTrue(result != null);

		result.Reset();
		SetRotationCommand cmd = GetFromPool(setRotationPool);
		cmd.Transform = transform;
		cmd.ZAngle = zAngle;
		cmd.Result = result;
		QueueCommand(cmd);
    }

	public void Execute(int maxMilliseconds = int.MaxValue)
    {
		Assert.IsTrue(maxMilliseconds > 0);

		executeLimitStopwatch.Reset();
		executeLimitStopwatch.Start();
		while (executeLimitStopwatch.ElapsedMilliseconds < maxMilliseconds)
        {
			BaseCommand baseCmd;
            lock (commandQueue)
            {

            }
        }
    }
}
