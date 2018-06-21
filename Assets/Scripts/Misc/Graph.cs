using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;

public class Graph {

    public List<Vertex> Vertices;

    public class Vertex {
        public Vector3 Coord {
            get; set;
        }
        public List<Edge> Edges {
            get; set;
        }

        public List<Vertex> AdjacentVertices() {
            Edges.Sort();
            return Edges.Select(edge => edge.OppositVertex(this)).ToList();
        }       

        public Vertex(Vector3 coord) {
            Coord = coord;
            Edges = new List<Edge>();
        }

        public void AddEdge(Edge edge) {
            Edges.Add(edge);
        }

    }

    public class Edge:IEquatable<Edge>, IComparable<Edge> {
        public Vertex Orig {
            get; set;
        }

        public Vertex Dest {
            get; set;
        }

        public float Distance {
            get; set;
        }

        public Vertex OppositVertex(Vertex vert) {
            return vert == Dest ? Orig : vert == Orig ? Dest : null;
        }

        public bool Equals(Edge other) {
            if(other == null)
                return false;
            return Orig == other.Orig &&
                Dest == other.Dest &&
                Math.Abs(Distance-other.Distance)<0.01;
        }

        public int CompareTo(Edge other) {
            if(other == null)
                return -1;
            if((Orig == other.Orig && Dest == other.Dest) || (Orig == other.Dest && Dest == other.Orig))
                return 0;
            return 1;
            /*if(Distance > other.Distance)
                return 1;
            if(Distance < other.Distance)
                return -1;
            return 0;*/
        }

        public Edge(Vertex origin,Vertex destination,float distance) {
            Orig = origin;
            Dest = destination;
            Distance = distance;
        }
    }

    private class RouteElem:IComparable<RouteElem> {
        public Vertex Vert {
            get; set;
        }
        public RouteElem PrevElem {
            get; set;
        }
        public float H {
            get; set;
        }
        public float G {
            get; set;
        }
        public float F {
            get {
                return H + G;
            }
        }

        public RouteElem(Vertex vert,float g) {
            Vert = vert;
            G = g;
        }
        public RouteElem(Vertex vert,float g,Vector3 dest,RouteElem prevElem) {
            Vert = vert;
            G = g;
            PrevElem = prevElem;
            FindH(dest);
        }

        public void FindH(Vector3 dest) {
            H = Vector3.Distance(Vert.Coord,dest);
        }

        public int CompareTo(RouteElem elem) {
            return F.CompareTo(elem.F);
        }

        public override string ToString() {
            return Vert.Coord.ToString() + "F: "+F+" G: "+G+" H: "+H;
        }
    }

    public void AddVertex(Vector3 coord) {
        Vertices.Add(new Vertex(coord));
    }
    public void AddEdge(Vector3 v1Coord,Vector3 v2Coord) {
        Vertex v1 = null, v2 = null;
        for(int i = Vertices.Count-1;i>=0;--i) {
            if(Vertices[i].Coord == v1Coord)
                v1 = Vertices[i];
            else if(Vertices[i].Coord == v2Coord)
                v2 = Vertices[i];
            if(v1 != null && v2 != null)
                break;
        }

        if(v1 == null || v2 == null)
            return;

        float length = 1.0f;
        if((int)v2.Coord.x != (int)v1.Coord.x && (int)v2.Coord.z != (int)v1.Coord.z)
            length = 1.4f;

        var edge = new Edge(v1,v2,length); //(float)Math.Sqrt(Math.Pow(Math.Abs(v2.Coord.x - v1.Coord.x),2) + Math.Pow(Math.Abs(v2.Coord.z - v1.Coord.z),2)));
        if(!v1.Edges.Exists(e => e.CompareTo(edge)==0))
            v1.AddEdge(edge);
        if(!v2.Edges.Exists(e => e.CompareTo(edge)==0))
            v2.AddEdge(edge);
    }

    public Graph() {
        Vertices = new List<Vertex>();
    }

    public void AddRange(Vector3[] arr) {
        foreach(var vect in arr)
            Vertices.Add(new Vertex(vect));
    }

    private Vertex FindVertex(Vector3 vert) {
        foreach(var vertex in Vertices)
            if(Math.Abs(vertex.Coord.x - vert.x) < 1.0f &&
               Math.Abs(vertex.Coord.z - vert.z) < 1.0f)
                return vertex;
        return null;
    }

    private Vertex FindVertex(float x, float z) {
        foreach(var vertex in Vertices)
            if((int)vertex.Coord.x == (int)x && (int)vertex.Coord.z == (int)z)
                return vertex;
        return null;
    }

    /** 
    * A* algorithm. Returns an empty list if there is no path. 
    * Returns a List, where the last element is the closest vertex to the destination point. 
    * 
    * @param origin
    * @param destination
    */ 
    public List<Vector3> FindRoute(Vector3 origin, Vector3 destination) {
        var open = new MinHeap<RouteElem>();    //Хипа вместо приоритетой очереди
        var orig = FindVertex(origin);          //Находим вершину старт
        var dest = FindVertex(destination);     //Вершину, ближайшую к точку назначения

        var closed = new List<Vector3>();       //Список пройденных вершин       

        if(orig == null || dest == null)
            throw new ArithmeticException();

        var first = new RouteElem(orig,0,destination,null);

        open.Add(first); //Добавляем стартовый элемент в хипу

        RouteElem current = null;
        while(!open.IsEmpty()) {
            current = open.Pop();   //Достаем наиболее оптимальную вершину
            var x = current.Vert;

            if(In(closed,x.Coord))
                continue;
            if(x.Coord == dest.Coord)
                break; //return path

            closed.Add(x.Coord);

            foreach(var vert in current.Vert.AdjacentVertices()) { //Проверяем все смежные вершины
                //var condition = closed.Find(v => CompareVectors(v,vert.Coord)==0);
                if(!In(closed, vert.Coord)) {//Если вершина не была еще рассмотрена //Vector3 camparing. TODO: check 
                    var tmp = new RouteElem(vert,current.G + Vector3.Distance(current.Vert.Coord,vert.Coord),destination,current); //Добавляем ее в хипу
                    open.Add(tmp);  //TODO: OPTIMIZE -> AddRange() - only 1 balance
                }
            }
        }
        //путь
        // Console.WriteLine("found");
        

        //if(current == null)//open.Peek() == null)             //Хипа пустая <==> путь не найден
        //    return new List<Vector3>();     //Пустой список

        if((int)current.Vert.Coord.x == 0 && (int)current.Vert.Coord.z == 0 && (int)destination.x != 0 && (int)destination.z != 0)
            return new List<Vector3>();

        var last = current;//open.Peek();
        var path = new List<Vector3>();

        var currElem = last;
        while(currElem.PrevElem != null) {
            path.Add(currElem.Vert.Coord);
            currElem = currElem.PrevElem;
        }
        path.Add(currElem.Vert.Coord);

        path.Reverse();
        return path;

        //throw new NotImplementedException();
    }

    public List<Vector3> FindFlatRoute(Vector3 origin,Vector3 destination) {
        var open = new MinHeap<RouteElem>();    //Хипа вместо приоритетой очереди
        var orig = FindVertex(origin);          //Находим вершину старт
        var dest = FindVertex(destination);     //Вершину, ближайшую к точку назначения

        var closed = new List<Vector3>();       //Список пройденных вершин       

        if(orig ==null || dest == null)
            throw new ArithmeticException();

        var first = new RouteElem(orig,0,destination,null);

        open.Add(first); //Добавляем стартовый элемент в хипу

        RouteElem current = null;
        while(!open.IsEmpty()) {
            current = open.Pop();   //Достаем наиболее оптимальную вершину
            var x = current.Vert;

            if(In(closed,x.Coord))
                continue;
            if(x.Coord == dest.Coord)
                break; //return path

            closed.Add(x.Coord);

            foreach(var vert in CloserVertices(current.Vert.Coord)) {
                if(!In(closed,vert.Coord)) {
                    var tmp = new RouteElem(vert,current.G + Vector3.Distance(current.Vert.Coord,vert.Coord),destination,current);
                    open.Add(tmp);  //TODO: OPTIMIZE -> AddRange() - only 1 balance
                }
            }
        }

        if(current == null)
            return new List<Vector3>();

        var last = current;
        var path = new List<Vector3>();

        var currElem = last;
        while(currElem.PrevElem != null) {
            path.Add(currElem.Vert.Coord);
            currElem = currElem.PrevElem;
        }
        path.Add(currElem.Vert.Coord);

        path.Reverse();
        return path;
    }

    public List<Vector3> OLDFindRoute(Vector3 origin,Vector3 destination) {
        var heap = new MinHeap<RouteElem>();    //Хипа вместо приоритетой очереди
        var orig = FindVertex(origin);          //Находим вершину старт
        var dest = FindVertex(destination);     //Вершину, ближайшую к точку назначения

        var closed = new List<Vector3>();       //Список пройденных вершин       

        if(orig ==null || dest == null)
            throw new ArithmeticException();

        var first = new RouteElem(orig,0,destination,null);

        heap.Add(first); //Добавляем стартовый элемент в хипу

        while(heap.Peek() != null && heap.Peek().Vert.Coord != dest.Coord) { //Пока есть что смотреть и пока не дошли до точки назначения
            //heap.PrintArr();
            //Console.WriteLine("\n");
            var current = heap.Pop();   //Достаем наиболее оптимальную вершину

            //while(In(closed,current.Vert.Coord))
            //    current = heap.Pop();

            //Console.WriteLine("Selected: "+current.Vert.Coord+" H:"+current.H+" F:"+current.F);
            //heap.Remove(current);
            //Console.Write("Popped: " + current.ToString()+"\n");
            //heap.PrintArr();
            //Console.WriteLine("\n");
            foreach(var vert in current.Vert.AdjacentVertices()) { //Проверяем все смежные вершины
                //var condition = closed.Find(v => CompareVectors(v,vert.Coord)==0);
                if(!In(closed,vert.Coord)) {//Если вершина не была еще рассмотрена //Vector3 camparing. TODO: check 
                    var tmp = new RouteElem(vert,current.G + Vector3.Distance(current.Vert.Coord,vert.Coord),destination,current); //Добавляем ее в хипу
                    heap.Add(tmp);  //TODO: OPTIMIZE -> AddRange() - only 1 balance
                }
            }
            //heap.Sort();
            //list.Reverse();
            closed.Add(current.Vert.Coord); //Добавляем рассматриваемую вершину в список пройденных
        }
        //путь
        // Console.WriteLine("found");

        Debug.LogWarning("CLOSED: "+closed.Count);

        if(heap.Peek() == null)             //Хипа пустая <==> путь не найден
            return new List<Vector3>();     //Пустой список

        var last = heap.Peek();
        var path = new List<Vector3>();

        var currElem = last;
        while(currElem.PrevElem != null) {
            path.Add(currElem.Vert.Coord);
            currElem = currElem.PrevElem;
        }
        path.Add(currElem.Vert.Coord);

        path.Reverse();
        return path;

        //throw new NotImplementedException();
    }

    private bool In(List<Vector3> closed, Vector3 vector ) {
        foreach (var vect in closed) {
            if(CompareVectors(vect,vector)==0)
                return true;
        }
        return false;
    }

    private int CompareVectors(Vector3 v1,Vector3 v2) {
        if((int)v1.x==(int)v2.x && (int)v1.y==(int)v2.y && (int)v1.z==(int)v2.z)
            return 0;
        return 1;
    }

    public void Print() {
        StringBuilder sb = new StringBuilder();
        foreach(var vertex in Vertices) {
            sb.Append(vertex.Coord.ToString()+" : ");
            foreach(var adjacentVertex in vertex.AdjacentVertices()) {
                sb.Append(adjacentVertex.Coord.ToString() + "  |  ");
            }
            sb.Append("\n");
        }
        Console.WriteLine(sb);
    }

    public Vector3 FindCoord(float x, float z) {
        foreach (var vertex in Vertices) {
            if((int)vertex.Coord.x == (int)x && (int)vertex.Coord.z == (int)z)
                return vertex.Coord;
        }
        return new Vector3(-1 ,-1, -1);
    }


    public List<Vertex> CloserVertices(Vector3 vertCoord) {
        var list = new List<Vertex>();

        for(int i = -1;i<2;++i)
            for(int j = -1;j<2;++j) {
                var vert = FindVertex(vertCoord.x + i, vertCoord.z + j);
                if(vert!=null)
                    list.Add(vert);
            }

        return list;
    }
}
