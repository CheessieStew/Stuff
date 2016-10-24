import System.Random
import System.Environment

type Square = [[Double]]


getSquare ::  Double -> Int -> IO Square
getSquare err 0 = do
  v <- randomAverage err 0 0
  return [[v]]
getSquare err n = do
  s1 <- getSquare err (n-1)
  s2 <- getSquare err (n-1)
  s3 <- getSquare err (n-1)
  s4 <- getSquare err (n-1)
  mergeSquares err s1 s2 s3 s4 [] []


revapp :: [a] -> [a] -> [a]
revapp [] ys = ys
revapp (x:xs) ys = revapp xs (x:ys)

randomAverage :: Double -> Double -> Double -> IO Double
randomAverage err a b = do
  er <- randomRIO(-err/2,err/2)
  return ((a+b)/2 + er)

--do sklejenia odpowiednich wierszy dwoch kwadratow lezacych obok siebie
mergeRows :: Double -> [Double] -> [Double] -> IO [Double]
mergeRows err r1 r2 = do
  er <- randomRIO(-err/2,err/2)
  return (revapp r1 ((head r1 + head r2)/2 +er: r2))

--do sklejenia dwoch par sklejonych kwadratow, jedna nad druga
gluePartials :: Double -> [[Double]] -> [[Double]] -> IO Square
gluePartials err p1 p2 = do
  er <- sequence $ zipWith (randomAverage err) (head p1) (head p2)
  return (revapp p1 (er: p2))

--pattern matching niepełny, acz bez większego znaczenia w tym kontekście
mergeSquares :: Double -> Square -> Square -> Square -> Square -> [[Double]] -> [[Double]]-> IO Square
mergeSquares err (r1:rs1) (r2:rs2) (r3:rs3) (r4:rs4) acc1 acc2 = do
  newAcc1 <- mergeRows err r1 r2
  newAcc2 <- mergeRows err r3 r4
  mergeSquares err rs1 rs2 rs3 rs4 (newAcc1:acc1) (newAcc2:acc2)
mergeSquares err [] [] [] [] acc1 acc2 = gluePartials err acc1 acc2


getGrid :: Square -> [(Int,Int,Double)]
getGrid sq = aux sq 0 0 []
  where aux [] _ _ acc = acc
        aux ([]:rs) _ y acc = aux rs 0 (y+1) acc
        aux ((r1:r):rs) x y acc = aux (r:rs) (x+1) y ((x,y,r1):acc)


main :: IO ()
main = do
  args <- getArgs
  name <- getProgName
  if (length args /= 2)
    then putStrLn $ "usage: " ++ name ++" [displacement] [recursion depth]"
    else do
      square <- getSquare (read $ args!!0) (read $ args!!1)
      mapM_ print' $ getGrid square
      where print' (a,b,c) = putStrLn (show a ++ " " ++ show b ++ " " ++ " " ++ show c)
