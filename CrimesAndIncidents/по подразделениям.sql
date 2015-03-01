--количество преступлений за период по частям
SELECT M.number, M.shortName, C.idMilitaryUnit FROM
    Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN SubUnit S ON S.idSubUnit = A.idSubUnit 
    LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit
WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1
--    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit 
--GROUP BY M.number, M.shortName
--ORDER BY M.idMilitaryUnit;