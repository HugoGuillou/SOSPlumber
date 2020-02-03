using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOS.Pools {

	[Serializable]
	public class Pool<T> {
		[SerializeField]
		protected List<T> _Pool;

		private int _PickOnceIndex = 0;
		protected List<T> _OncePool;

		public T Pick () {
			return _Pool[UnityEngine.Random.Range (0, _Pool.Count)];
		}

		public T PickOnce () {
			if (_OncePool == null || _OncePool.Count <= 0) {
				_OncePool = new List<T> (_Pool);
			}
			if (_PickOnceIndex >= _OncePool.Count - 1) {
				_PickOnceIndex = 0;
				return _OncePool[_OncePool.Count - 1];
			}
			else {
				int index = UnityEngine.Random.Range (_PickOnceIndex, _OncePool.Count);
				T tmp = _OncePool[index];
				_OncePool[index] = _OncePool[_PickOnceIndex];
				_OncePool[_PickOnceIndex] = tmp;
				_PickOnceIndex++;
				return tmp;
			}
		}
	}

	public class NativeTypePool<T> : Pool<T> {
	}

	[Serializable]
	public class FloatPool : NativeTypePool<float> { }

	[Serializable]
	public class IntPool : NativeTypePool<int> {
		public float Pick (float max) {
			return base.Pick () / max;
		}
	}

	[Serializable]
	public class StringPool : NativeTypePool<string> { }
}
