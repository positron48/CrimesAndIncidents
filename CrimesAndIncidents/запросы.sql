--общее количество преступлений за период
SELECT COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1;

--общее количество происшетвий за период
SELECT COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause IS NULL;

--количество преступлений за период по частям
SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1
GROUP BY M.number, M.shortName
ORDER BY M.idMilitaryUnit;

--количество происшествий за период по частям
SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause IS NULL
GROUP BY M.number, M.shortName
ORDER BY M.idMilitaryUnit;

--сведения по преступлениям за период
SELECT M.number, M.shortName, C.story FROM
    MilitaryUnit M INNER JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND  C.dateRegistration BETWEEN "2014.12.01" AND "2014.12.31"
--GROUP BY M.number, M.shortName
ORDER BY M.idMilitaryUnit;

--количество преступлений за период по подразделениям
SELECT Name, COUNT(idCrime) FROM 
    (SELECT M.number, M.shortName, S.Name, C.idCrime FROM
        Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
        LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
        LEFT JOIN SubUnit S ON S.idSubUnit = A.idSubUnit 
        LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit
    WHERE C.isRegistred = 1 AND C.dateRegistration 
        BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1
    GROUP BY C.idCrime
    ORDER BY M.idMilitaryUnit)
WHERE number = '19612'
GROUP BY Name
ORDER BY Name;

--количество происшествий за период по подразделениям
SELECT Name, COUNT(idCrime) FROM 
    (SELECT M.number, M.shortName, S.Name, C.idCrime FROM
        Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
        LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
        LEFT JOIN SubUnit S ON S.idSubUnit = A.idSubUnit 
        LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit
    WHERE C.isRegistred = 1 AND C.dateRegistration 
        BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause IS NULL
    GROUP BY C.idCrime
    ORDER BY M.idMilitaryUnit)
WHERE number = '31985'
GROUP BY Name
ORDER BY Name;

--количество призывников преступников
SELECT COUNT(DISTINCT A.idAccomplice) FROM
    Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 0 
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1;

--количество с/с контрактников преступников
SELECT COUNT(DISTINCT A.idAccomplice) FROM
    Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 1
    AND R.priority < 60 
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1;
    
--количество прапорщиков преступников
SELECT COUNT(DISTINCT A.idAccomplice) FROM
    Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 1
    AND R.priority BETWEEN 60 AND 75
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1;
    
--количество офицеров преступников
SELECT COUNT(DISTINCT A.idAccomplice) FROM
    Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 1
    AND R.priority > 75
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1;
    
--количество преступлений за период по участникам
SELECT M.number, M.shortName, R.shortName, R.priority, A.isContrakt, A.shortName, COUNT(C.idCrime) FROM
    Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1
GROUP BY A.idAccomplice
ORDER BY R.priority;

--количество преступлений по частям и участникам
SELECT M1.number, M1.name, T1.n1 as призывники, T2.n2 as контрактники, T3.n3 as прапорщики, T4.n4 as офицеры FROM 
(--воинские части
SELECT M.number, M.name FROM MilitaryUnit M) M1 LEFT JOIN 
(--количество призывников
SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n1 FROM
    MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit
    LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 0
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1
GROUP BY M.idMilitaryUnit
ORDER BY M.idMilitaryUnit) T1 ON T1.number = M1.number 
LEFT JOIN
(--количество контрактников
SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n2 FROM
    MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit
    LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 1
    AND R.priority < 60
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1
GROUP BY M.idMilitaryUnit
ORDER BY M.idMilitaryUnit) T2 ON T2.number = M1.number 
LEFT JOIN
(--количество прапорщиков преступников
SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n3 FROM
    MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit
    LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 1
    AND R.priority BETWEEN 60 AND 75
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1
GROUP BY M.idMilitaryUnit
ORDER BY M.idMilitaryUnit) T3  ON T3.number = M1.number 
LEFT JOIN
(--количество офицеров преступников
SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n4 FROM
    MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit
    LEFT JOIN Portaking P ON C.idCrime = P.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
    LEFT JOIN Rank R ON R.idRank = A.idRank
WHERE A.isContrakt = 1
    AND R.priority > 75
    AND C.isRegistred = 1 
    AND C.dateRegistration 
    BETWEEN "2014.01.01" AND "2014.12.31" 
    AND C.idClause > -1
GROUP BY M.idMilitaryUnit
ORDER BY M.idMilitaryUnit) T4  ON T4.number = M1.number;

--количество преступлений за период по видам
SELECT Cl.description, COUNT(C.idCrime) FROM
    Crime C LEFT JOIN Clause Cl ON Cl.idClause = C.idClause
WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1
GROUP BY Cl.description;

--количество преступлений за период по видам по частям
SELECT M.number, M.shortName, Cl.description, COUNT(C.idCrime) FROM
    Crime C LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit
    LEFT JOIN Clause Cl ON Cl.idClause = C.idClause
WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1
GROUP BY Cl.description, M.idMilitaryUnit
ORDER BY M.idMilitaryUnit;
