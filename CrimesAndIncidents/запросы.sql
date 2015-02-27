--общее количество преступлений за период
SELECT COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1;

--общее количество происшетвий за период
SELECT COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause = null;

--количество преступлений за период по частям
SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause > -1
GROUP BY M.number, M.shortName
ORDER BY M.idMilitaryUnit;

--количество происшествий за период по частям
SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM
    MilitaryUnit M LEFT JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN "2014.01.01" AND "2014.12.31" AND C.idClause = null
GROUP BY M.number, M.shortName
ORDER BY M.idMilitaryUnit;

--сведения по преступлениям за период
SELECT M.number, M.shortName, C.story FROM
    MilitaryUnit M INNER JOIN
    Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND  C.dateRegistration BETWEEN "2014.12.01" AND "2014.12.31"
--GROUP BY M.number, M.shortName
ORDER BY M.idMilitaryUnit;