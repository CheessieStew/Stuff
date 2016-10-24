// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.


let rec merge xss yss res =
    match (xss,yss) with
    | ((x::xs),(y::ys)) ->
        if (x<=y) then merge xs (y::ys) (x::res)
        else merge (x::xs) ys (y::res) 
    | ([],ys) -> (List.rev res) @ ys
    | (xs,[]) -> (List.rev res) @ xs

let split list = 
    let rec aux norm accel acc = 
        match accel with
        | [] -> 
            (acc,norm)
        | [a] -> 
            (a::acc,norm)
        | (a1::a2::ass) ->
            aux (List.tail norm) (ass) ((List.head norm)::acc)
    aux list list []

let rec mergesort list =
    match list with
    | [] -> []
    | [a] -> [a]
    | list ->
        let (left,right) = split list
        let (sleft, sright) = (mergesort left, mergesort right)
        merge sleft sright []

type tree = Node of int * tree * tree | Leaf of int

let flatten tree = 
    let rec aux tree list = match tree with
        | Node(x,t1,t2) -> aux t1 (x::(aux t2 list))
        | Leaf x -> x::list
    aux tree []

let rec sublist list:('a list) = 
    let rec addx (x:'a) (list:('a list list)) = 
        match list with
        | [] -> []
        | (y::ys) -> (x::y)::(addx x ys)
    match list with
    | [] -> [[]] : ('a list list)
    | (x::xs) -> 
        let xss = sublist xs
        xss @ (addx x xss)

[<EntryPoint>]
let main argv = 
    printfn "%A" (mergesort [5;6;7;8;1;2;3;4])
    printfn "%A" (flatten (Node(4,Node(2,Leaf 1, Leaf 3),Node(6,Leaf 5, Leaf 7))))
    System.Console.ReadKey()
    0 // return an integer exit code
