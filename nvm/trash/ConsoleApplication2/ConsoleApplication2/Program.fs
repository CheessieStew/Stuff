(* Auto-generated code below aims at helping you parse *)
(* the standard input according to the problem statement. *)
open System
open System.Collections.Generic




type vertex = int * int
type gameInfo = String list * int * int

let token = (Console.In.ReadLine()).Split [|' '|]
let R = int(token.[0]) (* R: number of rows. *)
let C = int(token.[1]) (* C: number of columns. *)
let A = int(token.[2]) (* A: number of rounds between the time the alarm countdown is activated and the time the alarm goes off. *)


let getPosAndRows _ =
    let rec aux list n = 
        match n with
        | 0 -> List.rev list
        | n -> aux (Console.In.ReadLine() :: list) (n-1)
    let token1 = (Console.In.ReadLine()).Split [|' '|]
    let KR = int(token1.[0])
    let KC = int(token1.[1])
    (aux [] R,KR,KC)


let moveTo (lab,kr,kc) (row,col) = 
    let closedset = Set.empty
    let openset = Set.empty.Add(kr,kc)
    let gscore = Array2D.zeroCreate<int> R C
    let fscore = Array2D.zeroCreate<int> R C
    a.[kr,kc] <- 0
    while (not openset.IsEmpty) do
        openset.
        ()
    lab,kr,kc

let neighbours ((lab,kr,kc):gameInfo) (row,col) =
    let poss = [(row-1,col); (row, col-1); (row+1,col); (row,col+1)]
    let unknown (a,b) = a >= 0 && b >= 0 && a <= R && b <= C && (lab.[a].[b] = '?')
    let (newLab,nkr,nkc) = if (List.filter unknown poss <> []) then moveTo (lab,kr,kc) (row,col) else (lab,kr,kc)
    let cond (a,b) = a >= 0 && b >= 0 && a <= R && b <= C && (newLab.[a].[b] <> '#')
    (List.filter cond poss,(newLab,nkr,nkc))
   




let dfs lab pos = 
    let rec aux lab (visited:Set<vertex>) stack = 
        match stack with
        | [] -> failwith "Not implemented yet"
        | stack -> let (nbrs,newlab) = (neighbours lab (List.head stack))
                   in aux newlab (visited.Add (List.head stack)) ( List.filter (fun vertex -> not (visited.Contains vertex)) nbrs @ List.tail stack)
    aux lab Set.empty [pos]


(* game loop *)
while true do
    ()
    
    
