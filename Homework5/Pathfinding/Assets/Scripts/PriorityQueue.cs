using System.Collections.Generic;

public class PriorityQueue<T>
{
	private List<T> heap_;
	private IComparer<T> comparer_;

	public int Count
	{
		get
		{
			return heap_.Count;
		}
	}

	public PriorityQueue(IComparer<T> comparer)
	{
		heap_ = new List<T>();
		comparer_ = comparer;
	}

	private void Swap(int a, int b)
	{
		T temp = heap_[a];
		heap_[a] = heap_[b];
		heap_[b] = temp;
	}

	public void Enqueue(T item)
	{
		int index = heap_.Count;
		heap_.Add(item);

		while (index > 0)
		{
			int parentIndex = index / 2;
			if (comparer_.Compare(heap_[parentIndex], heap_[index]) > 0)
			{
				Swap(parentIndex, index);
				index = parentIndex;
			}
			else
				break;
		}
	}

	public T Dequeue()
	{
		int index = 0;

		T result = heap_[0];
		Swap(index, heap_.Count - 1);
		heap_.RemoveAt(heap_.Count - 1);

		while (index < heap_.Count)
		{
			int left = index * 2 + 1;
			int right = index * 2 + 2;
			int swap = 0;

			if (left >= heap_.Count)
				if (right >= heap_.Count)
					swap = -1;
				else
					swap = right;
			else
				if (right >= heap_.Count)
					swap = left;
				else
					if (comparer_.Compare(heap_[left], heap_[right]) < 0)
						swap = left;
					else
						swap = right;

			if (swap != -1 && comparer_.Compare(heap_[swap], heap_[index]) < 0)
			{
				Swap(swap, index);
				index = swap;
			}
			else
				break;
		}

		return result;
	}
}