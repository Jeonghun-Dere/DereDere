using System;
using System.Collections;
using System.Collections.Generic;

public class LinkedList<T> : IEnumerable<T>
{
    // Node 클래스 정의
    private class Node
    {
        public T Data { get; set; }
        public Node Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    private Node head; // 리스트의 첫 번째 노드
    private int count; // 리스트의 노드 개수

    public LinkedList()
    {
        head = null;
        count = 0;
    }

    // 노드 추가 (리스트의 끝에 추가)
    public void Add(T data)
    {
        Node newNode = new Node(data);
        if (head == null)
        {
            head = newNode;
        }
        else
        {
            Node current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
        count++;
    }

    // 노드 제거 (특정 데이터 제거)
    public bool Remove(T data)
    {
        if (head == null) return false;

        if (head.Data.Equals(data))
        {
            head = head.Next;
            count--;
            return true;
        }

        Node current = head;
        while (current.Next != null && !current.Next.Data.Equals(data))
        {
            current = current.Next;
        }

        if (current.Next != null)
        {
            current.Next = current.Next.Next;
            count--;
            return true;
        }

        return false;
    }

    // 리스트 크기 반환
    public int Count()
    {
        return count;
    }

    // 리스트 비우기
    public void Clear()
    {
        head = null;
        count = 0;
    }

    // 리스트 출력 (디버깅용)
    public void PrintAll()
    {
        Node current = head;
        while (current != null)
        {
            Console.Write($"{current.Data} -> ");
            current = current.Next;
        }
        Console.WriteLine("null");
    }

    // Unity의 List<T>를 연결 리스트로 변환
    public void FromList(List<T> list)
    {
        Clear(); // 기존 노드 초기화
        foreach (var item in list)
        {
            Add(item);
        }
    }

    // IEnumerator<T> 구현으로 foreach 지원
    public IEnumerator<T> GetEnumerator()
    {
        Node current = head;
        while (current != null)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}